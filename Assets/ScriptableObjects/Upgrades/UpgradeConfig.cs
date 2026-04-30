using UnityEngine;

[CreateAssetMenu(fileName = "NewUpgrade", menuName = "Game/Upgrade Config")]
public class UpgradeConfig : ScriptableObject
{
    public string upgradeName;
    public Sprite icon;
    public GameObject visualPrefab;

    [Header("Biome Modifiers")]
    public BiomeModifier[] modifiers;
}

[System.Serializable]
public struct BiomeModifier
{
    public BiomeType biomeType;
    public float energyMultiplier;
}
