#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

#endregion

public class ToolController : Singleton<ToolController>
{
    readonly List<Tilemap> _tilemaps = new List<Tilemap>();

    void Start()
    {
        List<Tilemap> maps = FindObjectsByType<Tilemap>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID)
            .ToList();
        maps.ForEach(map =>
        {
            if (map.name.Contains("Tilemap_")) _tilemaps.Add(map);
        });

        _tilemaps.Sort((a, b) =>
        {
            var aRenderer = a.GetComponent<TilemapRenderer>();
            var bRenderer = b.GetComponent<TilemapRenderer>();

            return bRenderer.sortingOrder.CompareTo(aRenderer.sortingOrder);
        });
    }

    public void Eraser(Vector3Int position)
    {
        Debug.Log("Use Eraser");

        // _tilemaps.ForEach(map =>
        // {
        //     map.SetTile(position, null);
        // });

        _tilemaps.Any(map =>
        {
            if (map.HasTile(position))
            {
                map.SetTile(position, null);
                return true;
            }

            return false;
        });
    }
}