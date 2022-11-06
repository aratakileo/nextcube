using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 10;
    public const int ChunkHeight = 128;
    public const float BlockScale = 0.5f;

    private Mesh blocksMesh;
    
    private readonly List<Vector3> _vertexes = new();
    private readonly List<int> _triangles = new();

    public ChunkData chunkData;
    public GameWorld parentWorld;

    private void Start()
    {
        blocksMesh = new Mesh();

        GetComponent<MeshFilter>().mesh = blocksMesh;
        
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

        blocksMesh.triangles = Array.Empty<int>();
        blocksMesh.vertices = _vertexes.ToArray();
        blocksMesh.triangles = _triangles.ToArray();

        blocksMesh.Optimize();

        blocksMesh.RecalculateNormals();
        blocksMesh.RecalculateBounds();
        
        GetComponent<MeshCollider>().sharedMesh = blocksMesh;
    }

    private BlockType GetBlockAt(Vector3Int blockPosition)
    {
        if (blockPosition.x is >= 0 and < ChunkWidth 
            && blockPosition.y is >= 0 and < ChunkHeight 
            && blockPosition.z is >= 0 and < ChunkWidth)
        {
            return chunkData.blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }

        if (blockPosition.y is < 0 or >= ChunkHeight)
            return BlockType.Air;

        var adjacentChunkPosition = chunkData.position;

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

        if (parentWorld.chunksData.TryGetValue(adjacentChunkPosition, out var adjacentChunk))
            return adjacentChunk.blocks[blockPosition.x, blockPosition.y, blockPosition.z];

        return BlockType.Air;
    }

    public void SetBlock(Vector3Int inChunkPosition, BlockType blockType)
    {
        chunkData.blocks[inChunkPosition.x, inChunkPosition.y, inChunkPosition.z] = blockType;
        GenerateMesh();
        
        // Generate mesh nearby chunks
        if (inChunkPosition.x is 0 or ChunkWidth - 1)
        {
            print("Horizontal");
            if (parentWorld.chunksData.TryGetValue(chunkData.position + Vector2Int.left, out var nearbyLeft))
                nearbyLeft.renderer.GenerateMesh();

            if (parentWorld.chunksData.TryGetValue(chunkData.position + Vector2Int.right, out var nearbyRight))
                nearbyRight.renderer.GenerateMesh();
        }
        
        if (inChunkPosition.z is not (0 or ChunkWidth - 1)) return;
        
        if (parentWorld.chunksData.TryGetValue(chunkData.position + Vector2Int.up, out var nearbyUp))
            nearbyUp.renderer.GenerateMesh();

        if (parentWorld.chunksData.TryGetValue(chunkData.position + Vector2Int.down, out var nearbyDown))
            nearbyDown.renderer.GenerateMesh();
    }

    public void DestroyBlock(Vector3Int inChunkPosition)
    {
        SetBlock(inChunkPosition, BlockType.Air);
    }

    private void GenerateBlock(int x, int y, int z)
    {
        var blockPosition = new Vector3Int(x, y, z);

        if (GetBlockAt(blockPosition) == 0) return;

        if (GetBlockAt(blockPosition + Vector3Int.right) == 0) GenerateRightSide(blockPosition);
        if (GetBlockAt(blockPosition + Vector3Int.left) == 0) GenerateLeftSide(blockPosition);
        if (GetBlockAt(blockPosition + Vector3Int.forward) == 0) GenerateFrontSide(blockPosition);
        if (GetBlockAt(blockPosition + Vector3Int.back) == 0) GenerateBackSide(blockPosition);
        if (GetBlockAt(blockPosition + Vector3Int.up) == 0) GenerateTopSide(blockPosition);
        if (GetBlockAt(blockPosition + Vector3Int.down) == 0) GenerateBottomSide(blockPosition);
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
        _vertexes.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);

        AddLastVertexesSquare();
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        _vertexes.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVertexesSquare();
    }

    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        _vertexes.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVertexesSquare();
    }

    private void GenerateBackSide(Vector3Int blockPosition)
    {
        _vertexes.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);

        AddLastVertexesSquare();
    }

    private void GenerateTopSide(Vector3Int blockPosition)
    {
        _vertexes.Add((new Vector3(0, 1, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(0, 1, 1) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 1, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 1, 1) + blockPosition) * BlockScale);

        AddLastVertexesSquare();
    }

    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        _vertexes.Add((new Vector3(0, 0, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 0, 0) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(0, 0, 1) + blockPosition) * BlockScale);
        _vertexes.Add((new Vector3(1, 0, 1) + blockPosition) * BlockScale);

        AddLastVertexesSquare();
    }
}
