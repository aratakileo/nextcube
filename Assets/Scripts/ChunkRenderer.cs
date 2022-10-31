using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    public const int CHUNK_WIDTH = 10;
    public const int CHUNK_HEIGHT = 128;
    public const float BLOCK_SCALE = 0.5f;

    private List<Vector3> vertecies = new List<Vector3>();
    private List<int> triangles = new List<int>();

    public ChunkData chunkData;
    public GameWorld parentWorld;

    private void Start()
    {
        var chunkMesh = new Mesh();

        for (int y = 0; y < CHUNK_HEIGHT; y++)
            for (int x = 0; x < CHUNK_WIDTH; x++)
                for (int z = 0; z < CHUNK_WIDTH; z++)
                {
                    GenerateBlock(x, y, z);
                }

        chunkMesh.vertices = vertecies.ToArray();
        chunkMesh.triangles = triangles.ToArray();

        chunkMesh.Optimize();

        chunkMesh.RecalculateNormals();
        chunkMesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = chunkMesh;
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;
    }

    private BlockType GetBlockAt(Vector3Int blockPosition)
    {
        if (blockPosition.x >= 0 && blockPosition.x < CHUNK_WIDTH
            && blockPosition.y >= 0 && blockPosition.y < CHUNK_HEIGHT
            && blockPosition.z >= 0 && blockPosition.z < CHUNK_WIDTH)
        {
            return chunkData.blocks[blockPosition.x, blockPosition.y, blockPosition.z];
        }

        if (blockPosition.y < 0 || blockPosition.y >= CHUNK_HEIGHT)
            return BlockType.Air;

        var adjacentChunkPosition = chunkData.position;

        if (blockPosition.x < 0)
        {
            adjacentChunkPosition.x--;
            blockPosition.x += CHUNK_WIDTH;
        }
        else if (blockPosition.x >= CHUNK_WIDTH)
        {
            adjacentChunkPosition.x++;
            blockPosition.x -= CHUNK_WIDTH;
        }

        if (blockPosition.z < 0)
        {
            adjacentChunkPosition.y--;
            blockPosition.z += CHUNK_WIDTH;
        }
        else if (blockPosition.z >= CHUNK_WIDTH)
        {
            adjacentChunkPosition.y++;
            blockPosition.z -= CHUNK_WIDTH;
        }

        if (parentWorld.chunkDatas.TryGetValue(adjacentChunkPosition, out ChunkData adjacentChunk))
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

    private void AddLastVerticiesSquare()
    {
        triangles.Add(vertecies.Count - 4);
        triangles.Add(vertecies.Count - 3);
        triangles.Add(vertecies.Count - 2);

        triangles.Add(vertecies.Count - 3);
        triangles.Add(vertecies.Count - 1);
        triangles.Add(vertecies.Count - 2);
    }

    private void GenerateLeftSide(Vector3Int blockPosition)
    {
        vertecies.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);

        AddLastVerticiesSquare();
    }

    private void GenerateRightSide(Vector3Int blockPosition)
    {
        vertecies.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);

        AddLastVerticiesSquare();
    }

    private void GenerateFrontSide(Vector3Int blockPosition)
    {
        vertecies.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);

        AddLastVerticiesSquare();
    }

    private void GenerateBackSide(Vector3Int blockPosition)
    {
        vertecies.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);

        AddLastVerticiesSquare();
    }

    private void GenerateTopSide(Vector3Int blockPosition)
    {
        vertecies.Add((new Vector3(0, 1, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(0, 1, 1) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 1, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 1, 1) + blockPosition) * BLOCK_SCALE);

        AddLastVerticiesSquare();
    }

    private void GenerateBottomSide(Vector3Int blockPosition)
    {
        vertecies.Add((new Vector3(0, 0, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 0, 0) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(0, 0, 1) + blockPosition) * BLOCK_SCALE);
        vertecies.Add((new Vector3(1, 0, 1) + blockPosition) * BLOCK_SCALE);

        AddLastVerticiesSquare();
    }
}
