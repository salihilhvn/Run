using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private int coinValue = 1;
    [SerializeField] private GameObject collectEffectPrefab; // İstersen daha sonra partikül efekti ekleyebilirsin

    private bool isCollected = false;

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
