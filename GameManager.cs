using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// manages game states, UI, and acts as a glue for the project
// completely handles config, does not affect robot control 
public class GameManager : MonoBehaviour
{ 
    [Header("Menu")] // menu objects
    [SerializeField] Canvas menuHUD;

    [Header("Server")] // server hud objects (unused)
    [SerializeField] Canvas serverHUD;

    [Header("Settings")] // settings hud objects (settings tab unused)
    [SerializeField] Canvas settingsHUD;
    [SerializeField] Button audioToggleButton;
    [SerializeField] Button extrasToggleButton;
    [SerializeField] bool audioOn = true;
    [SerializeField] bool extrasOn = false;

    [Header("Game")] // game hud objects 
    [SerializeField] Canvas gameHUD; // game hud reference
    [SerializeField] Canvas endHUD; // end hud reference
    [SerializeField] TextMeshProUGUI scoreDisplayer; // displays score during game
    [SerializeField] TextMeshProUGUI timeDisplayer; // displays remaining time during ame
    [SerializeField] float timer; //current time (dynamic)
    [SerializeField] float betweenTimer; // the amount of time between game states - autonomous and teleop
    [SerializeField] char gameState; // populated with autonomous, between, teleop or over.

    [SerializeField] GameObject robotReference; // reference to robot object in scene
    [SerializeField] GameObject armPrefab; // unused
            
    [Header("Cameras")] // camera views
    [SerializeField] GameObject viewCameraReference; // driver view (what competitors see during competition). this object is an empty object that camera parents to
    [SerializeField] GameObject topDownCameraReference; // top down view - parent empty
    [SerializeField] GameObject onRobotCameraReference; // robot - camera parent empty
    [SerializeField] char currentCamPos; // current cam pos between the three listed above

    [Header("Misc")]
    [SerializeField] char projectState; // current state of project (ui hud control)

    [Header("Cached")]
    ScoreHandler sh; //score references
    InputHandler input; // input reference
    Drivetrain dt; // drivetrain reference

    private void Awake() //called before first frame
    {
        // sets all object references
        input = FindAnyObjectByType<InputHandler>();
        dt = FindAnyObjectByType<Drivetrain>();
        sh = GetComponent<ScoreHandler>();
    }

    private void Start()
    {
        //LoadGameCanvas();
    }

    void Update() // called every frame
    {
        switch (projectState) // state machine - not needed but there for versatility 
        {
            case 'M':
                break;
            case 'S': // server
                break;
            case 'C': // configuration/settings
                break;
            case 'G':
                GameLoop();
                break;
            case 'E':
                break;
        }
    }

    // disable all but menu, activate menu
    public void LoadMenuCanvas()
    {
        menuHUD.gameObject.SetActive(true);
        serverHUD.gameObject.SetActive(false);
        settingsHUD.gameObject.SetActive(false);
        gameHUD.gameObject.SetActive(false);
        endHUD.gameObject.SetActive(false);
        projectState = 'M';
    }

    // disable all but server, activate server
    public void LoadServerCanvas()
    {
        menuHUD.gameObject.SetActive(false);
        serverHUD.gameObject.SetActive(true);
        settingsHUD.gameObject.SetActive(false);
        gameHUD.gameObject.SetActive(false);
        endHUD.gameObject.SetActive(false);
        projectState = 'S';
    }

    // disable all but settings, activate settings
    public void LoadSettingsCanvas()
    {
        menuHUD.gameObject.SetActive(false);
        serverHUD.gameObject.SetActive(false);
        settingsHUD.gameObject.SetActive(true);
        gameHUD.gameObject.SetActive(false);
        endHUD.gameObject.SetActive(false);
        projectState = 'C';
    }

    // disable all but game, activate game
    public void LoadGameCanvas()
    {
        timer = 150;
        betweenTimer = 10;
        menuHUD.gameObject.SetActive(false);
        serverHUD.gameObject.SetActive(false);
        settingsHUD.gameObject.SetActive(false);
        gameHUD.gameObject.SetActive(true);
        endHUD.gameObject.SetActive(false);
        projectState = 'G';
        gameState = 'A';
    }

