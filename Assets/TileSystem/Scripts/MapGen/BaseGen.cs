using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace KL.TileSystem.MapGen
{
    [CreateAssetMenu(fileName = "BaseGen", menuName = "World Gen/BaseGen")]
    public class BaseGen : IGenerator
    {

        [Header("Perlin Noise")]
        public PerlinNoiseDef[] perlinNoises;

        public override Task Generate(int seed, System.Random ran, MapManager mm)
        {
            return Task.Factory.StartNew(() =>
            {
                mm.currentProgress = "BaseGen";
                for (int i = 0; i < perlinNoises.Length; i++)
                {
                    mm.progress = 0;
                    PerlinNoise(seed, i, mm);
                }
            });
        }

        public void PerlinNoise(int seed, int index, MapManager gen)
        {
            float noise = 1;
            int newPoint;
            double progStep = 1.0 / (double)gen.mapWidth;

            for (int x = 0; x < gen.mapWidth; x++)
            {
                gen.progress += progStep;
                //Generate the y-pos of this point
                noise = Mathf.PerlinNoise(x * perlinNoises[index].scale, 0);
                noise *= perlinNoises[index].magnitude;
                noise = Mathf.Pow(noise, perlinNoises[index].power);

                //Modify the position to fit where out floor should be around.
                newPoint = Mathf.FloorToInt(noise);
                newPoint += perlinNoises[index].baseHeight;
                newPoint = Mathf.Clamp(newPoint, 0, gen.mapHeight - 1);
                //Fill downwards from this point.
                if (perlinNoises[index].visible)
                {
                    for (int y = newPoint; y >= 0; y--)
                    {
                        gen.temporaryMap[MapLayers.FG][x, y] = perlinNoises[index].blockID;
                        gen.temporaryMap[MapLayers.BG][x, y] = perlinNoises[index].BGBlockID;
                    }
                }
            }
            gen.progress = 1;
        }
    }

    [System.Serializable]
    public struct PerlinNoiseDef
    {
        public float scale;
        public int baseHeight;
        public float magnitude;
        public float power;
        public TileString blockID;
        public TileString BGBlockID;
        public bool visible;
    }
}