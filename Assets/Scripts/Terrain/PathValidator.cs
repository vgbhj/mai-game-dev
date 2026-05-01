using UnityEngine;
using System.Collections.Generic;

public class PathValidator : MonoBehaviour
{
    public TerrainGenerator terrainGen;
    public float cellSize = 4f;
    public float maxSlopeAngle = 45f;
    public int maxIterations = 50000;

    public bool HasPath(Vector3 start, Vector3 end)
    {
        Terrain terrain = terrainGen.terrain;
        TerrainData td = terrain.terrainData;

        int gridW = Mathf.CeilToInt(td.size.x / cellSize);
        int gridH = Mathf.CeilToInt(td.size.z / cellSize);

        Vector2Int startCell = WorldToGrid(start, terrain, gridW, gridH);
        Vector2Int endCell = WorldToGrid(end, terrain, gridW, gridH);

        bool[,] visited = new bool[gridW, gridH];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startCell);
        visited[startCell.x, startCell.y] = true;

        Vector2Int[] dirs = {
            new(1, 0), new(-1, 0), new(0, 1), new(0, -1)
        };

        int iterations = 0;

        while (queue.Count > 0)
        {
            if (++iterations > maxIterations) return false;

            Vector2Int current = queue.Dequeue();
            if (current == endCell) return true;

            foreach (var dir in dirs)
            {
                Vector2Int next = current + dir;
                if (next.x < 0 || next.x >= gridW ||
                    next.y < 0 || next.y >= gridH) continue;
                if (visited[next.x, next.y]) continue;

                float h1 = td.GetInterpolatedHeight(
                    (float)current.x / gridW, (float)current.y / gridH
                );
                float h2 = td.GetInterpolatedHeight(
                    (float)next.x / gridW, (float)next.y / gridH
                );
                float slope = Mathf.Abs(h2 - h1) / cellSize;

                if (Mathf.Atan(slope) * Mathf.Rad2Deg < maxSlopeAngle)
                {
                    visited[next.x, next.y] = true;
                    queue.Enqueue(next);
                }
            }
        }

        return false;
    }

    Vector2Int WorldToGrid(Vector3 pos, Terrain t, int w, int h)
    {
        Vector3 local = pos - t.transform.position;
        int x = Mathf.Clamp(Mathf.RoundToInt(local.x / cellSize), 0, w - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt(local.z / cellSize), 0, h - 1);
        return new Vector2Int(x, z);
    }
}
