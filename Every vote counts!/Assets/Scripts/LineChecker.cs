using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineChecker : MonoBehaviour //this scripts sets on all the ticking boxes and works as a collider checker
{   
    GameManager myManager; //manager instance
    public GameObject Panel; //relevant to this tick box panel
    public bool AmIPutin; //also is this panel actually putin
    //public static GameObject Putin;
    //bool collided;

    void Start ()
    {

        myManager = GameManager.FindInstance(); //manager instance

        if (AmIPutin == true) //if I am Putin I send myself to the game manager to remember
        {
            myManager.Putin = Panel;
        }
    }

    void OnTriggerEnter2D (Collider2D col) //when we collide with something (it happens to be only the line prefabs instantiated in the draw.cs)
    {  
            Destroy(myManager.LineVote); //we destroy the previous line that collided with any other ticking box
            //Debug.Log("col entered!");
            GameManager.TickVoted = gameObject; //send itself to the gamemanager to process
            GameManager.PanelVoted = Panel; //also panel assosiated
            myManager.LineVote = col.gameObject; //also save the line that collided to delete next time person votes
            myManager.CurrentState = GameManager.State.CheckVote; //and finally change the state in the manager
            col.GetComponent<Collider2D>().enabled = false; //we disable the collider on the line to not collide with anything again 
    }

}
