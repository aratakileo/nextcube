using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public Dictionary<Vector2Int, ChunkData> chunkDatas = new Dictionary<Vector2Int, ChunkData>();
    public ChunkRenderer chunkPrefab;

    void Start()
    {
        for (int x = 0; x < 30; x++)
            for (int y = 0; y < 30; y++) {
                float ratio = ChunkRenderer.CHUNK_WIDTH * ChunkRenderer.BLOCK_SCALE,
                    xPos = x * ratio,
                    zPos = y * ratio;

                var chunkData = new ChunkData(
                    new Vector2Int(x, y),
                    TerrainGenerator.GenerateTerrain(
                        xPos, zPos
                        )
                    );
                chunkDatas.Add(new Vector2Int(x, y), chunkData);

                var chunk = Instantiate(chunkPrefab, new Vector3(xPos, 0, zPos), Quaternion.identity, transform);
                chunk.chunkData = chunkData;
                chunk.parentWorld = this;
            }
    }
}
