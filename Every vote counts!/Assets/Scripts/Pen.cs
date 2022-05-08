using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pen : MonoBehaviour
{
    GameManager myManager;
    private Sprite penSprite;
    [HideInInspector] public BoxCollider2D penCol;

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

    private Vector3 defaultEulerAngles;
    private Vector3 pickedUpEulerAngles = new Vector3 (0f, 0f, 20f);
    private Vector3 drawingEulerAngles = new Vector3 (0f, 0f, 40f);
    private Vector3 defaultPos;

    [HideInInspector] public bool isPickedUp = false;

    private void TransitionStates(State newState)
     {
         switch (newState)
         {  case State.WaitForBallot:
                break;
            case State.Untouched:
                //penSprite.sortingOrder = -3;
                GameManager.AllowDraw = false;
                transform.eulerAngles = defaultEulerAngles;
                transform.position = defaultPos;
                penCol.enabled = true;
                break;
            case State.PickedUp:
                //penSprite.sortingOrder = 0;
                GameManager.AllowDraw = true;
                transform.eulerAngles = pickedUpEulerAngles;
                penCol.enabled = false;

                break;
            case State.Drawing:
                transform.eulerAngles = drawingEulerAngles;
                penCol.enabled = false;
                break;
            default:
                //Debug.Log("default state");
                break;
         }
     }
    private void RunStates()
    {
        switch (CurrentState)
        {   
            case State.GivePen:
            float step = 5f * Time.deltaTime;
            Vector3 newPosY = new Vector3(transform.position.x, 0f, 5.4f);
            transform.position = Vector3.MoveTowards(transform.position, newPosY, step);
            if (transform.position.y == newPosY.y)
            {
                defaultPos = transform.position;
                CurrentState = State.Untouched;
            }
                break;
            case State.PickedUp:
                Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
                break;
            case State.Drawing:
                Vector3 mousePos2 = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);
                mousePos2 = Camera.main.ScreenToWorldPoint(mousePos2);
                transform.position = new Vector3(mousePos2.x, mousePos2.y, 0f);
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
        penSprite = GetComponent<Image>().sprite;
        penCol = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (CurrentState != State.WaitForBallot && CurrentState != State.GivePen)
        {
            Debug.Log("InputChecker going");
            InputChecker(); 
        }
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

