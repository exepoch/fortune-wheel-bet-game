using System;
using System.Collections.Generic;
using Other;
using UnityEngine;

namespace GameManagement
{
    public class HistoryBuilder : MonoBehaviour
    {
        [SerializeField] private List<HistoryVisual> visuals;

        private static readonly Queue<History> PlayHistory = new Queue<History>();

        public void GenerateHistoryPanel()
        {
            //Generate just last 10 proccess
            var count = PlayHistory.Count;

            foreach (var visual in visuals)
            {
                visual.gameObject.SetActive(false); //close all visuals
            }

            var arr = PlayHistory.ToArray();
            for (int i = arr.Length-1; i >= 0; i--)
            {
                visuals[i].SetVisual(arr[i]);
                visuals[i].gameObject.SetActive(true);
            }
        }
        
        public static void CreateHistoryElement(History history)
        {
            PlayHistory.Enqueue(history);

            
            
            if (PlayHistory.Count > 10)
                PlayHistory.Dequeue(); //Release Past 11.th element
        }
    }
    
    [Serializable]
    public struct History
    {
        public string time;
        public string action;
        public string bet;
        public string win;
        public string info;

        public History(string time, string action, string bet, string win, string info)
        {
            this.time = time;
            this.action = action;
            this.bet = bet;
            this.win = win;
            this.info = info;
        }
    }
}
