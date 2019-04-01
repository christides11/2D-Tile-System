using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapDefinition
{
    public ChunkDefinition[,] chunks;
    public int seed;
}

//4 bytes
[System.Serializable]
public class TileDefinition
{
    //The tile in relation to the chunk pallete.
    public short tile;
    //The texture variation this tile is using.
    public byte variation;
    //The bitmask of this tile, if applicable. 
    public byte bitmask;
}

[System.Serializable]
public class WaterTileDefinition : TileDefinition
{
    public float amt;
    public bool settled;
}

[System.Serializable]
public class ChunkDefinition
{
    //The last tick the chunk was updated.
    public uint lastTick;
    //The tiles in this chunk.
    public Dictionary<MapLayers, TileDefinition[,]> tileLayers;
    //Chunk pallete
    public List<TileString> chunkPallete;

    public ChunkDefinition(int chunkWidth, int chunkHeight)
    {
        //lastTick = 0;
        tileLayers = new Dictionary<MapLayers, TileDefinition[,]>();
        foreach(MapLayers ml in Enum.GetValues(typeof(MapLayers)))
        {
            tileLayers.Add(ml, new TileDefinition[chunkWidth, chunkHeight]);
        }
        chunkPallete = new List<TileString>();
    }

    TileDefinition td;
    public TileString GetTile(int x, int y, MapLayers layer)
    {
        if(chunkPallete.Count == 0)
        {
            return null;
        }
        td = tileLayers[layer][x, y];
        if (td == null)
        {
            return null;
        }
        if (td.tile == 0)
        {
            return null;
        }
        return chunkPallete[td.tile-1];
    }
}