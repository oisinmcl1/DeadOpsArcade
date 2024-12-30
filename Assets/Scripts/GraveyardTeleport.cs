using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardTeleport : MonoBehaviour
{
    public string tScene = "Colonial_graveyard";

    public GameObject player;

    // public Vector3 spawnPos = new Vector3(0.48f, -2.69f, -2.45f);
    // public Vector3 spawnPos = new Vector3(-497.62f, 77.01f, -511.9f);
    public Vector3 spawnPos = new Vector3(-2.898f, 6.132f, 20.256f);
    
    public Camera mainCamera;
    
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
        }
    }
}