    // disable all but end, activate end
    public void LoadEndCanvas()
    {
        menuHUD.gameObject.SetActive(false);
        serverHUD.gameObject.SetActive(false);
        settingsHUD.gameObject.SetActive(false);
        gameHUD.gameObject.SetActive(false);
        endHUD.gameObject.SetActive(true);
        dt.ResetVelocity();
        projectState = 'E';
    }
    
    //reload scene to restart game instance
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    } 

    // handles game state machine
    // handles camera views
    // displays timer and score
    private void GameLoop()
    {
        CameraPerspective();
        switch (gameState)
        {
            case 'A': // if in autonomous phase
                DisplayTimer('G'); // use 2:30 timer
                DisplayScore();
                if ((int)timer == 120) // if auton state done, swap to teleop
                {
                    gameState = 'B';
                }
                break;
            case 'B':
                DisplayTimer('B'); // use small timer
                if (betweenTimer <=0) // if between timer done, swap to teleop
                {
                    gameState = 'T';
                }
                break;
            case 'T':
                DisplayTimer('G'); // use 2:30 (now 2:00) timer
                DisplayScore();
                break;
            case 'O':
                LoadEndCanvas();
                break;
        }
    }

    // swaps camera perspectives based on input on dpad
    private void CameraPerspective()
    {
        Vector2 value = input.GetDpad1Reading();

        if (value.y > 0.1f) // normal view
        {
            ResetCamPosition(viewCameraReference);
            currentCamPos = 'D'; //driver
        }
        else if (value.x > 0.1f) // top down
        {
            ResetCamPosition(topDownCameraReference);
            currentCamPos = 'T'; //topdown
        }
        else if (value.y < -0.1f) // robot perspective
        {
            ResetCamPosition(onRobotCameraReference);
            currentCamPos = 'R'; //robot
        }
    }

    // actually sets the camera position, taking a gameobject param to signify which empty to parent to
    private void ResetCamPosition(GameObject temp)
    {
        Camera cam = Camera.main;
        cam.transform.parent = temp.transform;
        cam.transform.localPosition = Vector3.zero;
        cam.transform.localEulerAngles = Vector3.zero;
    }

    //displays the timers of the game based on a char input
    //input changes which timer is ticked down
    //ends game when timer hits 0
    private void DisplayTimer(char type)
    {
        int timersec = 0;
        if (type=='G') // if game timer, use game timer timings
        { 
            timer -= Time.deltaTime;

            timersec = (int)timer;
        }
        else if (type=='B') // if between timer, use between timer timings
        {
            betweenTimer -= Time.deltaTime;

            timersec = (int)betweenTimer;
        }

        // displays the time in seconds using modulous
        if (timersec % 60 < 10) // if :0 needed, otherwise would display as 2:9 or 2:1 instead of 2:09 and 2:01
        {
            timeDisplayer.text = (int)(timersec / 60) + ":0" + timersec % 60;
        }
        else // if value has tens place, do not add "0" to string  
        {
            timeDisplayer.text = (int)(timersec / 60) + ":" + timersec % 60;
        }

        if (timer < 0) // if game is over, set game state to over
        {
            gameState = 'O';
        }
    }

    // quit game - used when building
    public void QuitGame()
    {
        Application.Quit();
    }

    // unused
    public void ToggleAudio()
    {
        audioOn = !audioOn;
        if (audioOn)
        {
            audioToggleButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            audioToggleButton.GetComponent<Image>().color = Color.red;
        }
    }

    // unused
    public void ToggleExtras()
    {
        extrasOn = !extrasOn;
        if (extrasOn)
        {
            extrasToggleButton.GetComponent<Image>().color = Color.green;
        }
        else
        {
            extrasToggleButton.GetComponent<Image>().color = Color.red;
        }
    }

    // sets score text to actual score reading
    private void DisplayScore()
    {
        scoreDisplayer.text = sh.GetScore().ToString();
    }

    // getter methods for variables needed in other files
    public char GetProjectState()
    {
        return projectState;
    }

    public char GetCameraPosition()
    {
        return currentCamPos;
    }

    public bool GetAudioOn()
    {
        return audioOn;
    }

    public bool GetExtrasOn()
    {
        return extrasOn;
    }
}