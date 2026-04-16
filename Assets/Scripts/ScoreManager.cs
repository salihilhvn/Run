using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private TMP_Text coinText;

    private int currentCoins = 0;

    public int CurrentCoins => currentCoins;
    public Transform CoinUITransform => coinText != null ? coinText.transform : null;

    private void Awake()
    {
        // Singleton pattern kurulumu
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoin(int amount)
    {
        currentCoins += amount;
        UpdateCoinUI();
        
        // UI Punch Scale Animasyonu (Juicy efekt)
        if (coinText != null)
        {
            coinText.transform.DOKill(true); // Önceki animasyonları temizle (üst üste binmeyi engeller)
            coinText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 10, 1);
        }
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }
    }
}
