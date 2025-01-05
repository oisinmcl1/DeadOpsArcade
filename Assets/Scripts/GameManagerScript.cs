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

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
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
    }

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
        
        round = 0;
        highestRound = 0;
        baseAmtZombies = 2;
        zombHealthIncr = 0.5f;
        
        // since gm is swapping scenes i have to find most components programatically
        roundTextObj = GameObject.FindGameObjectWithTag("roundtxt");
        roundText = roundTextObj.GetComponent<TMP_Text>();
        highestRoundTextObj = GameObject.FindGameObjectWithTag("highestroundtxt");
        highestRoundText = highestRoundTextObj.GetComponent<TMP_Text>();
        
        startNextRound();
    }

    private void spawnCharacter()
    {
        // Instantiate and assign to character variable
        character = Instantiate(character);
        
        // get camera script for character
        CameraScript camScript = Camera.main.GetComponent<CameraScript>();
        camScript.player = character;
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
            highestRoundText.text = highestRound.ToString();
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
            
            // spawn zombies randomly
            float x, z;

            // lr edege
            if (Random.value < 0.5f)
                x = screenBottomLeft.x + Random.value * 0.15f * screenWidth;
            else
                x = screenTopRight.x - Random.value * 0.15f * screenWidth;

            // bt edge
            if (Random.value < 0.5f)
                z = screenBottomLeft.z + Random.value * 0.15f * screenHeight;
            else
                z = screenTopRight.z - Random.value * 0.15f * screenHeight;

            // zombie was emerging from ground but didnt look right
            float y = character.transform.position.y;
            
            newZombie.transform.position = new Vector3(x, y, z);
            zombies.Add(newZombie);
            Debug.Log("Spawned zombie at: " + newZombie.transform.position);
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
        
        Debug.Log("Zombies left in round: " + amtZombies);
        
        if (amtZombies == 0)
        {
            startNextRound();
        }
    }
}