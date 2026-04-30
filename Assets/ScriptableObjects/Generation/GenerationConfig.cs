using UnityEngine;

[CreateAssetMenu(fileName = "GenerationConfig", menuName = "Game/Generation Config")]
public class GenerationConfig : ScriptableObject
{
    [Header("Terrain")]
    public int terrainSize = 256;
    public int terrainHeight = 50;
    public float noiseScale = 0.02f;
    public int noiseOctaves = 4;

    [Header("Biomes")]
    public float biomeNoiseScale = 0.01f;
    public BiomeConfig[] biomes;

    [Header("Upgrades")]
    public UpgradeConfig[] upgrades;
    public int upgradeCount = 10;

    [Header("Gameplay")]
    public float startFinishMinDistance = 200f;
}
