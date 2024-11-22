using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TooltipTriggerUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string Header;
    [SerializeField] private string Content;
    // private DefaultSlot slot;
    private Coroutine showTooltipCoroutine;

    private void Start()
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

    private IEnumerator ShowTooltipWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        TooltipSystem.Show(Content, Header);
    }

}