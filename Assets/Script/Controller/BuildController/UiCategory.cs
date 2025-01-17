#region

using UnityEngine;

#endregion

[CreateAssetMenu(fileName = "UICategory", menuName = "LevelBuilding/Create UI Category")]
public class UiCategory : ScriptableObject
{
    [SerializeField] int siblingIndex;
    [SerializeField] Color backgroundColor;

    public int SiblingIndex => siblingIndex;

    public Color BackgroundColor => backgroundColor;
}