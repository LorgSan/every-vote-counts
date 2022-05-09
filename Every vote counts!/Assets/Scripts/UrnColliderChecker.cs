using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrnColliderChecker : MonoBehaviour
{
    GameManager myManager;
    [SerializeField]AudioSource sound;
    
    void Start()
    {
        myManager = GameManager.FindInstance();
    }

    void OnTriggerEnter2D(Collider2D col) //we just check if we hit the right collider when the ballot is dragged
    {
        if (col.gameObject.tag == "BottomCol" && myManager.isBallotDragged == true)
        {
            if (myManager.FirstVote == true)
            {
                myManager.hitBottom = true;
            } else 
            sound.Play();

        }
    }
    void OnTriggerExit2D(Collider2D col)
    {
        myManager.hitBottom = false;
    }
}

