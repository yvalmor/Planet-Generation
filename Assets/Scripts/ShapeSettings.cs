using UnityEngine;

[CreateAssetMenu]
public class ShapeSettings : ScriptableObject
{
    [Range(1, 200)]
    public float planetRadius = 1;

    public NoiseLayer[] noiseLayers;

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask;
        public NoiseSettings noiseSettings;
    }
}