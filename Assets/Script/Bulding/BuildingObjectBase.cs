#region

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#endregion

public enum Category
{
    Turret,
    Building,
    Trap,
    Tool
}

[CreateAssetMenu(fileName = "Buildable", menuName = "BuildingObject/Create Buildable")]
public class BuildingObjectBase : ScriptableObject
{
    [SerializeField] BuildingCategory category;
    [SerializeField] UiCategory uiCategory;
    [SerializeField] TileBase tileBase;
    [SerializeField] PlaceType placeType;
    [SerializeField] bool usePlacementRestriction;
    [SerializeField] List<BuildingCategory> placementRestriction = new List<BuildingCategory>();

    public List<BuildingCategory> PlacementRestriction =>
        usePlacementRestriction ? placementRestriction : category.PlacementRestriction;


    public TileBase Tile => tileBase;

    public BuildingCategory Category => category;

    public PlaceType PlaceType => placeType;

    public UiCategory UiCategory => uiCategory;

    public bool UsePlacementRestriction => usePlacementRestriction;
}