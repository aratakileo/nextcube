using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nextcube
{
    public class Chunk : MonoBehaviour
    {
        public const int ChunkWidth = 10;
        public const int ChunkHeight = 128;

        private Mesh _blocksMesh;

        private readonly List<Vector3> _vertexes = new();
        private readonly List<int> _triangles = new();

        public Vector2Int position;
        public BlockType[,,] blocks;

        public World world;

        private void Start()
        {
            _blocksMesh = new Mesh();

            GetComponent<MeshFilter>().mesh = _blocksMesh;

            GenerateMesh();
        }

        private void GenerateMesh()
        {
            _vertexes.Clear();
            _triangles.Clear();

            for (var y = 0; y < ChunkHeight; y++)
            for (var x = 0; x < ChunkWidth; x++)
            for (var z = 0; z < ChunkWidth; z++)
            {
                GenerateBlock(x, y, z);
            }

            _blocksMesh.triangles = Array.Empty<int>();
            _blocksMesh.vertices = _vertexes.ToArray();
            _blocksMesh.triangles = _triangles.ToArray();

            _blocksMesh.Optimize();

            _blocksMesh.RecalculateNormals();
            _blocksMesh.RecalculateBounds();

            GetComponent<MeshCollider>().sharedMesh = _blocksMesh;
        }

        private BlockType GetBlockAt(Vector3Int blockPosition)
        {
            if (blockPosition.x is >= 0 and < ChunkWidth
                && blockPosition.y is >= 0 and < ChunkHeight
                && blockPosition.z is >= 0 and < ChunkWidth) {
                return blocks[blockPosition.x, blockPosition.y, blockPosition.z];
            }

            if (blockPosition.y is < 0 or >= ChunkHeight)
                return BlockType.Air;

            var adjacentChunkPosition = position;

            switch (blockPosition.x)
            {
                case < 0:
                    adjacentChunkPosition.x--;
                    blockPosition.x += ChunkWidth;
                    break;
                case >= ChunkWidth:
                    adjacentChunkPosition.x++;
                    blockPosition.x -= ChunkWidth;
                    break;
            }

            switch (blockPosition.z)
            {
                case < 0:
                    adjacentChunkPosition.y--;
                    blockPosition.z += ChunkWidth;
                    break;
                case >= ChunkWidth:
                    adjacentChunkPosition.y++;
                    blockPosition.z -= ChunkWidth;
                    break;
            }

            if (world.chunks.TryGetValue(adjacentChunkPosition, out var adjacentChunk))
                return adjacentChunk.blocks[blockPosition.x, blockPosition.y, blockPosition.z];

            return BlockType.Air;
        }

        public void SetBlock(Vector3Int inChunkPosition, BlockType blockType)
        {
            blocks[inChunkPosition.x, inChunkPosition.y, inChunkPosition.z] = blockType;
            GenerateMesh();

            // Generate mesh nearby chunks
            if (inChunkPosition.x is 0 or ChunkWidth - 1)
            {
                if (world.chunks.TryGetValue(position + Vector2Int.left, out var nearbyLeft))
                    nearbyLeft.GenerateMesh();

                if (world.chunks.TryGetValue(position + Vector2Int.right, out var nearbyRight))
                    nearbyRight.GenerateMesh();
            }

            if (inChunkPosition.z is not (0 or ChunkWidth - 1)) return;

            if (world.chunks.TryGetValue(position + Vector2Int.up, out var nearbyUp))
                nearbyUp.GenerateMesh();

            if (world.chunks.TryGetValue(position + Vector2Int.down, out var nearbyDown))
                nearbyDown.GenerateMesh();
        }

        public void DestroyBlock(Vector3Int inChunkPosition)
        {
            SetBlock(inChunkPosition, BlockType.Air);
        }

        private void GenerateBlock(int x, int y, int z)
        {
            var blockPosition = new Vector3Int(x, y, z);

            if (GetBlockAt(blockPosition) == BlockType.Air) return;
            
            if (GetBlockAt(blockPosition + Vector3Int.right) == BlockType.Air) GenerateRightSide(blockPosition);
            if (GetBlockAt(blockPosition + Vector3Int.left) == BlockType.Air) GenerateLeftSide(blockPosition);
            if (GetBlockAt(blockPosition + Vector3Int.forward) == BlockType.Air) GenerateFrontSide(blockPosition);
            if (GetBlockAt(blockPosition + Vector3Int.back) == BlockType.Air) GenerateBackSide(blockPosition);
            if (GetBlockAt(blockPosition + Vector3Int.up) == BlockType.Air) GenerateTopSide(blockPosition);
            if (GetBlockAt(blockPosition + Vector3Int.down) == BlockType.Air) GenerateBottomSide(blockPosition);
        }

        private void AddLastVertexesSquare()
        {
            _triangles.Add(_vertexes.Count - 4);
            _triangles.Add(_vertexes.Count - 3);
            _triangles.Add(_vertexes.Count - 2);

            _triangles.Add(_vertexes.Count - 3);
            _triangles.Add(_vertexes.Count - 1);
            _triangles.Add(_vertexes.Count - 2);
        }

        private void GenerateLeftSide(Vector3Int blockPosition)
        {
            _vertexes.Add((new Vector3(0, 0, 0) + blockPosition));
            _vertexes.Add((new Vector3(0, 0, 1) + blockPosition));
            _vertexes.Add((new Vector3(0, 1, 0) + blockPosition));
            _vertexes.Add((new Vector3(0, 1, 1) + blockPosition));

            AddLastVertexesSquare();
        }

        private void GenerateRightSide(Vector3Int blockPosition)
        {
            _vertexes.Add((new Vector3(1, 0, 0) + blockPosition));
            _vertexes.Add((new Vector3(1, 1, 0) + blockPosition));
            _vertexes.Add((new Vector3(1, 0, 1) + blockPosition));
            _vertexes.Add((new Vector3(1, 1, 1) + blockPosition));

            AddLastVertexesSquare();
        }

        private void GenerateFrontSide(Vector3Int blockPosition)
        {
            _vertexes.Add((new Vector3(0, 0, 1) + blockPosition));
            _vertexes.Add((new Vector3(1, 0, 1) + blockPosition));
            _vertexes.Add((new Vector3(0, 1, 1) + blockPosition));
            _vertexes.Add((new Vector3(1, 1, 1) + blockPosition));

            AddLastVertexesSquare();
        }

        private void GenerateBackSide(Vector3Int blockPosition)
        {
            _vertexes.Add((new Vector3(0, 0, 0) + blockPosition));
            _vertexes.Add((new Vector3(0, 1, 0) + blockPosition));
            _vertexes.Add((new Vector3(1, 0, 0) + blockPosition));
            _vertexes.Add((new Vector3(1, 1, 0) + blockPosition));

            AddLastVertexesSquare();
        }

        private void GenerateTopSide(Vector3Int blockPosition)
        {
            _vertexes.Add((new Vector3(0, 1, 0) + blockPosition));
            _vertexes.Add((new Vector3(0, 1, 1) + blockPosition));
            _vertexes.Add((new Vector3(1, 1, 0) + blockPosition));
            _vertexes.Add((new Vector3(1, 1, 1) + blockPosition));

            AddLastVertexesSquare();
        }

        private void GenerateBottomSide(Vector3Int blockPosition)
        {
            _vertexes.Add((new Vector3(0, 0, 0) + blockPosition));
            _vertexes.Add((new Vector3(1, 0, 0) + blockPosition));
            _vertexes.Add((new Vector3(0, 0, 1) + blockPosition));
            _vertexes.Add((new Vector3(1, 0, 1) + blockPosition));

            AddLastVertexesSquare();
        }
    }
}
