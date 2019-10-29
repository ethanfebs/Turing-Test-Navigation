using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DirectorScript : MonoBehaviour
{

    private GameObject selected;
    public Material human;
    public Material agent;

    private Animator[] animatorsInTheScene;

    public TMP_Text EndText;
    float timer = 0;
    float EndTime;
    bool over = false;

    // Start is called before the first frame update
    void Start()
    {
        animatorsInTheScene = FindObjectsOfType(typeof(Animator)) as Animator[];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        if (!over)
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
                            selected.GetComponentInChildren<Renderer>().material = agent;
                        selected = hit.transform.gameObject;
                        selected.GetComponentInChildren<Renderer>().material = human;
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

            if (Input.GetKeyDown(KeyCode.Return))
            {

                over = true;

                EndTime = timer;

                foreach (Animator animatorItem in animatorsInTheScene)
                {
                    animatorItem.GetComponent<Animator>().enabled = false;
                }

                string minutes = Mathf.Floor(EndTime / 60).ToString("0");
                string seconds = Mathf.Floor(EndTime % 60).ToString("00");

                EndText.text = "Total Time: " + minutes + ":" + seconds;

                print(EndTime.ToString());
            }

        }
    }
}
