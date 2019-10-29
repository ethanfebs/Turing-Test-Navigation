using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObstacleController : MonoBehaviour
{

    public float speed;
    public Transform transform;
    private Vector3 startPos;
    private int direction;

    // Start is called before the first frame update
    void Start()
    { 
        startPos = transform.position;
        direction = 1;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.Translate(0,0,direction*speed);
        float dist = transform.position.z - startPos.z;
        if ( dist > 9 || dist < -9)
            direction = direction * -1;

    }


}
