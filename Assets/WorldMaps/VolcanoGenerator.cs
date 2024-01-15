using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class VolcanoGenerator : BiomeGenerator
{
    public VolcanoGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.Volcano;
    }

    override public void generate() {
        Debug.Log("Volcano generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        List<Tuple<int, int>> borderTiles = getBorderTiles();
        borderTiles = growSelection(borderTiles);
    
        List<Tuple<int, int>> lavaMoat = growSelection(borderTiles);
        lavaMoat = growSelection(lavaMoat);
        lavaMoat = growSelection(lavaMoat);
        lavaMoat = growSelection(lavaMoat);

        replaceTiles(lavaMoat, TileID.volcanoLavaPitMiddle, false);
        replaceTiles(borderTiles, TileID.volcanoRockyGround, false);

        List<TileID> lavaPitTiles = new List<TileID>();
        lavaPitTiles.Add(TileID.volcanoLavaPitBottom);
        lavaPitTiles.Add(TileID.volcanoLavaPitMiddle);
        lavaPitTiles.Add(TileID.volcanoLavaPitTop);
        for (int x = boundX; x < boundX + boundWidth; x++) {
            for (int y = boundY; y < boundY + boundHeight; y++) {

                if (WorldMap.currentMap.getCellBackgroundID(x, y) != TileID.volcanoLavaPitMiddle) continue;

                if (!lavaPitTiles.Contains(WorldMap.currentMap.getCellBackgroundID(x, y - 1))) {
                    // bottom of lava pit
                    WorldMap.currentMap.setCellBackground(x, y, TileID.volcanoLavaPitBottom);
                    continue;
                } 

                if (!lavaPitTiles.Contains(WorldMap.currentMap.getCellBackgroundID(x, y + 1))) {
                    // top of lava pit
                    WorldMap.currentMap.setCellBackground(x, y, TileID.volcanoLavaPitTop);
                    continue;
                }
            }
        }

        // adding encounter zones
        for (int j = 0; j < 4; j++) {
            List<Tuple<int, int>> encounterZone1 = new List<Tuple<int, int>>();
            encounterZone1.Add(getRandomWalkableTile());

            for (int i = 0; i < 10; i++) {
                encounterZone1 = growSelection(encounterZone1, 0.2f);
            }
            replaceTiles(encounterZone1, TileID.volcanoDeepSiltEZ, false);

            List<ElementalType> elements = new List<ElementalType>();
            elements.Add(ElementalType.fire);
            elements.Add(ElementalType.none);
            int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
            addEncounterZone(encounterZone1, elements, encounterZoneLevel);
        }

    }
}
