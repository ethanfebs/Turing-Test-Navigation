using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorScript : MonoBehaviour
{

    private GameObject selected;
    public Material human;
    public Material agent;

    private Animator[] animatorsInTheScene;

    // Start is called before the first frame update
    void Start()
    {
        animatorsInTheScene = FindObjectsOfType(typeof(Animator)) as Animator[];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit/*, 100*/))
            {

                if (hit.transform.tag == "Player")
                {
                    if (selected != null)
                        selected.GetComponent<Renderer>().material = agent;
                    selected = hit.transform.gameObject;
                    selected.GetComponent<Renderer>().material = human;
                }


            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (Animator animatorItem in animatorsInTheScene)
            {
                animatorItem.GetComponent<Animator>().enabled = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (Animator animatorItem in animatorsInTheScene)
            {
                animatorItem.GetComponent<Animator>().enabled = true;
            }
        }
    }
}
