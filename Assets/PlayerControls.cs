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
    public int transitionFrames; // frames it takes to move from one tile to the next
    public GameObject portal;

    public GameObject[] scientists;

    private int tileX;
    private int tileY;
    private bool isGhost = false;

    public int getTileX(){
        return tileX;
    }

    public int getTileY() {
        return tileY;
    }

    bool isPortalTile(int x, int y) {
        return (int)portal.transform.position.x == (x + 1) && (int)portal.transform.position.y == (y + 1);
    }

    Vector3 getRealpos() {
        return new Vector3((tileX + 1) * tileGrid.transform.localScale.x, (tileY + 1) * tileGrid.transform.localScale.y, 0);
    }

    public Vector3 getRealpos(int tileX, int tileY) {
        return new Vector3((tileX + 1) * tileGrid.transform.localScale.x, (tileY + 1) * tileGrid.transform.localScale.y, 0);
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

    bool isWalkable(int x, int y) {
        if (isGhost) return true;
        if (!WorldMap.currentMap.getCellIsWalkable(x, y)) return false;
        if (isPortalTile(x, y)) return false;
        
        if (generateMap.currentWorldNum == 0) {
            for (int i = 0; i < scientists.Length; i++) {
                int sx = (int)scientists[i].transform.position.x;
                int sy = (int)scientists[i].transform.position.y;
                if (sx == x + 1 && sy == y + 1) return false;
            }
        }

        return true;
    }

    int currentMovementFrame = -1;
    int nextX, nextY;
    void Update()
    {   

        // Movement Controls
        if (Input.GetKey("w") && currentMovementFrame == -1) {
            nextX = tileX;
            nextY = tileY + 1;

            if (isWalkable(nextX, nextY)) currentMovementFrame = 0;
        }

        if (Input.GetKey("a") && currentMovementFrame == -1) {
            nextX = tileX - 1;
            nextY = tileY;
            if (isWalkable(nextX, nextY)) currentMovementFrame = 0;
        }

        if (Input.GetKey("s") && currentMovementFrame == -1) {
            nextX = tileX;
            nextY = tileY - 1;
            if (isWalkable(nextX, nextY)) currentMovementFrame = 0;
        }
        
        if (Input.GetKey("d") && currentMovementFrame == -1) {
            nextX = tileX + 1;
            nextY = tileY;
            if (isWalkable(nextX, nextY)) currentMovementFrame = 0;
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

    // Used in the level editor
    public void setGhostMode(bool isGhost) {
        this.isGhost = isGhost;
    }
}
