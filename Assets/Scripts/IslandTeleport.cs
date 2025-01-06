using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandTeleport : MonoBehaviour
{
    public string tScene = "Colonial_graveyard";
    public GameObject player;
    private CharacterScript cs;
    // public Vector3 spawnPos = new Vector3(0.48f, -2.69f, -2.45f);
    public Vector3 spawnPos = new Vector3(-497.62f, 77.01f, -511.9f);
    public Camera mainCamera;
    
    // teleport camera too + 10f on y axis
    void Start()
    {
        // DontDestroyOnLoad(mainCamera);
        // DontDestroyOnLoad(player);
        
        player = GameObject.FindGameObjectWithTag("Player");
        cs = player.GetComponent<CharacterScript>();
    }
    
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // disable the boundry ontriggerexit that kills player if leaving the island
            cs.onIsland = false;
            
            player.transform.position = spawnPos;
            
            mainCamera.transform.position = spawnPos + new Vector3(0f, 10f, 0f);
            
            // change gm scripts map enum and teleport zombies
            GameManagerScript gms = FindObjectOfType<GameManagerScript>();
            gms.currentMap = GameManagerScript.map.Island;
            
            // move any alive zombies to other map
            gms.moveZombiesTo(spawnPos);
            
            Debug.Log("Island boundry: " + cs.onIsland);

            // if (player == null)
            // {
            //     // player = other.gameObject;
            //     // DontDestroyOnLoad(player);
            // }

            // SceneManager.sceneLoaded += OnSceneLoaded;
            // SceneManager.LoadScene(tScene);
        }
    }


    /*private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        if (player != null)
        {
            player.transform.position = spawnPos;
            Debug.Log("Player moved to spawn position: " + spawnPos);
        }
        else
        {
            Debug.LogError("Player object is null after scene load!");
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }*/
}
