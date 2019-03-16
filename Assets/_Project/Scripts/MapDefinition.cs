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
    //The tiles in this chunk.
    public Dictionary<MapLayers, short[,]> tileLayers;
    //Chunk pallete
    public List<string> chunkPallete;

    //Info for each block in the chunk.
    //[SerializeField] public BlockStateInfo[,] blockStateInfo;

    public ChunkDefinition(int chunkWidth, int chunkHeight)
    {
        lastTick = 0;
        tileLayers = new Dictionary<MapLayers, short[,]>();
        foreach(MapLayers ml in Enum.GetValues(typeof(MapLayers)))
        {
            tileLayers.Add(ml, new short[chunkWidth, chunkHeight]);
        }
        chunkPallete = new List<string>();
    }

    short[,] sh;
    public string GetTile(int x, int y, MapLayers layer)
    {
        if (!tileLayers.TryGetValue(layer, out sh))
        {
            return null;
        }
        if (sh[x,y] == 0 || chunkPallete.Count == 0)
        {
            return null;
        } else
        {
            return chunkPallete[sh[x,y]-1];
        }
    }
}