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
    GameManager myManager;

    // Start is called before the first frame update
    void Start()
    {
        myManager = GameManager.FindInstance();        
    }

    // Update is called once per frame
    void Update()
    {
        StatementChecker();
        if (LineWasStarted == true && Input.GetMouseButtonUp(0))
        {
            Debug.Log("debug the line");
            //BakeLineDebuger(currentLine);
            
        }
    }

    void StatementChecker()
    {
            if (GameManager.AllowDraw == true)
            {
                Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));

                if(col != null)
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

    }

    void CreateLine()
    {
        LineWasStarted = true;
        currentLine = Instantiate(Resources.Load("Line") as GameObject, Vector3.zero, Quaternion.identity);
        currentLine.transform.parent = myManager.ballotPanel.transform;
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

    public static void BakeLineDebuger(GameObject lineObj)
    {
        var CreatedLineRenderer = lineObj.GetComponent<LineRenderer>();
        Material LineMat = CreatedLineRenderer.material;
        var meshFilter = lineObj.AddComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        CreatedLineRenderer.BakeMesh(mesh);
        meshFilter.sharedMesh = mesh;

        var meshRenderer = lineObj.AddComponent<MeshRenderer>();
        meshRenderer.material = LineMat;

        GameObject.Destroy(CreatedLineRenderer);
    }
}