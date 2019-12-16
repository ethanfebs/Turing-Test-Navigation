using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlaybackController : MonoBehaviour
{
    int numPlayers = 9; // total number of players to spawn (including controlled player)
    int spawnDist = 6; // distance between players at spawn

    public int currPlayingExp = -1;
    private List<int> selectedPlayer;

    public List<GameObject> players = new List<GameObject>();
    private List<Button> buttons = new List<Button>();

    public GameObject canvas;
    public GameObject menuUI;
    public GameObject experimentUI;
    public GameObject playbackPrefab;
    public Button buttonPrefab;

    // Each animation array represents one scene, the elements in the array
    // represent the animations of players in the scene
    public List<AnimationClip[]> animations = new List<AnimationClip[]>();
    public AnimationClip[] K1;
    public AnimationClip[] K2;
    public AnimationClip[] K3;
    public AnimationClip[] K4;
    public AnimationClip[] K5;
    public AnimationClip[] K6;
    public AnimationClip[] K7;
    public AnimationClip[] K8;
    public AnimationClip[] K9;
    public AnimationClip[] K10;
    public AnimationClip[] A1;
    public AnimationClip[] A2;
    public AnimationClip[] A3;
    public AnimationClip[] A4;
    public AnimationClip[] A5;
    public AnimationClip[] A6;
    public AnimationClip[] A7;
    public AnimationClip[] A8;
    public AnimationClip[] A9;
    public AnimationClip[] A10;
    public AnimationClip[] F1;
    public AnimationClip[] F2;
    public AnimationClip[] F3;
    public AnimationClip[] F4;
    public AnimationClip[] F5;
    public AnimationClip[] F6;
    public AnimationClip[] F7;
    public AnimationClip[] F8;
    public AnimationClip[] F9;
    public AnimationClip[] F10;

    float timer = 0;
    float EndTime;

    // Start is called before the first frame update
    void Start()
    {
        // Maintain list of animations for all players in each scene
        // K1 is a collection of anims for a scene, K1[0] is the anim for Player(0)
        animations = new List<AnimationClip[]>
        {
            K1, K2, K3, K4, K5, K6, K7, K8, K9, K10, A1, A2, A3, A4, A5, A6, A7, A8, A9, A10, F1, F2, F3, F4, F5, F6, F7, F8, F9, F10
        };

        for (int i = 0; i < animations.Count; i++)
        {
            // Create button
            var newButton = Instantiate(buttonPrefab, menuUI.transform);
            var experimentIndex = i;
            newButton.onClick.AddListener(delegate { SetAnimations(experimentIndex); });
            var butPos = newButton.GetComponent<RectTransform>().position;
            butPos.y -= 20 * (i % 10);
            butPos.x += 300 * (i / 10);
            newButton.GetComponent<RectTransform>().position = butPos;
            newButton.GetComponentInChildren<Text>().text = i + 1 + "";
            buttons.Add(newButton);
        }

        selectedPlayer = new List<int>();

        SpawnPlayers();
        SetAnimations(K1);
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
    }

    public void SelectPlayer(int playerIndex)
    {
        if (currPlayingExp >= 0)
        {
            if (selectedPlayer.Contains(playerIndex))
            {
                (players[playerIndex].GetComponent("Halo") as Behaviour).enabled = false;
                selectedPlayer.Remove(playerIndex);
            }
            else
            {
                (players[playerIndex].GetComponent("Halo") as Behaviour).enabled = true;
                selectedPlayer.Add(playerIndex);
            }

        }
    }

    public void DoneChoosing()
    {
        EndTime = timer;
        Debug.Log(selectedPlayer);
        if (selectedPlayer.Count > 0)
        {
            string humans = "Human(s): " + selectedPlayer.ElementAt(0).ToString();
            (players[selectedPlayer.ElementAt(0)].GetComponent("Halo") as Behaviour).enabled = false;
            for (int i = 1; i < selectedPlayer.Count; i++)
            {
                humans = humans + ", " + selectedPlayer.ElementAt(i).ToString();
                (players[selectedPlayer.ElementAt(i)].GetComponent("Halo") as Behaviour).enabled = false;
            }

            buttons[currPlayingExp].GetComponentInChildren<Text>().text = humans + " " + EndTime;
            buttons[currPlayingExp].image.color = Color.gray;

        }
        else
        {
            buttons[currPlayingExp].GetComponentInChildren<Text>().text = "No Humans";
            buttons[currPlayingExp].image.color = Color.gray;
        }
        currPlayingExp = -1;
        selectedPlayer = new List<int>();
        experimentUI.SetActive(false);
        menuUI.SetActive(true);
    }

    // Called by buttons
    public void SetAnimations(int experimentNumber)
    {
        currPlayingExp = experimentNumber;
        menuUI.SetActive(false);
        SetAnimations(animations[experimentNumber]);
        experimentUI.SetActive(true);
    }

    private void SetAnimations(AnimationClip[] animationSet)
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetActive(false);
            animationSet[i].legacy = true;
            animationSet[i].wrapMode = WrapMode.Loop;
            players[i].GetComponent<Animation>().clip = animationSet[i];
            players[i].SetActive(true);
        }
    }

    private void PlayAnimations()
    {
        foreach (var player in players)
        {
            if (!player.GetComponent<Animation>().Play())
            {
                Debug.Log("Animation for " + name + "could not be played.");
            }
        }
    }

    // Spawns single controlled player in group of agents at center of map
    private void SpawnPlayers()
    {
        int square = (int)Mathf.Sqrt(numPlayers);
        //int startPos = 4;
        GameObject spawnObject;

        for (int i = 0; i < numPlayers; i++)
        {
            int row = (i / square) - square / 2;
            int col = (i % square) - square / 2;

            // Spawn agent
            spawnObject = Instantiate(playbackPrefab);
            spawnObject.name = $"Player({i})";
            spawnObject.transform.position = new Vector3(row, 0f, col) * spawnDist + new Vector3(0, 0.0f, 0);
            //spawnObject.AddComponent<Animation>();
            players.Add(spawnObject);
        }
    }
}
