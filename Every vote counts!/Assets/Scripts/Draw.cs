using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    [HideInInspector]
    public GameObject Line;
    [HideInInspector]
    public GameObject currentLine;

    [HideInInspector]
    public LineRenderer lineRenderer;
    [HideInInspector]    
    public EdgeCollider2D edgeCollider;
    [HideInInspector]
    public List<Vector2> fingerPositions; 
    bool LineWasStarted = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        StatementChecker();
    }

    void StatementChecker()
    {
            if (GameManager.AllowDraw == true)
            {
                if(Input.GetMouseButtonDown(0)) 
                {
                    CreateLine();

                }
                if(Input.GetMouseButton(0))
                {
                    if (LineWasStarted == true){
                        
                        Vector2 tempFingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        if (Vector2.Distance(tempFingerPos, fingerPositions[fingerPositions.Count -1]) > 0.1f)
                        {
                            UpdateLine(tempFingerPos);
                        }
                    }
                }  
            }          

    }

    void CreateLine()
    {
        LineWasStarted = true;
        currentLine = Instantiate(Resources.Load("Line") as GameObject, Vector3.zero, Quaternion.identity);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        edgeCollider = currentLine.GetComponent<EdgeCollider2D>();
        fingerPositions.Clear();
        fingerPositions.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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