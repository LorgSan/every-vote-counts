using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{

    #region Declaring

    public float ChargePower = 0;
    public float force = 100f;

    #endregion



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        InputCheck();
    }

    void InputCheck(){
        if (Input.GetMouseButton(0))
        {
            ChargePower += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0)){
            Debug.Log("spit");
            ChargePower = 0;
        }
    }
}
