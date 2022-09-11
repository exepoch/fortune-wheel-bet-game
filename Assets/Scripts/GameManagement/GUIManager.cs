using System.Collections.Generic;
using Other;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameManagement
{
    public class GUIManager : MonoBehaviour
    {
        public static GUIManager Get;

        private WheelController _wheelController;
        private GameManager _gameManager;
        private HistoryBuilder _historyBuilder;

        private bool _firstPopInited;

        #region UI Elements

        [Header("Buttons")] [SerializeField] private Button spinButton1;

        [SerializeField] private Button spinButton2,
            minBetButton,
            maxBetButton,
            increaseBetButton,
            decreaseBetButton,
            returnButton,
            mainMenuButton,
            copyButton,
            leaveButton,
            openHistoryPanel,
            openSettingsPanel,
            openRulesPanel;

        [Header("TextFields")]
        [SerializeField] private TextMeshProUGUI betAmountText;
        [SerializeField] private TextMeshProUGUI bankAmountText;

        [Header("Panels")] 
        [SerializeField] private GameObject leaveCheckPanel;
        [SerializeField] private GameObject bottomMenuPanel, rulesPanel, historyPanel, settingsPanel;
        
        [SerializeField] private PopUp topPop1, topPop2;
        [SerializeField] private PopUp bottomPop;

        #endregion

        private void Awake()
        {
            Get = this;
            
            _wheelController = FindObjectOfType<WheelController>();
            _gameManager = FindObjectOfType<GameManager>();
            _historyBuilder = FindObjectOfType<HistoryBuilder>();
            
            SetButtonListeners();
        }

        private void SetButtonListeners()
        {
            spinButton1.onClick.AddListener(()=> _wheelController.Spin());
            spinButton2.onClick.AddListener(() => _wheelController.Spin());
            leaveButton.onClick.AddListener(Application.Quit);
            returnButton.onClick.AddListener(()=> leaveCheckPanel.SetActive(true));
            mainMenuButton.onClick.AddListener(()=> bottomMenuPanel.SetActive(true));
            copyButton.onClick.AddListener(() =>
            {
                bottomPop.SetPopUp("The text has been copied successfully");
                bottomPop.gameObject.SetActive(true);
            });
            minBetButton.onClick.AddListener(() =>
            {
                _gameManager.SetMinBet(out float bet);
                betAmountText.text = bet.ToString();

                decreaseBetButton.interactable = false;
                increaseBetButton.interactable = true;
                
                print("Bet Setted to : " + bet);
            });
            maxBetButton.onClick.AddListener(() =>
            {
                _gameManager.SetMaxBet(out float bet);
                betAmountText.text = bet.ToString();

                increaseBetButton.interactable = false;
                decreaseBetButton.interactable = true;

                print("Bet Setted to : " + bet);
            });
            increaseBetButton.onClick.AddListener(() =>
            {
                _gameManager.IncreaseBet(out float bet, out bool max);
                betAmountText.text = bet.ToString();
                
                if (max)
                    increaseBetButton.interactable = false;

                decreaseBetButton.interactable = true;
                
                print("Bet Setted to : " + bet);
            });
            decreaseBetButton.onClick.AddListener(() =>
            {
                _gameManager.DecreaseBet(out float bet, out bool min);
                betAmountText.text = bet.ToString();
                
                if (min)
                    decreaseBetButton.interactable = false;

                increaseBetButton.interactable = true;
                
                print("Bet Setted to : " + bet);
            });
            openHistoryPanel.onClick.AddListener(() =>
            {
                _historyBuilder.GenerateHistoryPanel();
                historyPanel.SetActive(true);
                rulesPanel.SetActive(false);
                settingsPanel.SetActive(false);
            });
            openRulesPanel.onClick.AddListener(() =>
            {
                rulesPanel.SetActive(true);
                settingsPanel.SetActive(false);
                historyPanel.SetActive(false);
            });
            openSettingsPanel.onClick.AddListener(() =>
            {
                settingsPanel.SetActive(true);
                rulesPanel.SetActive(false);
                historyPanel.SetActive(false);
            });
        }

        //Update UI when resul input
        public void UpdateGui(string result, string multiplier)
        {
            bankAmountText.text = "DEM " + result;

            if (!_firstPopInited)
            {
                topPop1.SetPopUp(result, multiplier);
                topPop1.gameObject.SetActive(true);
                _firstPopInited = true;
            }
            else
            {
                topPop2.SetPopUp(topPop1.GetResult(),topPop1.GetMultiper());
                topPop1.SetPopUp(result, multiplier);
                topPop2.gameObject.SetActive(true);
            }
        }
    }
}
