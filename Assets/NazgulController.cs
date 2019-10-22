using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NazgulController : PlayerController
{

    //public Camera cam;
    //public NavMeshAgent agent;
    //public Text modeText;

    private void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {


    }

    private void FixedUpdate()
    {

    }

    public void setTarget(Vector3 target)
    {
        agent.SetDestination(target);
    }
}

