using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileBitmask48", menuName = "Tiles/Tile Bitmask 48")]
public class TileBitmask47 : TileBase
{

    public Texture2D[] textures;

    // bitmask value : sprite index
    public static Dictionary<byte, int> maskIndex = new Dictionary<byte, int>() {
        {64, 0},
        {208, 1},
        {248, 2},
        {104, 3},
        {80, 4},
        {88, 5},
        {72, 6},
        {127, 7},
        {95, 8},
        {223, 9},
        {66, 10},
        {214, 11},
        {255, 12},
        {107, 13},
        {82, 14},
        {0, 15},
        {74, 16},
        {123, 17},
        {90, 18},
        {222, 19},
        {2, 20},
        {22, 21},
        {31, 22},
        {11, 23},
        {18, 24},
        {26, 25},
        {10, 26},
        {251, 27},
        {250, 28},
        {254, 29},
        {16, 30},
        {24, 31},
        {8, 32},
        {120, 33},
        {75, 34},
        {86, 35},
        {216, 36},
        {91, 37},
        {94, 38},
        {219, 39},
        {126, 40},
        {210, 41},
        {30, 42},
        {27, 43},
        {106, 44},
        {122, 45},
        {218, 46}
    };

    Vector2Int ul = new Vector2Int(-1, 1);
    Vector2Int ur = new Vector2Int(1, 1);
    Vector2Int dl = new Vector2Int(-1, -1);
    Vector2Int dr = new Vector2Int(-1, 1);

    public override void RefreshTile(Vector2Int position, MapLayers layer)
    {
        byte top = GetBitmask(position + Vector2Int.up, layer);
        byte right = GetBitmask(position + Vector2Int.right, layer);
        byte bottom = GetBitmask(position + Vector2Int.down, layer);
        byte left = GetBitmask(position + Vector2Int.left, layer);
        byte topLeft = GetBitmask(position + ul, layer);
        byte topRight = GetBitmask(position + ur, layer);
        byte bottomLeft = GetBitmask(position + dl, layer);
        byte bottomRight = GetBitmask(position + dr, layer);

        byte mask = (byte)((1 * topLeft) + (2 * top) + (4 * topRight) + (8 * left) + (16 * right) + (32 * bottomLeft) + (64 * bottom) + (128 * bottomRight));
        MapManager.instance.SetBitmask(position.x, position.y, mask, layer);
    }

    public byte GetBitmask(Vector2Int location, MapLayers layer)
    {
        if (ReferenceEquals(MapManager.instance.GetTile(location.x, location.y, layer), null)) return 0;
        return 1;
    }
}
