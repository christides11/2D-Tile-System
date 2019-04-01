using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KL.TileSystem.MapGen
{
    [CreateAssetMenu(fileName = "CaveGen", menuName = "World Gen/CaveGen")]
    public class CaveGen : IGenerator
    {
        [System.Serializable]
        public struct CellularAutoDef
        {
            public int passes;
            [Range(0, 100)]
            public int fillChance;
            public int smoothPasses;
            public int cellMinHeight;
            public int cellMaxHeight;
            public int cellularPerWidth;
        }

        [Header("Cellular Automata")]
        public CellularAutoDef[] cellPasses;

        public override Task Generate(int seed, System.Random ran, MapManager mm)
        {
            return Task.Factory.StartNew(() =>
            {
                mm.currentProgress = "CaveGen";
                mm.progress = 0;
                for (int w = 0; w < cellPasses.Length; w++)
                {
                    mm.progress += 1.0f / (double)cellPasses.Length;
                    for (int i = 0; i < (mm.mapWidth / cellPasses[w].cellularPerWidth); i++)
                    {
                        GenerateCellularAutomata(false, w, i, ran, mm);
                    }
                    SmoothMooreCellularAutomata(w, mm);
                }
            });
        }

        #region Cellular Automata
        private void GenerateCellularAutomata(bool edgesAreWalls, int index, int widthIndex, System.Random ran, MapManager gen)
        {
            for (int i = 0; i < cellPasses[index].passes; i++)
            {
                for (int x = cellPasses[index].cellularPerWidth * widthIndex; x < (cellPasses[index].cellularPerWidth * widthIndex) + gen.chunkWidth; x++)
                {
                    for (int y = cellPasses[index].cellMinHeight; y < cellPasses[index].cellMaxHeight; y++)
                    {
                        if (gen.temporaryMap[MapLayers.FG][x, y] != null)
                        {
                            if (ran.Next(0, 100) < cellPasses[index].fillChance)
                            {
                                gen.temporaryMap[MapLayers.FG][x, y] = null;
                            }
                        }
                    }
                }
            }
        }


        private void SmoothMooreCellularAutomata(int index, MapManager gen)
        {
            TileString rID = null;
            for (int i = 0; i < cellPasses[index].smoothPasses; i++)
            {
                for (int x = 0; x < gen.mapWidth; x++)
                {
                    for (int y = cellPasses[index].cellMinHeight; y < cellPasses[index].cellMaxHeight; y++)
                    {
                        int surroundingTiles = GetMooreSurroundingTiles(x, y, gen, ref rID);

                        //The default moore rule requires more than 4 neighbours
                        if (surroundingTiles > 4)
                        {
                            gen.temporaryMap[MapLayers.FG][x, y] = rID;
                        } else if (surroundingTiles < 4)
                        {
                            gen.temporaryMap[MapLayers.FG][x, y] = null;
                        }
                    }
                }
            }
        }

        private int GetMooreSurroundingTiles(int x, int y, MapManager gen, ref TileString rID)
        {
            int tileCount = 0;
            for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
            {
                for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
                {
                    if (neighbourX >= 0 && neighbourX < gen.mapWidth && neighbourY >= 0 && neighbourY < gen.mapHeight)
                    {
                        //We don't want to count the tile we are checking the surroundings of
                        if (neighbourX != x || neighbourY != y)
                        {
                            tileCount += gen.temporaryMap[MapLayers.FG][neighbourX, neighbourY] == null ? 0 : 1;
                            if (!ReferenceEquals(gen.temporaryMap[MapLayers.FG][neighbourX, neighbourY], null))
                            {
                                rID = gen.temporaryMap[MapLayers.FG][neighbourX, neighbourY];
                            }
                        }
                    }
                }
            }
            return tileCount;
        }
        #endregion
    }
}