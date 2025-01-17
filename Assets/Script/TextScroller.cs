using System.Collections;
using TMPro;
using UnityEngine;

public class TextScroller : Singleton<TextScroller>
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] float scrollDuration = 3f;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float scrollSpeed = 50f;
    bool _isLaunched;

    public void LaunchTextScroll()
    {
        if (!_isLaunched)
        {
            StartCoroutine(ScrollAndFadeText());
            _isLaunched = true;
        }
    }

    IEnumerator ScrollAndFadeText()
    {
        text.enabled = true;
        float elapsedTime = 0f;
        Vector3 originalPosition = text.transform.position;
        Vector3 resetPosition = text.transform.position;

        // Scroll down
        while (elapsedTime < scrollDuration)
        {
            text.transform.position =
                originalPosition - new Vector3(0, scrollSpeed * (elapsedTime / scrollDuration), 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Fade out
        elapsedTime = 0f;
        Color originalColor = text.color;
        while (elapsedTime < fadeDuration)
        {
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b,
                Mathf.Lerp(1, 0, elapsedTime / fadeDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        text.enabled = false;
        text.transform.position = resetPosition;
        text.color = Color.white;
        _isLaunched = false;
    }
}