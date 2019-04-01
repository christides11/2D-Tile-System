using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System;
using Random = System.Random;
using Unity.Collections;
using System.Threading;
using UnityEngine.UI;
using TMPro;

public class MapManager : MonoBehaviour
{
    public delegate void PlaceTileAction(Vector2Int chunk, Vector2Int pos, TileBase tb, MapLayers layer);
    public static event PlaceTileAction OnPlaceTile;

    public delegate void TickAction();
    public static event TickAction OnTick;

    public static MapManager instance;
    public MapDefinition map;
    public TileCollections tCollections;
    public IGenerator[] worldGens;
    public int mapWidth;
    public int mapHeight;
    public int chunkWidth;
    public int chunkHeight;
    public string seed;
    public Vector2 scale = new Vector2(1,1);

    public Dictionary<MapLayers, TileString[,]> temporaryMap = new Dictionary<MapLayers, TileString[,]>();

    [HideInInspector] public bool mapGenerated;

    public int chunksX { get; private set; }
    public int chunksY { get; private set; }
    public int currentTick { get; private set; }
    [SerializeField] [Range(1, 100)]private int ticksPerSecond = 1;

    [Header("Debug")]
    public TextMeshProUGUI progressText;
    public Image progressBar;
    public double progress;
    public string currentProgress;
    private float tickTimer;

    void Start()
    {
        instance = this;
        tCollections.BuildDictionary();
        mapGenerated = false;
    }

    private void Update()
    {
        if(!mapGenerated && Input.GetKeyDown(KeyCode.T))
        {
            var thread = new Thread(InitMap);
            thread.Start();
            //InitMap();
        }

        if (mapGenerated)
        {
            tickTimer += Time.deltaTime;
            while(tickTimer >= (1.0f/ticksPerSecond))
            {
                tickTimer -= (1.0f/ticksPerSecond);
                if (OnTick != null)
                {
                    OnTick();
                }
            }
        }
    }

    
    private void FixedUpdate()
    {
        if (!mapGenerated)
        {
            progressBar.fillAmount = (float)progress;
            progressText.text = currentProgress;
        }
    }

    Random ran;
    public void InitMap()
    {
        map = new MapDefinition();
        temporaryMap.Add(MapLayers.FG, new TileString[mapWidth, mapHeight]);
        temporaryMap.Add(MapLayers.BG, new TileString[mapWidth, mapHeight]);
        currentTick = 0;
        //Seed
        try
        {
            map.seed = int.Parse(seed);
        } catch
        {
            map.seed = seed.GetHashCode();
        }
        ran = new Random(map.seed);
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

    public async void GenWorld()
    {
        for (int i = 0; i < worldGens.Length; i++)
        {
            IGenerator g = worldGens[i];
            if (g != null)
            {
            await g.Generate(map.seed, ran, this);
            }
        }

        TransferToMap();
    }

    void TransferToMap()
    {
        currentProgress = "Transfering Map";
        progress = 0;
        double pDiv = 1.0/(mapWidth*mapHeight);
        for(int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                foreach(MapLayers ml in temporaryMap.Keys)
                {
                    SetTile(i, j, temporaryMap[ml][i,j], ml, false, false);
                }
                progress += pDiv;
            }
        }
        currentProgress = "Refreshing tiles";
        progress = 0;
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                foreach (MapLayers ml in temporaryMap.Keys)
                {
                    RefreshTile(new Vector2Int(i, j), ml);
                }
                progress += pDiv;
            }
        }
        progress = 1;
        temporaryMap = null;
        mapGenerated = true;
        Debug.Log("Done!");
    }

    public Vector2Int WorldToBlock(Vector2 pos)
    {
        Vector2Int bPos = new Vector2Int();
        bPos.x = Mathf.FloorToInt(pos.x / scale.x);
        bPos.y = Mathf.FloorToInt(pos.y / scale.y);
        return bPos;
    }

    #region Set Tile
    public void SetTile(int chunkX, int chunkY, int x, int y, TileString blockID, MapLayers layer, 
        bool callEvent = true, bool refreshSurronding = true, bool ignoreSameTile = false)
    {
        TileBase tb;
        short chunkBlockID = 0;
        ChunkDefinition chun = map.chunks[chunkX, chunkY];
        int X = x - (chunkX * chunkWidth);
        int Y = y - (chunkY * chunkHeight);
        if (ignoreSameTile)
        {
            if(chun.GetTile(X, Y, layer) == blockID)
            {
                return;
            }
        }
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
        if (ReferenceEquals(chun.tileLayers[layer][X, Y], null))
        {
            chun.tileLayers[layer][X, Y] = new TileDefinition();
        }
        chun.tileLayers[layer][X, Y].tile = chunkBlockID;
        tb = null;
        if (refreshSurronding)
        {
            if (!ReferenceEquals(blockID, null))
            {
                tb = MapManager.instance.GetTileFromCollection(chun.chunkPallete[chunkBlockID - 1]);
                tb.RefreshTile(new Vector2Int(x, y), layer);
            }
            RefreshTilesAround(new Vector2Int(x, y), layer);
        }
        if (callEvent)
        {
            OnPlaceTile(new Vector2Int(chunkX, chunkY), new Vector2Int(X, Y), tb, layer);
        }
    }

    public void SetTile(int x, int y, TileString blockID, MapLayers layer, 
        bool callEvent = true, bool refreshSurronding = true, bool ignoreSameTile = false)
    {
        if (x < 0 || y < 0)
        {
            return;
        }
        int cX = x / chunkWidth;
        int cY = y / chunkHeight;
        SetTile(cX, cY, x, y, blockID, layer, callEvent, refreshSurronding, ignoreSameTile);
    }
    #endregion

    #region Get Tile
    public TileString GetTile(int chunkX, int chunkY, int x, int y, MapLayers layer)
    {
        return map.chunks[chunkX, chunkY].GetTile(x - (chunkX * chunkWidth), y - (chunkY * chunkHeight), layer);
    }

    public TileString GetTile(int x, int y, MapLayers layer)
    {
        if (x >= mapWidth || x < 0 || y >= mapHeight || y < 0)
        {
            return null;
        }
        int cX = x / chunkWidth;
        int cY = y / chunkHeight;
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
        SetBitmask(cX, cY, x - (cX * chunkWidth), y - (cY * chunkHeight), mask, layer, callEvent);
    }

    public byte GetBitmask(int x, int y, MapLayers layer)
    {
        int cX = x / chunkWidth;
        int cY = y / chunkHeight;
        return map.chunks[cX, cY].tileLayers[layer][x - (cX * chunkWidth), y - (cY * chunkHeight)].bitmask;
    }
    #endregion

    public void RefreshTilesAround(Vector2Int position, MapLayers layer)
    {
        RefreshTile(position+Vector2Int.up, layer);
        RefreshTile(position + Vector2Int.down, layer);
        RefreshTile(position + Vector2Int.left, layer);
        RefreshTile(position + Vector2Int.right, layer);
    }

    public void RefreshTile(Vector2Int position, MapLayers layer)
    {
        TileString t = GetTile(position.x, position.y, layer);
        if (!ReferenceEquals(t, null))
        {
            TileBase tb = GetTileFromCollection(t);
            tb.RefreshTile(position, layer);
        }
    }

    public TileBase GetTileFromCollection(TileString ts)
    {
        return tCollections.GetTile(ts);
    }
}
