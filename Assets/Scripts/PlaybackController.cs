using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaybackController : MonoBehaviour
{
    int numPlayers = 9; // total number of players to spawn (including controlled player)
    int spawnDist = 6; // distance between players at spawn

    public int currPlayingExp = -1;
    private int selectedPlayer = -1;

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

    // Start is called before the first frame update
    void Start()
    {
        // Maintain list of animations for all players in each scene
        // K1 is a collection of anims for a scene, K1[0] is the anim for Player(0)
        animations = new List<AnimationClip[]>
        {
            K1, K2, K3, K4, K5, K6, K7, K8, K9, K10
        };

        for (int i = 0; i < animations.Count; i++)
        {
            // Create button
            var newButton = Instantiate(buttonPrefab, menuUI.transform);
            var experimentIndex = i;
            newButton.onClick.AddListener(delegate { SetAnimations(experimentIndex); });
            var butPos = newButton.GetComponent<RectTransform>().position;
            butPos.y -= 50 * i;
            newButton.GetComponent<RectTransform>().position = butPos;
            newButton.GetComponentInChildren<Text>().text = i + 1 + "";
            buttons.Add(newButton);
        }

        SpawnPlayers();
        SetAnimations(K1);
    }

    public void SelectPlayer(int playerIndex)
    {
        if (currPlayingExp >= 0)
        {
            if (selectedPlayer >= 0)
            {
                (players[selectedPlayer].GetComponent("Halo") as Behaviour).enabled = false;
            }

            (players[playerIndex].GetComponent("Halo") as Behaviour).enabled = true;
            selectedPlayer = playerIndex;
        }
    }

    public void DoneChoosing()
    {
        if (selectedPlayer >= 0)
        {
            buttons[currPlayingExp].GetComponentInChildren<Text>().text = "Human = Player " + selectedPlayer;
            buttons[currPlayingExp].image.color = Color.gray;
        }
        currPlayingExp = -1;
        selectedPlayer = -1;
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
