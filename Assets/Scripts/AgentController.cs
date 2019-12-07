using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityAnimationRecorder;

public class AgentController : MonoBehaviour
{
    public Transform controlledTransform;
    Vector2 rotation;
    Camera cam;
    List<PlayerController> pControllers;

    public GameObject controlledPrefab;
    public GameObject playerPrefab;

    GameObject[] playerArr;
    GameObject[] exitArr;
    bool endGame = false;
    Vector3 goal;
    int doneCount = 0;
    int numPlayers = 9; // total number of players to spawn (including controlled player)
    int spawnDist = 6; // distance between players at spawn
    bool[] doneArr;
    bool noAnimations = true;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayers();

        cam = gameObject.GetComponent("Camera") as Camera;
        pControllers = new List<PlayerController>();

        playerArr = GameObject.FindGameObjectsWithTag("Player");
        exitArr = GameObject.FindGameObjectsWithTag("Exit");
        doneArr = new bool[9];

        for(int x = 0; x < doneArr.Length; x++)
        {
            doneArr[x] = false;
        }

        print("length: "+playerArr.Length.ToString());

        int dest;

        foreach (GameObject p in playerArr)
        {
            dest = Random.Range(0, 7);

            p.GetComponent<NavMeshAgent>().SetDestination(exitArr[0].GetComponent<Transform>().position);
        }

        dest = Random.Range(0, 7);

        exitArr[0].GetComponent<Renderer>().material.SetColor("_Color", Color.red);

        goal = exitArr[0].GetComponent<Transform>().position;
        goal.y += 2.6f;

        //pPositions = new Vector3[6];
        //nPositions = new Vector3[2];
    }

    // Spawns single controlled player in group of agents at center of map
    private void SpawnPlayers()
    {
        int square = (int)Mathf.Sqrt(numPlayers);
        var players = new List<GameObject>(); // replace Player array?
        //int startPos = 4;
        GameObject spawnObject;
        int startPos = Random.Range(0, numPlayers);

        for (int i = 0; i < numPlayers; i++)
        {
            int row = (i / square) - square / 2;
            int col = (i % square) - square / 2;

            if (i == startPos)
            {
                // Spawn controlled player at start position
                spawnObject = Instantiate(controlledPrefab);
                controlledTransform = spawnObject.transform;
                spawnObject.transform.position = new Vector3(row, 0f, col) * spawnDist;
                spawnObject.GetComponent<UnityAnimationRecorder>().fileName = $"Player-{i}-Animation_" + System.DateTime.Now.ToString("MMddyy_Hmmss");
                spawnObject.GetComponent<UnityAnimationRecorder>().StartRecording();
                print("HUMAN CONTROLLED PLAYER: " + i);
            }
            else
            {
                // Spawn agent
                spawnObject = Instantiate(playerPrefab);
                spawnObject.name = $"Player({i})";
                spawnObject.GetComponent<UnityAnimationRecorder>().fileName = $"Player-{i}-Animation_" + System.DateTime.Now.ToString("MMddyy_Hmmss");
                spawnObject.transform.position = new Vector3(row, 0f, col) * spawnDist + new Vector3(0,0.0f,0);
                players.Add(spawnObject);
                spawnObject.GetComponent<UnityAnimationRecorder>().StartRecording();
            }

            spawnObject.AddComponent<InfoCollector>();
            
        }
    }

    private void FixedUpdate()
    {
        Transform pTrans;
        Transform p2Trans;
        PlayerController pCont;
        PlayerController p2Cont;

        print("SPEED: " + Input.GetAxis("Vertical"));

        controlledTransform.gameObject.GetComponent<Animator>().SetFloat("Vertical", Input.GetAxis("Vertical") );

        for (int x = 0; x < playerArr.Length; x++)
        {
            GameObject p = playerArr[x];

            pTrans = p.GetComponent("Transform") as Transform;


            pCont = p.GetComponent("PlayerController") as PlayerController;

            if (p.GetComponent<NavMeshAgent>() != null)
            {
                p.GetComponent<Animator>().SetFloat("Vertical", Vector3.Magnitude(p.GetComponent<NavMeshAgent>().velocity));
            }
            else if (p.GetComponent<RRTAlgo>() != null)
            {
                p.GetComponent<Animator>().SetFloat("Vertical", p.GetComponent<RRTAlgo>().speed);
            }
            else Debug.Log("The Animator could not animate: speed needs to be designated");

            if (pTrans.position.z > 47 && pTrans.position.z < 53 && pTrans.position.x > -3 && pTrans.position.x < 3) {

                BenchmarkUtility.StopInfoCollector(p.GetComponent<InfoCollector>());

                if (p.GetComponent<NavMeshAgent>() != null) p.GetComponent<NavMeshAgent>().enabled = false;
                //p.GetComponent<UnityAnimationRecorder>().StopRecording();
                //p.SetActive(false);


                p.transform.position = new Vector3(-500+doneCount*5, 0, 0);
                doneArr[x] = true;
                //pCont.setTarget(pTrans.position + new Vector3(0, 0, 20));
                doneCount++;
                //print(doneCount);

                
            }

            //"Bunching" Prevention Algorithm
            foreach (GameObject p2 in playerArr)
            {
                p2Trans = p.GetComponent("Transform") as Transform;


                p2Cont = p.GetComponent("PlayerController") as PlayerController;

                /*if (!ReferenceEquals(p, p2))
                {
                    if (Vector3.Distance(p2Cont.agent.destination, pCont.agent.destination) < 2)
                    {

                        

                        if (Vector3.Distance(p2Trans.position, p2Cont.agent.destination) < 2)
                        {
                            p2Cont.setTarget(p2Trans.position);
                        }


                    }
                }*/
            }
        }

        //print("cont pos: " + controlledTransform.position.ToString());

        if (controlledTransform.position.z > 47 && controlledTransform.position.z < 53 && controlledTransform.position.x > -3 && controlledTransform.position.x < 3)
        {
            controlledTransform.gameObject.GetComponent<FirstPersonAIO>().playerCanMove = false;
            controlledTransform.gameObject.GetComponentInChildren<CapsuleCollider>().enabled = false;
            //controlledTransform.gameObject.GetComponentInChildren<UnityAnimationRecorder>().StopRecording();
            //controlledTransform.gameObject.SetActive(false);
            controlledTransform.position = new Vector3(-500+doneCount*5, -2, 0);
            doneArr[8] = true;
            doneCount++;
            //print(doneCount);
            BenchmarkUtility.StopInfoCollector(controlledTransform.gameObject.GetComponent<InfoCollector>());
        }


        bool allDone = true;

        for(int x = 0; x < 9; x++)
        {
            if (!doneArr[x]) { allDone = false; }
            //print(x + " is " + doneArr[x]);
        }

        /*foreach (bool b in doneArr)
        {

            if (!b) { allDone = false; }
        }*/

        if (allDone && noAnimations)
        {

            print("BIG DAY");

            noAnimations = false;

            foreach (GameObject p in playerArr)
            {
                p.GetComponent<UnityAnimationRecorder>().StopRecording();
            }

            controlledTransform.gameObject.GetComponent<UnityAnimationRecorder>().StopRecording();

            BenchmarkUtility.ComputeStatistics();

        }

        //endGame = Vector3.Distance(controlledTransform.position, goal) < 1;
    }
}
