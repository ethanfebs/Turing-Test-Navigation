using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public CameraController cam;
    public NavMeshAgent agent;
    public Text modeText;

    private bool isScared;

    private void Start()
    {
        Transform t = gameObject.GetComponent("Transform") as Transform;

        isScared = false;
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

    public void SetScared(bool b)
    {
        isScared = b;
    }

    public bool IsScared()
    {
        return isScared;
    }





}
