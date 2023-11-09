using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    public int startingX;
    public int startingY;
    public Grid tileGrid;
    public GenerateMap generateMap;
    public GameObject mainCamera;
    public int transitionFrames; // frames it takes to get from one tile to the next

    private int tileX;
    private int tileY;

    public int getTileX(){
        return tileX;
    }

    public int getTileY() {
        return tileY;
    }

    Vector3 getRealpos() {
        return new Vector3((tileX + 0.5f) * tileGrid.cellSize.x, (tileY + 0.5f) * tileGrid.cellSize.y, 0);
    }

    Vector3 getRealpos(int tileX, int tileY) {
        return new Vector3((tileX + 0.5f) * tileGrid.cellSize.x, (tileY + 0.5f) * tileGrid.cellSize.y, 0);
    }
    
    public void setPosition(int tileX, int tileY) {
        this.tileX = tileX;
        this.tileY = tileY;
        gameObject.transform.position = getRealpos();
    }

    void Start()
    {
        setPosition(startingX, startingY);
    }

    int currentMovementFrame = -1;
    int nextX, nextY;
    void Update()
    {   

        // Movement Controls
        if (Input.GetKey("w") && currentMovementFrame == -1) {
            nextX = tileX;
            nextY = tileY + 1;

            if (generateMap.getWorldMap().getTileIsWalkable(nextX, nextY)) currentMovementFrame = 0;
        }

        if (Input.GetKey("a") && currentMovementFrame == -1) {
            nextX = tileX - 1;
            nextY = tileY;
            if (generateMap.getWorldMap().getTileIsWalkable(nextX, nextY)) currentMovementFrame = 0;
        }

        if (Input.GetKey("s") && currentMovementFrame == -1) {
            nextX = tileX;
            nextY = tileY - 1;
            if (generateMap.getWorldMap().getTileIsWalkable(nextX, nextY)) currentMovementFrame = 0;
        }
        
        if (Input.GetKey("d") && currentMovementFrame == -1) {
            nextX = tileX + 1;
            nextY = tileY;
            if (generateMap.getWorldMap().getTileIsWalkable(nextX, nextY)) currentMovementFrame = 0;
        }

        if (currentMovementFrame == -1) {
            gameObject.transform.position = getRealpos();
        } else {
            Vector3 startingPos = getRealpos();
            Vector3 finalPos = getRealpos(nextX, nextY);
            
            gameObject.transform.position = Vector3.Lerp(startingPos, finalPos, (float)currentMovementFrame / (float)transitionFrames);

            currentMovementFrame++;
            if (currentMovementFrame > transitionFrames) {
                currentMovementFrame = -1;
                tileX = nextX;
                tileY = nextY;
            }
        }

        // Makes the camera center on the player
        float camZ = mainCamera.transform.position.z;
        Vector3 rPos = gameObject.transform.position;
        rPos.z = camZ;
        mainCamera.transform.position = rPos;
        
    }
}
