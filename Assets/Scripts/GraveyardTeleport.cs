using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveyardTeleport : MonoBehaviour
{
    public string tScene = "Colonial_graveyard";

    public GameObject player;
    private CharacterScript cs;

    // public Vector3 spawnPos = new Vector3(0.48f, -2.69f, -2.45f);
    // public Vector3 spawnPos = new Vector3(-497.62f, 77.01f, -511.9f);
    public Vector3 spawnPos = new Vector3(-2.898f, 6.132f, 20.256f);
    
    public Camera mainCamera;
    
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
            cs.onIsland = true;
            
            player.transform.position = spawnPos;

            mainCamera.transform.position = spawnPos + new Vector3(0f, 10f, 0f);
            
            // change gm scripts map enum and teleport zombies
            GameManagerScript gms = FindObjectOfType<GameManagerScript>();
            gms.currentMap = GameManagerScript.map.Graveyard;
            
            // move any alive zombies to other map
            gms.moveZombiesTo(spawnPos);
            
            Debug.Log("Island boundry: " + cs.onIsland);
        }
    }
}