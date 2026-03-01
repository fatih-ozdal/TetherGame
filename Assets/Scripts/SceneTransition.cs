using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private Color fadeColor = Color.black;

    [Header("Auto Setup")]
    [SerializeField] private bool autoCreateFadeImage = true;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Auto-create fade image if needed
        if (fadeImage == null && autoCreateFadeImage)
        {
            CreateFadeCanvas();
        }

        // Start with transparent
        if (fadeImage != null)
        {
            SetFadeAlpha(0f);
        }
    }

    private void Start()
    {
        // Fade in when scene starts
        if (fadeImage != null)
        {
            StartCoroutine(FadeIn());
        }
    }

    private void CreateFadeCanvas()
    {
        // Create a canvas for the fade image
        GameObject canvasObj = new GameObject("TransitionCanvas");
        canvasObj.transform.SetParent(transform);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Always on top

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // Create the fade image
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);

        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = fadeColor;

        RectTransform rect = imageObj.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneWithFade(sceneName));
    }

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneWithFade(sceneIndex));
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        // Fade out
        yield return StartCoroutine(FadeOut());

        // Load scene
        SceneManager.LoadScene(sceneName);

        // Fade in
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator LoadSceneWithFade(int sceneIndex)
    {
        // Fade out
        yield return StartCoroutine(FadeOut());

        // Load scene
        SceneManager.LoadScene(sceneIndex);

        // Fade in
        yield return StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(1f);
    }

    private IEnumerator FadeIn()
    {
        if (fadeImage == null) yield break;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }

        SetFadeAlpha(0f);
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = alpha;
            fadeImage.color = color;
        }
    }
}
