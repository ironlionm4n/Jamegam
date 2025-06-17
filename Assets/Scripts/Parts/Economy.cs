using TMPro;
using UnityEngine;

namespace Parts
{
    public class Economy : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currencyText;
        public float PlayerCurrency { get; private set; }
        
        private void Start()
        {
            PlayerCurrency = 0f;
            OnCurrencyChanged();
        }
        
        // Maybe use scriptable objects to pass currency values around
        public void CollectPartCurrency(float currencyValue)
        {
            PlayerCurrency += currencyValue;
            OnCurrencyChanged();
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