#region

using UnityEngine;

#endregion

public class GetContentPanel : MonoBehaviour
{
    public Transform Content;
    [SerializeField] GameObject panel;


    public void OpenAndClose()
    {
        panel.SetActive(!panel.activeSelf);
    }
}