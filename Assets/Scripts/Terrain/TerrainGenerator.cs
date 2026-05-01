using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GenerationConfig config;
    public Terrain terrain;
    public Transform robot;
    public bool autoGenerate = true;

    private float[,,] alphamaps;
    private BiomeType[,] biomeGrid;

    void Start()
    {
        if (autoGenerate)
            Generate(Random.Range(0, 99999));
    }

    public void Generate(int seed)
    {
        Random.InitState(seed);

        TerrainData td = terrain.terrainData;
        td.heightmapResolution = config.terrainSize + 1;
        td.size = new Vector3(config.terrainSize, config.terrainHeight, config.terrainSize);

        GenerateHeightmap(td);
        GenerateBiomes(td);

        terrain.Flush();

        if (robot != null)
            PlaceOnTerrain(robot);
    }

    public void PlaceOnTerrain(Transform obj)
    {
        Vector3 center = terrain.transform.position
            + new Vector3(config.terrainSize * 0.5f, 0, config.terrainSize * 0.5f);
        center.y = terrain.SampleHeight(center) + 2f;
        obj.position = center;
    }

    void GenerateHeightmap(TerrainData td)
    {
        int res = td.heightmapResolution;
        float[,] heights = new float[res, res];

        float offsetX = Random.Range(0f, 10000f);
        float offsetZ = Random.Range(0f, 10000f);

        for (int z = 0; z < res; z++)
        {
            for (int x = 0; x < res; x++)
            {
                float height = 0f;
                float amplitude = 1f;
                float frequency = config.noiseScale;

                for (int oct = 0; oct < config.noiseOctaves; oct++)
                {
                    float nx = (x + offsetX) * frequency;
                    float nz = (z + offsetZ) * frequency;
                    height += Mathf.PerlinNoise(nx, nz) * amplitude;

                    amplitude *= 0.5f;
                    frequency *= 2f;
                }

                heights[z, x] = height / config.noiseOctaves;
            }
        }

        td.SetHeights(0, 0, heights);
    }

    void GenerateBiomes(TerrainData td)
    {
        int res = td.alphamapResolution;
        int biomeCount = config.biomes.Length;

        TerrainLayer[] layers = new TerrainLayer[biomeCount];
        for (int i = 0; i < biomeCount; i++)
            layers[i] = config.biomes[i].terrainLayer;
        td.terrainLayers = layers;

        alphamaps = new float[res, res, biomeCount];
        biomeGrid = new BiomeType[res, res];

        float offsetX = Random.Range(0f, 10000f);
        float offsetZ = Random.Range(0f, 10000f);

        for (int z = 0; z < res; z++)
        {
            for (int x = 0; x < res; x++)
            {
                float n1 = Mathf.PerlinNoise(
                    (x + offsetX) * config.biomeNoiseScale,
                    (z + offsetZ) * config.biomeNoiseScale
                );
                float n2 = Mathf.PerlinNoise(
                    (x + offsetX + 5000f) * config.biomeNoiseScale * 1.5f,
                    (z + offsetZ + 5000f) * config.biomeNoiseScale * 1.5f
                );

                int biomeIndex = GetBiomeIndex(n1, n2, biomeCount);
                biomeGrid[z, x] = config.biomes[biomeIndex].biomeType;

                for (int b = 0; b < biomeCount; b++)
                    alphamaps[z, x, b] = (b == biomeIndex) ? 1f : 0f;
            }
        }

        SmoothAlphamaps(res, biomeCount);
        td.SetAlphamaps(0, 0, alphamaps);
    }

    int GetBiomeIndex(float n1, float n2, int count)
    {
        if (n1 < 0.4f && n2 < 0.5f) return 0;
        if (n1 < 0.4f && n2 >= 0.5f) return Mathf.Min(1, count - 1);
        if (n1 >= 0.4f && n2 < 0.5f) return Mathf.Min(2, count - 1);
        return Mathf.Min(3, count - 1);
    }

    void SmoothAlphamaps(int res, int biomeCount)
    {
        float[,,] smoothed = new float[res, res, biomeCount];

        for (int z = 0; z < res; z++)
        {
            for (int x = 0; x < res; x++)
            {
                for (int b = 0; b < biomeCount; b++)
                {
                    float sum = 0;
                    int count = 0;
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int sz = z + dz;
                            int sx = x + dx;
                            if (sz < 0 || sz >= res || sx < 0 || sx >= res) continue;
                            sum += alphamaps[sz, sx, b];
                            count++;
                        }
                    }
                    smoothed[z, x, b] = sum / count;
                }
            }
        }

        alphamaps = smoothed;
    }

    public BiomeType GetBiomeAt(Vector3 worldPos)
    {
        Vector3 terrainPos = worldPos - terrain.transform.position;
        TerrainData td = terrain.terrainData;

        int mapX = Mathf.Clamp(
            Mathf.RoundToInt((terrainPos.x / td.size.x) * (biomeGrid.GetLength(1) - 1)),
            0, biomeGrid.GetLength(1) - 1
        );
        int mapZ = Mathf.Clamp(
            Mathf.RoundToInt((terrainPos.z / td.size.z) * (biomeGrid.GetLength(0) - 1)),
            0, biomeGrid.GetLength(0) - 1
        );

        return biomeGrid[mapZ, mapX];
    }
}
