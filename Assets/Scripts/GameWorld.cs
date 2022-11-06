using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public readonly Dictionary<Vector2Int, ChunkData> chunksData = new();
    public ChunkRenderer chunkPrefab;

    void Start()
    {
        for (var x = 0; x < 30; x++)
            for (var y = 0; y < 30; y++) {
                const float ratio = ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                float xPos = x * ratio,
                    zPos = y * ratio;

                var chunkData = new ChunkData(
                    new Vector2Int(x, y),
                    TerrainGenerator.GenerateTerrain(
                        xPos, zPos
                        )
                    );
                chunksData.Add(new Vector2Int(x, y), chunkData);

                var chunk = Instantiate(chunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
                chunk.chunkData = chunkData;
                chunk.parentWorld = this;
            }
    }
}
