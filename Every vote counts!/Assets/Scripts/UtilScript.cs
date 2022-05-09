using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UtilScript : MonoBehaviour
{

public static void GoToScene(string sceneName)
{
   SceneManager.LoadScene(sceneName);
}

public static Vector3 SaveOffset(Transform objectToSaveFrom) //saving the offset between the object pivot and the mouse to use in the movewithmouse
{
   Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);
   mousePos = Camera.main.ScreenToWorldPoint(mousePos);
   Vector3 ballotOffset = mousePos - objectToSaveFrom.position;
   return ballotOffset;
}

public static void MoveWithMouse(Transform objectToMove, Vector3 offset) //this function drags the object with along with the mouse
{
   Vector3 mousePos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0f);
   mousePos = Camera.main.ScreenToWorldPoint(mousePos) - offset;
   objectToMove.position = new Vector3(mousePos.x, mousePos.y, 0f);
}

public static Vector3 VectorLerp(Vector3 startVector, Vector3 targetVector, float Speed) //okay so this is literally Vector3.MoveTowards, but
{ //it calculates the step for me so I don't have to right it down each time I'm using this static 
   float scaleSpeed =  Speed * Time.deltaTime;
   Vector3 newVector3 = Vector3.MoveTowards(startVector, targetVector, scaleSpeed);
   return newVector3; //I still need to use a vector3 set up line before that, because all of my checks are based on them
}

}