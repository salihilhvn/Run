using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("Transition Settings")]
    [Tooltip("Sahneler arası erime (dissolve) süresi saniye cinsinden")]
    [SerializeField] private float transitionDuration = 1.0f;

    private Canvas transitionCanvas;
    private RawImage transitionImage;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupTransitionCanvas();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SetupTransitionCanvas()
    {
        // Geçişler için en üstte duracak görünmez bir Canvas üretiyoruz
        GameObject canvasObj = new GameObject("TransitionCanvas");
        canvasObj.transform.SetParent(transform);

        transitionCanvas = canvasObj.AddComponent<Canvas>();
        transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        transitionCanvas.sortingOrder = 999; // Her şeyin en üstünde olması çok önemli

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);

        GameObject imageObj = new GameObject("TransitionImage");
        imageObj.transform.SetParent(canvasObj.transform, false);

        transitionImage = imageObj.AddComponent<RawImage>();
        transitionImage.rectTransform.anchorMin = Vector2.zero;
        transitionImage.rectTransform.anchorMax = Vector2.one;
        transitionImage.rectTransform.offsetMin = Vector2.zero;
        transitionImage.rectTransform.offsetMax = Vector2.zero;
        
        transitionImage.raycastTarget = false; // Tıklamaları engellemesin
        transitionImage.color = new Color(1, 1, 1, 0); // Başlangıçta görünmez (Alpha 0)
        transitionImage.gameObject.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        // Eğer halihazırda bir geçiş varsa, oyuncunun üst üste butonlara basıp buga sokmasını engelle
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;

        // 1. Ekran tam çizilene kadar bekle
        yield return new WaitForEndOfFrame();
        
        // 2. Fotoğrafı çek (Sevdiğin smooth crossfade geçişine geri dönüyoruz)
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        
        // 3. Ekrana yapıştır. 
        // O gıcık parlama (Exposure) patlamasını engellemek için, ekranı tam beyaz değil 
        // %15 daha koyu (0.85) bir filtreyle basıyoruz. Bu sayede patlama sönümlenmiş oluyor.
        transitionImage.texture = screenshot;
        transitionImage.color = new Color(0.85f, 0.85f, 0.85f, 1f); 
        transitionImage.gameObject.SetActive(true);

        // 4. Arkadan yeni sahneyi asenkron yükle
        yield return SceneManager.LoadSceneAsync(sceneName);

        // 5. Resmi yavaşça erit (Dissolve)
        yield return transitionImage.DOFade(0f, transitionDuration).SetEase(Ease.InOutQuad).WaitForCompletion();

        // 6. Temizlik
        transitionImage.gameObject.SetActive(false);
        Destroy(screenshot);
        
        isTransitioning = false;
    }
}
