using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject storePanel;
    public GameObject characterPanel; // Karakter ekranı için panel referansı
    
    [Header("Top Bar UI")]
    public TMPro.TMP_Text globalCoinText;
    public TMPro.TMP_Text globalDiamondText;

    private void Start()
    {
        UpdateCurrencyUI();

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged += UpdateCurrencyUI;
        }
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateCurrencyUI;
        }
    }

    private void UpdateCurrencyUI()
    {
        if (CurrencyManager.Instance != null)
        {
            if (globalCoinText != null) globalCoinText.text = CurrencyManager.Instance.TotalCoins.ToString();
            if (globalDiamondText != null) globalDiamondText.text = CurrencyManager.Instance.TotalDiamonds.ToString();
        }
    }

    public void OnPlayClicked()
    {
        Debug.Log("PLAY Butonuna Basıldı -> Oyun Sahnesine Geçiliyor...");
        
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadScene("GameScene");
        }
        else
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    public void OnLevelsClicked()
    {
        Debug.Log("LEVELS Butonuna Basıldı -> Levels Ekranı Açılacak");
        // Örn: levelsPanel.SetActive(true);
    }

    public void OnSettingsClicked()
    {
        Debug.Log("AYARLAR Butonuna Basıldı -> Ayarlar Pop-up'ı");
    }

    public void OnNotificationClicked()
    {
        Debug.Log("BİLDİRİM Butonuna Basıldı");
    }

    // İstersen sağ üstteki elmas veya altın artı (+) butonları için de fonksiyonlar
    public void OnBuyCoinsClicked()
    {
        Debug.Log("ALTIN ALMA Butonuna Basıldı (Market)");
    }

    // --- YENİ EKLENEN BUTONLAR ---

    public void OnRankClicked()
    {
        Debug.Log("RANK Butonuna Basıldı -> Liderlik / Sıralama ekranı açılacak...");
    }

    public void OnStoreClicked()
    {
        Debug.Log("STORE Butonuna Basıldı -> Market paneli açılıyor...");
        if (storePanel != null)
        {
            storePanel.SetActive(true);
        }
    }

    public void CloseStorePanel()
    {
        Debug.Log("STORE paneli kapatılıyor...");
        if (storePanel != null)
        {
            storePanel.SetActive(false);
        }
    }

    public void OnCollectionClicked()
    {
        Debug.Log("COLLECTION Butonuna Basıldı -> Koleksiyon paneli açılacak...");
    }

    public void OnCharacterClicked()
    {
        Debug.Log("CHARACTER Butonuna Basıldı -> Karakter geliştirme paneli açılıyor...");
        if (characterPanel != null)
        {
            characterPanel.SetActive(true);
        }
    }

    public void CloseCharacterPanel()
    {
        Debug.Log("CHARACTER paneli kapatılıyor...");
        if (characterPanel != null)
        {
            characterPanel.SetActive(false);
        }
    }
}
