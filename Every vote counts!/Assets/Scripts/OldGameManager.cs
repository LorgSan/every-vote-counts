using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldGameManager : MonoBehaviour
{
    // Start is called before the first frame update

    #region SingletonDeclaration 
    private static OldGameManager instance; 
    public static OldGameManager FindInstance()
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
    //singleton is mostly done the way we normally do singletons, but withouth DontDestroyOnLoad, as I wanted to to reload the scene freely
    // so that the Start() happens again, and, considering that I don't use any other scenes, that seemed like a good decision

    Vector3 ballotScale;
    Vector3 ballotPos;

    void Start()
    {
        CurrentState = State.CreateBlank; //we just set the state 
    }


    void Update()
    {
        RunStates(); //this is a updated state switch void
        InputChecker(); //this void has more of a utility use and just checks a bunch of stuff
    }   

    void InputChecker()
    {
        if (Input.GetKey(KeyCode.R)) //it's just an easy fast scene reloading
        {
            UtilScript.GoToScene("GameScene");
        }
        if (FirstVote == true && CurrentState != State.End) //so this check is a bit weird and I didn't figure out how to do it better,
        //but in short, it moves the button "I voted!" after our first void and since the state where it happens switches the boolean
        //and never comes back to it again, I'm checking it in here and move it in update, BUT only until state where it should move up with
        //everything else, so it technically runs most of the game and I don't like it 
        {
            ButtonMove();
        }

        if (pen.CurrentState == Pen.State.Untouched && CurrentState == State.Vote)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log(ballotPanel.transform.position);
                ballotPanel.transform.localScale = new Vector3(2.5f, 2.5f, 1f);
            }
            if (Input.GetMouseButton(1))
            { 
                Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                ballotPanel.transform.position = new Vector3(mousePos.x, mousePos.y, ballotScale.z);
            }
            if (Input.GetMouseButtonUp(1))
            {
                ballotPanel.transform.localScale = ballotScale;
                ballotPanel.transform.position = ballotPos;
            }
        }
    }

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

    private State _currentState; //this is our protection level that also states runs the "one-time" state switcher
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

    #region StateMachine

    public Camera cam; //I needed to set the cam for the instantianted canvas to work properly:0
    [HideInInspector]
    public GameObject ballot; //ballot is basically the whole prefab of a ballot saved in resources
    [HideInInspector]
    public GameObject ballotPanel; //is a gameobject on the canvas saved on the ballot prefab, bc we can't move the canvas and only stuff on it
    [HideInInspector]
    public GameObject endingPanel; //this is a panel with the end text
    public static bool AllowDraw = false; //this thing checks if you're allowed to draw and is used in the Draw.cs too 
    float step; //it's just the MoveTowards thingy that changes from time to time depending on what moves
    bool FirstVote = false; //check for the button "I voted!" to appear
    GameObject Button; //button itself

    #region OneTimeStateSwitcher
        // <summary>
    // Sets any initial values or one off methods when we're moving between states
        // </summary>
     private void TransitionStates(State newState)
     {
         switch (newState)
         {
             case State.CreateBlank:
                CreateBlank();
                 break;
            case State.GiveBlank:
                break;
             case State.CheckVote:
                 break;
             case State.SwitchVote:
                SetPosition(); 
                 break;
            case State.End:
                //LinesDestroyer();
                break;
            default:
                Debug.Log("default state");
                break;
         }
     }

    void CreateBlank() //this thing creates the ballot itself
     {
        //Debug.Log("Create Blank!");
        ballot = Instantiate(Resources.Load("Ballot")) as GameObject; //here we're creating the prefab;
        GameObject canvas1 = ballot.transform.GetChild(0).gameObject; //and saving it's canvas to get stuff from it
        canvas1.GetComponent<Canvas>().worldCamera = cam; //here we're setting the world camera as the main camera in the scene
        ballotPanel = canvas1.transform.GetChild(1).gameObject; //saving the gameobject that's gonna be moved
        ballotScale = ballotPanel.transform.localScale;
        //Button = ballotPanel.transform.GetChild(0).gameObject;
        Button = canvas1.transform.GetChild(0).gameObject; //saving the vutton to move separately from the ballot panel
        endingPanel = canvas1.transform.GetChild(2).gameObject; //same with the text in the end of the game
        CurrentState = State.GiveBlank; //switch the state after we created everything       
     }

    [HideInInspector]
    public GameObject Putin; //main putin panel saved through the LineChecker.cs 
    Vector3 putinPos; //his position
    Vector3 panelPos; //position of the actual panel voted for
    
    void SetPosition() //this thing sets the position of both the putin's panel and the voted panel 
    //so that they don't get updated in the runstates and they move properly
    {
        putinPos = Putin.transform.position; //putin's position
        panelPos = PanelVoted.transform.position; //voted panel position
    }

    #endregion

    #region UpdatedStateSwitcher

    private void RunStates() //updated state swwitcher that happens each tick
    {
        switch (CurrentState)
        {
            case State.CreateBlank:
                break;
            case State.GiveBlank:
                GiveBlank();
                break;
            case State.Vote:
            if (pen.CurrentState != Pen.State.Untouched)
            {
                AllowDraw = true;
            }
                break;
            case State.CheckVote:
                if (!Input.GetMouseButton(0))
                { //here we start the checking and moving the panels only after the person has stopped drawing the line
                    AllowDraw = false; //and disallow drawing while those are moving
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

    public static GameObject TickVoted; //tick that was voted!
    public static GameObject PanelVoted; //and the panel it was assigned to 
    [HideInInspector]
    public GameObject LineVote;//used in the linechecker.cs to delete the previous line that voted
    [SerializeField] Pen pen;

    void GiveBlank() //this thing moves the ballot into the screen
    {
        step = 50f * Time.deltaTime; //how smoothly/fast it moves
        Vector3 newPosY = new Vector3(ballotPanel.transform.position.x, 0f, +1f); //setting the vector3 for the ballotpanel
        //which is onle the ballot itself, not the button and the final text
        ballotPanel.transform.position = Vector3.MoveTowards(ballotPanel.transform.position, newPosY, step); //this thing actually moves it
        //each tick for the value that's set in the step, which makes it the "speed" basically

        if (ballotPanel.transform.position.y == 0f) //when it moved it to the last point
        {
            step = 7f * Time.deltaTime; //we do the same thing but horizontally
            Vector3 newPosX = new Vector3(0f, ballotPanel.transform.position.y, +1f);
            ballotPanel.transform.position = Vector3.MoveTowards(ballotPanel.transform.position, newPosX, step);
            if (ballotPanel.transform.position == newPosX) //and when it finished
            {
                CurrentState = State.Vote; //we change the state
                ballotPanel.transform.position = ballotPos;
                //to the state that just allows us to draw and vote at any point
                pen.CurrentState = Pen.State.GivePen;
            }
        }
    }

    void CheckVote() //this thing checks who we voted for and if it's not putin we switch his panel with the voted panel
    {

        if (FirstVote == false) //and if we voted for the first time, the button is moved down 
        {
            FirstVote = true; //and set as true to never do that again
            Button.SetActive(true);
        }

        bool IsPutin = TickVoted.GetComponent<LineChecker>().AmIPutin; //so we just check the tick that has called the state if it's putin
        if (IsPutin == false)  //if it's not putin
        {
            //Debug.Log("not putin!");
            CurrentState = State.SwitchVote; //we go switch the vote
        } else CurrentState = State.Vote; //if it is putin you can just continue playing with it, change it the way you want
    }

    void ButtonMove() //this function moves the button if the first vote (called in the input checker)
    {
        step = 50f * Time.deltaTime;
        Vector3 firstButtonPos = new Vector3 (Button.transform.position.x, 0f, 10f);
        Button.transform.position = Vector3.MoveTowards(Button.transform.position, firstButtonPos, step);
    }

    void SwitchVote() //this thing moves the panel voted for to putin's position and him on the panel's position
    {
        step = 4f * Time.deltaTime; 
        Putin.transform.position = Vector3.MoveTowards(Putin.transform.position, panelPos, step);
        PanelVoted.transform.position = Vector3.MoveTowards(PanelVoted.transform.position, putinPos, step);
        //if (Vector3.Distance(Putin.transform.position, panelPos) < 0.01f)
        if (PanelVoted.transform.position == putinPos) //we switch vote when one of them finishes the move (it happens simultaneously anyway)
        {
            CurrentState = State.Vote;
            //AllowDraw = true;
            //PanelVoted = null;
        }

    }

    public void FinishButton() //this is the endgame function that moves everything up and shows the finale text 
    {
        AllowDraw = false; //we're not allowed to draw anymore

        step = 3f * Time.deltaTime; //set the step lower so the people would be able to read if they want

        Vector3 newPosUp = new Vector3 (ballotPanel.transform.position.x, +12f, 0f);
        ballotPanel.transform.position = Vector3.MoveTowards(ballotPanel.transform.position, newPosUp, step);

        Vector3 buttonPosUp = new Vector3 (Button.transform.position.x, +12f, 0f);
        Button.transform.position = Vector3.MoveTowards(Button.transform.position, buttonPosUp, step);

        Vector3 newPosEnd = new Vector3 (endingPanel.transform.position.x, +15f, 0f);
        endingPanel.transform.position = Vector3.MoveTowards(endingPanel.transform.position, newPosEnd, step);

        if (endingPanel.transform.position == newPosEnd)
        {
            UtilScript.GoToScene("GameScene"); //when the finale text gets off the screen we restart everything
        }
    }

    #endregion

    #endregion

}
