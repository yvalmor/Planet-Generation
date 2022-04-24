using Editor;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType
    {
        Simple,
        Rigid
    };

    public FilterType filterType;

    [ConditionalHide("filterType", 0)] public SimpleNoiseSettings simpleNoiseSettings;
    [ConditionalHide("filterType", 1)] public RigidNoiseSettings rigidNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings
    {
        public float strength = 1;
        public float roughness = 1;
        public float persistence = .5f;
        public float baseRoughness = 1;
        public float minValue;

        [Range(1, 8)] public int nbLayers = 1;

        public Vector3 center;
    }

    [System.Serializable]
    public class RigidNoiseSettings : SimpleNoiseSettings
    {
        public float weightMultiplier = .8f;
    }
}