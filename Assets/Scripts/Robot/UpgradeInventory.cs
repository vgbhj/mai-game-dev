using UnityEngine;
using System.Collections.Generic;
using System;

public class UpgradeInventory : MonoBehaviour
{
    public int maxSlots = 3;

    public List<UpgradeConfig> ActiveUpgrades { get; private set; } = new();
    public event Action OnInventoryChanged;

    public bool TryAddUpgrade(UpgradeConfig upgrade)
    {
        if (ActiveUpgrades.Count >= maxSlots) return false;
        if (ActiveUpgrades.Contains(upgrade)) return false;

        ActiveUpgrades.Add(upgrade);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public void RemoveUpgrade(UpgradeConfig upgrade)
    {
        ActiveUpgrades.Remove(upgrade);
        OnInventoryChanged?.Invoke();
    }

    public float GetTotalMultiplier(BiomeType biome)
    {
        float total = 1f;
        foreach (var upgrade in ActiveUpgrades)
        {
            foreach (var mod in upgrade.modifiers)
            {
                if (mod.biomeType == biome)
                    total *= mod.energyMultiplier;
            }
        }
        return total;
    }
}
