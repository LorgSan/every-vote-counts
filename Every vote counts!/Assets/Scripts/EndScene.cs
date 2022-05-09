using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScene : MonoBehaviour
{
    [SerializeField] GameObject certificate;
    Vector3 certificatePos;
    bool OnPlace = false;
    Vector3 offset;
    Vector3 certificateScale;

    void Update()
    {
        if (Input.GetKey(KeyCode.R)) //it's just an easy fast scene reloading
        {
            UtilScript.GoToScene("StartScene");
        }

        if (OnPlace==false) //decided not to make a statemachine here and just use this bool
        {
            Vector3 certificateMove = new Vector3 (certificate.transform.position.x, -0.9f, 1f);
            certificate.transform.position = UtilScript.VectorLerp(certificate.transform.position, certificateMove, 8f); 
            if (certificate.transform.position == certificateMove)
            {
                certificatePos = certificate.transform.position;
                certificateScale = certificate.transform.localScale;
                OnPlace = true;
            }
        } else 
        {   
            Collider2D col = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (col!=null)
            {
                if (Input.GetMouseButtonDown(0))
                {   
                    offset = UtilScript.SaveOffset(certificate.transform);
                }

                if (Input.GetMouseButton(0))
                {
                    UtilScript.MoveWithMouse(certificate.transform, offset);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    certificate.transform.position = certificatePos;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    offset = UtilScript.SaveOffset(certificate.transform);
                    certificate.transform.localScale = new Vector3(90f, 90f, 1f);
                }
                if (Input.GetMouseButton(1))
                { 
                    UtilScript.MoveWithMouse(certificate.transform, offset);
                }
                if (Input.GetMouseButtonUp(1))
                {
                    certificate.transform.localScale = certificateScale;
                    certificate.transform.position = certificatePos;
                }
            }
        }
        
    }
}
