#region

using UnityEngine;

#endregion

public class GetContentPanel : MonoBehaviour
{
    public Transform Content;
    [SerializeField] GameObject panel;

    public GameObject Panel
    {
        get => panel;
        set => panel = value;
    }


    public void OpenAndClose()
    {
        panel.SetActive(!panel.activeSelf);

        for (int i = 0; i < transform.parent.transform.childCount; i++)
        {
            if (transform.parent.transform.GetChild(i).transform != transform)
                transform.parent.transform.GetChild(i).gameObject.GetComponent<GetContentPanel>().Panel.SetActive(false);
        }
    }
}