using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    #region SingletonDeclaration
    private static GameManager instance;
    public static GameManager FindInstance()
    {
        return instance; //that's just a singletone as the region says
    }

    void Awake() //this happens before the game even starts and it's a part of the singletone
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else if (instance == null)
        {
            //DontDestroyOnLoad(this);
            instance = this;
        }
    }
    #endregion



    #region StateDeclartion
    [HideInInspector]
    public enum State //creating an enumeration of all the states we possess
    {
        CreateBlank,
        GiveBlank,
        Vote,
        CheckVote,
        SwitchVote,
        End
        
    }

    private State _currentState;
    public State CurrentState
    {
        get
        {
            return _currentState;
        }
        set
        {
            _currentState = value;
            TransitionStates(value);
        }
    }
    #endregion

    Scene scene;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Scene scene = SceneManager.GetActiveScene();
        CurrentState = State.CreateBlank;
    }


    void Update()
    {
        RunStates();
        //Debug.Log(AllowDraw);
        InputChecker();
    }   

    void InputChecker()
    {
        if (Input.GetKey(KeyCode.R))
        {
            UtilScript.GoToScene("GameScene");
        }
    }


    #region StateMachine

    [HideInInspector]
    public Camera cam; 
    [HideInInspector]
    public GameObject ballot;
    [HideInInspector]
    public GameObject ballotPanel;
    [HideInInspector]
    public GameObject endingPanel;
    public static bool AllowDraw = false;
    float step;
    bool FirstVote = false;
    GameObject Button;

        // <summary>
    // Sets any initial values or one off methods when we're moving between states
        // </summary>
     private void TransitionStates(State newState)
     {
         switch (newState)
         {
             case State.CreateBlank:
                Debug.Log("Create Blank!");
                ballot = Instantiate(Resources.Load("Ballot")) as GameObject; //here we're creating the prefab;
                GameObject canvas1 = ballot.transform.GetChild(0).gameObject;
                canvas1.GetComponent<Canvas>().worldCamera = cam;
                ballotPanel = canvas1.transform.GetChild(0).gameObject;
                Button = ballotPanel.transform.GetChild(0).gameObject;
                endingPanel = canvas1.transform.GetChild(1).gameObject;
                CurrentState = State.GiveBlank;
                 break;
            case State.GiveBlank:
                break;
             case State.CheckVote:
                 break;
             case State.SwitchVote:
                OneTimeSwitchVote();
                 break;
            case State.End:
                //LinesDestroyer();
                break;
            default:
                Debug.Log("default state");
                break;
         }
     }

    private void RunStates()
    {
        switch (CurrentState)
        {
            case State.CreateBlank:
                break;
            case State.GiveBlank:
                GiveBlank();
                break;
            case State.Vote:
                AllowDraw = true;
                break;
            case State.CheckVote:
                if (!Input.GetMouseButton(0)){
                     AllowDraw = false;
                    CheckVote();
                }
                break;
            case State.SwitchVote:
                SwitchVote();
                break;
            case State.End:
                FinishButton();
                break;
            default:
                //Debug.Log("default state");
                break;
        }
    }

    void GiveBlank(){

        step = 50f * Time.deltaTime;
        Vector3 newPosY = new Vector3(ballotPanel.transform.position.x, 0f, +1f);
        ballotPanel.transform.position = Vector3.MoveTowards(ballotPanel.transform.position, newPosY, step);

        if (ballotPanel.transform.position.y == 0f)
        {
            step = 7f * Time.deltaTime;
            Vector3 newPosX = new Vector3(0f, ballotPanel.transform.position.y, +1f);
            ballotPanel.transform.position = Vector3.MoveTowards(ballotPanel.transform.position, newPosX, step);
            if (ballotPanel.transform.position == newPosX)
            {
                CurrentState = State.Vote;
            }
        }
    }

    void CheckVote()
    {

        if (FirstVote == false)
        {
            FirstVote = true;
            Button.SetActive(true);
        }

        bool IsPutin = TickVoted.GetComponent<LineChecker>().AmIPutin;
        if (IsPutin == false) 
        {
            //Debug.Log("not putin!");
            CurrentState = State.SwitchVote;
        } else         
        
        if (PanelVoted.transform.position == putinPos)
        {
            CurrentState = State.Vote;
            //PanelVoted = null;
        }
    }

    public static GameObject TickVoted;
    public static GameObject PanelVoted;
    [HideInInspector]
    public GameObject Putin;
    Vector3 putinPos;
    Vector3 panelPos;
    [HideInInspector]
    public GameObject LineVote;

    void SwitchVote()
    {
        step = 4f * Time.deltaTime;
        bool IsPutin = TickVoted.GetComponent<LineChecker>().AmIPutin;
        Putin.transform.position = Vector3.MoveTowards(Putin.transform.position, panelPos, step);
        PanelVoted.transform.position = Vector3.MoveTowards(PanelVoted.transform.position, putinPos, step);
        //if (Vector3.Distance(Putin.transform.position, panelPos) < 0.01f)
        if (PanelVoted.transform.position == putinPos)
        {
            CurrentState = State.Vote;
            //PanelVoted = null;
        }

    }

    void OneTimeSwitchVote()
    {
        putinPos = Putin.transform.position;
        panelPos = PanelVoted.transform.position;
    }

    List<LineRenderer> lineChildren;
    public void FinishButton()
    {
        AllowDraw = false;
        //ballotPanel.GetComponent<Animator>().SetTrigger("End");
        
        step = 4f * Time.deltaTime;
        Vector3 newPosUp = new Vector3 (ballotPanel.transform.position.x, +12f, 0f);
        ballotPanel.transform.position = Vector3.MoveTowards(ballotPanel.transform.position, newPosUp, step);
        Vector3 buttonPosUp = new Vector3 (Button.transform.position.x, +12f, 0f);
        Button.transform.position = Vector3.MoveTowards(Button.transform.position, buttonPosUp, 0f);
        Vector3 newPosEnd = new Vector3 (endingPanel.transform.position.x, +15f, 0f);
        endingPanel.transform.position = Vector3.MoveTowards(endingPanel.transform.position, newPosEnd, step);
        if (endingPanel.transform.position == newPosEnd)
        {
            UtilScript.GoToScene("GameScene");
        }
    }

    #endregion

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) 
    {
        Debug.Log("scene loaded");
        //CurrentState = State.CreateBlank;
    }

}
