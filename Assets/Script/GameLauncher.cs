using System;
using Script.Controller.Save;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLauncher : MonoBehaviour
{
    [SerializeField] Button LoadButton;
    
    public void Play()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    void Update()
    {
        // if (SaveHandler.GetInstance().HasSave())
        // {
        //     LoadButton.interactable = true;
        // }
        // else
        // {
        //     LoadButton.interactable = false;
        //     Debug.Log("No save file found.");
        // }
    }

    public void Load()
    {
        SaveHandler.GetInstance().OnLoad();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
