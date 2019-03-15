using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System;
using Random = UnityEngine.Random;

public class MapManager : MonoBehaviour
{
    public MapDefinition map;
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

    public int chunksX;
    public int chunksY;

    [Header("Debug")]
    public string tempTile;

    void Start()
    {
        tCol.BuildDictionary();
        mapGenerated = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            InitMap();
        }
    }

    public void InitMap()
    {
        map = new MapDefinition();
        temporaryMap = new string[mapWidth, mapHeight];
        temporaryMapBG = new string[mapWidth, mapHeight];
        //Seed
        try
        {
            map.seed = int.Parse(seed);
            UnityEngine.Random.InitState(map.seed);
        } catch (Exception e)
        {
            map.seed = seed.GetHashCode();
            UnityEngine.Random.InitState(map.seed);
        }
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
                temporaryMap[i, j] = Random.Range(0, 100) > 50 ? tempTile : null;
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
                SetTile(i, j, temporaryMap[i,j], true);
            }
        }
        temporaryMap = null;
        mapGenerated = true;
    }

    #region Set Tile
    public void SetTile(int chunkX, int chunkY, int x, int y, string blockID, bool FG)
    {
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
        if (FG)
        {
            chun.fgTiles[X, Y] = chunkBlockID;
        } else
        {
            chun.bgTiles[X, Y] = chunkBlockID;
        }
        if (!ReferenceEquals(blockID, null))
        {
            TileCollection.GetTile(chun.chunkPallete[chunkBlockID - 1]).OnAddedToMap(x, y);
        }
    }

    public void SetTile(int x, int y, string blockID, bool FG)
    {
        int cX = x / chunkWidth;
        int cY = y / chunkHeight;
        SetTile(cX, cY, x, y, blockID, FG);
    }
    #endregion

    #region Get Tile
    public string GetTile(int chunkX, int chunkY, int x, int y, bool FG = true)
    {
        return map.chunks[chunkX, chunkY].GetTile(x - (chunkX * chunkWidth), y - (chunkY * chunkHeight), FG);
    }

    public string GetTile(int x, int y, bool FG = true)
    {
        int cX = x / chunkWidth;
        int cY = y / chunkHeight;
        if(cX > map.chunks.GetLength(0) || cX < 0 || cY > map.chunks.GetLength(1) || cY < 0)
        {
            return null;
        }
        return map.chunks[cX, cY].GetTile(x - (cX * chunkWidth), y - (cY * chunkHeight), FG);
    }
    #endregion

}
