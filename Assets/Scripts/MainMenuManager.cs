using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // [Header("Pop-up Panelleri")]
    // İleride buraya Pop-up ekranlarının referanslarını koyacağız
    // public GameObject levelsPanel;
    // public GameObject settingsPanel;

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
        Debug.Log("STORE Butonuna Basıldı -> Market paneli açılacak...");
    }

    public void OnCollectionClicked()
    {
        Debug.Log("COLLECTION Butonuna Basıldı -> Koleksiyon paneli açılacak...");
    }

    public void OnCharacterClicked()
    {
        Debug.Log("CHARACTER Butonuna Basıldı -> Karakter seçimi / Özelleştirme paneli açılacak...");
    }
}
