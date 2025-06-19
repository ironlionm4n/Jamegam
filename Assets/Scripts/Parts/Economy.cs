using TMPro;
using UnityEngine;

namespace Parts
{
    public class Economy : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private AudioClip[] currencySounds;
        public float PlayerCurrency { get; private set; }
        private AudioSource _currencyAudioSource;
        
        private void Start()
        {
            PlayerCurrency = 0f;
            OnCurrencyChanged();
            _currencyAudioSource = GetComponent<AudioSource>();
        }
        
        // Maybe use scriptable objects to pass currency values around
        public void CollectPartCurrency(float currencyValue)
        {
            PlayerCurrency += currencyValue;
            OnCurrencyChanged();
            _currencyAudioSource.PlayOneShot(currencySounds[Random.Range(0, currencySounds.Length)]);
        }

        private void OnCurrencyChanged()
        {
            Debug.Log($"Currency updated: {PlayerCurrency}");
            if (currencyText != null)
            {
                currencyText.text = PlayerCurrency.ToString("F2");
            }
            else
            {
                Debug.LogWarning("Currency text UI element is not assigned.");
            }
        }
    }
}