using System.Collections;
using System.Collections.Generic;
//using Unity.Mathematics;
using UnityEngine;

public class InfoCollector : MonoBehaviour {
    
    public Vector3 initialPos;
    public Vector3 targetPos;
    public float mass;
    public int agentIndex;
    public int isAgent;

    public int agentCollisionCount = 0;
    public int obstacleCollisionCount = 0;
    public int totalFrames = 0;

    int iter = 0;

    #region Metrics

    public MetricBuffer<Vector3, Vector2> posToVel;
    public MetricSEQ<Vector2, float> velToSpeed;
    public float maxSpeed = 0;
    public MetricBuffer<float, float> speedToSpeedChange;
    public MetricSEQ<float, float> speedToMetabolicEnergy;
    public float maxSpeedChange = 0;
    public float speedChangeInflections = 0;
    public MetricBuffer<Vector2, float> velToAccel;
    public float maxAccel = 0;
    public MetricBuffer<Vector2, float> velToAngle;
    public int angleInflections = 0;
    public MetricSEQ<float, float> angleToAbs;
    public float maxAbsAngle = 0;

    #endregion
    
    void Start()
    {
        posToVel = new MetricBuffer<Vector3, Vector2>(2, (buffer) => {
            var vel3 = buffer.GetAt(1) - buffer.GetAt(0);

            return new Vector2(vel3.x, vel3.z);
        }, (a, b) => a + b, Vector2.zero);

        velToSpeed = new MetricSEQ<Vector2, float>((data) => {
            return data.magnitude;
        }, (a, b) => a + b, 0);

        speedToSpeedChange = new MetricBuffer<float, float>(2, (buffer) => {
            return buffer.GetAt(1) - buffer.GetAt(0);
        }, (a, b) => a + b, 0);

        //Refer to Equation 2 in "PLEdestrians: A Least-Effort Approach to Crowd Simulation"
        speedToMetabolicEnergy = new MetricSEQ<float, float>((data) => {
            var speed = data / Time.fixedDeltaTime;

            var kineticEnergy = mass * (2.23f + 1.26f * speed * speed);
            kineticEnergy *= Time.fixedDeltaTime;

            return kineticEnergy;
        }, (a, b) => a + b, 0);

        velToAccel = new MetricBuffer<Vector2, float>(2, (buffer) => {
            return (buffer.GetAt(1) - buffer.GetAt(0)).magnitude;
        }, (a, b) => a + b, 0);

        velToAngle = new MetricBuffer<Vector2, float>(2, (buffer) => {
            return Vector2.SignedAngle(buffer.GetAt(0), buffer.GetAt(1));
        }, (a, b) => a + b, 0);

        angleToAbs = new MetricSEQ<float, float>((data) => {
            return Mathf.Abs(data);
        }, (a, b) => a + b, 0);

        isAgent = gameObject.CompareTag("Player") ? 1 : -1; // agents are tagged "Player"
        BenchmarkUtility.AddInfoCollector(this);
    }

    private void FixedUpdate()
    {
        /*if (iter == 10)
        {
            PositionUpdate(transform.position);
            iter = 0;
        }
        else
            iter++;*/

        PositionUpdate(transform.position);

    }

    public void PositionUpdate(Vector3 position) {
        if (posToVel.Update(position))    //Compute velocity from position
        {
            var vel = posToVel.Next();

            if (velToSpeed.Update(vel)) //Compute speed from velocity
            {
                var speed = velToSpeed.Next();
                
                maxSpeed = Mathf.Max(maxSpeed, speed);

                //print(speed/Time.fixedDeltaTime);

                if (speedToSpeedChange.Update(speed))   //Compute speed change from speed
                {
                    var speedChange = speedToSpeedChange.Next();

                    maxSpeedChange = Mathf.Max(maxSpeedChange, speedChange);
                    var sign = System.Math.Sign(speedChange);
                    speedChangeInflections = System.Math.Sign(speedChangeInflections) == sign ? speedChangeInflections : (Mathf.Abs(speedChangeInflections) + 1) * sign;
                }
                if (speedToMetabolicEnergy.Update(speed)) //Compute kinetic energy from speed
                {
                    
                }
            }
            if (velToAccel.Update(vel)) //Compute acceleration from velocity
            {
                var accel = velToAccel.Next();

                maxAccel = Mathf.Max(maxAccel, accel);
            }
            if (velToAngle.Update(vel)) //Compute angular deviation from velocity
            {
                var ang = velToAngle.Next();

                var sign = System.Math.Sign(ang);
                angleInflections = System.Math.Sign(angleInflections) == sign ? angleInflections : (Mathf.Abs(angleInflections) + 1) * sign;

                if (angleToAbs.Update(ang)) //Compute absolute angular deviation from angular deviation
                {
                    var absAng = angleToAbs.Next();

                    maxAbsAngle = Mathf.Max(maxAbsAngle, absAng);
                }
            }
        }

        totalFrames++;
    }
    
    #region Public Functions

    public void IncrementAgentCollisions()
    {
        agentCollisionCount++;
    }

    public void IncrementObstacleCollisions()
    {
        obstacleCollisionCount++;
    }

    #endregion
}
