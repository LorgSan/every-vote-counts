using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pen : MonoBehaviour
{
    GameManager myManager; //manager ref
    [HideInInspector] public BoxCollider2D penCol; //saving the collider

    #region StateDeclartion
    [HideInInspector]
    public enum State //creating an enumeration of all the states we possess
    {
        WaitForBallot,
        GivePen,
        Untouched,
        PickedUp,
        Drawing
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

    //I need to have multiple rotation position to jump between depending on the pen state
    private Vector3 defaultEulerAngles; //saving the deafult rot
    private Vector3 pickedUpEulerAngles = new Vector3 (0f, 0f, 20f); //for the picked up state
    private Vector3 drawingEulerAngles = new Vector3 (0f, 0f, 40f); //drawing state
    private Vector3 defaultPos; //also pos! I need only the default though

    private void TransitionStates(State newState) // one-time state changer
     {
         switch (newState)
         {  case State.WaitForBallot: //this state is our "start" one that waits until the game manager will finish the giveblank state
                break;

            case State.Untouched: //this is when the pen is just laying on the table
                //penSprite.sortingOrder = -3;
                GameManager.AllowDraw = false; //no drawing when the pen is not picked up!
                myManager.ballotPanel.GetComponent<BoxCollider2D>().enabled = true; //also this? is done to switch between ballot colliders (the thing I was asking you in class!)
                transform.eulerAngles = defaultEulerAngles; //resetting the rotation
                transform.position = defaultPos; //and position
                penCol.enabled = true; //returning the collision so we can pick the pen up again
                break;

            case State.PickedUp:
                //penSprite.sortingOrder = 0;
                myManager.ballotPanel.GetComponent<BoxCollider2D>().enabled = false; //same!
                GameManager.AllowDraw = true; //and we can draw now
                transform.eulerAngles = pickedUpEulerAngles; //changing the rotation
                penCol.enabled = false; //as soon as we pick the pen up we disable it's collider so it doesn't collide with ballot colliders
                break;

            case State.Drawing:
                transform.eulerAngles = drawingEulerAngles; //new rotation
                penCol.enabled = false; //and doing that also? for some reason? just in case
                break;
            default:
                //Debug.Log("default state");
                break;
         }
     }
    private void RunStates() //updated state switcher
    {
        switch (CurrentState)
        {   
            case State.GivePen: //this state happens when gamemanager is done with its give ballot state
            float step = 7f * Time.deltaTime; 
            Vector3 newPosY = new Vector3(transform.position.x, 0f, 5.4f);
            transform.position = Vector3.MoveTowards(transform.position, newPosY, step); //we just move it into the screen
            if (transform.position.y == newPosY.y) //and when we're done
            {
                defaultPos = transform.position; //we're saving the position for the untouched state to use
                CurrentState = State.Untouched; //and change the statee
            }
                break;
            case State.PickedUp: //this state happens when we pick the pen up and just move it with the mouse pos
                UtilScript.MoveWithMouse(transform, new Vector3(0f,0f,transform.position.z));
                break;
            case State.Drawing: //same, but it's a different state because of the one-time states and I need to continue updating the pos
                UtilScript.MoveWithMouse(transform, new Vector3(0f,0f,transform.position.z));
                break;
            default:
                //Debug.Log("default state");
                break;
        }
    }

    void Start()
    {   
        defaultEulerAngles = transform.eulerAngles;
        myManager = GameManager.FindInstance();
        CurrentState = State.WaitForBallot;
        penCol = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (CurrentState != State.WaitForBallot && CurrentState != State.GivePen)
        {
            //  Debug.Log("InputChecker going");
            InputChecker(); 
        }
        //Debug.Log(CurrentState);
        RunStates();
    }

    void InputChecker()
    {
        Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //Debug.DrawLine(Camera.main.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.red);

        if (col != null)
        {
            if (col.gameObject.tag == "Pen")
            {
                if (Input.GetMouseButtonDown(0))
                {
                        CurrentState = State.PickedUp;
                }
            }
        } else 
        if (Input.GetMouseButtonDown(0))
        {
            CurrentState = State.Untouched;
        }

        if (Input.GetMouseButtonUp(0) && CurrentState == State.Drawing)
        {
            //Debug.Log("mouse up state to picked up");
            CurrentState = State.PickedUp;
        }

    }
}

