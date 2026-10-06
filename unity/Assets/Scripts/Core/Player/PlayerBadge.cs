using TMPro;
using UnityEngine;

namespace DEMSIFT.Player
{
    [RequireComponent(typeof(TMP_Text))]
    public class PlayerBadge : MonoBehaviour
    {
        private void Start()
        {
            if (!PlayerSession.HasPlayer)
            {
                gameObject.SetActive(false);
                return;
            }

            GetComponent<TMP_Text>().text = $"{PlayerSession.PlayerName} · {PlayerSession.ClassName}";
        }
    }
}
