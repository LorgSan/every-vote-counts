using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHolder : MonoBehaviour
{
    GameManager myManager;

    void Start()
    {
        myManager = GameManager.FindInstance();
    }
    public void Click()
    {
        if (myManager.CurrentState != GameManager.State.SwitchVote)
        {
            myManager.CurrentState = GameManager.State.End;
            Debug.Log(myManager.CurrentState);
        }
    }
}
