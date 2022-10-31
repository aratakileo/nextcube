using UnityEngine;

namespace Player.Camera
{
    public enum TransformTarget
    {
        Position,
        Rotation,
        Both
    }

    [CreateAssetMenu(fileName = "PerlinNoiseData", menuName = "FirstPersonController/Data/PerlinNoiseData", order = 2)]
    public class PerlinNoiseData : ScriptableObject
    {
        public TransformTarget transformTarget;

        [Space]
        public float amplitude;
        public float frequency;
    }
}