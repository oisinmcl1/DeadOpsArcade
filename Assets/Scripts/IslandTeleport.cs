using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandTeleport : MonoBehaviour
{
    public string tScene = "Colonial_graveyard";
    public GameObject player;
    // public Vector3 spawnPos = new Vector3(0.48f, -2.69f, -2.45f);
    public Vector3 spawnPos = new Vector3(-497.62f, 77.01f, -511.9f);
    public Camera mainCamera;
    
    // teleport camera too + 10f on y axis
    void Start()
    {
        // DontDestroyOnLoad(mainCamera);
        // DontDestroyOnLoad(player);
    }
    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player.transform.position = spawnPos;
            
            mainCamera.transform.position = spawnPos + new Vector3(0f, 10f, 0f);
            
            // p transform = target tansofmr pos
            
            
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
