#region

using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

public class TooltipTriggerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string Header;

    [SerializeField] string Content;

    // private DefaultSlot slot;
    Coroutine showTooltipCoroutine;

    void Start()
    {
        //slot = GetComponent<DefaultSlot>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // if (slot.Data != null)
        // {
        //     Header = slot.Data.nameItem;
        //     Content = slot.Count.ToString();
        //     
        //     showTooltipCoroutine = StartCoroutine(ShowTooltipWithDelay());
        // }
        // else if(slot.ItemAccepted != null)
        // {
        //     Header = slot.ItemAccepted.nameItem;
        //     Content = slot.CountNeeded.ToString();
        //     showTooltipCoroutine = StartCoroutine(ShowTooltipWithDelay());
        // }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
            Header = string.Empty;
            Content = string.Empty;
        }

        TooltipSystem.Hide();
    }

    IEnumerator ShowTooltipWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        TooltipSystem.Show(Content, Header);
    }
}