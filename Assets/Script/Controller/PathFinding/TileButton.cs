#region

using UnityEngine;

#endregion

public class TileButton : MonoBehaviour
{
    [SerializeField] TileType tileType;

    public TileType TileType
    {
        get => tileType;
        set => tileType = value;
    }
}