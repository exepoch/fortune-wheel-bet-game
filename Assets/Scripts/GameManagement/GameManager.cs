using System;
using UnityEngine;

namespace GameManagement
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private float bankAmount;//players money
        
        [Space]
        [SerializeField] private float[] betAmounts; //Bet amount player can bet
        [SerializeField] private int betAmountIndex = 10; 
        private float _currentBet;

        private const float MinBetAmount = .1f;
        private const float MaxBetAmount = 500f;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            
            //Register wheel actions
            WheelController.OnSpinResult += OnSpinResult;
            WheelController.OnSpinStart += OnSpinStart;

            _currentBet = betAmounts[betAmountIndex];
        }

        private void OnSpinStart()
        {
            bankAmount -= _currentBet;
        }

        //Calls when spin action ended
        private void OnSpinResult(string result)
        {
            if (result == "next")
                return;

            var multiplier = float.Parse(result.Replace(".",","));

            var win = _currentBet * multiplier;
            bankAmount += win;
            
            //Create action history save
            HistoryBuilder.CreateHistoryElement(new History(
                DateTime.Now.ToShortTimeString(),
                "Spin",
                _currentBet.ToString(),
                win.ToString(),
                "MagicWheel"));
            
            GUIManager.Get.UpdateGui(bankAmount.ToString(), multiplier.ToString());
        }

        public void SetMinBet(out float x)
        {
            x = MinBetAmount;
            _currentBet = MinBetAmount;
            betAmountIndex = 0;
        }
        
        public void SetMaxBet(out float x)
        {
            x = MaxBetAmount;
            _currentBet = MaxBetAmount;
            betAmountIndex = betAmounts.Length-1;
        }
        
        public void IncreaseBet(out float x, out bool y)
        {
            if (betAmountIndex < betAmounts.Length-1)
                betAmountIndex++;
            
            x = betAmounts[betAmountIndex];
            _currentBet = x;

            y = _currentBet == MaxBetAmount; //Is current bet setted to max bet?
        }
        
        public void DecreaseBet(out float x, out bool y)
        {
            if (betAmountIndex > 0)
                betAmountIndex--;
            
            x = betAmounts[betAmountIndex];
            _currentBet = x;

            y = _currentBet == MinBetAmount; //Is current bet setted to min bet?
        }

        private void OnDestroy()
        {
            WheelController.OnSpinResult -= OnSpinResult;
            WheelController.OnSpinStart -= OnSpinStart;
        }


    }
}
