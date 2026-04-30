using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("Pause Panel Ayarları")]
    [Tooltip("Arka planı hafif karartan ana Pause Panel objesi")]
    [SerializeField] private GameObject pausePanel;
    
    [Tooltip("Animasyonla yaylanarak çıkacak olan ortadaki asıl Pause Pop-up penceresi")]
    [SerializeField] private RectTransform pausePopupContent;

    [Header("Fail Panel Ayarları")]
    [Tooltip("Arka planı hafif karartan ana Fail Panel objesi")]
    [SerializeField] private GameObject failPanel;
    
    [Tooltip("Animasyonla yaylanarak çıkacak olan ortadaki asıl Pop-up penceresi")]
    [SerializeField] private RectTransform failPopupContent;

    [Tooltip("Sadece ilk ölümde çıkacak olan Reklam (Continue) Butonu")]
    [SerializeField] private GameObject continueAdButton;

    private bool hasUsedRevive = false; // Oyuncu dirilme hakkını kullandı mı?

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Oyun başlarken panelleri kapalı tut
        if (failPanel != null)
            failPanel.SetActive(false);
            
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    // --- PAUSE MENU (DURDURMA) MANTIKLARI ---

    // Sol üstteki Pause butonuna (veya ESC tuşuna) basıldığında çalışır
    public void OnPauseClicked()
    {
        if (pausePanel == null) return;

        // Oyunu durdur
        Time.timeScale = 0f;
        
        // Paneli aktif et
        pausePanel.SetActive(true);

        if (pausePopupContent != null)
        {
            // Pop-up animasyonu için önce boyutunu sıfırla
            pausePopupContent.localScale = Vector3.zero;
            
            // Animasyon oynat (oyun durmuş olsa bile)
            pausePopupContent.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    // Pause menüsündeki "Resume" (Devam Et) butonuna basıldığında çalışır
    public void OnResumeClicked()
    {
        if (pausePopupContent != null)
        {
            // Önce paneli animasyonla küçülterek kapat, tamamen kapanınca oyunu devam ettir
            pausePopupContent.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                pausePanel.SetActive(false);
                Time.timeScale = 1f; // Zamanı normal akışına çevir
            });
        }
        else
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    // --- FAIL MENU (ÖLÜM) MANTIKLARI ---

    public void ShowFailPanel()
    {
        if (failPanel == null) return;

        // Oyunu durdur (Karakter, engeller vs. dursun)
        Time.timeScale = 0f;

        // Eğer oyuncu daha önce reklam izleyip canlandıysa, reklam butonunu gizle!
        if (continueAdButton != null)
        {
            continueAdButton.SetActive(!hasUsedRevive);
        }
        
        // Paneli aktif et
        failPanel.SetActive(true);

        if (failPopupContent != null)
        {
            // Pop-up animasyonu için önce boyutunu sıfırla
            failPopupContent.localScale = Vector3.zero;
            
            // Oyun durmuş olsa bile (timeScale = 0) animasyonun oynaması için SetUpdate(true) KULLANILMALIDIR!
            failPopupContent.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
        }
    }

    // Try Again Butonuna bağlanacak
    public void OnTryAgainClicked()
    {
        // Zamanı geri normal akışına çevir
        Time.timeScale = 1f;
        
        // Mevcut sahneyi yeniden yükle
        if (SceneController.Instance != null)
            SceneController.Instance.LoadScene(SceneManager.GetActiveScene().name);
        else
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Back to Menu Butonuna bağlanacak
    public void OnBackToMenuClicked()
    {
        // Zamanı geri normal akışına çevir
        Time.timeScale = 1f;
        
        if (SceneController.Instance != null)
            SceneController.Instance.LoadScene("MainMenu");
        else
            SceneManager.LoadScene("MainMenu");
    }

    // Continue (Watch AD) Butonuna bağlanacak
    public void OnContinueAdClicked()
    {
        // Zamanı geri düzelt ki oyun aksın
        Time.timeScale = 1f;

        // Oyuncu dirilme hakkını kullandı, bir sonraki ölümde buton çıkmayacak!
        hasUsedRevive = true;

        if (failPopupContent != null)
        {
            // Önce paneli küçülterek kapat, kapanma bitince can ver
            failPopupContent.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
            {
                failPanel.SetActive(false);
                RevivePlayer();
            });
        }
        else
        {
            failPanel.SetActive(false);
            RevivePlayer();
        }
    }

    private void RevivePlayer()
    {
        // Sahnedeki oyuncuyu bul ve can ver (ileride Ad Manager tetiklenir)
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Revive(50); // Şimdilik 50 can ile diriltiyoruz
        }
    }
}
