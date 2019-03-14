using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles deciding which chunks we should be rendering.
/// </summary>
public class ChunkHandler : MonoBehaviour
{
    [SerializeField] private MapManager mm;
    [SerializeField] private GameObject player;
    public TileRenderer fg;
    public TileRenderer bg;
    public int chunksToRenderX;
    public int chunksToRenderY;
    [HideInInspector] public List<Vector2Int> currentLoadedChunks = new List<Vector2Int>();

    public string tempTile;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            fg.GenTerrain(TileCollection.GetTile(tempTile));
        }
    }

}
