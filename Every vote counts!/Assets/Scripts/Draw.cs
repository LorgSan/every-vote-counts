using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{
    public GameObject Line;


    // Start is called before the first frame update
    void Start()
    {
        Line = Resources.Load("Line") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
