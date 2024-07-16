using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private MapManager mapManager;
    [SerializeField] private Node startNode;
    [SerializeField] private Node goalNode;
    public List<Node> path = new List<Node>();
    private int[] roadNodes;
    public void SetStartNode(Node node) {
        startNode = node;
    }

    public void SetGoalNode(Node node) {
        goalNode = node;
    }
    private void Start() {
        mapManager = GetComponent<MapManager>();
    }
    public string GetPath(int start, int goal) {
        startNode = mapManager.nodeData2D[start%mapManager.mapData.width, start/mapManager.mapData.width];
        goalNode = mapManager.nodeData2D[goal%mapManager.mapData.width, goal/mapManager.mapData.width];
        FindPath();
        mapManager.resetNodeCosts();
        string pathString = "";
        foreach (Node node in path) {
            pathString += node.nodeIndex + "_";
        }
        pathString += "-1";
        return pathString;
    }
    public string GetRoad(int start, int goal) {
        startNode = mapManager.nodeData2D[start%mapManager.mapData.width, start/mapManager.mapData.width];
        goalNode = mapManager.nodeData2D[goal%mapManager.mapData.width, goal/mapManager.mapData.width];
        FindPath();
        mapManager.resetNodeCosts();
        string pathString = "";
        foreach (Node node in path) {
            if (node.nodeType == NodeType.CrossNode || node.nodeType == NodeType.TemporaryBlock) {
                pathString += node.nodeIndex + "_";
            }
        }
        pathString += "-1";
        return pathString;
    }
    public void FindPath() {
        path.Clear();
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        mapManager.UpdateNodeType(goalNode.nodeIndex, NodeType.TemporaryBlock);
        openList.Add(startNode);
        while (openList.Count > 0) {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++) {
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost && openList[i].hCost < currentNode.hCost) {
                    currentNode = openList[i];
                }
            }
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            if (currentNode == goalNode) {
                RetracePath(startNode, goalNode);
                return;
            }
            foreach (Node neighbour in mapManager.GetNeighbours(currentNode)) {
                if (neighbour.nodeType == NodeType.Block || closedList.Contains(neighbour)) {
                    continue;
                }
                if (neighbour.nodeType == NodeType.TemporaryBlock && neighbour.nodeIndex != goalNode.nodeIndex) {
                    continue;
                }
                if(neighbour.nodeIndex == 49 && goalNode.nodeIndex == 50)
                {
                    continue;
                }
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openList.Contains(neighbour)) {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, goalNode);
                    neighbour.parentNode = currentNode;
                    if (!openList.Contains(neighbour)) {
                        openList.Add(neighbour);
                    }
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node goalNode) {
        Node currentNode = goalNode;
        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        path.Add(startNode);
        path.Reverse();
    }

    private int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);
        if (dstX > dstY) {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

}
