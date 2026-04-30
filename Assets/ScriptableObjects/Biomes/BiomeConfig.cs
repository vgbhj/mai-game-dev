using UnityEngine;

[CreateAssetMenu(fileName = "NewBiome", menuName = "Game/Biome Config")]
public class BiomeConfig : ScriptableObject
{
    public string biomeName;
    public BiomeType biomeType;
    public Color mapColor;
    public TerrainLayer terrainLayer;
    public PhysicsMaterial physicsMaterial;

    [Header("Energy")]
    public float energyMultiplier = 1f;

    [Header("Obstacles")]
    public GameObject[] obstaclePrefabs;
    public float obstacleDensity = 0.3f;

    [Header("VFX")]
    public GameObject biomeVFXPrefab;
}

public enum BiomeType
{
    Sand,
    Ice,
    Rock,
    Swamp
}
