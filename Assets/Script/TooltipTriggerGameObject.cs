using System.Collections;
using UnityEngine;

public class TooltipTriggerGameObject : MonoBehaviour
{
    [SerializeField] private string Header;
    [SerializeField] private string Content;
    private Coroutine showTooltipCoroutine;


    public void OnMouseEnter()
    {
        showTooltipCoroutine = StartCoroutine(ShowTooltipWithDelay());
    }

    public void OnMouseExit()
    {
        if (showTooltipCoroutine != null)
        {
            StopCoroutine(showTooltipCoroutine);
        }

        TooltipSystem.Hide();
    }

    private IEnumerator ShowTooltipWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        TooltipSystem.Show(Content, Header);
    }
}