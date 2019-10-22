using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public Transform transform;
    public float speed;
    public float speedRot;
    Vector2 rotation;
    Camera cam;

    public ObstacleController oController;
    List<PlayerController> pControllers;

    

    GameObject[] playerArr;
    GameObject[] exitArr;
    bool endGame = false;
    Vector3 goal;

    // Start is called before the first frame update
    void Start()
    {
        cam = gameObject.GetComponent("Camera") as Camera;
        pControllers = new List<PlayerController>();

        playerArr = GameObject.FindGameObjectsWithTag("Player");
        exitArr = GameObject.FindGameObjectsWithTag("Exit");



        print(playerArr.Length.ToString());

        int dest;

        foreach(GameObject p in playerArr)
        {

            dest = (int)Random.Range(0, 7.99f);

            p.GetComponent<NavMeshAgent>().SetDestination(exitArr[dest].GetComponent<Transform>().position);


        }

        dest = (int)Random.Range(0, 7.99f);

        exitArr[dest].GetComponent<Renderer>().material.SetColor("_Color", Color.red);

        goal = exitArr[dest].GetComponent<Transform>().position;
        goal.y += 2.6f;


        //pPositions = new Vector3[6];
        //nPositions = new Vector3[2];
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
        

        if( Vector3.Distance( oController.gameObject.GetComponent<Transform>().position, goal) < 1)
        {
            endGame = true;
        }


        //Detecting where user clicks the mouse
        /*if (Input.GetMouseButtonDown(0))
        {

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {


                if (hit.collider.gameObject.tag.Equals("Enemy") && !obstacleSelected)
                {


                    oController = hit.collider.gameObject.GetComponent("ObstacleController") as ObstacleController;
                    obstacleSelected = true;

                }
                else if (hit.collider.gameObject.tag.Equals("Player"))
                {

                    PlayerController playerController = hit.collider.gameObject.GetComponent("PlayerController") as PlayerController;
                    pControllers.Add(playerController);
                
                }
                else
                {
                    foreach (PlayerController p in pControllers)
                    {
                        p.setTarget(hit.point);
                    }
                    pControllers.Clear();


                }

            }


        }*/



    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //camera movement controls
        /*if (!obstacleSelected)
        {


            if (Input.GetKey("a"))
            {
                transform.Translate(-1*speed,0,0);
            }
            if (Input.GetKey("d"))
            {
                transform.Translate(speed, 0, 0);
            }
            if (Input.GetKey("w"))
            {
                transform.Translate(0, 0, speed);
            }
            if (Input.GetKey("s"))
            {
                transform.Translate(0, 0, -1 * speed);
            }

            if (Input.GetKey(KeyCode.Mouse1))
            {
                rotation.y += Input.GetAxis("Mouse X");
                rotation.x += -Input.GetAxis("Mouse Y");
                transform.eulerAngles = (Vector2)rotation * speedRot;

            }

        }
        else //obstacle movement controlls
        {*/

        if (!endGame)
        {

            if (Input.GetKey("a"))
            {
                oController.Move("LEFT");
            }
            if (Input.GetKey("d"))
            {
                oController.Move("RIGHT");
            }
            if (Input.GetKey("w"))
            {
                oController.Move("UP");
            }
            if (Input.GetKey("s"))
            {
                oController.Move("DOWN");
            }


        }

        //}


    }


}
