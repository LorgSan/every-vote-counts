using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHolder : MonoBehaviour
{
    GameManager myManager; //manager instance

    void Start()
    {
        myManager = GameManager.FindInstance(); //manager instance
    }
    public void Click() //tbh this script exist only to use on the OnClick() event on the button itself 
    {
        if (myManager.CurrentState != GameManager.State.SwitchVote) 
        //this check lets us vote only if we're not changing the panels right now
        {
            myManager.CurrentState = GameManager.State.End; //jsut change the state basically
            //Debug.Log(myManager.CurrentState);
        }
    }
}
