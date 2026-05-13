using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    // Eventler: Para değiştiğinde UI'ların otomatik güncellenmesini sağlar
    public event Action OnCurrencyChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Ekrana her sahne yüklendiğinde silinmemesi için:
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int TotalCoins
    {
        get { return PlayerPrefs.GetInt("TotalCoins", 0); }
        private set { PlayerPrefs.SetInt("TotalCoins", value); }
    }

    private void Update()
    {
        // Klavyeden 'M' tuşuna basıldığında para ateşle! (Hile)
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.mKey.wasPressedThisFrame)
        {
            AddCoins(50000);
            AddDiamonds(1000);
            Debug.Log("HİLE AKTİF: 50.000 Altın ve 1.000 Elmas eklendi!");
        }
    }

    public int TotalDiamonds
    {
        get { return PlayerPrefs.GetInt("TotalDiamonds", 0); }
        private set { PlayerPrefs.SetInt("TotalDiamonds", value); }
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke();
    }

    public bool SpendCoins(int amount)
    {
        if (TotalCoins >= amount)
        {
            TotalCoins -= amount;
            PlayerPrefs.Save();
            OnCurrencyChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void AddDiamonds(int amount)
    {
        TotalDiamonds += amount;
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke();
    }

    public bool SpendDiamonds(int amount)
    {
        if (TotalDiamonds >= amount)
        {
            TotalDiamonds -= amount;
            PlayerPrefs.Save();
            OnCurrencyChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void ResetCurrencies()
    {
        TotalCoins = 0;
        TotalDiamonds = 0;
        PlayerPrefs.Save();
        OnCurrencyChanged?.Invoke();
        Debug.Log("Para ve Elmaslar Sıfırlandı!");
    }
}
