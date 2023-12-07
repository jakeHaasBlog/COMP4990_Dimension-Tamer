using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomeID {
    none,
    Grasslands,
    Forest,
    PineForest,
    Mountain,
    Cave,
    Tundra,
    SnowyForest,
    IcePond,
    Volcano,
    Tropics,
    Desert,
    SecretMeadow,
}

// Abstract class that is the basis for each specific biome's generation algorithm
public abstract class BiomeGenerator {
    public BiomeGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) {
        this.boundX = boundX;
        this.boundY = boundY;
        this.boundWidth = boundWidth;
        this.boundHeight = boundHeight;
    }

    public abstract BiomeID getBiome();
    public abstract void generate();

    protected int boundX, boundY, boundWidth, boundHeight;
}