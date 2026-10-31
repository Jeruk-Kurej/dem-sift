using UnityEngine;

namespace DEMSIFT.UI
{
    public static class QuitGateSpawner
    {
        private const string PrefabName = "QuitGate";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Spawn()
        {
            if (Object.FindAnyObjectByType<QuitGate>(FindObjectsInactive.Include) != null) return;

            GameObject prefab = Resources.Load<GameObject>(PrefabName);
            if (prefab == null)
            {
                Debug.LogWarning("[QuitGate] Resources/" + PrefabName + " is missing, so Esc does nothing.");
                return;
            }

            GameObject gate = Object.Instantiate(prefab);
            gate.name = PrefabName;
            Object.DontDestroyOnLoad(gate);
        }
    }
}
