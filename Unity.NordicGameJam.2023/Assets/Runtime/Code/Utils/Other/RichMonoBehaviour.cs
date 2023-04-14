using UnityEngine;

namespace Amheklerior.NordicGameJam2023.Utils
{

    public abstract class RichMonoBehaviour : MonoBehaviour
    {

#if UNITY_EDITOR

        [Header("Dev Settings:")]

        [Tooltip(tooltip: "Activate/deactivate logs.")]
        [SerializeField] protected bool _debugMode = false;

        [SerializeField, Multiline] protected string _description;

#endif

    }
}
