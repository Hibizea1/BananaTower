#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class BuildingBoutonHandler : MonoBehaviour
{
    [SerializeField] BuildingObjectBase item;
    Button _button;
    BuildingCreator _creator;
    public SetBuildingPanel Panel;

    public BuildingObjectBase Item
    {
        set => item = value;
    }

    void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ButtonClicked);
        _creator = BuildingCreator.GetInstance();
    }

    void ButtonClicked()
    {
        Debug.Log("Button was Clicked : " + item.name); 
        _creator.ObjectSelected(item);
        Panel.ClosePanel.Invoke();
    }
}