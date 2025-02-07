using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FadeInImage : MonoBehaviour
{
    public Image imageToFade;             // Main image (death screen)
    public CanvasGroup retryButtonCanvas; // Retry button and background (uses CanvasGroup)
    public Button retryButton;            // Retry button
    public TextMeshProUGUI retrytxt;      // Retry button text (TextMeshPro)

    public float fadeSpeed = 2f;          // Speed of fading for the image
    public float buttonFadeDuration = 3f; // Duration for retry button fade-in
    public float textFadeDuration = 3f; // Duration for retry text fade-in

    private float currentAlpha = 0f;
    public bool fadeInTriggered = false;
    private bool imageFullyVisible = false;

    void Start()
    {
        currentAlpha = 0f;
        SetImageAlpha(currentAlpha);

        // Set retry button UI and text to invisible & non-interactable initially
        retryButtonCanvas.alpha = 0f;
        retrytxt.alpha = 0f;  // TextMeshPro uses `alpha`
        retryButton.interactable = false;
    }

    void Update()
    {
        if (fadeInTriggered && !imageFullyVisible)
        {
            currentAlpha += fadeSpeed * Time.deltaTime;
            currentAlpha = Mathf.Clamp01(currentAlpha);
            SetImageAlpha(currentAlpha);

            if (currentAlpha >= 1f)
            {
                imageFullyVisible = true;
                fadeInTriggered = false;

                // Start fading in retry button first, then fade in text separately
                StartCoroutine(FadeInRetryButton());
            }
        }
    }

    public void SetImageAlpha(float alpha)
    {
        if (imageToFade != null)
        {
            Color imageColor = imageToFade.color;
            imageColor.a = alpha;
            imageToFade.color = imageColor;
        }
        else
        {
            Debug.LogError("Image reference is missing on " + gameObject.name);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            fadeInTriggered = true;
        }
    }

    private IEnumerator FadeInRetryButton()
    {
        float elapsedTime = 0f;
        while (elapsedTime < buttonFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            retryButtonCanvas.alpha = Mathf.Lerp(0, 1, elapsedTime / buttonFadeDuration);
            yield return null;
        }

        retryButtonCanvas.alpha = 1f;
        // Enable button after fade-in

        // Start fading in text separately
        StartCoroutine(FadeInRetryText());
    }

    private IEnumerator FadeInRetryText()
    {
        float elapsedTime = 0f;
        while (elapsedTime < textFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            retrytxt.alpha = Mathf.Lerp(0, 1, elapsedTime / textFadeDuration);
            yield return null;
        }

        retrytxt.alpha = 1f;
        retryButton.interactable = true;
    }
}
