using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public bool isMovable = false;
    public float speed;
    public float speed_rot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
  
    }

    public void Move(string str)
    {
        switch (str)
        {
            case "RIGHT":
                transform.Rotate(0, 1*speed_rot, 0);
                break;
            case "LEFT":
                transform.Rotate(0, -1*speed_rot, 0);
                break;
            case "UP":
                transform.Translate(0, 0, speed, Space.Self);
                break;
            case "DOWN":
                transform.Translate(0, 0, -1 * speed, Space.Self);
                break;
        }
    }
}
