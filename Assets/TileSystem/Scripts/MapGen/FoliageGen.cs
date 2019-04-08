using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace KL.TileSystem.MapGen
{
    [CreateAssetMenu(fileName = "FoliageGen", menuName = "World Gen/FoliageGen")]
    public class FoliageGen : IGenerator
    {
        public FoliageDef[] foliage;

        public override Task Generate(int seed, System.Random ran, MapManager mm)
        {
            return Task.Factory.StartNew(() =>
            {
                //mm.currentProgress = "FoliageGen";
                for(int i = 0; i < foliage.Length; i++)
                {
                    for(int x = 0; x < mm.mapWidth; x++)
                    {
                        for(int y = mm.mapHeight-1; y > 0; y--)
                        {
                            if (mm.temporaryMap[foliage[i].layer][x, y] != null)
                            {
                                if (ran.Next(0, 100) <= foliage[i].chance)
                                {
                                    mm.temporaryMap[foliage[i].layer][x, y + 1] = foliage[i].blockID;
                                }
                                break;
                            }
                        }
                    }
                }
            });
        }
    }

    [System.Serializable]
    public struct FoliageDef
    {
        public TileString blockID;
        public MapLayers layer;
        public int chance;
    }
}
