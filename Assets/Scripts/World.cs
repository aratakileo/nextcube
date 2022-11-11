using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Nextcube
{
    public class World : MonoBehaviour
    {
        public const int ChunksByX = 20;
        public const int ChunksByY = 20;

        public readonly Dictionary<Vector2Int, Chunk> chunks = new();
        public Chunk chunkPrefab;
        public Player playerPrefab;

        public Player player;

        private void Start()
        {
            for (var x = 0; x < ChunksByX; x++)
            for (var y = 0; y < ChunksByY; y++)
            {
                const float ratio = Chunk.ChunkWidth;
                var xPos = x * ratio;
                var yPos = y * ratio;
                var newChunk = Instantiate(chunkPrefab, new Vector3(xPos, 0, yPos), Quaternion.identity, transform);

                newChunk.position = new Vector2Int(x, y);
                newChunk.blocks = TerrainGenerator.GenerateTerrain(xPos, yPos);
                newChunk.world = this;
                
                chunks.Add(newChunk.position, newChunk);
            }

            SpawnPlayer();
        }

        public Vector2Int GetChunkPositionContainingBlock(Vector3Int blockWorldPos)
        {
            return new Vector2Int(blockWorldPos.x / Chunk.ChunkWidth,
                blockWorldPos.z / Chunk.ChunkWidth);
        }

        private void SpawnPlayer()
        {
            var chunkPos = new Vector2Int(Random.Range(0, ChunksByX - 1), Random.Range(0, ChunksByY - 1));

            if (!chunks.TryGetValue(chunkPos, out var chunk)) return;

            var playerInChunkPos = new Vector2Int();

            for (var i = 0; i < 2; i++)
                playerInChunkPos[i] = Random.Range(0, Chunk.ChunkWidth - 1);

            var playerWorldPos = new Vector3(
                playerInChunkPos.x + Chunk.ChunkWidth * chunkPos.x,
                0,
                playerInChunkPos.y + Chunk.ChunkWidth * chunkPos.y
            );

            for (var chunkY = 0; chunkY < Chunk.ChunkHeight; chunkY++)
                if (chunk.blocks[playerInChunkPos.x, chunkY, playerInChunkPos.y] == BlockType.Air)
                {
                    playerWorldPos.y = chunkY + 1;
                    break;
                } else if (chunkY == Chunk.ChunkHeight - 1)
                    playerWorldPos.y = chunkY + 2;

            player = Instantiate(playerPrefab, playerWorldPos, Quaternion.identity, transform);
            player.world = this;
        }
    }
}
