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

    public string[,] temporaryMap = null;
    public string[,] temporaryMapBG = null;

    public string tempTile;

    void Start()
    {
        tCol.BuildDictionary();
        temporaryMap = new string[mapWidth, mapHeight];
        temporaryMapBG = new string[mapWidth, mapHeight];
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
        map.chunks = new Chunk[mapWidth/chunkWidth, mapHeight/chunkHeight];
        for(int i = 0; i < map.chunks.GetLength(0); i++)
        {
            for(int j = 0; j < map.chunks.GetLength(1); j++)
            {
                map.chunks[i, j] = new Chunk(chunkWidth, chunkHeight);
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
    }

    #region Set Tile
    public void SetTile(int chunkX, int chunkY, int x, int y, string blockID, float LAbs, float LEm, bool FG)
    {
        short chunkBlockID = 0;
        Chunk chun = map.chunks[chunkX, chunkY];
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
            //chun.lightAbsorbed[X, Y] = LAbs;
            //chun.lightEmitted[X, Y] = LEm;
        } else
        {
            chun.bgTiles[X, Y] = chunkBlockID;
            //if (chun.blocks[X, Y] == 0)
            //{
            //    chun.lightAbsorbed[X, Y] = LAbs;
            //    chun.lightEmitted[X, Y] = LEm;
            //}
        }
        //if (!ReferenceEquals(blockID, null))
        //{
        //    TileCollection.GetTile(chun.chunkPallete[chunkBlockID - 1]).OnAddedToMap(x, y);
        //}
    }

    public void SetTile(int x, int y, string blockID, bool FG)
    {
        float LAbs = 0;
        float LEm = 0;
        int cX = x / chunkWidth;
        int cY = y / chunkHeight;
        //if (!ReferenceEquals(blockID, null))
        //{
        //    Block b = ItemList.GetBlock(blockID);
        //    LAbs = b.lightAbsorbed;
        //    LEm = b.lightEmitted;
        //
        //}
        //SetBlock(cX, cY, x, y, blockID, LAbs, LEm, FG);
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
        return map.chunks[cX, cY].GetTile(x - (cX * chunkWidth), y - (cY * chunkHeight), FG);
    }
    #endregion

}
