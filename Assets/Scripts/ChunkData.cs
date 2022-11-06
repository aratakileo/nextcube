using UnityEngine;

public class ChunkData
{
    public Vector2Int position;
    public readonly BlockType[,,] blocks;

    public ChunkData(Vector2Int position, BlockType[,,] blocks) {
        this.position = position;
        this.blocks = blocks;
    }
}
