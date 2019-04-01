using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapLayers
{
    BG, FG
}

public struct MeshData
{
    public List<Vector3> vertices;
    public List<int> triangles;
    public List<Vector3> uv;
}

public struct CollisionData
{
    public List<Vector3> vertices;
    public List<int> triangles;
}

[System.Serializable]
public class TileString
{
    public string nspace; //namespace of the tile
    public string tile;

    public override bool Equals(object obj)
    {
        var otherOb = obj as TileString;
        if(otherOb == null)
        {
            return false;
        }

        return this.nspace == otherOb.nspace && this.tile == otherOb.tile;
    }

    public override int GetHashCode()
    {
        return this.nspace.GetHashCode() + this.tile.GetHashCode();
    }
}