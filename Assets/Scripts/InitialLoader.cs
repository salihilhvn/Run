using UnityEngine;
using System.Collections;

public class InitialLoader : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("İlk yükleme ekranında kaç saniye beklenecek?")]
    [SerializeField] private float waitTime = 2.5f;

    [Tooltip("Sıradaki yüklenecek sahne")]
    [SerializeField] private string nextSceneName = "MainMenu";

    private void Start()
    {
        // Yükleme işlemini Coroutine ile başlatıyoruz
        StartCoroutine(LoadNextSceneRoutine());
    }

    private IEnumerator LoadNextSceneRoutine()
    {
        Debug.Log("Oyun başlatıldı, gerekli veriler yükleniyor (Simülasyon)...");
        
        // Burada DOTween ile bir barı doldurabilir, logo döndürebilirsin.
        // Biz şimdilik saniye bekliyoruz.
        yield return new WaitForSeconds(waitTime);

        // SceneController üzerinden bir sonraki sahneye geç
        if (SceneController.Instance != null)
        {
            SceneController.Instance.LoadScene(nextSceneName);
        }
        else
        {
            // Eğer SceneController sahnede unutulmuşsa (olmamalı ama önlem)
            Debug.LogWarning("SceneController bulunamadı! Eski yöntemle yükleniyor.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }
    }
}
