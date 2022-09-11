using TMPro;
using UnityEngine;

namespace Other
{
    public class PopUp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI multiplierText;
        [SerializeField] private TextMeshProUGUI resultText;

        private string result;
        private string multiplier;
        
        public void SetPopUp(string result, string multiplier)
        {
            multiplierText.text = "x"+ multiplier;
            resultText.text = "DEM " + result;

            this.result = result;
            this.multiplier = multiplier;
        }
        
        public void SetPopUp(string result)
        {
            CancelInvoke();
            resultText.text = result;
            Invoke(nameof(AutoClose), 2);
        }

        private void AutoClose()
        {
            gameObject.SetActive(false);
        }

        public string GetMultiper()
        {
            return multiplier;
        }
        
        public string GetResult()
        {
            return result;
        }
    }
}
