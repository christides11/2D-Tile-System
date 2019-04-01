using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

namespace KL.TileSystem.MapGen
{
    public class IGenerator : ScriptableObject
    {
        public virtual Task Generate(int seed, Random ran, MapManager mm)
        {
            return null;
        }
    }
}