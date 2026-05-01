using UnityEngine;
using System;

public class EnergySystem : MonoBehaviour
{
    [Header("Config")]
    public float maxEnergy = 100f;
    public float baseEnergyDrain = 2f;

    [field: SerializeField, Header("Runtime")]
    public float CurrentEnergy { get; private set; }
    [field: SerializeField]
    public float DrainRate { get; private set; }
    [field: SerializeField]
    public BiomeType CurrentBiome { get; private set; }

    public event Action OnEnergyDepleted;
    public event Action<float> OnEnergyChanged;
    public event Action<BiomeType> OnBiomeChanged;

    private RobotController robot;
    private TerrainGenerator terrainGen;
    private UpgradeInventory inventory;
    private BiomeConfig[] biomes;
    private bool depleted;

    void Start()
    {
        robot = GetComponent<RobotController>();
        inventory = GetComponent<UpgradeInventory>();
        terrainGen = FindFirstObjectByType<TerrainGenerator>();

        biomes = terrainGen.config.biomes;
        CurrentEnergy = maxEnergy;
    }

    void Update()
    {
        if (depleted || !robot.IsMoving) return;

        BiomeType newBiome = terrainGen.GetBiomeAt(transform.position);
        if (newBiome != CurrentBiome)
        {
            CurrentBiome = newBiome;
            OnBiomeChanged?.Invoke(CurrentBiome);
        }

        float biomeMult = GetBiomeMultiplier(CurrentBiome);
        float upgradeMult = inventory != null
            ? inventory.GetTotalMultiplier(CurrentBiome)
            : 1f;

        DrainRate = baseEnergyDrain * biomeMult * upgradeMult;

        CurrentEnergy -= DrainRate * Time.deltaTime;
        CurrentEnergy = Mathf.Max(0, CurrentEnergy);

        OnEnergyChanged?.Invoke(CurrentEnergy / maxEnergy);

        if (CurrentEnergy <= 0)
        {
            depleted = true;
            OnEnergyDepleted?.Invoke();
        }
    }

    float GetBiomeMultiplier(BiomeType type)
    {
        foreach (var b in biomes)
            if (b.biomeType == type) return b.energyMultiplier;
        return 1f;
    }

    public void RestoreEnergy(float amount)
    {
        CurrentEnergy = Mathf.Min(maxEnergy, CurrentEnergy + amount);
        depleted = false;
        OnEnergyChanged?.Invoke(CurrentEnergy / maxEnergy);
    }
}
