﻿using System.Collections;
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