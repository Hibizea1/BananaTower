#region

using UnityEngine;
using UnityEngine.Events;

#endregion

public class SetBuildingPanel : MonoBehaviour
{
    public UnityEvent ClosePanel;

    [SerializeField] GameObject categories;

    void Start()
    {
        ClosePanel.AddListener(OpenAndClose);
    }

    public void OpenAndClose()
    {
        categories.SetActive(!categories.activeSelf);
    }
}