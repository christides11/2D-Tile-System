using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles deciding which chunks we should be rendering.
/// </summary>
public class ChunkHandler : MonoBehaviour
{
    static Vector2Int[] chunkPos = {
        new Vector2Int(0, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, 0),
        new Vector2Int(1, -1),
        new Vector2Int(1, 1)
    };

    [SerializeField] private MapManager mm;
    [SerializeField] private Transform player;
    public ChunkRenderer fg;
    public ChunkRenderer bg;
    [HideInInspector] public List<Vector2Int> currentLoadedChunks = new List<Vector2Int>();

    public string tempTile;

    private void Awake()
    {
        fg.InitChuns(mm.mapWidth / mm.chunkWidth, mm.mapHeight / mm.chunkHeight);
        bg.InitChuns(mm.mapWidth / mm.chunkWidth, mm.mapHeight / mm.chunkHeight);
    }

    void Update()
    {
        if(mm.mapGenerated)
        {
            FindChunksToRender();
        }
    }

    Vector2Int pChunk = new Vector2Int();
    Vector2Int cPos = new Vector2Int();
    Chunk nChunk;
    void FindChunksToRender()
    {
        pChunk.x = Mathf.FloorToInt(player.position.x / mm.chunkWidth);
        pChunk.y = Mathf.FloorToInt(player.position.y / mm.chunkHeight);

        for (int i = 0; i < chunkPos.Length; i++)
        {
            cPos.x = chunkPos[i].x + pChunk.x;
            cPos.y = chunkPos[i].y + pChunk.y;

            nChunk = fg.GetChunk(cPos);

            if (nChunk != null && nChunk.rendered)
                continue;

            GenChunk(cPos.x, cPos.y);
        }
    }

    void GenChunk(int x, int y)
    {
        fg.AddChunk(x, y, mm.chunkWidth, mm.chunkHeight);
        fg.RenderChunk(x, y, mm.map.chunks[x,y]);
    }

    void UnloadChunk(int x, int y)
    {
        fg.DestroyChunk(x, y);
    }
}
