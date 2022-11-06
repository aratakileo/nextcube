using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    public const int ChunkWidth = 10;
    public const int ChunkHeight = 128;
    public const float BlockScale = 0.5f;

    private readonly List<Vector3> _vertexes = new();
    private readonly List<int> _triangles = new();

    public ChunkData chunkData;
    public GameWorld parentWorld;

    private void Start()
    {
        var chunkMesh = new Mesh();

        for (var y = 0; y < ChunkHeight; y++)
            for (var x = 0; x < ChunkWidth; x++)
                for (var z = 0; z < ChunkWidth; z++)
                {
                    GenerateBlock(x, y, z);
                }

        chunkMesh.vertices = _vertexes.ToArray();
        chunkMesh.triangles = _triangles.ToArray();

        chunkMesh.Optimize();

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = chunkMesh;
        UpdateMeshCollider(chunkMesh);
    }

    private void UpdateMeshCollider(Mesh chunkMesh)
    {
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
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
