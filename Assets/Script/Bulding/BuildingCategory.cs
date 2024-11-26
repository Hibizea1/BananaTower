#region

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#endregion

public enum PlaceType
{
    None,
    Single,
    Line,
    Rectangle
}

[CreateAssetMenu(fileName = "Category", menuName = "LevelBuilding/Create Category")]
public class BuildingCategory : ScriptableObject
{
    [SerializeField] PlaceType placeType;
    [SerializeField] int sortingOrder;
    [SerializeField] List<BuildingCategory> placementRestriction = new List<BuildingCategory>();

    public List<BuildingCategory> PlacementRestriction => placementRestriction;

    public PlaceType PlaceType => placeType;

    public Tilemap Tilemap { get; set; }

    public int SortingOrder => sortingOrder;
}