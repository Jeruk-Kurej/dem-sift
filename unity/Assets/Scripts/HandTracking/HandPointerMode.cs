using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace DEMSIFT.HandTracking
{
    public class HandPointerMode : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SeparatePointers();
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SeparatePointers();
        }

        private static void SeparatePointers()
        {
            InputSystemUIInputModule module = Object.FindAnyObjectByType<InputSystemUIInputModule>();
            if (module != null) module.pointerBehavior = UIPointerBehavior.AllPointersAsIs;
        }
    }
}
