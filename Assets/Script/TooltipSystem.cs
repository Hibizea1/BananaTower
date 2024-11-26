#region

using UnityEngine;

#endregion

public class TooltipSystem : MonoBehaviour
{
    static TooltipSystem sCurrent;

    [SerializeField] Tooltip Tooltip;

    public void Awake()
    {
        sCurrent = this;
    }

    public static void Show(string _content, string _header = "")
    {
        sCurrent.Tooltip.SetText(_content, _header);
        sCurrent.Tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        sCurrent.Tooltip.gameObject.SetActive(false);
    }
}