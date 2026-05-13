using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("UI")]
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TMP_Text healthText;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    private void Start()
    {
        ResetHealth();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.hKey.wasPressedThisFrame)
        {
            TakeDamage(25);
        }
    }

    public void TakeDamage(int damage)
    {
        int resLevel = PlayerPrefs.GetInt("StatLevel_1", 0); // 1 = Resistance Stat Index
        float resistanceVal = 10f * Mathf.Pow(1.015f, resLevel);

        int finalDamage = damage;
        
        // Sadece varsayılan 25 damage için veya genel bir kural olarak hasar azaltma mantığı
        if (damage == 25)
        {
            if (resistanceVal >= 10f && resistanceVal < 12f) finalDamage = 25;
            else if (resistanceVal >= 12f && resistanceVal < 15f) finalDamage = 23;
            else if (resistanceVal >= 15f && resistanceVal < 17f) finalDamage = 22;
            else if (resistanceVal >= 17f && resistanceVal <= 20f) finalDamage = 21;
            else if (resistanceVal > 20f) finalDamage = 20;
        }

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();

        Debug.Log($"Incoming Damage: {damage} | Final Damage Applied: {finalDamage} | Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("Player Dead");
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.ShowFailPanel();
            }
        }
    }

    public void Revive(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        Debug.Log($"Player Revived with {amount} extra health!");
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthFillImage != null)
            healthFillImage.fillAmount = (float)currentHealth / maxHealth;
            
        if (healthText != null)
            healthText.text = currentHealth.ToString();
    }
}