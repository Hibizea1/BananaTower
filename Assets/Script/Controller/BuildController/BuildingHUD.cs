using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class BuildingHUD : Singleton<BuildingHUD>
{
    [SerializeField] List<UiCategory> categories = new List<UiCategory>();
    [SerializeField] Transform wrapperElement;
    [SerializeField] GameObject categoryPrefab;
    [SerializeField] GameObject itemPrefab;


    Dictionary<UiCategory, GameObject> _uiElement = new Dictionary<UiCategory, GameObject>();
    Dictionary<GameObject, Transform> _elementItemSlot = new Dictionary<GameObject, Transform>();

    void Start()
    {
        BuildUI();
    }

    private void BuildUI()
    {
        foreach (UiCategory cat in categories)
        {
            if (!_uiElement.ContainsKey(cat))
            {
                var inst = Instantiate(categoryPrefab, Vector3.zero, Quaternion.identity);
                inst.transform.SetParent(wrapperElement, false);

                _uiElement[cat] = inst;
                _elementItemSlot[inst] = inst.transform.Find("Items");
            }

            _uiElement[cat].name = cat.name;

            TextMeshProUGUI text = _uiElement[cat].GetComponentInChildren<TextMeshProUGUI>();
            text.text = cat.name;

            Image img = _uiElement[cat].GetComponentInChildren<Image>();
            img.color = cat.BackgroundColor;
        }

        BuildingObjectBase[] buildables = GetAllBuildables();

        foreach (BuildingObjectBase b in buildables)
        {
            if (b.UiCategory == null)
            {
                continue;
            }
            var itemsParent = _elementItemSlot[_uiElement[b.UiCategory]];

            var inst = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            inst.transform.SetParent(itemsParent, false);

            Image img = inst.GetComponent<Image>();
            Tile t = (Tile)b.Tile;
            img.sprite = t.sprite;

            var script = inst.GetComponent<BuildingBoutonHandler>();
            script.Item = b;
        }
    }

    private BuildingObjectBase[] GetAllBuildables()
    {
        return Resources.LoadAll<BuildingObjectBase>("Scriptable/Buildables");
    }
}