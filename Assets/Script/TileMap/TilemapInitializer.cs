using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        foreach (BuildingCategory category in categoriesToCreateTilemapFor)
        {
            GameObject obj = new GameObject("Tilemap_" + category.name);


            Tilemap map = obj.AddComponent<Tilemap>();
            TilemapRenderer tr = obj.AddComponent<TilemapRenderer>();

            obj.transform.SetParent(grid);

            tr.sortingOrder = category.SortingOrder;

            category.Tilemap = map;
        }
    }
}
