using TMPro;
using UnityEngine;

namespace DEMSIFT.Player
{
    public class PlayerBadge : MonoBehaviour
    {
        private void Start()
        {
            if (!PlayerSession.HasPlayer)
            {
                gameObject.SetActive(false);
                return;
            }

            GetComponentInChildren<TMP_Text>().text = $"{PlayerSession.PlayerName} · {PlayerSession.ClassName}";
        }
    }
}
