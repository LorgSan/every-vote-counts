using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    public void ChangeScene(string sceneName) //this exists because animation events didn't want to use static function of the util script:(
    {
    SceneManager.LoadScene(sceneName);
    }
}
