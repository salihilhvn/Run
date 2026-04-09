using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class JuicyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Tıklama Efekti Ayarları")]
    [Tooltip("Butona basıldığında küçülme oranı (0.9 = %10 küçülür)")]
    [SerializeField] private float shrinkScale = 0.9f;
    [Tooltip("Animasyonun hızı (ne kadar yüksekse o kadar sert seker)")]
    [SerializeField] private float animationSpeed = 20f;
    
    private Vector3 originalScale;
    private bool isPressed = false;
    private Button buttonComponent;

    private void Awake()
    {
        originalScale = transform.localScale;
        buttonComponent = GetComponent<Button>();
    }

    private void Update()
    {
        // Buton tıklanabilir (Interactable) değilse animasyon yapma
        if (buttonComponent != null && !buttonComponent.interactable)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.unscaledDeltaTime * animationSpeed);
            return;
        }

        Vector3 targetScale = isPressed ? originalScale * shrinkScale : originalScale;
        
        // Time.unscaledDeltaTime kullanıyoruz çünkü oyun dondurulmuş (Time.timeScale=0) olsa bile menü butonları çalışmalı.
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animationSpeed);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Sadece sol tıklandığında ve buton aktifse
        if (buttonComponent != null && buttonComponent.interactable && eventData.button == PointerEventData.InputButton.Left)
        {
            isPressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }
}
