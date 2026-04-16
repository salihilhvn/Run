using UnityEngine;
using DG.Tweening;

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

            // Toplanma efekti varsa yarat (Hemen yaratılsın)
            if (collectEffectPrefab != null)
            {
                Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);
            }

            // Çifte toplamayı engellemek için collider'ı kapat
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            Camera mainCam = Camera.main;
            // Arayüz noktası (ScoreManager ve CoinUITransform) bulunabiliyorsa uçuş animasyonunu başlat
            if (mainCam != null && ScoreManager.Instance != null && ScoreManager.Instance.CoinUITransform != null)
            {
                // Karakter ilerlerken altının havada kalmaması için kameraya bağla
                transform.SetParent(mainCam.transform, true);
                
                // UI objesinin ekrandaki konumunu dünya konumuna (kameranın 5 birim önü) çevir
                Vector3 screenPos = ScoreManager.Instance.CoinUITransform.position;
                screenPos.z = 5f; 
                Vector3 targetWorld = mainCam.ScreenToWorldPoint(screenPos);
                
                // Kameraya bağlı olduğu için local konumu hesapla
                Vector3 targetLocal = mainCam.transform.InverseTransformPoint(targetWorld);

                Sequence seq = DOTween.Sequence();
                
                // UI'a doğru süzül (Ease.InQuad akıcı bir süzülme sağlar) ve aynı anda küçül
                seq.Append(transform.DOLocalMove(targetLocal, 0.5f).SetEase(Ease.InQuad));
                seq.Join(transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InQuad));

                // Animasyon bitince puanı ekle ve objeyi gizle
                seq.OnComplete(() =>
                {
                    ScoreManager.Instance.AddCoin(coinValue);
                    gameObject.SetActive(false);
                });
            }
            else
            {
                // Kamera veya UI yoksa eski usül hemen puan ekle ve yok ol
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddCoin(coinValue);
                }
                gameObject.SetActive(false);
            }
        }
    }
}
