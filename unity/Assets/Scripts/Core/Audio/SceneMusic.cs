using UnityEngine;

namespace DEMSIFT.Core
{
    public class SceneMusic : MonoBehaviour
    {
        [SerializeField] private Music music = Music.Soal;

        private void Start()
        {
            AudioPlayer.PlayMusic(music);
        }
    }
}
