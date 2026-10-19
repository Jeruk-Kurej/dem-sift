using UnityEngine;

namespace DEMSIFT.HandTracking
{
    public static class HandTrackingSpawner
    {
        private const string PrefabName = "HandTrackingRig";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Spawn()
        {
            if (Object.FindAnyObjectByType<HandTrackingRig>(FindObjectsInactive.Include) != null) return;

            GameObject prefab = Resources.Load<GameObject>(PrefabName);
            if (prefab == null)
            {
                Debug.LogWarning("[HandTracking] Resources/" + PrefabName + " is missing, so hand tracking is off.");
                return;
            }

            GameObject rig = Object.Instantiate(prefab);
            rig.name = PrefabName;
        }
    }
}
