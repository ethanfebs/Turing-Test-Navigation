using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class DirectorScript : MonoBehaviour
{

    private GameObject selected;
    public Material human;
    public Material agent;
    public int humanControlled;

    private Animator[] animatorsInTheScene;
    private Animation[] animationsInTheScene;

    public TMP_Text EndText;
    float timer = 0;
    float EndTime;
    bool over = false;

    // Start is called before the first frame update
    void Start()
    {
        animatorsInTheScene = FindObjectsOfType(typeof(Animator)) as Animator[];
        animationsInTheScene = FindObjectsOfType(typeof(Animation)) as Animation[];

        //print("HAH" + animationsInTheScene.Length);

        //animationsInTheScene[0].GetClip("Player-0-Animation").legacy = true;

        foreach (Animation animationItem in animationsInTheScene)
        {

            animationItem.clip.legacy = true;
            animationItem.clip.wrapMode = WrapMode.Loop;
            Transform t = animationItem.gameObject.GetComponentsInChildren<Transform>()[1];
            //print("name " + animationItem.clip.name);
            if (animationItem.clip.name.Equals($"Player-{humanControlled}-Animation"))
            {
                //print("pos of human "+t.position.ToString());
                t.localPosition = new Vector3(0, 1, 0);
            }
            else
            {
                //print("pos "+t.position.ToString());
                t.localPosition = new Vector3(0, 0, 0);
            }


            if (animationItem.clip.ToString().Equals("Player-Animation (UnityEngine.AnimationClip)"))
            {
                //print("lol");
                //print(animationItem.GetComponentInParent<Transform>().localScale.ToString());

            }



            //print(animationItem.clip.ToString());
            //print(animationItem.clip.isLooping.ToString());
        }
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
                /*foreach (Animator animatorItem in animatorsInTheScene)
                {
                    animatorItem.GetComponent<Animator>().enabled = false;
                }*/

                foreach (Animation animationItem in animationsInTheScene)
                    {


                    if (animationItem.enabled)
                    {
                        animationItem.enabled = false;
                    }
                    else
                    {
                        animationItem.enabled = true;
                    }
                    //animationItem.enabled = !animationItem.enabled;
                    //animationItem.Stop();
                    //animationItem.Rewind();

                    }


            }

            /*if (Input.GetKeyDown(KeyCode.A))
            {
                foreach (Animator animatorItem in animatorsInTheScene)
                {
                    animatorItem.GetComponent<Animator>().enabled = true;
                }
            }*/

            if (Input.GetKeyDown(KeyCode.Return))
            {

                over = true;

                EndTime = timer;

                /*foreach (Animator animatorItem in animatorsInTheScene)
                {
                    animatorItem.GetComponent<Animator>().enabled = false;
                }*/

                foreach (Animation animation in animationsInTheScene)
                {
                    animation.enabled = false;
                }

                string minutes = Mathf.Floor(EndTime / 60).ToString("0");
                string seconds = Mathf.Floor(EndTime % 60).ToString("00");

                EndText.text = "Time: " + minutes + ":" + seconds + " for "+ selected.name;
                //print(EndTime.ToString());
            }

        }
    }
}
