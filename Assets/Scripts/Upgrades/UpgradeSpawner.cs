using UnityEngine;

public class UpgradeSpawner : MonoBehaviour
{
    public TerrainGenerator terrainGen;
    public GenerationConfig config;
    public GameObject pickupShellPrefab;

    private bool spawned;

    void LateUpdate()
    {
        if (spawned) return;
        spawned = true;
        Spawn();
        enabled = false;
    }

    public void Spawn()
    {
        Terrain terrain = terrainGen.terrain;
        TerrainData td = terrain.terrainData;

        for (int i = 0; i < config.upgradeCount; i++)
        {
            float x = Random.Range(20f, td.size.x - 20f);
            float z = Random.Range(20f, td.size.z - 20f);

            Vector3 pos = terrain.transform.position + new Vector3(x, 0, z);
            pos.y = terrain.SampleHeight(pos) + 1.5f;

            BiomeType nearBiome = terrainGen.GetBiomeAt(pos);
            UpgradeConfig upgrade = PickUpgradeForBiome(nearBiome);

            GameObject obj = Instantiate(pickupShellPrefab, pos, Quaternion.identity);
            var pickup = obj.GetComponent<UpgradePickup>();
            pickup.upgradeConfig = upgrade;

            if (upgrade.visualPrefab != null)
                Instantiate(upgrade.visualPrefab, obj.transform);
        }
    }

    UpgradeConfig PickUpgradeForBiome(BiomeType biome)
    {
        if (Random.value < 0.6f)
        {
            foreach (var u in config.upgrades)
            {
                foreach (var m in u.modifiers)
                {
                    if (m.biomeType == biome && m.energyMultiplier < 1f)
                        return u;
                }
            }
        }
        return config.upgrades[Random.Range(0, config.upgrades.Length)];
    }
}
