using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    public TerrainGenerator terrainGen;
    public GenerationConfig config;
    public float spacing = 6f;

    private List<GameObject> spawnedObjects = new List<GameObject>();

    public void Spawn()
    {
        ClearSpawned();

        Terrain terrain = terrainGen.terrain;
        TerrainData td = terrain.terrainData;

        for (float x = spacing; x < td.size.x - spacing; x += spacing)
        {
            for (float z = spacing; z < td.size.z - spacing; z += spacing)
            {
                Vector3 worldPos = terrain.transform.position + new Vector3(x, 0, z);
                BiomeType biome = terrainGen.GetBiomeAt(worldPos);
                BiomeConfig bc = GetBiomeConfig(biome);

                if (bc == null || bc.obstaclePrefabs.Length == 0) continue;
                if (Random.value > bc.obstacleDensity) continue;

                float ox = Random.Range(-spacing * 0.3f, spacing * 0.3f);
                float oz = Random.Range(-spacing * 0.3f, spacing * 0.3f);

                Vector3 pos = new Vector3(x + ox, 0, z + oz) + terrain.transform.position;
                pos.y = terrain.SampleHeight(pos);

                GameObject prefab = bc.obstaclePrefabs[Random.Range(0, bc.obstaclePrefabs.Length)];
                GameObject obj = Instantiate(
                    prefab, pos,
                    Quaternion.Euler(0, Random.Range(0f, 360f), 0)
                );
                spawnedObjects.Add(obj);
            }
        }
    }

    BiomeConfig GetBiomeConfig(BiomeType type)
    {
        foreach (var b in config.biomes)
            if (b.biomeType == type) return b;
        return null;
    }

    void ClearSpawned()
    {
        foreach (var obj in spawnedObjects)
            if (obj != null) Destroy(obj);
        spawnedObjects.Clear();
    }
}
