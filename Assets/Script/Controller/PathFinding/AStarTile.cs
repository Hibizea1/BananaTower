#region

using UnityEngine;
using UnityEngine.Tilemaps;

#endregion

[CreateAssetMenu(fileName = "New AStarTile", menuName = "Tiles/AStarTile")]
public class AStarTile : Tile
{
    public TileType Type;
}