using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BenchmarkUtility
{
    //Logging
    /*private static Func<string> logHeader = () => Parameters.Benchmarking.COLUMNAR_LOGGING ? "" :
        string.Join(",\t", Enum.GetValues(typeof(IterationStat)).Cast<IterationStat>().Select(s => s.ToString())) + ",\t" +
        string.Join(",\t", Enum.GetValues(typeof(AgentStat)).Cast<AgentStat>().Select(s => "avg_" + s.ToString())) + ",\t" +
        string.Join(",\t", Enum.GetValues(typeof(AgentStat)).Cast<AgentStat>().Select(s => "max_" + s.ToString())) + ",\t" +
        string.Join(",\t", Enum.GetValues(typeof(AgentStat)).Cast<AgentStat>().Select(s => "min_" + s.ToString()));
    private static string log = "";*/

    public static List<InfoCollector> allInfoCollectors = new List<InfoCollector>();
    public static int agentCount;

    #region Enums

    public enum AgentStat
    {
        // Human or Agent
        IS_AGENT,

        // Primary Metric
        AGENT_COLLISION_COUNT,
        OBSTACLE_COLLISION_COUNT,
        AGENT_COMPLETION_TIME,
        TOTAL_METABOLIC_ENERGY,
        
        // Turning Metrics
        TOTAL_DEGREES_TURNED,
        AVG_ANGULAR_SPEED,
        MAX_INSTANT_ANG_SPEED,
        //MAX_TURNING_IN_WINDOW,
        ANGULAR_INFLECTION_COUNT,

        // Distance/Speed Metrics
        TOTAL_DISTANCE,
        AVG_SPEED,
        MAX_INSTANT_SPEED,
        //MAX_DIST_IN_WINDOW,
        //MIN_DIST_IN_WINDOW,

        // Speed-Change Metrics
        TOTAL_SPEED_CHANGE,
        AVG_SPEED_CHANGE,
        MAX_INSTANT_SPEED_CHANGE,
        //MAX_SPEED_CHANGE_IN_WINDOW,
        SPEED_CHANGE_INFLECT_COUNT,

        // Acceleration Metrics
        TOTAL_ACCELERATION,
        AVG_ACCELERATION,
        MAX_INSTANT_ACCEL,
        //MAX_ACCEL_IN_WINDOW
    }

    public enum IterationStat
    {
        AGENT_COUNT,
        ELAPSED_FRAMES,
        SIMULATION_DURATION,
        REAL_TIME_DURATION,
        AVERAGE_FPS
    }

    #endregion

    #region Public Functions


    public static void AddInfoCollector(InfoCollector ic)
    {
        allInfoCollectors.Add(ic);
        if (ic.isAgent == 1) agentCount++;
    }

    public static void ComputeStatistics()
    {
        string delim = ",";
        string lineEnd = "\n";
        string metricCSV = "";
        string generalCSV = "";
        string iterationStatsCSV = "";
        var agentStatsEnums = Enum.GetValues(typeof(AgentStat)).Cast<AgentStat>();

        var iterationStats = ComputeIterationStatistics();

        var agentStats = new List<Dictionary<AgentStat, float>>();
        foreach (var infoCollector in allInfoCollectors)
        {
            agentStats.Add(ComputeAgentStatistics(infoCollector));
        }

        var avgStats = AverageAgentStats(agentStats);
        var maxStats = MaxAgentStats(agentStats);
        var minStats = MinAgentStats(agentStats);

        // Individual metrics
        metricCSV += string.Join(delim, agentStatsEnums.Select(x=>x.ToString())) + lineEnd; //Title
        foreach (var stat in agentStats)
        {
            metricCSV += StatsToString(stat);
            metricCSV += lineEnd;
        }

        // General metrics
        // Titles
        generalCSV += string.Join(delim, agentStatsEnums.Select(x => x.ToString()));
        generalCSV += lineEnd;
        // Data
        generalCSV += "Average: " + StatsToString(avgStats) + lineEnd;
        generalCSV += "Max: " + StatsToString(maxStats) + lineEnd;
        generalCSV += "Min: " + StatsToString(minStats) + lineEnd;

        iterationStatsCSV += string.Join(delim, Enum.GetValues(typeof(IterationStat)).Cast<IterationStat>().Select(x => x.ToString()));
        iterationStatsCSV += lineEnd;
        iterationStatsCSV += StatsToString(iterationStats);

        // Save file
        var postFilename = "_" + DateTime.Now.ToString("MMddyy_Hmmss") + ".csv";
        Debug.Log("Metric reporting (Individual): \n" + metricCSV);
        Save(Application.dataPath + "/Report", "MetricReporting" + postFilename, metricCSV);
        /*
        Debug.Log("Metric reporting (General): \n" + generalCSV);
        Save(Application.dataPath + "/Report", "MetricReportingGen" + postFilename, generalCSV);
        */
        Debug.Log("Iteration statistics: \n" + iterationStatsCSV);
        Save(Application.dataPath + "/Report", "IterationStatistics" + postFilename, iterationStatsCSV + lineEnd + generalCSV);

        //if (Parameters.Benchmarking.COLUMNAR_LOGGING)
        /*
        if(false)
        {
            log = StatsToString(iterationStats) + "\n" + StatsToString(avgStats, "avg_") + "\n" + StatsToString(maxStats, "max_") + "\n" + StatsToString(minStats, "min_") + "\n";
        } else
        {
            log = StatsToString(iterationStats) + ",\t" + StatsToString(avgStats) + ",\t" + StatsToString(maxStats) + ",\t" + StatsToString(minStats);
        }
        */
    }
    
    public static void PrintLog()
    {
        //Debug.Log(logHeader.Invoke() + "\n" + log);
    }

    public static bool Save(string path, string fileName, string data)
    {
        bool retValue;
        try
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            File.WriteAllText($"{path}/{fileName}", data);
            retValue = true;
            Debug.Log($"Saved {path}/{fileName}");
        }
        catch (Exception e)
        {
            string ErrorMessages = "File Write Error\n" + e.Message;
            retValue = false;
            Debug.LogError(ErrorMessages);
        }

        return retValue;
    }

    public static void Save(string path, string data)
    {
        if (File.Exists(path))
        {
            File.AppendAllText(path, data + "\n");
        } else
        {
            var sr = File.AppendText(path);
            sr.WriteLine(data);
            sr.Close();
        }
    }

    #endregion

    #region Private Functions

    private static Dictionary<IterationStat, float> ComputeIterationStatistics()
    {
        var stats = new Dictionary<IterationStat, float>();

        foreach (var stat in Enum.GetValues(typeof(IterationStat)).Cast<IterationStat>())
        {
            float val = 0;

            switch (stat)
            {
                case IterationStat.AGENT_COUNT:
                    val = agentCount;
                    break;
                case IterationStat.ELAPSED_FRAMES:
                    val = Time.frameCount;
                    break;
                case IterationStat.SIMULATION_DURATION:
                    val = Time.frameCount * Time.fixedDeltaTime; 
                    break;
                case IterationStat.REAL_TIME_DURATION:
                    //val = Updater.iterationStopwatch.ElapsedMilliseconds / 1000f;
                    val = Time.time; // time in seconds since game start
                    break;
                case IterationStat.AVERAGE_FPS:
                    val = Time.frameCount / Time.time;
                    break;
            }

            stats.Add(stat, val);
        }

        return stats;
    }

    private static Dictionary<AgentStat, float> ComputeAgentStatistics(InfoCollector info)
    {
        var stats = new Dictionary<AgentStat, float>();
        var fixedDeltaTime = Time.fixedDeltaTime;

        foreach (var stat in Enum.GetValues(typeof(AgentStat)).Cast<AgentStat>())
        {
            float val = 0;

            switch (stat)
            {
                // Human or Agent
                case AgentStat.IS_AGENT:
                    val = info.isAgent;
                    break;
                //Primary Metrics
                case AgentStat.AGENT_COLLISION_COUNT:
                    val = info.agentCollisionCount;
                    break;
                case AgentStat.OBSTACLE_COLLISION_COUNT:
                    val = info.obstacleCollisionCount;
                    break;
                case AgentStat.AGENT_COMPLETION_TIME:
                    //val = info.totalFrames * Parameters.Simulation.FIXED_DELTA_TIME;
                    val = info.totalFrames * fixedDeltaTime; 
                    break;
                case AgentStat.TOTAL_METABOLIC_ENERGY:
                    val = info.speedToMetabolicEnergy.GetTotal();
                    break;
                
                // Turning Metrics
                case AgentStat.TOTAL_DEGREES_TURNED:
                    val = info.angleToAbs.GetTotal();
                    break;
                case AgentStat.AVG_ANGULAR_SPEED:
                    //val = info.angleToAbs.GetTotal() / info.angleToAbs.GetTotalCount() / Parameters.Simulation.FIXED_DELTA_TIME;
                    val = info.angleToAbs.GetTotal() / info.angleToAbs.GetTotalCount() / fixedDeltaTime;
                    break;
                case AgentStat.MAX_INSTANT_ANG_SPEED:
                    //val = info.maxAbsAngle / Parameters.Simulation.FIXED_DELTA_TIME;
                    val = info.maxAbsAngle / fixedDeltaTime;
                    break;
                //case Stat.MAX_TURNING_IN_WINDOW:
                //    val = Util.GetMaxAngularDevInWindow(motion, 5);
                //    break;
                case AgentStat.ANGULAR_INFLECTION_COUNT:
                    val = Mathf.Abs(info.angleInflections);
                    break;

                // Distance/Speed Metrics
                case AgentStat.TOTAL_DISTANCE:
                    val = info.velToSpeed.GetTotal();
                    break;
                case AgentStat.AVG_SPEED:
                    val = info.velToSpeed.GetTotal() / info.velToSpeed.GetTotalCount() / fixedDeltaTime;
                    break;
                case AgentStat.MAX_INSTANT_SPEED:
                    val = info.maxSpeed / fixedDeltaTime;
                    break;
                //case Stat.MAX_DIST_IN_WINDOW:
                //    val = Util.GetMaxDistInWindow(motion, 5);
                //    break;
                //case Stat.MIN_DIST_IN_WINDOW:
                //    val = Util.GetMinDistInWindow(motion, 5);
                //    break;

                // Speed-Change Metrics
                case AgentStat.TOTAL_SPEED_CHANGE:
                    val = info.speedToSpeedChange.GetTotal();
                    break;
                case AgentStat.AVG_SPEED_CHANGE:
                    val = info.speedToSpeedChange.GetTotal() / info.speedToSpeedChange.GetTotalCount() / fixedDeltaTime;
                    break;
                case AgentStat.MAX_INSTANT_SPEED_CHANGE:
                    val = info.maxSpeedChange / fixedDeltaTime;
                    break;
                //case Stat.MAX_SPEED_CHANGE_IN_WINDOW:
                //    val = Util.GetMaxSpeedChangeInWindow(motion, 5);
                //    break;
                case AgentStat.SPEED_CHANGE_INFLECT_COUNT:
                    val = Mathf.Abs(info.speedChangeInflections);
                    break;

                // Acceleration MEtrics
                case AgentStat.TOTAL_ACCELERATION:
                    val = info.velToAccel.GetTotal();
                    break;
                case AgentStat.AVG_ACCELERATION:
                    val = info.velToAccel.GetTotal() / info.velToAccel.GetTotalCount() / fixedDeltaTime;
                    break;
                case AgentStat.MAX_INSTANT_ACCEL:
                    val = info.maxAccel / fixedDeltaTime;
                    break;
                //case Stat.MAX_ACCEL_IN_WINDOW:
                //    val = Util.GetMaxAccelInWindow(motion);
                //    break;
            }

            stats.Add(stat, val);
        }

        return stats;
    }

    private static Dictionary<AgentStat, float> AverageAgentStats(List<Dictionary<AgentStat, float>> agentStats)
    {
        var stats = new Dictionary<AgentStat, float>();

        foreach (var stat in Enum.GetValues(typeof(AgentStat)).Cast<AgentStat>())
        {
            stats.Add(stat, 0);

            foreach (var agent in agentStats)
            {
                stats[stat] += agent[stat];
            }

            stats[stat] /= agentStats.Count;
        }

        return stats;
    }

    private static Dictionary<AgentStat, float> MaxAgentStats(List<Dictionary<AgentStat, float>> agentStats)
    {
        var stats = new Dictionary<AgentStat, float>();

        foreach (var stat in Enum.GetValues(typeof(AgentStat)).Cast<AgentStat>())
        {
            stats.Add(stat, float.MinValue);

            foreach (var agent in agentStats)
            {
                stats[stat] = Mathf.Max(stats[stat], agent[stat]);
            }
        }

        return stats;
    }

    private static Dictionary<AgentStat, float> MinAgentStats(List<Dictionary<AgentStat, float>> agentStats)
    {
        var stats = new Dictionary<AgentStat, float>();

        foreach (var stat in Enum.GetValues(typeof(AgentStat)).Cast<AgentStat>())
        {
            stats.Add(stat, float.MaxValue);

            foreach (var agent in agentStats)
            {
                stats[stat] = Mathf.Min(stats[stat], agent[stat]);
            }
        }

        return stats;
    }

    private static string StatsToString<T>(Dictionary<T, float> stats, string columnarPrefix = "")
    {
        string str;

        /* (!Parameters.Benchmarking.COLUMNAR_LOGGING)
        if (true)
        {*/
            str = string.Join(",", Enum.GetValues(typeof(T)).Cast<T>().Select(stat =>
            {
                return stats[stat].ToString();
            }).ToArray());
        /*}
        else
        {
            str = string.Join("\n", Enum.GetValues(typeof(T)).Cast<T>().Select(stat =>
            {
                return columnarPrefix + stat + "," + stats[stat].ToString();
            }).ToArray());
        }*/

        return str;
    }
    
    #endregion

}
