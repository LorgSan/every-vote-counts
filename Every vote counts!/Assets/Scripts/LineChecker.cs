using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineChecker : MonoBehaviour
{   
    GameManager myManager;
    public GameObject Panel;
    public bool AmIPutin;
    //public static GameObject Putin;
    //bool collided;

    void Start ()
    {

        myManager = GameManager.FindInstance();

        if (AmIPutin == true)
        {
            myManager.Putin = Panel;
        }
    }

    void OnCollisionEnter2D (Collision2D col)
    {  
            Destroy(myManager.LineVote);
            Debug.Log("col entered!");
            GameManager.TickVoted = gameObject;
            GameManager.PanelVoted = Panel;
            myManager.LineVote = col.gameObject;
            myManager.CurrentState = GameManager.State.CheckVote;
            //Debug.Log(myManager.CurrentState);
            // если линия колизится с 
    }

}
