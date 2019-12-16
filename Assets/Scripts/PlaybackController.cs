using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaybackController : MonoBehaviour
{
    int numPlayers = 9; // total number of players to spawn (including controlled player)
    int spawnDist = 6; // distance between players at spawn

    private List<GameObject> players;
    private List<Animation> playerAnimComponents;

    public GameObject playbackPrefab;

    // Each animation array represents one scene, the elements in the array
    // represent the animations of players in the scene
    public AnimationClip[][] animations;

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlayers();
    }

    private void SetAnimations(int sceneIndex)
    {
        for (int i = 0; i < players.Count; i++)
        {
            playerAnimComponents[i].clip = animations[sceneIndex][i];
        }
    }

    private void PlayAnimations()
    {
        foreach (var anim in playerAnimComponents)
        {
            anim.Play();
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
            playerAnimComponents.Add(spawnObject.AddComponent<Animation>());
            players.Add(spawnObject);
        }
    }
}
