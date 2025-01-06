using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManagerScript : MonoBehaviour
{
    public enum gameState
    {
        Menu,
        Playing
    };

    public gameState currentState = gameState.Menu;
    public static Vector3 screenBottomLeft, screenTopRight;
    public static float screenWidth, screenHeight;
    public static GameManagerScript instance;
    public GameObject character;
    public GameObject characterPrefab;
    public GameObject zombie;
    public int round;
    private int highestRound;
    private int baseAmtZombies;
    private float zombHealthIncr;
    // had to use global list to keep track of zombies because it kept counting one more
    private List<GameObject> zombies = new List<GameObject>();
    private GameObject roundTextObj;
    private GameObject highestRoundTextObj;
    private TMP_Text roundText;
    private TMP_Text highestRoundText;
    public int points;
    private GameObject pointTextObj;
    private TMP_Text pointText;
    private CharacterScript cs;
    private Shooter shooter;
    
    // Island and Graveyard boundries
    public Vector3 islandTopLeft = new Vector3(-20.97f, 6.38322163f, 16.62f);
    public Vector3 islandBottomRight = new Vector3(20.5f, 6.38322163f, -18.6f);
    public Vector3 graveyardTopLeft = new Vector3(-529.3f, 86.0851746f, -486.5f);
    public Vector3 graveyardBottomRight = new Vector3(-481.4f, 86.0851746f, -516.8f);

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            highestRound = 0;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // testPurchase(2);
    }
    
    // keep track of where the player is
    public enum map
    {
        Island,
        Graveyard
    }
    public map currentMap;

    public void onStartGameButtonClick()
    {
        changeScene(gameState.Playing);
        Debug.Log("Button clicked!");
    }

    private void changeScene(gameState newGameState)
    {
        currentState = newGameState;
        switch (newGameState)
        {
            case gameState.Menu:
                Debug.Log("switching to menu");
                SceneManager.LoadScene("MenuScene");
                SceneManager.sceneLoaded += OnSceneLoaded;
                break;

            case gameState.Playing:
                Debug.Log("switching to game");
                SceneManager.LoadScene("GameScene");
                SceneManager.sceneLoaded += OnSceneLoaded; // Attach the event for scene load
                break;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the "GameScene" is loaded and game state is Playing, start the game
        if (scene.name == "GameScene" && currentState == gameState.Playing)
        {
            Debug.Log("calling startGame()");
            startGame();
            Debug.Log("GameScene loaded, calling startGame()");
        }
        // If the "MenuScene" is loaded, display the high score
        else if (scene.name == "MenuScene" && currentState == gameState.Menu)
        {
            Button startButton = GameObject.Find("StartButton").GetComponent<Button>();
            if (startButton != null)
            {
                startButton.onClick.AddListener(onStartGameButtonClick);
                Debug.Log("StartButton's OnClick listener added.");
                
                // find highscore text and set it
                highestRoundTextObj = GameObject.FindGameObjectWithTag("highestroundtxt");
                highestRoundText = highestRoundTextObj.GetComponent<TMP_Text>();
                highestRoundText.text = highestRound.ToString();
            }
            else
            {
                Debug.LogError("StartButton not found in MenuScene.");
            }
        }

        // Detach event to prevent duplicate calls
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void startGame()
    {
        Debug.Log("Starting Game");
        // Assigning values to screen position variables
        screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 30f));
        screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 30f));
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.z - screenBottomLeft.z;

        // Spawn the character
        spawnCharacter();
        cs = character.GetComponent<CharacterScript>();
        cs.onIsland = true;
        
        round = 0;
        baseAmtZombies = 2;
        zombHealthIncr = 0.5f;
        
        // since gm is swapping scenes i have to find most components programatically
        roundTextObj = GameObject.FindGameObjectWithTag("roundtxt");
        roundText = roundTextObj.GetComponent<TMP_Text>();

        points = 0;
        pointTextObj = GameObject.FindGameObjectWithTag("pointstxt");
        pointText = pointTextObj.GetComponent<TMP_Text>();
        
        startNextRound();
    }

    private void spawnCharacter()
    {
        // Instantiate and assign to character variable
        character = Instantiate(characterPrefab);
        
        // get camera script for character
        CameraScript camScript = Camera.main.GetComponent<CameraScript>();
        camScript.player = character;
        
        // get shooter script
        shooter = character.GetComponentInChildren<Shooter>();
        
        /*if (shooter == null)
        {
            Debug.Log("shooter is null");
        }
        else
        {
            Debug.Log("shooter not null");
        }*/
    }


    public void gameOver()
    {
        Debug.Log("Game Over!");
        changeScene(gameState.Menu);

        // Destroy all game objects with Rigidbody components
        foreach (Rigidbody rb in GameObject.FindObjectsOfType<Rigidbody>())
        {
            Destroy(rb.gameObject);
        }
    }

    public void startNextRound()
    {
        // incrase round
        round++;

        Debug.Log("Round " + round);
        roundText.text = round.ToString();
        
        if (round > highestRound)
        {
            highestRound = round;
        }
        
        Vector3 topLeft, bottomRight;
            
        if (cs.onIsland)
        {
            topLeft = islandTopLeft;
            bottomRight = islandBottomRight;
        }
        else
        {
            topLeft = graveyardTopLeft;
            bottomRight = graveyardBottomRight;
        }

        // spawn zombies depending on round
        int amtZombies = baseAmtZombies * round;
        
        for (int i = 0; i < amtZombies; i++)
        {
            // instantiate new zombie
            GameObject newZombie = Instantiate(zombie);
            
            // increase health of zombie
            float newHealth = newZombie.GetComponent<zombieScript>().health + zombHealthIncr;
            newZombie.GetComponent<zombieScript>().setHealth(newHealth);
            
            // increase damage of zombie by .5 every 5 rounds
            if (round % 5 == 0)
            {
                float newDamage = newZombie.GetComponent<zombieScript>().damage + 0.5f;
                newZombie.GetComponent<zombieScript>().damage = newDamage;
            }
            
            float x, z;

            // lr edge
            if (Random.value < 0.5f)
            {
                x = topLeft.x + Random.value * 0.15f * (bottomRight.x - topLeft.x);
            }
            else
            {
                x = bottomRight.x - Random.value * 0.15f * (bottomRight.x - topLeft.x);
            }

            // tb edge
            if (Random.value < 0.5f)
            {
                z = topLeft.z - Random.value * 0.15f * (topLeft.z - bottomRight.z);
            }
            else
            {
                z = bottomRight.z + Random.value * 0.15f * (topLeft.z - bottomRight.z);
            }
            
            // same y as player
            float y = character.transform.position.y;
            
            newZombie.transform.position = new Vector3(x, y, z);
            Debug.Log("Spawned zombie at: " + newZombie.transform.position);
            
            zombies.Add(newZombie);
        }
    }
    
    // Remove all zombies
    public void removeZombies()
    {
        /*GameObject[] allZombies = GameObject.FindGameObjectsWithTag("zombie");
        foreach (GameObject zombie in allZombies)
        {
            Destroy(zombie);
        }*/

        // iterate through zombies destroy and remove from list
        for (int i = 0; i < zombies.Count; i++)
        {
            Destroy(zombies[i]);
            zombies.RemoveAt(i);
        }
    }

    public void afterPlayerDies()
    {
        // Remove all zombies and pan camera out using coroutine
        removeZombies();

        StartCoroutine(cameraPanToMenu());
    }

    IEnumerator cameraPanToMenu()
    {
        // get camscript comp and set follow to false
        CameraScript camScript = Camera.main.GetComponent<CameraScript>();
        camScript.follow = false;
        
        // call cameras pan method using coroutine to pan camera out and return to menu after 6 seconds
        yield return StartCoroutine(camScript.pan(6f));
        
        gameOver();
    }
    
    public void zombieDead(GameObject zombie)
    {
        Destroy(zombie);
        zombies.Remove(zombie);
        
        // find amt zombies
        /*GameObject[] allZombies = GameObject.FindGameObjectsWithTag("zombie");
        int amtZombies = allZombies.Length;*/
        
        int amtZombies = zombies.Count;

        addPoints(100);
        
        Debug.Log("Zombies left in round: " + amtZombies);
        
        if (amtZombies == 0)
        {
            startNextRound();
        }
    }
    
    public void addPoints(int amt)
    {
        points += amt;
        pointText.text = points.ToString();
    }
    
    public void removePoints(int amt)
    {
        points -= amt;
        
        if (points < 0)
        {
            points = 0;
        }
        
        pointText.text = points.ToString();
    }

    public void moveZombiesTo(Vector3 pos)
    {
        // teleport all zombies to new map randomly
        for (int i = 0; i < zombies.Count; i++)
        {
            Vector3 random = new Vector3(
                Random.Range(-5f, 5f),
                0f,
                Random.Range(-5f, 5f)
            );

            zombies[i].transform.position = pos + random;
        }
    }
    
    public bool purchaseWeapon(int i)
    {
        // if weapon is already owned and player has enough points
        if ((!shooter.owned[i]) && points >= shooter.prices[i])
        {
            // remove points and set weapon to owned
            removePoints(shooter.prices[i]);
            shooter.owned[i] = true;
            
            shooter.EquipWeapon(i);
            
            return true;
        }
        
        return false;
    }

    public void testPurchase(int i)
    {
        if (i < 0 || i >= shooter.prices.Length)
        {
            Debug.Log("Invalid index");
            return;
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Attempting to purchase " + i);
            Debug.Log("Weapon owned: " + shooter.owned[i]);
            Debug.Log("Points: " + points);
            Debug.Log("Price: " + shooter.prices[i]);
            
            if (points >= shooter.prices[i])
            {
                Debug.Log("Player has enough points");
            }
            if (points < shooter.prices[i])
            {
                Debug.Log("Player does not have enough points");
            }
            
            if (purchaseWeapon(i)) 
            {
                    Debug.Log("Purchased weapon"); 
            }
        }
    }
}