using UnityEngine;

public class ColorGenerator
{
    private ColorSettings settings;
    private Texture2D texture;

    private const int textureResolution = 5;

    private INoiseFilter biomeNoiseFilter;

    private static readonly int ElevationMinMax_id = Shader.PropertyToID("_elevationMinMax");
    private static readonly int Texture_id = Shader.PropertyToID("_texture");

    public void UpdateSettings(ColorSettings settings)
    {
        this.settings = settings;
        if (texture == null || texture.height != settings.biomeColorSettings.biomes.Length)
            texture = new Texture2D(textureResolution * 2, settings.biomeColorSettings.biomes.Length,
                TextureFormat.RGBA32,
                false);

        biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
    }

    public void UpdateElevation(MinMax elevationMinMax)
    {
        settings.planetMaterial.SetVector(ElevationMinMax_id, new Vector4(elevationMinMax.Min, elevationMinMax.Max));
    }

    public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
    {
        float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
        heightPercent +=
            (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings
                .biomeColorSettings.noiseStrength;

        float biomeIndex = 0;

        // We add .001 to make sure the blend range is always greater than 0
        float blendRange = settings.biomeColorSettings.blendAmount / 2f + .001f;

        int numBiomes = settings.biomeColorSettings.biomes.Length;

        for (int i = 0; i < numBiomes; i++)
        {
            float dst = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
            float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);

            biomeIndex *= 1 - weight;
            biomeIndex += i * weight;
        }

        return biomeIndex / Mathf.Max(1, numBiomes - 1);
    }

    public void UpdateColors()
    {
        Color[] colors = new Color[texture.width * texture.height];
        int colorIndex = 0;

        foreach (var biome in settings.biomeColorSettings.biomes)
        {
            for (int i = 0; i < textureResolution * 2; i++)
            {
                var gradientCol = i < textureResolution
                    ? settings.oceanColor.Evaluate(i / (textureResolution - 1f))
                    : biome.gradient.Evaluate((i - textureResolution) / (textureResolution - 1f));
                Color tintCol = biome.tint;

                colors[colorIndex] = gradientCol * (1 - biome.tintPercent) + tintCol * biome.tintPercent;
                colorIndex++;
            }
        }

        texture.SetPixels(colors);
        texture.Apply();

        settings.planetMaterial.SetTexture(Texture_id, texture);
    }
}