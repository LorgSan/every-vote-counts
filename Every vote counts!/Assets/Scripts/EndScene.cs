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

        if (OnPlace==false)
        {
            float step = 8f* Time.deltaTime;
            Vector3 certificateMove = new Vector3 (certificate.transform.position.x, -0.9f, 1f);
            certificate.transform.position = Vector3.MoveTowards(certificate.transform.position, certificateMove, step);  
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
                    Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);
                    mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                    offset = mousePos - certificate.transform.position;
                }

                if (Input.GetMouseButton(0))
                {
                    Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);
                    mousePos = Camera.main.ScreenToWorldPoint(mousePos) - offset;
                    certificate.transform.position = new Vector3(mousePos.x, mousePos.y, certificatePos.z);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    certificate.transform.position = certificatePos;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);
                    mousePos = Camera.main.ScreenToWorldPoint(mousePos);
                    offset = mousePos - certificate.transform.position;
                    certificate.transform.localScale = new Vector3(90f, 90f, 1f);
                }
                if (Input.GetMouseButton(1))
                { 
                    Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);
                    mousePos = Camera.main.ScreenToWorldPoint(mousePos) - offset;
                    certificate.transform.position = new Vector3(mousePos.x, mousePos.y, certificatePos.z);
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
