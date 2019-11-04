using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

    int numPlayers = 9; // total number of players to spawn (including controlled player)
    int spawnDist = 3; // distance between players at spawn

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayers();

        cam = gameObject.GetComponent("Camera") as Camera;
        pControllers = new List<PlayerController>();

        playerArr = GameObject.FindGameObjectsWithTag("Player");
        exitArr = GameObject.FindGameObjectsWithTag("Exit");

        print(playerArr.Length.ToString());

        int dest;

        foreach (GameObject p in playerArr)
        {
            dest = Random.Range(0, 7);

            p.GetComponent<NavMeshAgent>().SetDestination(exitArr[dest].GetComponent<Transform>().position);
        }

        dest = Random.Range(0, 7);

        exitArr[dest].GetComponent<Renderer>().material.SetColor("_Color", Color.red);

        goal = exitArr[dest].GetComponent<Transform>().position;
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
            }
            else
            {
                // Spawn agent
                spawnObject = Instantiate(playerPrefab);
                spawnObject.name = $"Player({i})";
                spawnObject.GetComponent<UnityAnimationRecorder>().fileName = $"Player-{i}-Animation";
                players.Add(spawnObject);
            }

            // Change position of spawn based on list index
            spawnObject.transform.position = new Vector3(row, 0.1f, col) * spawnDist;
        }
    }

    private void Update()
    {
        Transform pTrans;
        Transform p2Trans;
        PlayerController pCont;
        PlayerController p2Cont;

        foreach (GameObject p in playerArr)
        {
            pTrans = p.GetComponent("Transform") as Transform;


            pCont = p.GetComponent("PlayerController") as PlayerController;


            //"Bunching" Prevention Algorithm
            foreach (GameObject p2 in playerArr)
            {
                p2Trans = p.GetComponent("Transform") as Transform;


                p2Cont = p.GetComponent("PlayerController") as PlayerController;

                if (!ReferenceEquals(p, p2))
                {
                    if (Vector3.Distance(p2Cont.agent.destination, pCont.agent.destination) < 2)
                    {
                        if (Vector3.Distance(p2Trans.position, p2Cont.agent.destination) < 2)
                        {
                            p2Cont.setTarget(p2Trans.position);
                        }


                    }
                }
            }
        }

        endGame = Vector3.Distance(controlledTransform.position, goal) < 1;
    }
}
