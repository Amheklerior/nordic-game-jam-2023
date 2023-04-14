using System;


namespace Amheklerior.NordicGameJam2023.Utils
{
    [Serializable]
    public struct FloatRange
    {
        public float min;
        public float max;

        public float Random => UnityEngine.Random.Range(min, max);
    }
}