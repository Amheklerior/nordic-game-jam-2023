using UnityEngine;

namespace NordicGameJam2023.Utils
{

    public abstract class RichScriptableObject : ScriptableObject
    {

#if UNITY_EDITOR

        [Header("Dev Settings:")]

        [Tooltip(tooltip: "Activate/deactivate logs.")]
        [SerializeField] protected bool _debugMode = false;

        [SerializeField, Multiline] protected string _description;

#endif

    }
}
