using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            DontDestroyOnLoad(this);
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


    void Start()
    {
        CurrentState = State.CreateBlank;
    }


    void Update()
    {
        RunStates();
        //Debug.Log(AllowDraw);
    }   


    #region StateMachine

    public Camera cam; 
    [HideInInspector]
    public GameObject ballot;
    [HideInInspector]
    public GameObject ballotPanel;
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
                ballot = Instantiate(Resources.Load("Ballot")) as GameObject; //here we're creating the prefab;
                GameObject canvas1 = ballot.transform.GetChild(0).gameObject;
                canvas1.GetComponent<Canvas>().worldCamera = cam;
                ballotPanel = canvas1.transform.GetChild(0).gameObject;
                Button = ballotPanel.transform.GetChild(0).gameObject;
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
            Debug.Log("not putin!");
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
    public GameObject Putin;
    public Vector3 putinPos;
    public Vector3 panelPos;
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

        step = 20f * Time.deltaTime;
        Vector3 newPosUp = new Vector3 (ballotPanel.transform.position.x, +12f, 0f);
        ballotPanel.transform.position = Vector3.MoveTowards(ballotPanel.transform.position, newPosUp, step);

    }

    #endregion

}
