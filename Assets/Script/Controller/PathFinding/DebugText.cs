#region

using TMPro;
using UnityEngine;

#endregion

public class DebugText : MonoBehaviour
{
    [SerializeField] RectTransform arrow;

    [SerializeField] TextMeshProUGUI f, g, h, p;

    public TextMeshProUGUI F
    {
        get => f;
        set => f = value;
    }

    public TextMeshProUGUI G
    {
        get => g;
        set => g = value;
    }

    public TextMeshProUGUI H
    {
        get => h;
        set => h = value;
    }

    public TextMeshProUGUI P
    {
        get => p;
        set => p = value;
    }


    public RectTransform Arrow
    {
        get => arrow;
        set => arrow = value;
    }
}