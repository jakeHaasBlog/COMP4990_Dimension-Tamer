using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class WorldCell {
    public TileID backgroundTileID = TileID.none;
    public TileID foregroundTileID = TileID.none;
    public int encounterZoneID = -1;
    public int branchID = -1;
};

public class WorldMap {

    public static bool validateCoordsAsCircle = true;
    public const int WIDTH = 350;
    public const int HEIGHT = 350;
    public const int WATER_RING_THICKNESS = 50;

    public int worldNumber = 0;

    public static WorldMap currentMap = new WorldMap();

    private WorldCell[,] worldCells;

    private WorldMap() {
        worldCells = new WorldCell[WIDTH,HEIGHT];
        for (int i = 0; i < WIDTH; i++) {
            for (int j = 0; j < HEIGHT; j++) {
                worldCells[i, j] = new WorldCell();
                worldCells[i, j].backgroundTileID = TileID.water;
            }
        }
    }

    // returns true if the given coordinate is within the circular map
    public bool isValidCoord(int x, int y) {

        if (!validateCoordsAsCircle) return x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT;

        float rad = (float)WIDTH / 2;
        rad -= WATER_RING_THICKNESS;

        float tx = (float)x - (float)WIDTH / 2;
        float ty = (float)y - (float)HEIGHT / 2;
        if (tx * tx + ty * ty < rad * rad) {
            return x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT;
        }

        return false;
    }

    public void setCellForeground(int x, int y, TileID foregroundTileID) {
        if (!isValidCoord(x, y)) return;
        worldCells[x, y].foregroundTileID = foregroundTileID;
        TilemapData.foregroundTilemap.SetTile(new Vector3Int(x, y, 0), TilemapData.foregroundTiles[foregroundTileID].tileImage);
    }

    public void setCellBackground(int x, int y, TileID backgroundTileID) {
        if (!isValidCoord(x, y)) return;
        worldCells[x, y].backgroundTileID = backgroundTileID;
        TilemapData.backgroundTilemap.SetTile(new Vector3Int(x, y, 0), TilemapData.backgroundTiles[backgroundTileID].tileImage);
    }

    public void setTileEncounterZoneID(int x, int y, int encounterZoneID) {
        if (!isValidCoord(x, y)) return;
        worldCells[x, y].encounterZoneID = encounterZoneID;
    }

    public void setCellBiomeID(int x, int y, int branchID) {
        worldCells[x, y].branchID = branchID;
    }

    public TileID getCellForegroundID (int x, int y) {
        if (!isValidCoord(x, y)) return TileID.none;
        return worldCells[x, y].foregroundTileID;
    }

    public TileID getCellBackgroundID (int x, int y) {
        if (!isValidCoord(x, y)) return TileID.none;
        return worldCells[x, y].backgroundTileID;
    }

    public bool getCellIsWalkable(int x, int y) {
        bool wasCheckingCircle = validateCoordsAsCircle;
        validateCoordsAsCircle = false;
        if (!isValidCoord(x, y)) {
            validateCoordsAsCircle = wasCheckingCircle;
            return false;
        }
        validateCoordsAsCircle = wasCheckingCircle;
        
        TileID bgID = worldCells[x, y].backgroundTileID;
        TileID fgID = worldCells[x, y].foregroundTileID;

        if (TilemapData.foregroundTiles[fgID].isWalkable && TilemapData.backgroundTiles[bgID].isWalkable) {
            return true;
        } else {
            return false;
        }

    }

    public int getCellEncounterZoneID(int x, int y) {
        if (!isValidCoord(x, y)) return -1;
        
        TileID bgID = worldCells[x, y].backgroundTileID;

        if (TilemapData.backgroundTiles[bgID].isWalkable && TilemapData.backgroundTiles[bgID].isEncounterZone) {
            return worldCells[x, y].encounterZoneID;
        }

        return -1;
    }

    public int getCellBranchID(int x, int y) {
        if (x < 0 || x >= WIDTH) return -1;
        if (y < 0 || y >= HEIGHT) return -1;
        return worldCells[x, y].branchID;
    }

    public void clearCell(int x, int y) {
        if (!isValidCoord(x, y)) return;
        worldCells[x, y] = new WorldCell();
        TilemapData.foregroundTilemap.SetTile(new Vector3Int(x, y, 0), null);
        TilemapData.backgroundTilemap.SetTile(new Vector3Int(x, y, 0), null);
    }

    public void clearAllCells() {
        for (int i = 0; i < worldCells.GetLength(0); i++) {
            for (int j = 0; j < worldCells.GetLength(1); j++) {
                worldCells[i, j] = new WorldCell();
                TilemapData.foregroundTilemap.SetTile(new Vector3Int(i, j, 0), null);
                TilemapData.backgroundTilemap.SetTile(new Vector3Int(i, j, 0), null);
                
                worldCells[i, j].backgroundTileID = TileID.water;
                TilemapData.backgroundTilemap.SetTile(new Vector3Int(i, j, 0), TilemapData.backgroundTiles[TileID.water].tileImage);
            }
        }
    }

    
    
};
