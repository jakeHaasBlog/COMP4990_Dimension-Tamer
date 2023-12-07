using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGraphNode {

    public BiomeGraphNode(float x, float y, int branchID) {
        this.x = x;
        this.y = y;
        this.branchID = branchID;
        connections = new List<int>();
    }

    public float distTo(float x, float y) {
        return Mathf.Sqrt(distToSquared(x, y));
    }

    public float distToSquared(float x, float y) {
        float dx = x - this.x;
        float dy = y - this.y;
        return dx * dx + dy * dy;
    }

    public float x, y;
    public int branchID;
    public List<int> connections;
}

public class BiomeGraph
{
    public BiomeGraph() {
        nodes = new List<BiomeGraphNode>();
    }

    public int addNode(float x, float y, int branchID) {
        nodes.Add(new BiomeGraphNode(x, y, branchID));
        return nodes.Count - 1;
    }

    public BiomeGraphNode getNode(int i) {
        return nodes[i];
    }

    public int getNodeCount() {
        return nodes.Count;
    }

    public void connectNodes(int a, int b) {
        if (!nodes[a].connections.Contains(b)) nodes[a].connections.Add(b);
        if (!nodes[b].connections.Contains(a)) nodes[b].connections.Add(a);
    }

    public int getNearestNodeIndex(float x, float y) {
        float nearestDistSq = 0;
        int nearestIndex = -1;
        
        for (int i = 0; i < nodes.Count; i++) {
            float distSq = nodes[i].distToSquared(x, y);
            if (distSq < nearestDistSq || nearestIndex == -1) {
                nearestIndex = i;
                nearestDistSq = distSq;
            }
        }
        return nearestIndex;
    }

    private bool withinBounds(Vector4 bounds, float x, float y) {
        float cx = bounds[0] + bounds[2] / 2;
        float cy = bounds[1] + bounds[3] / 2;
        float rad = Mathf.Min(bounds[2], bounds[3]);
        rad /= 2;

        x -= cx;
        y -= cy;

        if (x * x + y * y > rad * rad) return false;
        return true;
        
        //if (x < bounds[0]) return false;
        //if (x > bounds[0] + bounds[2]) return false;
        //if (y < bounds[1]) return false;
        //if (y > bounds[1] + bounds[3]) return false;

    }

    public void generateMapGraph(Vector4 bounds, float edgeLength, float minNodeDistance, Vector2 startingPos, int maxBranchLength) {
        int currentBranch = 0;
        int branchLength = 0;

        Stack<int> addedNodeIndices = new Stack<int>();

        // add starting node to graph and stack
        addedNodeIndices.Push(addNode(250, 250, currentBranch));

        // Continue adding nodes until there are no spaces left to add them
        while (addedNodeIndices.Count > 0) {
            
            // try to add a branching node in random direction n times, if not added then work backwards and add branches on earilier nodes
            if (!tryAddingBranchNode(ref addedNodeIndices, edgeLength, minNodeDistance, 30, bounds, currentBranch)) {
                addedNodeIndices.Pop();
                currentBranch++;
                branchLength = 0;
            } else {

                // branches must not exceed maxBranchLength (tests found that some branches are 30+ nodes and others are only 1, we want greater consistancy)
                branchLength++;
                if (branchLength > maxBranchLength) {
                    currentBranch++;
                    branchLength = 0;
                }
                
            }

        }
    }

    private bool tryAddingBranchNode(ref Stack<int> addedNodeIndices, float edgeLength, float minNodeDistance, int itterations, Vector4 bounds, int branchID) {

        List<float> possibleAngles = new List<float>();
        for (float r = 0; r < Mathf.PI * 2; r += Mathf.PI / 16) possibleAngles.Add(r);

        for (int i = 0; i < itterations; i++) {

            // make a new node that is 'edgeLength' distance away from current node
            //float r = UnityEngine.Random.Range(0.0f, 2.0f * Mathf.PI);
            float r = possibleAngles[Random.Range(0, possibleAngles.Count)];
            float newX = getNode(addedNodeIndices.Peek()).x + (Mathf.Cos(r) * edgeLength);
            float newY = getNode(addedNodeIndices.Peek()).y + (Mathf.Sin(r) * edgeLength);

            // if the new node is not too close to another node, add it. Otherwise, try again
            int nearestNode = getNearestNodeIndex(newX, newY);
            float nearestNodeDist = getNode(nearestNode).distTo(newX, newY);
            if (nearestNodeDist > minNodeDistance && withinBounds(bounds, newX, newY)) {

                // new nodes are given 'currentBiome' integer value
                // add the new node to stack
                int newNodeIndex = addNode(newX, newY, branchID);
                connectNodes(newNodeIndex, addedNodeIndices.Peek());
                addedNodeIndices.Push(newNodeIndex);

                return true;
            }
        }

        return false;
    }

    private List<BiomeGraphNode> nodes;
}
