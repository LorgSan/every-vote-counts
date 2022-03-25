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
        Unvote,
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
    }   


    #region StateMachine

    public Camera cam; 
    [HideInInspector]
    public GameObject ballot;
    [HideInInspector]
    public GameObject ballotPanel;

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
                CurrentState = State.GiveBlank;
                 break;
             case State.Vote:
                 break;
             case State.Unvote:
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
                Debug.Log("Voting!");
                break;
            case State.Unvote:
                break;
        }
    }

    float step;

    void GiveBlank(){

        step = 50f * Time.deltaTime;
        Vector3 newPosY = new Vector3(ballotPanel.transform.position.x, 0f, +1f);
        ballotPanel.transform.position = Vector3.MoveTowards(ballotPanel.transform.position, newPosY, step);

        if (ballotPanel.transform.position.y == 0f)
        {
            step = 5f * Time.deltaTime;
            Vector3 newPosX = new Vector3(-1f, ballotPanel.transform.position.y, 0f);
            ballotPanel.transform.position = Vector3.MoveTowards(ballotPanel.transform.position, newPosX, step);
        }

        if (ballotPanel.transform.position.x == -1f)
        {
            CurrentState = State.Vote;
        }
    }

    #endregion

}
