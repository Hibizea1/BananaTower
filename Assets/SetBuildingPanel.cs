using System;
using UnityEngine;
using UnityEngine.Events;

public class SetBuildingPanel : MonoBehaviour
{

    public UnityEvent ClosePanel;
    
    void Start()
    {
        ClosePanel.AddListener(OpenAndClose);
    }

    [SerializeField] GameObject categories;
    public void OpenAndClose()
    {
        categories.SetActive(!categories.activeSelf);
    }
}
