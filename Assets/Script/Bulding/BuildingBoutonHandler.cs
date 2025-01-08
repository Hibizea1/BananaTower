#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class BuildingBoutonHandler : MonoBehaviour
{
    [SerializeField] BuildingObjectBase item;
    public SetBuildingPanel Panel;
    Button _button;
    BuildingCreator _creator;

    public BuildingObjectBase Item
    {
        get => item;
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
        if (MoneyManager.GetInstance().GetMoneyCount() >= item.BananaCost)
        {
            Debug.Log("Button was Clicked : " + item.name);
            _creator.ObjectSelected(item);
            Panel.ClosePanel.Invoke();
        }
        else
        {
            TextScroller.GetInstance().LaunchTextScroll();
        }
    }
}