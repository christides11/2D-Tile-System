using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace KL.TileSystem
{
    [CreateAssetMenu(fileName = "TileBitmask47", menuName = "Tiles/Tile Bitmask 47")]
    public class TileBitmask47 : TileBase
    {

        // bitmask value : sprite index
        public static Dictionary<byte, int> maskIndex = new Dictionary<byte, int>() {
        {0, 47},
        {64, 43},
        {208, 36},
        {248, 22},
        {104, 38},
        {80, 36},
        {88, 24},
        {72, 6},
        {127, 7},
        {95, 4},
        {223, 8},
        {66, 34},
        {214, 18},
        {255, 0},
        {107, 26},
        {82, 14},
        {74, 16},
        {123, 17},
        {90, 18},
        {222, 19},
        {2, 45},
        {22, 41},
        {31, 30},
        {11, 39},
        {18, 41},
        {26, 31},
        {10, 39},
        {251, 2},
        {250, 28},
        {254, 29},
        {16, 44},
        {24, 35},
        {8, 46},
        {120, 22},
        {75, 26},
        {86, 18},
        {216, 22},
        {91, 37},
        {94, 9},
        {219, 10},
        {126, 1},
        {210, 18},
        {30, 30},
        {27, 32},
        {106, 28},
        {122, 7},
        {218, 46}
    };

        Vector2Int ul = new Vector2Int(-1, 1);
        Vector2Int ur = new Vector2Int(1, 1);
        Vector2Int dl = new Vector2Int(-1, -1);
        Vector2Int dr = new Vector2Int(-1, 1);

        public override void RefreshTile(Vector2Int position, MapLayers layer)
        {
            byte top = GetBitmask(position + Vector2Int.up, layer);
            byte left = GetBitmask(position + Vector2Int.left, layer);
            byte right = GetBitmask(position + Vector2Int.right, layer);
            byte bottom = GetBitmask(position + Vector2Int.down, layer);
            byte topLeft = (byte)(GetBitmask(position + ul, layer) & top & left);
            byte topRight = (byte)(GetBitmask(position + ur, layer) & top & right);
            byte bottomRight = (byte)(GetBitmask(position + dr, layer) & bottom & right);
            byte bottomLeft = (byte)(GetBitmask(position + dl, layer) & bottom & left);
            byte mask = (byte)((1 * topLeft) + (2 * top) + (4 * topRight) + (8 * left) + (16 * right) +
                (32 * bottomLeft) + (64 * bottom) + (128 * bottomRight));
            MapManager.instance.SetBitmask(position.x, position.y, mask, layer);
        }

        public byte GetBitmask(Vector2Int location, MapLayers layer)
        {
            if (ReferenceEquals(MapManager.instance.GetTile(location.x, location.y, layer), null)) return 0;
            return 1;
        }

        public override int GetTextureIndex(int x, int y, MapLayers layer)
        {
            return textureBaseIndex + maskIndex[MapManager.instance.GetBitmask(x, y, layer)];
        }
    }
}
