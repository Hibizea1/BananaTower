#region

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

#endregion

public class BuildingBoutonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
            SetButtonAfterClick.GetInstance().ActiveButton();
        }
        else
        {
            TextScroller.GetInstance().LaunchTextScroll();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetInformation.GetInstance().Show();
        SetInformation.GetInstance().SetPicture(item.Picture1);
        if (!item.IsWall && item.CategoryType1 != Category.Tool)
        {
            Tile itemTile = (Tile)item.Tile;
            Turret turret = itemTile.gameObject.GetComponent<Turret>();
            SetInformation.GetInstance().SetText(item.name.ToString(), item.BananaCost, turret.Damage,
                turret.MagazineSize,
                turret.ReloadTime);
        }
        else if (item.CategoryType1 != Category.Tool)
        {
            SetInformation.GetInstance().SetText(item.name.ToString(), item.BananaCost);
        }
        else
        {
            SetInformation.GetInstance().SetText(item.name);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetInformation.GetInstance().Hide();
    }
}