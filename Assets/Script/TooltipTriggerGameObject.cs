#region

using System.Collections;
using UnityEngine;

#endregion

public class TooltipTriggerGameObject : MonoBehaviour
{
    [SerializeField] string Header;
    [SerializeField] string Content;
    Coroutine showTooltipCoroutine;


    public void OnMouseEnter()
    {
        showTooltipCoroutine = StartCoroutine(ShowTooltipWithDelay());
    }

    public void OnMouseExit()
    {
        if (showTooltipCoroutine != null) StopCoroutine(showTooltipCoroutine);

        TooltipSystem.Hide();
    }

    IEnumerator ShowTooltipWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        TooltipSystem.Show(Content, Header);
    }
}