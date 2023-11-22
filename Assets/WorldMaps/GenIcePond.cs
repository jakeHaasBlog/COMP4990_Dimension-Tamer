using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GenIcePond : BiomeGenerator
{

    public GenIcePond(int boundX, int boundY, int boundWidth, int boundHeight, int branchID) 
    : base(boundX, boundY, boundWidth, boundHeight, branchID)
    {}

    override public BiomeID getBiome() {
        return BiomeID.IcePond;
    }

    override public void generate() {
        Debug.Log("Ice pond generating at " + boundX + ", " + boundY + "  width=" + boundWidth + " height=" + boundHeight);
    }

}
