using UnityEngine;

namespace DEMSIFT.Core
{
    public class SceneMusic : MonoBehaviour
    {
        [SerializeField] private Music music = Music.Menu;

        private void Start()
        {
            AudioPlayer.PlayMusic(music);
        }
    }
}
