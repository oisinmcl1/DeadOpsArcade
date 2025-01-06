using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyRifle : MonoBehaviour
{
    public GameObject player;
    private Shooter shooter;
    private GameManagerScript gm;
    public GameObject rifleText;
    private bool isPurchased;
    private bool playerClose;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        shooter = player.GetComponentInChildren<Shooter>();
        gm = FindObjectOfType<GameManagerScript>();
        rifleText.SetActive(false);
        playerClose = false;
    }

    // Update is called once per frame
    void Update()
    {
        // if player hasnt purchased and is in trigger purchase with e key
        if (!isPurchased && playerClose)
        {
            rifleText.SetActive(true);
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                purchaseRifle();
            }
        }
        
        else
        {
            rifleText.SetActive(false);
        }
    }

    // used to display text when player in trigger of purchasable gun
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerClose = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerClose = false;
        }
    }

    private void purchaseRifle()
    {
        if (gm.purchaseWeapon(2))
        {
            Debug.Log("Purchased Rifle");
            isPurchased = true;
            
            rifleText.SetActive(false);
        }
    }
}