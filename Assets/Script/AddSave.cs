using Script.Controller.Save;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void SaveAndReturnToScene0()
    {
        SceneManager.LoadScene(0);
    }  
}