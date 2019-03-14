using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapDefinition
{
    [SerializeField] public ChunkDefinition[,] chunks;
    [SerializeField] public int seed;
}

[System.Serializable]
public class ChunkDefinition
{
    [NonSerialized] public bool rendered = false;
    //The last tick the chunk was updated.
    public uint lastTick;
    //The background blocks in this chunk.
    public short[,] bgTiles;
    //The foreground blocks in this chunk.
    public short[,] fgTiles;
    //Chunk pallete
    public List<string> chunkPallete;

    //Info for each block in the chunk.
    //[SerializeField] public BlockStateInfo[,] blockStateInfo;

    public ChunkDefinition(int chunkWidth, int chunkHeight)
    {
        lastTick = 0;
        bgTiles = new short[chunkWidth, chunkHeight];
        fgTiles = new short[chunkWidth, chunkHeight];
        chunkPallete = new List<string>();
    }

    public string GetTile(int x, int y, bool FG)
    {
        if (FG)
        {
            if (fgTiles[x, y] == 0 || chunkPallete.Count == 0)
            {
                return null;
            } else
            {
                return chunkPallete[fgTiles[x, y] - 1];
            }
        } else
        {
            if (bgTiles[x, y] == 0 || chunkPallete.Count == 0)
            {
                return null;
            } else
            {
                return chunkPallete[bgTiles[x, y] - 1];
            }
        }
    }
}