using UnityEngine;

namespace DEMSIFT.Core
{
    public static class AudioPlayer
    {
        private static AudioSource musicSource;
        private static AudioSource effectSource;

        public static void PlayMusic(Music music)
        {
            EnsureSources();

            AudioClip clip = Resources.Load<AudioClip>("Audio/Music/" + MusicFileName(music));
            if (clip == null || musicSource.clip == clip) return;

            musicSource.clip = clip;
            musicSource.Play();
        }

        public static void PlaySound(SoundEffect soundEffect)
        {
            EnsureSources();

            AudioClip clip = Resources.Load<AudioClip>("Audio/SFX/" + SoundFileName(soundEffect));
            if (clip != null) effectSource.PlayOneShot(clip);
        }

        // --- Sources ---
        private static void EnsureSources()
        {
            if (musicSource != null) return;

            GameObject root = new GameObject("AudioPlayer");
            Object.DontDestroyOnLoad(root);

            musicSource = root.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            effectSource = root.AddComponent<AudioSource>();
            effectSource.playOnAwake = false;
        }

        private static string MusicFileName(Music music) => music switch
        {
            Music.Menu => "bgm_menu",
            Music.Soal => "bgm_soal",
        };

        private static string SoundFileName(SoundEffect soundEffect) => soundEffect switch
        {
            SoundEffect.Click => "sfx_click",
            SoundEffect.Pickup => "sfx_pickup",
            SoundEffect.Correct => "sfx_correct",
            SoundEffect.WrongDrop => "sfx_wrong_drop",
            SoundEffect.PopupCorrect => "sfx_popup_correct",
            SoundEffect.PopupWrong => "sfx_popup_wrong",
            SoundEffect.Leaderboard => "sfx_leaderboard",
        };

        // --- Play mode without domain reload ---
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Clear()
        {
            musicSource = null;
            effectSource = null;
        }
    }
}
