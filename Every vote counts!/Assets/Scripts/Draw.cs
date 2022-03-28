using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    [HideInInspector]
    public GameObject currentLine; //the current line we are currently creating

    [HideInInspector]
    public LineRenderer lineRenderer; //prefab's linerenderer component
    [HideInInspector]    
    public EdgeCollider2D edgeCollider; //prefab's collider
    [HideInInspector]
    public List<Vector2> fingerPositions; //basically vector2 between each line dots
    bool LineWasStarted = false; //this bool helps me update the line only if the line was already started
    //without this it sometimes lags and doesnt finish the line if the ending was outside the collision
    GameManager myManager; //gamemanager instance saving

    // Start is called before the first frame update
    void Start()
    {
        myManager = GameManager.FindInstance();      //gamemanager instance saving   
    }

    // Update is called once per frame
    void Update()
    {
        StatementChecker(); //having a function to check it all!
    }

    void StatementChecker()
    {
            if (GameManager.AllowDraw == true) //we can only create a line if we're allowed to draw
            {
                Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                //checking if there's a line under the mouse

                if(col != null) //and if there is 
                {
                    if(Input.GetMouseButtonDown(0))  //on button down we do the creating line function
                    {
                        CreateLine();
                    }

                    if(Input.GetMouseButton(0)) //after that if the mouse is still down we update the line
                    {
                        if (LineWasStarted == true){
                        
                            Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            ////basically if the mouse position changed we update the line
                            if (Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count -1]) > 0.1f)
                            {
                                UpdateLine(tempFingerPos);
                            }
                        }
                    }                     
                }

            }          

    }

    void CreateLine() //tbh I wanted tutorial for this whole line thing, but I'll do my best to explain what I understand
    {
        LineWasStarted = true;
        currentLine = Instantiate(Resources.Load("Line") as GameObject, Vector3.zero, Quaternion.identity); //we create the line prefab
        currentLine.transform.parent = myManager.ballotPanel.transform; //put it under the ballot game object so it moves with it
        // ^^^ for the same reason the line doesn't use the world space (and I had to change it's collider layer to not collide with ballot)
        // but collide with the tickbox
        lineRenderer = currentLine.GetComponent<LineRenderer>();  //setting the linerederer component from the instaantiated prefab
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>(); //same with collider
        //thanks god edgecolliders never collide with each other unity is so good for that
        fingerPositions.Clear(); //after we create new line we clear previous positions for them to not mix up
        //also this thing doesn't happen if the line ends outside the ballot collider!
        //that's why I'm checing if the line was started
        //at the same time I never
        fingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition)); //
        fingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        lineRenderer.SetPosition(0, fingerPositions[0]);
        lineRenderer.SetPosition(1, fingerPositions[1]);
        edgeCollider.points = fingerPositions.ToArray();
    }

    void UpdateLine(Vector2 newFingerPos)
    {
        fingerPositions.Add(newFingerPos);
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount -1, newFingerPos);
        edgeCollider.points = fingerPositions.ToArray();
    }
}