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
    
    [Header("Top Bar UI")]
    public TMPro.TMP_Text statValueText; // Yandaki değer texti (1.00, 10 vs)
    public Image statProgressBar; // Gelişmişlik seviyesi barı
    public int maxLevel = 50; // Maksimum seviye sınırı
    
    [Header("Coin Upgrade UI")]
    public GameObject coinUpgradeActive;
    public GameObject coinUpgradeInactive;
    
    [Header("Diamond Upgrade UI")]
    public GameObject diamondUpgradeActive;
    public GameObject diamondUpgradeInactive;
    
    [Header("Ad Upgrade UI")]
    public GameObject adButtonActive;
    public GameObject adButtonGet;
    public Image adProgressFill;
}

public class CharacterUpgradeManager : MonoBehaviour
{
    [Header("UI Settings")]
    public StatUIElements[] statUIList;

    private int[] adWatchedCounts = new int[4]; 
    private const int requiredAds = 3;
    
    private int[] statLevels = new int[4];
    
    private int coinCost = 500;
    private int diamondCost = 20;

    private void Start()
    {
        LoadStats();
        UpdateAllUI();
        
        // Currency değiştiğinde UI'ı güncellemek için event'e abone oluyoruz
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged += UpdateAllUI;
        }
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCurrencyChanged -= UpdateAllUI;
        }
    }

    private void LoadStats()
    {
        for (int i = 0; i < statLevels.Length; i++)
        {
            statLevels[i] = PlayerPrefs.GetInt("StatLevel_" + i, 0);
            adWatchedCounts[i] = PlayerPrefs.GetInt("StatAdCount_" + i, 0);
        }
    }

    private void SaveStat(int index)
    {
        PlayerPrefs.SetInt("StatLevel_" + index, statLevels[index]);
        PlayerPrefs.SetInt("StatAdCount_" + index, adWatchedCounts[index]);
        PlayerPrefs.Save();
    }

    public void BuyWithCoin(int statIndex)
    {
        int maxLvl = statUIList[statIndex].maxLevel;
        if (statLevels[statIndex] >= maxLvl)
        {
            Debug.Log("Maksimum seviyeye ulaşıldı!");
            return;
        }

        if (CurrencyManager.Instance != null && CurrencyManager.Instance.SpendCoins(coinCost))
        {
            statLevels[statIndex]++;
            SaveStat(statIndex);
            Debug.Log(((StatType)statIndex).ToString() + " Coin ile Upgrade Edildi! Yeni Seviye: " + statLevels[statIndex]);
            UpdateAllUI();
        }
        else
        {
            Debug.Log("Yeterli Coin Yok!");
        }
    }

    public void BuyWithDiamond(int statIndex)
    {
        int maxLvl = statUIList[statIndex].maxLevel;
        if (statLevels[statIndex] >= maxLvl)
        {
            Debug.Log("Maksimum seviyeye ulaşıldı!");
            return;
        }

        if (CurrencyManager.Instance != null && CurrencyManager.Instance.SpendDiamonds(diamondCost))
        {
            statLevels[statIndex]++;
            SaveStat(statIndex);
            Debug.Log(((StatType)statIndex).ToString() + " Diamond ile Upgrade Edildi! Yeni Seviye: " + statLevels[statIndex]);
            UpdateAllUI();
        }
        else
        {
            Debug.Log("Yeterli Diamond Yok!");
        }
    }

    public void WatchAd(int statIndex)
    {
        int maxLvl = statUIList[statIndex].maxLevel;
        if (statLevels[statIndex] >= maxLvl)
        {
            Debug.Log("Maksimum seviyeye ulaşıldı!");
            return;
        }

        if (adWatchedCounts[statIndex] < requiredAds)
        {
            adWatchedCounts[statIndex]++;
            SaveStat(statIndex);
            Debug.Log(((StatType)statIndex).ToString() + " için Reklam İzlendi: " + adWatchedCounts[statIndex] + "/" + requiredAds);
            UpdateAllUI();
        }
    }

    public void GetAdUpgrade(int statIndex)
    {
        int maxLvl = statUIList[statIndex].maxLevel;
        if (statLevels[statIndex] >= maxLvl) return;

        if (adWatchedCounts[statIndex] >= requiredAds)
        {
            adWatchedCounts[statIndex] = 0;
            statLevels[statIndex]++;
            SaveStat(statIndex);
            Debug.Log(((StatType)statIndex).ToString() + " Reklam ile Upgrade Edildi! Yeni Seviye: " + statLevels[statIndex]);
            UpdateAllUI();
        }
    }

    public void ResetUpgrades()
    {
        Debug.Log("Tüm Upgradeler ve Paralar Sıfırlandı!");
        for (int i = 0; i < statLevels.Length; i++)
        {
            statLevels[i] = 0;
            adWatchedCounts[i] = 0;
            PlayerPrefs.DeleteKey("StatLevel_" + i);
            PlayerPrefs.DeleteKey("StatAdCount_" + i);
        }
        
        PlayerPrefs.Save();

        // Paraları da sıfırlayalım ki tam test olsun
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ResetCurrencies();
        }
        
        UpdateAllUI();
    }

    public void InfoButtonClicked(int statIndex)
    {
        Debug.Log(((StatType)statIndex).ToString() + " özelliği için info butonuna basıldı.");
    }

    private void UpdateAllUI()
    {
        int currentCoins = CurrencyManager.Instance != null ? CurrencyManager.Instance.TotalCoins : 0;
        int currentDiamonds = CurrencyManager.Instance != null ? CurrencyManager.Instance.TotalDiamonds : 0;

        for (int i = 0; i < statUIList.Length; i++)
        {
            StatUIElements ui = statUIList[i];
            int index = (int)ui.statType;

            bool isMaxLevel = statLevels[index] >= ui.maxLevel;

            bool canAffordCoin = currentCoins >= coinCost && !isMaxLevel;
            if (ui.coinUpgradeActive != null) ui.coinUpgradeActive.SetActive(canAffordCoin);
            if (ui.coinUpgradeInactive != null) ui.coinUpgradeInactive.SetActive(!canAffordCoin);

            bool canAffordDiamond = currentDiamonds >= diamondCost && !isMaxLevel;
            if (ui.diamondUpgradeActive != null) ui.diamondUpgradeActive.SetActive(canAffordDiamond);
            if (ui.diamondUpgradeInactive != null) ui.diamondUpgradeInactive.SetActive(!canAffordDiamond);

            bool isAdComplete = adWatchedCounts[index] >= requiredAds;
            if (ui.adButtonActive != null) ui.adButtonActive.SetActive(!isAdComplete && !isMaxLevel);
            if (ui.adButtonGet != null) ui.adButtonGet.SetActive(isAdComplete && !isMaxLevel);

            if (ui.adProgressFill != null)
            {
                ui.adProgressFill.fillAmount = (float)adWatchedCounts[index] / requiredAds;
            }

            if (ui.statProgressBar != null)
            {
                ui.statProgressBar.fillAmount = (float)statLevels[index] / ui.maxLevel;
            }

            if (ui.statValueText != null)
            {
                if (ui.statType == StatType.Speed)
                {
                    float val = 1.0f * Mathf.Pow(1.015f, statLevels[index]);
                    ui.statValueText.text = val.ToString("F2"); // Örn: 1.02
                }
                else if (ui.statType == StatType.Resistance)
                {
                    float val = 10f * Mathf.Pow(1.015f, statLevels[index]);
                    ui.statValueText.text = val.ToString("F2"); // Örn: 10.15
                }
                else
                {
                    // Explorer ve Lucky şimdilik level sayısını veya 0 gösterebilir
                    ui.statValueText.text = statLevels[index].ToString();
                }
            }
        }
    }
}
