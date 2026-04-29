using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    [Header("Fail Panel Ayarları")]
    [Tooltip("Arka planı hafif karartan ana Fail Panel objesi")]
    [SerializeField] private GameObject failPanel;
    
    [Tooltip("Animasyonla yaylanarak çıkacak olan ortadaki asıl Pop-up penceresi")]
    [SerializeField] private RectTransform failPopupContent;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Oyun başlarken paneli kapalı tut
        if (failPanel != null)
            failPanel.SetActive(false);
    }

    public void ShowFailPanel()
    {
        if (failPanel == null) return;

        // Oyunu durdur (Karakter, engeller vs. dursun)
        Time.timeScale = 0f;
        
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
