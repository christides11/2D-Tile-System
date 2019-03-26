using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System;
using Random = UnityEngine.Random;
using Unity.Collections;

public class MapManager : MonoBehaviour
{
    public delegate void PlaceTileAction(Vector2Int chunk, Vector2Int pos, TileBase tb, MapLayers layer);
    public event PlaceTileAction OnPlaceTile;

    public delegate void TickAction();
    public event TickAction OnTick;

    public static MapManager instance;
    public static MapDefinition map;
    public TileCollection tCol;
    public int mapWidth;
    public int mapHeight;
    public int chunkWidth;
    public int chunkHeight;
    public string seed;
    public Vector2 scale = new Vector2(1,1);

    public string[,] temporaryMap = null;
    public string[,] temporaryMapBG = null;

    [HideInInspector] public bool mapGenerated;

    public int chunksX { get; private set; }
    public int chunksY { get; private set; }
    public int currentTick { get; private set; }
    [SerializeField] [Range(1, 100)]private int ticksPerSecond = 1;

    [Header("Debug")]
    public string[] tempTiles;

    private float tickTimer;

    void Start()
    {
        instance = this;
        tCol.BuildDictionary();
        mapGenerated = false;
        InitMap();
    }

    private void Update()
    {
        if (mapGenerated)
        {
            tickTimer += Time.deltaTime;
            while(tickTimer >= (ticksPerSecond/1.0f))
            {
                tickTimer -= (ticksPerSecond/1.0f);
                if (OnTick != null)
                {
                    OnTick();
                }
            }
        }
    }

    public void InitMap()
    {
        map = new MapDefinition();
        temporaryMap = new string[mapWidth, mapHeight];
        temporaryMapBG = new string[mapWidth, mapHeight];
        currentTick = 0;
        //Seed
        try
        {
            map.seed = int.Parse(seed);
        } catch
        {
            map.seed = seed.GetHashCode();
        }
        UnityEngine.Random.InitState(map.seed);
        chunksX = mapWidth / chunkWidth;
        chunksY = mapHeight / chunkHeight;
        map.chunks = new ChunkDefinition[mapWidth/chunkWidth, mapHeight/chunkHeight];
        for(int i = 0; i < map.chunks.GetLength(0); i++)
        {
            for(int j = 0; j < map.chunks.GetLength(1); j++)
            {
                map.chunks[i, j] = new ChunkDefinition(chunkWidth, chunkHeight);
            }
        }
        GenWorld();
    }

    public void GenWorld()
    {
        for(int i = 0; i < temporaryMap.GetLength(0); i++)
        {
            for (int j = 0; j < temporaryMap.GetLength(1); j++)
            {
                if (Random.Range(0, 100) > 50) {
                    int inx = Random.Range(0, tempTiles.Length);
                    temporaryMap[i, j] = tempTiles[inx];
                } else
                {
                    temporaryMap[i, j] = null;
                }
            }
        }
        TransferToMap();
    }

    void TransferToMap()
    {
        for(int i = 0; i < temporaryMap.GetLength(0); i++)
        {
            for (int j = 0; j < temporaryMap.GetLength(1); j++)
            {
                SetTile(i, j, temporaryMap[i,j], MapLayers.FG, false);
            }
        }
        temporaryMap = null;
        mapGenerated = true;
    }

    Vector2Int bPos = new Vector2Int();
    public Vector2Int WorldToBlock(Vector2 pos)
    {
        bPos.x = Mathf.FloorToInt(pos.x / scale.x);
        bPos.y = Mathf.FloorToInt(pos.y / scale.y);
        return bPos;
    }

    #region Set Tile
    TileBase tb = null;
    public void SetTile(int chunkX, int chunkY, int x, int y, string blockID, MapLayers layer, bool callEvent = true)
    {
        if(x < 0 || y < 0)
        {
            return;
        }
        short chunkBlockID = 0;
        ChunkDefinition chun = map.chunks[chunkX, chunkY];
        //Update pallete
        if (!ReferenceEquals(blockID, null))
        {
            if (!chun.chunkPallete.Contains(blockID))
            {
                chun.chunkPallete.Add(blockID);
                chunkBlockID = (short)(chun.chunkPallete.Count);
            } else
            {
                chunkBlockID = (short)(chun.chunkPallete.IndexOf(blockID) + 1);
            }
        }
        //Place block
        int X = x - (chunkX * chunkWidth);
        int Y = y - (chunkY * chunkHeight);
        chun.tileLayers[layer][X, Y] = new TileDefinition();
        chun.tileLayers[layer][X, Y].tile = chunkBlockID;
        tb = null;
        if (!ReferenceEquals(blockID, null))
        {
            tb = TileCollection.GetTile(chun.chunkPallete[chunkBlockID - 1]);
            tb.OnAddedToMap(x, y, layer);
        }
        if (callEvent)
        {
            OnPlaceTile(new Vector2Int(chunkX, chunkY), new Vector2Int(X, Y), tb, layer);
        }
    }

    public void SetTile(int x, int y, string blockID, MapLayers layer, bool callEvent = true)
    {
        int cX = x / chunkWidth;
        int cY = y / chunkHeight;
        SetTile(cX, cY, x, y, blockID, layer, callEvent);
    }
    #endregion

    #region Get Tile
    public string GetTile(int chunkX, int chunkY, int x, int y, MapLayers layer)
    {
        return map.chunks[chunkX, chunkY].GetTile(x - (chunkX * chunkWidth), y - (chunkY * chunkHeight), layer);
    }

    public string GetTile(int x, int y, MapLayers layer)
    {
        int cX = x / chunkWidth;
        int cY = y / chunkHeight;
        if(cX > map.chunks.GetLength(0) || cX < 0 || cY > map.chunks.GetLength(1) || cY < 0)
        {
            return null;
        }
        return map.chunks[cX, cY].GetTile(x - (cX * chunkWidth), y - (cY * chunkHeight), layer);
    }
    #endregion

    #region Bitmask
    public void SetBitmask(int chunkX, int chunkY, int x, int y, byte mask, MapLayers layer, bool callEvent = true)
    {
        ChunkDefinition chun = map.chunks[chunkX, chunkY];
        map.chunks[chunkX, chunkY].tileLayers[layer][x, y].bitmask = mask;
    }

    public void SetBitmask(int x, int y, byte mask, MapLayers layer, bool callEvent = true)
    {
        int cX = x / chunkWidth;
        int cY = y / chunkHeight;
        SetBitmask(cX, cY, x, y, mask, layer, callEvent);
    }

    public byte GetBitmask(int x, int y, MapLayers layer)
    {
        int cX = x / chunkWidth;
        int cY = y / chunkHeight;
        return map.chunks[cX, cY].tileLayers[layer][x - (cX * chunkWidth), y - (cY * chunkHeight)].bitmask;
    }
    #endregion
}
