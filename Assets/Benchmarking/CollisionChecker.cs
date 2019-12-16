using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var ic = GetComponent<InfoCollector>();

        // Ensure info collector is available
        if (ic == null)
        {
            Debug.Log("Could not find InfoCollector for " + this.name);
            return;
        }

        // Identify if other is Player or Obstacle
        if (other.CompareTag("Player") || other.CompareTag("ControlledPlayer"))
        {
            ic.IncrementAgentCollisions();
        }
        else ic.IncrementObstacleCollisions();
    }
}
