#region

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#endregion

public class TilemapInitializer : Singleton<TilemapInitializer>
{
    [SerializeField] List<BuildingCategory> categoriesToCreateTilemapFor;
    [SerializeField] Transform grid;

    void Start()
    {
        CreateMap();
    }

    void CreateMap()
    {
        foreach (var category in categoriesToCreateTilemapFor)
        {
            var obj = new GameObject("Tilemap_" + category.name);


            var map = obj.AddComponent<Tilemap>();
            var tr = obj.AddComponent<TilemapRenderer>();

            obj.transform.SetParent(grid);

            tr.sortingOrder = category.SortingOrder;

            category.Tilemap = map;
        }
    }
}