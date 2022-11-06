using System.Collections.Generic;
using UnityEngine;

public class GameWorld : MonoBehaviour
{
    public const int ChunksByX = 30;
    public const int ChunksByY = 30;
    
    public readonly Dictionary<Vector2Int, ChunkData> chunksData = new();
    public ChunkRenderer chunkPrefab;
    public PlayerControls playerPrefab;

    void Start()
    {
        for (var x = 0; x < ChunksByX; x++)
            for (var y = 0; y < ChunksByY; y++) {
                const float ratio = ChunkRenderer.ChunkWidth * ChunkRenderer.BlockScale;
                var xPos = x * ratio;
                var zPos = y * ratio;

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
        
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        var chunkPos = new Vector2Int(Random.Range(0, ChunksByX - 1), Random.Range(0, ChunksByY - 1));

        if (!chunksData.TryGetValue(chunkPos, out var chunkData)) return;
        
        var playerInChunkPos = new Vector2Int();

        for (var i = 0; i < 2; i++)
            playerInChunkPos[i] = Random.Range(0, ChunkRenderer.ChunkWidth - 1);

        var playerWorldPos = new Vector3(
            (playerInChunkPos.x + ChunkRenderer.ChunkWidth * chunkPos.x) * ChunkRenderer.BlockScale,
            0,
            (playerInChunkPos.y + ChunkRenderer.ChunkWidth * chunkPos.y) * ChunkRenderer.BlockScale
            );

        for (var chunkY = 0; chunkY < ChunkRenderer.ChunkHeight; chunkY++)
            if (chunkData.blocks[playerInChunkPos.x, chunkY, playerInChunkPos.y] == BlockType.Air)
            {
                playerWorldPos.y = chunkY * ChunkRenderer.BlockScale;
                break;
            }
        
        Instantiate(playerPrefab, playerWorldPos, Quaternion.identity, transform);
    }
}
