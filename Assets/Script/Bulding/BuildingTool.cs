#region

using UnityEngine;

#endregion

enum ToolType
{
    None,
    Eraser
}


[CreateAssetMenu(fileName = "Tool", menuName = "LevelBuilding/Create Tool")]
public class BuildingTool : BuildingObjectBase
{
    [SerializeField] ToolType toolType;

    void Awake()
    {
    }

    public void Use(Vector3Int position)
    {
        var t = ToolController.GetInstance();

        switch (toolType)
        {
            case ToolType.Eraser:
                t.Eraser(position);
                break;
            default:
                Debug.Log("ToolTip Not set");
                break;
        }
    }
}