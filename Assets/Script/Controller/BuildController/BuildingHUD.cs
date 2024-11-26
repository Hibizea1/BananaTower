#region

using System.Collections.Generic;
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
    readonly Dictionary<GameObject, Transform> _elementItemSlot = new Dictionary<GameObject, Transform>();


    readonly Dictionary<UiCategory, GameObject> _uiElement = new Dictionary<UiCategory, GameObject>();

    void Start()
    {
        BuildUI();
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
            var itemsParent = _elementItemSlot[_uiElement[b.UiCategory]];

            var inst = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            inst.transform.SetParent(itemsParent, false);

            var img = inst.GetComponent<Image>();
            var t = (Tile)b.Tile;
            img.sprite = t.sprite;

            var script = inst.GetComponent<BuildingBoutonHandler>();
            script.Item = b;
            script.Panel = buildingButtonPanel.GetComponent<SetBuildingPanel>();
        }
    }

    BuildingObjectBase[] GetAllBuildables()
    {
        return Resources.LoadAll<BuildingObjectBase>("Scriptable/Buildables");
    }
}