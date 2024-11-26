#region

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

[ExecuteInEditMode]
public class Tooltip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI HeaderField;
    [SerializeField] TextMeshProUGUI ContentField;
    [SerializeField] LayoutElement LayoutElement;
    [SerializeField] int CharacterWrapLimit;


    void Update()
    {
        if (Application.isEditor)
        {
            var headerLength = HeaderField.text.Length;
            var contentLength = ContentField.text.Length;

            LayoutElement.enabled =
                headerLength > CharacterWrapLimit || contentLength > CharacterWrapLimit ? true : false;
        }
    }


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

        var headerLength = HeaderField.text.Length;
        var contentLength = ContentField.text.Length;

        LayoutElement.enabled =
            headerLength > CharacterWrapLimit || contentLength > CharacterWrapLimit ? true : false;
    }
}