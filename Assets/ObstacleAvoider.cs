using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoider : MonoBehaviour
{
    GameObject[] obs;

    // Start is called before the first frame update
    void Start()
    {
        obs = GameObject.FindGameObjectsWithTag("obstacles");

        foreach (GameObject ob in obs)
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), ob.GetComponent<Collider>());
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
