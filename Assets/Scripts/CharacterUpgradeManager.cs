using UnityEngine;
using UnityEngine.UI;

public enum StatType
{
    Speed,
    Resistance,
    Explorer,
    Lucky
}

[System.Serializable]
public class StatUIElements
{
    public StatType statType;
    
    [Header("Coin Upgrade UI")]
    public GameObject coinUpgradeActive;
    public GameObject coinUpgradeInactive; // Bakiye yetersizse gösterilecek buton objesi
    
    [Header("Diamond Upgrade UI")]
    public GameObject diamondUpgradeActive;
    public GameObject diamondUpgradeInactive; // Bakiye yetersizse gösterilecek buton objesi
    
    [Header("Ad Upgrade UI")]
    public GameObject adButtonActive;
    public GameObject adButtonGet; // 3 kere izlenince çıkacak GET butonu
    public Image adProgressFill; // Arka plan dolum barı (Image componenti, Fill Amount kullanılacak)
}

public class CharacterUpgradeManager : MonoBehaviour
{
    [Header("UI Settings")]
    public StatUIElements[] statUIList;

    [Header("Global Currency (Geçici Test İçin)")]
    public int currentCoins = 1000; // Sonradan gerçek coin sistemine bağlanacak
    public int currentDiamonds = 50; // Sonradan gerçek elmas sistemine bağlanacak

    // Bu verileri daha sonra PlayerPrefs ile kaydedeceğiz
    // Şimdilik sadece bellekte tutuyoruz
    private int[] adWatchedCounts = new int[4]; 
    private const int requiredAds = 3;
    
    private int[] statLevels = new int[4];
    
    private int coinCost = 500;
    private int diamondCost = 20;

    private void Start()
    {
        UpdateAllUI();
    }

    // Parametreleri Button'ların OnClick olayından (Inspector üzerinden) int olarak vereceğiz.
    // 0: Speed, 1: Resistance, 2: Explorer, 3: Lucky
    
    public void BuyWithCoin(int statIndex)
    {
        if (currentCoins >= coinCost)
        {
            currentCoins -= coinCost;
            statLevels[statIndex]++;
            Debug.Log(((StatType)statIndex).ToString() + " Coin ile Upgrade Edildi! Yeni Seviye: " + statLevels[statIndex]);
            UpdateAllUI();
        }
    }

    public void BuyWithDiamond(int statIndex)
    {
        if (currentDiamonds >= diamondCost)
        {
            currentDiamonds -= diamondCost;
            statLevels[statIndex]++;
            Debug.Log(((StatType)statIndex).ToString() + " Diamond ile Upgrade Edildi! Yeni Seviye: " + statLevels[statIndex]);
            UpdateAllUI();
        }
    }

    public void WatchAd(int statIndex)
    {
        // Burada gerçek bir reklam gösterme (AdMob, UnityAds vb.) kodu çağrılacak.
        // Reklam başarıyla izlendiğinde alttaki kod çalışmalı:
        
        if (adWatchedCounts[statIndex] < requiredAds)
        {
            adWatchedCounts[statIndex]++;
            Debug.Log(((StatType)statIndex).ToString() + " için Reklam İzlendi: " + adWatchedCounts[statIndex] + "/" + requiredAds);
            UpdateAllUI();
        }
    }

    public void GetAdUpgrade(int statIndex)
    {
        if (adWatchedCounts[statIndex] >= requiredAds)
        {
            adWatchedCounts[statIndex] = 0; // Reklam sayacını sıfırla
            statLevels[statIndex]++;
            Debug.Log(((StatType)statIndex).ToString() + " Reklam ile Upgrade Edildi! Yeni Seviye: " + statLevels[statIndex]);
            UpdateAllUI();
        }
    }

    public void ResetUpgrades()
    {
        Debug.Log("Tüm Upgradeler Sıfırlandı!");
        for (int i = 0; i < statLevels.Length; i++)
        {
            statLevels[i] = 0;
            adWatchedCounts[i] = 0;
        }
        UpdateAllUI();
    }

    public void InfoButtonClicked(int statIndex)
    {
        Debug.Log(((StatType)statIndex).ToString() + " özelliği için info butonuna basıldı. (Daha sonra eklenecek)");
    }

    // Arayüzü güncelleyen fonksiyon
    private void UpdateAllUI()
    {
        for (int i = 0; i < statUIList.Length; i++)
        {
            StatUIElements ui = statUIList[i];
            int index = (int)ui.statType;

            // Coin Butonu Durumu
            bool canAffordCoin = currentCoins >= coinCost;
            if (ui.coinUpgradeActive != null) ui.coinUpgradeActive.SetActive(canAffordCoin);
            if (ui.coinUpgradeInactive != null) ui.coinUpgradeInactive.SetActive(!canAffordCoin);

            // Diamond Butonu Durumu
            bool canAffordDiamond = currentDiamonds >= diamondCost;
            if (ui.diamondUpgradeActive != null) ui.diamondUpgradeActive.SetActive(canAffordDiamond);
            if (ui.diamondUpgradeInactive != null) ui.diamondUpgradeInactive.SetActive(!canAffordDiamond);

            // AD ve GET Butonu Durumu
            bool isAdComplete = adWatchedCounts[index] >= requiredAds;
            if (ui.adButtonActive != null) ui.adButtonActive.SetActive(!isAdComplete);
            if (ui.adButtonGet != null) ui.adButtonGet.SetActive(isAdComplete);

            // Ad Progress Bar (1/3, 2/3, 3/3)
            if (ui.adProgressFill != null)
            {
                // FillAmount 0 ile 1 arasında değer alır
                ui.adProgressFill.fillAmount = (float)adWatchedCounts[index] / requiredAds;
            }
        }
    }
}
