using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IslandTeleport : MonoBehaviour
{
    // public string tScene = "Colonial_graveyard";
    public GameObject player;
    private CharacterScript cs;
    // public Vector3 spawnPos = new Vector3(0.48f, -2.69f, -2.45f);
    public Vector3 spawnPos = new Vector3(-497.62f, 77.01f, -511.9f);
    public Camera mainCamera;
    public GameManagerScript gm;

    public int portalCost;
    private bool portalPurchased = false;

    public GameObject portalTextObject;
    // private TMP_Text portalText;
    
    // teleport camera too + 10f on y axis
    void Start()
    {
        // DontDestroyOnLoad(mainCamera);
        // DontDestroyOnLoad(player);
        
        player = GameObject.FindGameObjectWithTag("Player");
        cs = player.GetComponent<CharacterScript>();
        gm = FindObjectOfType<GameManagerScript>();
        portalTextObject.SetActive(false);
        
        portalCost = 3000;

        // islandportaltxt
        // portalText = portalTextObject.GetComponent<TMP_Text>();
    }
    
    void Update()
    {
        // display text if portal not purchased
        if (!portalPurchased)
        {
            // If player is in range, show the text
            if (isPlayerInRange())
            {
                displayPortalText();

                // If player presses E, purchase the portal
                if (Input.GetKeyDown(KeyCode.E))
                {
                    purchasePortal();
                }
            }
            else
            {
                // If player is not in range, hide the text
                portalTextObject.SetActive(false);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (portalPurchased)
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
    }

    public void purchasePortal()
    {
        if (gm.points >= portalCost)
        {
            Debug.Log("Purchased portal: " + portalCost);
            gm.removePoints(portalCost);
            portalPurchased = true;
            portalTextObject.SetActive(false);
        }
    }

    public bool isPlayerInRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        
        if (distanceToPlayer <= 5f)
        {
            // Debug.Log("Player is in range of portal");
            return true;
        }
        return false;
    }

    public void displayPortalText()
    {
        // portalTextObject = GameObject.FindGameObjectWithTag("islandportaltxt");
        portalTextObject.SetActive(true);
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
