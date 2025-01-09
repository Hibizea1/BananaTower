#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

#endregion

public class BuildingHUD : Singleton<BuildingHUD>
{
    [SerializeField] List<UiCategory> categories = new List<UiCategory>();
    [SerializeField] Transform wrapperElement;
    [SerializeField] GameObject categoryPrefab;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject buildingButtonPanel;
    [SerializeField] bool debugAll;
    [SerializeField] Button BanaDerButton;
    [SerializeField] List<Turret> Turrets = new List<Turret>();

    public List<Turret> Turrets1
    {
        get => Turrets;
        set => Turrets = value;
    }

    readonly Dictionary<GameObject, Transform> _elementItemSlot = new Dictionary<GameObject, Transform>();


    readonly Dictionary<UiCategory, GameObject> _uiElement = new Dictionary<UiCategory, GameObject>();

    protected override void Awake()
    {
        base.Awake();
        EventMaster.GetInstance().CreateNewEvent("DebugMode");
        EventMaster.GetInstance().GetEvent("DebugMode").AddListener(DebugMode);
        EventMaster.GetInstance().CreateNewEvent("ActiveBanaDer");
    }

    void Update()
    {
        if (Turrets.Count > 0)
        {
            BanaDerButton.interactable = true;
        }
    }

    public void ActiveBanaDer()
    {
        EventMaster.GetInstance().InvokeEvent("ActiveBanaDer");
    }
    
    void Start()
    {
        BuildUI();
    }

    void DebugMode()
    {
        _uiElement[categories[1]].SetActive(!_uiElement[categories[1]].activeSelf);
    }

    void BuildUI()
    {
        foreach (var cat in categories)
        {
            if (!_uiElement.ContainsKey(cat))
            {
                var inst = Instantiate(categoryPrefab, Vector3.zero, Quaternion.identity);
                inst.transform.SetParent(wrapperElement, false);

                _uiElement[cat] = inst;
                _elementItemSlot[inst] = inst.GetComponent<GetContentPanel>().Content;
            }

            _uiElement[cat].name = cat.name;

            var text = _uiElement[cat].GetComponentInChildren<TextMeshProUGUI>();
            text.text = cat.name;

            var img = _uiElement[cat].GetComponentInChildren<Image>();
            img.color = cat.BackgroundColor;
        }

        BuildingObjectBase[] buildables = GetAllBuildables();

        foreach (var b in buildables)
        {
            if (b.UiCategory == null) continue;
            if (b.IsInvisible) continue;
            if (b.UiCategory == categories[1])
            {
                var itemsParent = _elementItemSlot[_uiElement[b.UiCategory]];

                var inst = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                inst.transform.SetParent(itemsParent, false);
                inst.AddComponent<TileButton>();

                if (b.IsWall)
                {
                    var t = (RuleTile)b.Tile;
                    var img = inst.GetComponent<Image>();
                    img.sprite = t.m_DefaultSprite;
                    var script = inst.GetComponent<BuildingBoutonHandler>();
                    script.Item = b;
                    script.Panel = buildingButtonPanel.GetComponent<SetBuildingPanel>();
                    var btn = inst.GetComponent<TileButton>();
                    btn.TileType = ((AStarTileRule)script.Item.Tile).Type;
                }
                else
                {
                    var t = (Tile)b.Tile;
                    var img = inst.GetComponent<Image>();
                    img.sprite = t.sprite;
                    var script = inst.GetComponent<BuildingBoutonHandler>();
                    script.Item = b;
                    script.Panel = buildingButtonPanel.GetComponent<SetBuildingPanel>();
                    var btn = inst.GetComponent<TileButton>();
                    btn.TileType = ((AStarTile)script.Item.Tile).Type;
                }

                if (debugAll)
                {
                    print("Type is : " + inst.GetComponent<TileButton>().TileType);
                }
            }
            else
            {
                
                
                if (b.IsWall)
                {                
                    var itemsParent = _elementItemSlot[_uiElement[b.UiCategory]];
                    var inst = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                    inst.transform.SetParent(itemsParent, false);
                    inst.AddComponent<TileButton>();
                    var t = (RuleTile)b.Tile;
                    var img = inst.GetComponent<Image>();
                    img.sprite = t.m_DefaultSprite;
                    var script = inst.GetComponent<BuildingBoutonHandler>();
                    script.Item = b;
                    script.Panel = buildingButtonPanel.GetComponent<SetBuildingPanel>();
                    var btn = inst.GetComponent<TileButton>();
                    btn.TileType = ((AStarTileRule)script.Item.Tile).Type;
                }
                else
                {
                    Transform itemsParent = _elementItemSlot[_uiElement[b.UiCategory]];
                    GameObject inst = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                    inst.transform.SetParent(itemsParent, false);
                    Image img = inst.GetComponent<Image>();
                    Tile t = (Tile)b.Tile;
                    img.sprite = t.sprite;
                    BuildingBoutonHandler script = inst.GetComponent<BuildingBoutonHandler>();
                    script.Item = b;
                    script.Panel = buildingButtonPanel.GetComponent<SetBuildingPanel>();
                }
                
            }
        }
        _uiElement[categories[1]].SetActive(!_uiElement[categories[1]].activeSelf);

    }

    BuildingObjectBase[] GetAllBuildables()
    {
        return Resources.LoadAll<BuildingObjectBase>("Scriptable/Buildables");
    }
}