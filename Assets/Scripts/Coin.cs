using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int coinValue = 1;
    [SerializeField] private GameObject collectEffectPrefab;

    [Header("Animation")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 150f, 0f); // İstersen Inspector'dan hangi eksende döneceğini ayarlayabilirsin

    private bool isCollected = false;

    private void Update()
    {
        // Altını belirlenen hızda ve eksende sürekli olarak döndürür
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;

        // Karakterin collider'ı çarptı mı kontrol ediyoruz. 'RunnerController' veya 'PlayerHealth' arayabiliriz
        RunnerController player = other.GetComponentInParent<RunnerController>();
        if (player != null)
        {
            isCollected = true;
            
            // ScoreManager üzerinden coini artırıyoruz
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddCoin(coinValue);
            }

            // Toplanma efekti varsa yarat
            if (collectEffectPrefab != null)
            {
                Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            }

            // Coini ekrandan yok et
            gameObject.SetActive(false);
        }
    }
}
