using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CaveGenerator : BiomeGenerator
{
    public CaveGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {
        endPoints = new List<Tuple<int, int>>();
    }

    override public BiomeID getBiome() {
        return BiomeID.Cave;
    }

    override public void generate() {
        Debug.Log("Cave generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        List<Tuple<int, int>> allTiles = getAllTiles();
        replaceTiles(allTiles, TileID.caveFilling, false);

        int giveUpParameter = 0;
        int tunnelCount = (allTiles.Count / 200) + 1;
        List<Tuple<int, int>> tunnelEntrances = new List<Tuple<int, int>>();
        for (int i = 0; i < tunnelCount; i++) {
            Tuple<int, int> pos = getRandomWalkableTile();
            if (pos.Item1 % 4 == 0 && pos.Item2 % 4 == 0) {
                tunnelEntrances.Add(pos);   
            } else{
                i--;
            }

            giveUpParameter++;
            if (giveUpParameter > 100) break;
        }

        for (int i = 0; i < tunnelEntrances.Count; i++) {
            genMaze(20, tunnelEntrances[i].Item1, tunnelEntrances[i].Item2);
        }

        // place encounter zones at end points of tunnels
        // growSelection(endPoints);
        endPoints = growSelection(endPoints);
        endPoints = growSelection(endPoints);
        endPoints = growSelection(endPoints);
        replaceTiles(endPoints, TileID.caveRoughGroundEZ, false, true, false);

        List<ElementalType> elements = new List<ElementalType>();
        elements.Add(ElementalType.dark);
        elements.Add(ElementalType.none);
        int encounterZoneLevel = WorldMap.currentMap.worldNumber * 20;
        addEncounterZone(endPoints, elements, encounterZoneLevel);

        for (int y = boundY; y < boundY + boundHeight; y++) {
            for (int x = boundX; x < boundX + boundWidth; x++) {
                if (WorldMap.currentMap.getCellBackgroundID(x, y) == TileID.persistantPath && WorldMap.currentMap.getCellBranchID(x, y) == branchID) {
                    WorldMap.currentMap.setCellBackground(x, y, TileID.caveGround);
                }
            }
        }

        // if there are no tunnels, there will be no encounter zones. In this case, add a random high level encounter zone
        if (endPoints.Count == 0) {
            List<Tuple<int, int>> eZone = new List<Tuple<int, int>>();
            eZone.Add(getRandomWalkableTile());
            eZone = growSelection(eZone, 0.9f);
            eZone = growSelection(eZone, 0.9f);
            eZone = growSelection(eZone, 0.9f);
            replaceTiles(eZone, TileID.caveRoughGroundEZ, false);
            addEncounterZone(eZone, elements, encounterZoneLevel + 10);
        }

    }

    bool isWalkable(int x, int y) {
        return WorldMap.currentMap.getCellIsWalkable(x, y) && WorldMap.currentMap.getCellBranchID(x, y) == branchID;
    }

    void genMaze(int maxLength, int startX, int startY) {
        Stack<Tuple<int, int>> previousLocations = new Stack<Tuple<int, int>>();
        previousLocations.Push(new Tuple<int, int>(startX, startY));
        //WorldMap.currentMap.setCellBackground(startX, startY, TileID.caveGround); // not necessary, mazes are only generated from the path

        int[] directionsX = { 0, 0, -1, 1 };
        int[] directionsY = { 1, -1, 0, 0 };

        bool pushedLastItter = false;
        for (int i = 0; i < maxLength; i++) {
            
            // choose random direction that is valid
            int directionIndex = -1;
            scrambleArrays(ref directionsX, ref directionsY);
            for (int j = 0; j < directionsX.Length; j++) {
                if (directionIsValid(previousLocations.Peek().Item1, previousLocations.Peek().Item2, directionsX[j], directionsY[j])){
                    directionIndex = j;
                    break;
                }
            }

            if (directionIndex != -1) {
                // break down wall in chosen direction
                int newX1 = previousLocations.Peek().Item1 + directionsX[directionIndex] * 4;
                int newY1 = previousLocations.Peek().Item2 + directionsY[directionIndex] * 4;

                int newX2 = previousLocations.Peek().Item1 + directionsX[directionIndex] * 3;
                int newY2 = previousLocations.Peek().Item2 + directionsY[directionIndex] * 3;

                int newX3 = previousLocations.Peek().Item1 + directionsX[directionIndex] * 2;
                int newY3 = previousLocations.Peek().Item2 + directionsY[directionIndex] * 2;

                int newX4 = previousLocations.Peek().Item1 + directionsX[directionIndex] * 1;
                int newY4 = previousLocations.Peek().Item2 + directionsY[directionIndex] * 1;
                
                WorldMap.currentMap.setCellBackground(newX1, newY1, TileID.caveGround);
                WorldMap.currentMap.setCellBackground(newX2, newY2, TileID.caveGround);
                WorldMap.currentMap.setCellBackground(newX3, newY3, TileID.caveGround);
                WorldMap.currentMap.setCellBackground(newX4, newY4, TileID.caveGround);

                // add new position to stack
                previousLocations.Push(new Tuple<int, int>(newX1, newY1));
                pushedLastItter = true;

            } else {
                // if no direction is valid pop stack and repeat checks
                if (pushedLastItter) endPoints.Add(new Tuple<int, int>(previousLocations.Peek().Item1, previousLocations.Peek().Item2));
                previousLocations.Pop();
                pushedLastItter = false;
                if (previousLocations.Count == 0) {
                    return;
                } else {
                    i--;
                    continue;
                }
            }

        }

        endPoints.Add(new Tuple<int, int>(previousLocations.Peek().Item1, previousLocations.Peek().Item2));

    }

    void scrambleArrays(ref int[] array1, ref int[] array2) {

        for (int i = 0; i < 5; i++) {
            int j = UnityEngine.Random.Range(1, array1.Length);

            int tmp1 = array1[j];
            int tmp2 = array2[j];

            array1[j] = array1[0]; 
            array2[j] = array2[0];

            array1[0] = tmp1; 
            array2[0] = tmp2; 

        }

    }

    bool directionIsValid(int currentX, int currentY, int dx, int dy) {

        if (!WorldMap.currentMap.isValidCoord(currentX + dx * 1, currentY + dy * 1)) return false;
        if (!WorldMap.currentMap.isValidCoord(currentX + dx * 2, currentY + dy * 2)) return false;
        if (!WorldMap.currentMap.isValidCoord(currentX + dx * 3, currentY + dy * 3)) return false;
        if (!WorldMap.currentMap.isValidCoord(currentX + dx * 4, currentY + dy * 4)) return false;

        if (isWalkable(currentX + dx, currentY + dy)) return false;
        if (isWalkable(currentX + dx * 2, currentY + dy * 2)) return false;
        if (isWalkable(currentX + dx * 3, currentY + dy * 3)) return false;
        if (isWalkable(currentX + dx * 4, currentY + dy * 4)) return false;

        if (WorldMap.currentMap.getCellBranchID(currentX + dx, currentY + dy) != branchID) return false;
        if (WorldMap.currentMap.getCellBranchID(currentX + dx * 2, currentY + dy * 2) != branchID) return false;
        if (WorldMap.currentMap.getCellBranchID(currentX + dx * 3, currentY + dy * 3) != branchID) return false;
        if (WorldMap.currentMap.getCellBranchID(currentX + dx * 4, currentY + dy * 4) != branchID) return false;

        if (WorldMap.currentMap.getCellBackgroundID(currentX + dx, currentY + dy) == TileID.persistantPath) return false;
        if (WorldMap.currentMap.getCellBackgroundID(currentX + dx * 2, currentY + dy * 2) == TileID.persistantPath) return false;
        if (WorldMap.currentMap.getCellBackgroundID(currentX + dx * 3, currentY + dy * 3) == TileID.persistantPath) return false;
        if (WorldMap.currentMap.getCellBackgroundID(currentX + dx * 4, currentY + dy * 4) == TileID.persistantPath) return false;

        return true;
    }

    List<Tuple<int, int>> endPoints; 

}
