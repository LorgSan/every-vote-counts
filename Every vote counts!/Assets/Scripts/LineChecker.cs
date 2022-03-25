using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineChecker : MonoBehaviour
{

    void OnCollisionEnter2D (Collision2D col)
    {
        Debug.Log("col entered!");
        //GameManager.CurrentState = GameManager.State.Unvote;
    }

}
