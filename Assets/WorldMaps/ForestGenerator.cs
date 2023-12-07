using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : BiomeGenerator
{
    public ForestGenerator(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.Forest;
    }

    override public void generate() {
        Debug.Log("Forest generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);

        

    }
}
