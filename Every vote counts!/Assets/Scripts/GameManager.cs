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
            //DontDestroyOnLoad(this);
            instance = this;
        }
    }
    #endregion
    //singleton is mostly done the way we normally do singletons, but withouth DontDestroyOnLoad, as I wanted to to reload the scene freely
    // so that the Start() happens again, and, considering that I don't use any other scenes, that seemed like a good decision

    [SerializeField] GameObject urnMask;
    [SerializeField] GameObject urnBlocker;
    [SerializeField] AudioSource soundsSource;
    [SerializeField] GameObject Fade;
    [SerializeField] Pen pen;
    [SerializeField] public GameObject ballotPanel; //is a gameobject on the canvas saved on the ballot prefab, bc we can't move the canvas and only stuff on it

    Vector3 ballotScale;
    Vector3 ballotSmallScale = new Vector3(0.18f, 0.18f,1f);
    Vector3 ballotPos;
    Vector3 ballotLMBOffset; //this is used so that ballot is dragged by the mouse position without snapping to the center
    Vector3 ballotRMBOffset;
    Vector3 putinPos; //his position
    Vector3 panelPos; //position of the actual panel voted for
    [HideInInspector] public GameObject graphicsHolder;
    [HideInInspector] public bool hitBottom = false;
    [HideInInspector] public static bool AllowDraw = false; //this thing checks if you're allowed to draw and is used in the Draw.cs too 
    [HideInInspector] public bool FirstVote = false; //check for the button "I voted!" to appear
    [HideInInspector] public GameObject Putin; //main putin panel saved through the LineChecker.cs 
    bool isBallotDragged = false;
    bool onUrnPos = false;
    
    public static GameObject TickVoted; //tick that was voted!
    public static GameObject PanelVoted; //and the panel it was assigned to 
    [HideInInspector]
    public GameObject LineVote; //used in the linechecker.cs to delete the previous line that voted

    void Start()
    {
        graphicsHolder = ballotPanel.transform.GetChild(0).gameObject;
        ballotScale = graphicsHolder.transform.localScale;
        CurrentState = State.GiveBlank; //we just set the state 
    }


    void Update()
    {
        RunStates(); //this is a updated state switch void
        InputChecker(); //this void has more of a utility use and just checks a bunch of stuff
        if (FirstVote == true && CurrentState != State.End)
        {
            OpenUrn();
        }
    }   

    void InputChecker()
    {
        if (Input.GetKey(KeyCode.R)) //it's just an easy fast scene reloading
        {
            UtilScript.GoToScene("StartScene");
        } 
        MouseInput();
    }

    void MouseInput()
    {
        if (pen.CurrentState == Pen.State.Untouched && CurrentState == State.Vote) //okay I'm unsure about placing this whole mess here
        //should've probably moved to a new script
        {
            Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (col!=null) 
            {
                if (col.gameObject.tag == "BottomCol")
                {
                    if (Input.GetMouseButtonDown(0))
                    {   
                        isBallotDragged = true;
                        ballotLMBOffset = UtilScript.SaveOffset(ballotPanel.transform);
                        ballotPanel.transform.SetSiblingIndex(4);
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        ballotPanel.transform.localScale = new Vector3(2.5f, 2.5f, 1f);
                        ballotRMBOffset = UtilScript.SaveOffset(ballotPanel.transform);
                        ballotPanel.transform.SetSiblingIndex(4);
                    }
                    if (Input.GetMouseButton(1))
                    {
                        UtilScript.MoveWithMouse(ballotPanel.transform, ballotRMBOffset);

                    }
                    if (Input.GetMouseButtonUp(1))
                    {
                        ballotPanel.transform.SetSiblingIndex(2);
                        ballotPanel.transform.localScale = new Vector3 (1f, 1f, 1f);
                        ballotPanel.transform.position = ballotPos;
                    }
                }
                
            }
                    if (hitBottom == true)
                    {
                        graphicsHolder.transform.localScale = UtilScript.VectorLerp(graphicsHolder.transform.position, ballotSmallScale, 5f);
                        
                    } else 
                    if (hitBottom == false)
                    {
                        graphicsHolder.transform.localScale = UtilScript.VectorLerp(graphicsHolder.transform.position, ballotScale, 5f);
                    }

                    if (isBallotDragged)
                    {
                        if (Input.GetMouseButtonUp(0))
                        {
                            isBallotDragged = false;
                            ballotPanel.transform.SetSiblingIndex(2);
                            if (hitBottom == true)
                            {
                                CurrentState = State.End;
                            } else 
                            {
                                ballotPanel.transform.position = ballotPos;
                            }
                            return;
                        }
                        UtilScript.MoveWithMouse(ballotPanel.transform, ballotLMBOffset);
                    } 
        }
    }

    #region StateDeclartion
    [HideInInspector]
    public enum State //creating an enumeration of all the states we possess
    {
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

    #region OneTimeStateSwitcher
        // <summary>
    // Sets any initial values or one off methods when we're moving between states
        // </summary>
     private void TransitionStates(State newState)
     {
         switch (newState)
         {
            case State.GiveBlank:
                break;
             case State.CheckVote:
                 break;
             case State.SwitchVote:
                SetPosition(); 
                 break;
            case State.End:
                //LinesDestroyer();
                AllowDraw = false;
                pen.CurrentState = Pen.State.WaitForBallot;
                graphicsHolder.transform.GetChild(0).gameObject.GetComponent<BoxCollider2D>().enabled = false;
                //ballotPanel.transform.SetParent(urnMask.transform);
                //urnMask.SetActive(true);
                break;
            default:
                //Debug.Log("default state");
                break;
         }
     }
    
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
            case State.GiveBlank:
                GiveBlank();
                break;
            case State.Vote:
            if (pen.CurrentState == Pen.State.PickedUp || pen.CurrentState == Pen.State.Drawing)
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
                FinishGame();
                break;
            default:
                //Debug.Log("default state");
                break;
        }
    }

    void GiveBlank() //this thing moves the ballot into the screen
    {
        Vector3 newPosY = new Vector3(ballotPanel.transform.position.x, -0.5f, +1f);
        ballotPanel.transform.position = UtilScript.VectorLerp(ballotPanel.transform.position, newPosY, 7f);
        if (ballotPanel.transform.position == newPosY) //when it moved it to the last point
        {
            CurrentState = State.Vote; //we change the state
            ballotPos = ballotPanel.transform.position;
            pen.CurrentState = Pen.State.GivePen;
        }
    }

    void CheckVote() //this thing checks who we voted for and if it's not putin we switch his panel with the voted panel
    {

        if (FirstVote == false) //and if we voted for the first time, the button is moved down 
        {
            FirstVote = true; //and set as true to never do that again
            soundsSource.Play();
        }

        bool IsPutin = TickVoted.GetComponent<LineChecker>().AmIPutin; //so we just check the tick that has called the state if it's putin
        if (IsPutin == false)  //if it's not putin
        {
            CurrentState = State.SwitchVote; //we go switch the vote
        }   else CurrentState = State.Vote; //if it is putin you can just continue playing with it, change it the way you want
    }

    void OpenUrn()
    {
        Vector3 urnMove = new Vector3 (urnBlocker.transform.position.x, -5f, 1f);
        urnBlocker.transform.position = UtilScript.VectorLerp(urnBlocker.transform.position, urnMove, 12f);
    }

    void SwitchVote() //this thing moves the panel voted for to putin's position and him on the panel's position
    {
        Putin.transform.position = UtilScript.VectorLerp(Putin.transform.position, panelPos, 4f);
        PanelVoted.transform.position = UtilScript.VectorLerp(PanelVoted.transform.position, putinPos, 4f);
        if (PanelVoted.transform.position == putinPos) //we switch vote when one of them finishes the move (it happens simultaneously anyway)
        {
            CurrentState = State.Vote;
        }

    }
    public void FinishGame() //this is the endgame function  
    {
        if (onUrnPos == false)
        {
            Vector3 urnPosition = new Vector3 (urnMask.transform.position.x+0.3f, 5f, 1f);
            ballotPanel.transform.position = UtilScript.VectorLerp(ballotPanel.transform.position, urnPosition, 5f);
            graphicsHolder.transform.localScale = UtilScript.VectorLerp(graphicsHolder.transform.localScale, ballotSmallScale, 5f);
            if (ballotPanel.transform.position == urnPosition)
            {
                onUrnPos = true;
            }
        }

        if (onUrnPos == true)
        {
            if (urnMask.activeSelf == false)
            {
                urnMask.SetActive(true);
            }

            Vector3 endPosition = new Vector3 (ballotPanel.transform.position.x, -10f, 1f);
            ballotPanel.transform.position = UtilScript.VectorLerp(ballotPanel.transform.position, endPosition, 7f);

            if (ballotPanel.transform.position == endPosition)
            {
                Fade.GetComponent<Animator>().SetTrigger("FadeOut");
            }
        } 
    }
}

    #endregion

    #endregion
