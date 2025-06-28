using System;
using UnityEngine;

namespace Shipico.BehaviourTrees
{
    [Serializable]
    public struct NormalizedFloat
    {
        [SerializeField]
        private float value;
        public float Value
        {
            get => value;
            set => this.value = Mathf.Clamp01(value);
        }
        private NormalizedFloat(float value)
        {
            this.value = Mathf.Clamp01(value);
            if (value > 1 || value < 0)
            {
                Debug.LogWarning($"Value {value} is outside normalized range");
            }
        }

        public static NormalizedFloat Create(float value, float max, float min = 0) => new((value - min) / (max - min));
        public static implicit operator float(NormalizedFloat normalizedFloat) => normalizedFloat.Value;
    }
}