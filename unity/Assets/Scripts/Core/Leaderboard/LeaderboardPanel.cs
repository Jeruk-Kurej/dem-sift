using System.Collections.Generic;
using DEMSIFT.Data;
using DEMSIFT.Player;
using UnityEngine;
using UnityEngine.UI;

namespace DEMSIFT.Leaderboard
{
    public class LeaderboardPanel : MonoBehaviour
    {
        [SerializeField] private LeaderboardRow rowPrefab;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject emptyMessage;

        private int rowCount;
        private int currentPlayerRow;

        // --- Called by buttons ---
        public void Open()
        {
            Rebuild();
            ScrollToTop();
        }

        public void OpenOnCurrentPlayer()
        {
            Rebuild();
            ScrollToRow(Mathf.Max(0, currentPlayerRow));
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void ScrollToTop()
        {
            ScrollToRow(0);
        }

        // --- Rows ---
        private void Rebuild()
        {
            gameObject.SetActive(true);

            foreach (Transform oldRow in scrollRect.content)
            {
                oldRow.gameObject.SetActive(false);
                Destroy(oldRow.gameObject);
            }

            List<PlayerResult> ranking = LeaderboardRanking.Build(ResultFile.ReadAll());
            string currentPlayerKey = PlayerSession.HasPlayer
                ? PlayerResult.KeyOf(PlayerSession.PlayerName, PlayerSession.ClassName)
                : null;

            rowCount = ranking.Count;
            currentPlayerRow = -1;
            emptyMessage.SetActive(rowCount == 0);

            for (int i = 0; i < rowCount; i++)
            {
                bool isCurrentPlayer = ranking[i].Key == currentPlayerKey;
                if (isCurrentPlayer) currentPlayerRow = i;

                Instantiate(rowPrefab, scrollRect.content).Show(i + 1, ranking[i], isCurrentPlayer);
            }
        }

        private void ScrollToRow(int row)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = rowCount > 1 ? 1f - (float)row / (rowCount - 1) : 1f;
        }
    }
}
