using UnityEngine;

public static class TerrainGenerator
{
    public static BlockType[,,] GenerateTerrain(float xOff, float yOff)
    {
        var terrain = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];

        for (var x = 0; x < ChunkRenderer.ChunkWidth; x++)
            for (var z = 0; z < ChunkRenderer.ChunkWidth; z++)
            {
                var height = Mathf.PerlinNoise((x + xOff) * .2f, (z + yOff) * .2f) * 7 + 10;

                for (var y = 0; y < height; y++)
                    terrain[x, y, z] = BlockType.Grass;
            }

        return terrain;
    }
}
