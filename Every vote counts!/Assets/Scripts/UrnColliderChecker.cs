using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrnColliderChecker : MonoBehaviour
{
    GameManager myManager;
    
    void Start()
    {
        myManager = GameManager.FindInstance();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (myManager.FirstVote == true)
        {
            if (col.gameObject.tag == "BottomCol")
            {
                myManager.hitBottom = true;
            }   
        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        myManager.hitBottom = false;
    }
}

