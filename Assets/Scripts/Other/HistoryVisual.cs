using GameManagement;
using TMPro;
using UnityEngine;

namespace Other
{
    public class HistoryVisual : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI time, action, bet, win, info;

        public void SetVisual(History history)
        {
            this.time.text = history.time;
            this.action.text = history.action;
            this.bet.text = history.bet;
            this.win.text = history.win;
            this.info.text = history.info;
        }
    }
}
