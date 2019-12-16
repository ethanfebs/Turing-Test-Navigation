using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionController : MonoBehaviour
{
    private PlaybackController pc;

    private void Start()
    {
        pc = GetComponent<PlaybackController>();
    }

    void Update()
    {
        if (pc.currPlayingExp >= 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    var hitGO = hit.transform.gameObject;
                    Debug.Log(hitGO.name);
                    var selected = pc.players.SingleOrDefault(x => x.name.Equals(hitGO.name));
                    if (selected != null)
                    {
                        pc.SelectPlayer(pc.players.IndexOf(selected));
                    }
                }
            }
        }
    }
}
