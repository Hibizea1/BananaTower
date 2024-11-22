using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI HeaderField;
    [SerializeField] private TextMeshProUGUI ContentField;
    [SerializeField] private LayoutElement LayoutElement;
    [SerializeField] private int CharacterWrapLimit;


    public void SetText(string _content, string _header = "")
    {
        Vector2 position = Input.mousePosition;

        if (string.IsNullOrEmpty(_header))
        {
            HeaderField.gameObject.SetActive(false);
        }
        else
        {
            HeaderField.gameObject.SetActive(true);
            HeaderField.text = _header;
        }

        ContentField.text = _content;

        int headerLength = HeaderField.text.Length;
        int contentLength = ContentField.text.Length;

        LayoutElement.enabled =
            (headerLength > CharacterWrapLimit || contentLength > CharacterWrapLimit) ? true : false;
    }


    private void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = HeaderField.text.Length;
            int contentLength = ContentField.text.Length;

            LayoutElement.enabled =
                (headerLength > CharacterWrapLimit || contentLength > CharacterWrapLimit) ? true : false;
        }
    }
}