using UnityEngine;

public class TerrainGenerator
{
    public static BlockType[,,] GenerateTerrain(float xOff, float yOff)
    {
        var terrain = new BlockType[ChunkRenderer.CHUNK_WIDTH, ChunkRenderer.CHUNK_HEIGHT, ChunkRenderer.CHUNK_WIDTH];

        for (int x = 0; x < ChunkRenderer.CHUNK_WIDTH; x++)
            for (int z = 0; z < ChunkRenderer.CHUNK_WIDTH; z++)
            {
                var height = Mathf.PerlinNoise((x * ChunkRenderer.BLOCK_SCALE + xOff) * .2f, (z * ChunkRenderer.BLOCK_SCALE + yOff) * .2f) * 7 + 10;

                for (int y = 0; y < height; y++)
                    terrain[x, y, z] = BlockType.Grass;
            }

        return terrain;
    }
}
