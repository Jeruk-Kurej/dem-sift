using UnityEngine;
using UnityEngine.SceneManagement;

namespace DemSift.Core
{
    // Reusable scene navigation - attach to any GameObject (e.g. an empty
    // "SceneLoader" object) and call LoadScene from a Button's OnClick.
    public class SceneLoader : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
