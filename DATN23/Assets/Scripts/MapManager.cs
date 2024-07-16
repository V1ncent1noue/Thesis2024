using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private Sprite CrossSprite;
    [SerializeField] private Sprite walkableSprite;
    [SerializeField] private Sprite blockSprite;
    [SerializeField] private Sprite temporaryBlockSprite;
    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private CarManager carManager;
    public MapData mapData;
    private NodeType[,] mapData2D;
    public Node[,] nodeData2D;
    [SerializeField] private List<GameObject> NodeGameObjects = new List<GameObject>();
    [SerializeField] private List<Placement> placements = new List<Placement>();

    public void DrawMap() {
        nodeData2D = new Node[mapData.width, mapData.height];
        for (int y = 0; y < mapData.height; y++)
        {
            for (int x = 0; x < mapData.width; x++)
            {
                NodeType nodeType = mapData2D[x, y];
                GameObject node = Instantiate(nodePrefab, new Vector3(x, y, 0), Quaternion.identity);
                node.name = $"{x}_{y}";
                node.transform.SetParent(transform);
                node.GetComponent<Node>().nodeType = nodeType;
                node.GetComponent<Node>().nodeIndex = x + y * mapData.width;
                node.GetComponent<Node>().x = x;
                node.GetComponent<Node>().y = y;
                node.GetComponent<Node>().OnNodeCtrlLeftClicked += OnNodeCtrlLeftClicked;
                node.GetComponent<Node>().OnNodeAltLeftClicked += OnNodeAltLeftClicked;
                ChangeNodeSprite(node, nodeType);
                NodeGameObjects.Add(node);
                nodeData2D[x, y] = node.GetComponent<Node>();
                //wait 0.5s
                // yield return new WaitForSeconds(0.5f);
            }
        }
    }
    
    private void Convert1DMapTo2DMap(MapData _mapData) {
        mapData2D = new NodeType[_mapData.width, _mapData.height];
        for (int x = 0; x < _mapData.width; x++)
        {
            for (int y = 0; y < _mapData.height; y++)
            {
                mapData2D[x, y] = _mapData.nodeMap[x + y * _mapData.width];
            }
        }
    }

    public void UpdateNodeType(int index, NodeType nodeType) {
        int x = index % mapData.width;
        int y = index / mapData.width;
        mapData.nodeMap[index] = nodeType;
        ChangeNodeSprite(NodeGameObjects[index], nodeType);
        NodeGameObjects[index].GetComponent<Node>().nodeType = nodeType;
        mapData2D[x, y] = nodeType;
        databaseManager.UpdateNodeType(index, nodeType);
        if(nodeType == NodeType.CrossNode || nodeType == NodeType.TemporaryBlock)
            {
            return;
            }
        carManager.UpdatePathFinding();
    }

    void ChangeNodeSprite(GameObject node, NodeType nodeType) {
        switch (nodeType)
                {
                    case NodeType.CrossNode:
                        node.GetComponent<Node>().SetNodeSprite(CrossSprite);
                        break;
                    case NodeType.Walkable:
                        node.GetComponent<Node>().SetNodeSprite(walkableSprite);
                        break;
                    case NodeType.Block:
                        node.GetComponent<Node>().SetNodeSprite(blockSprite);
                        break;
                    case NodeType.TemporaryBlock:
                        node.GetComponent<Node>().SetNodeSprite(temporaryBlockSprite);
                        break;
                }
    }

    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();
        int x = node.x;
        int y = node.y;
        if (x > 0) neighbours.Add(nodeData2D[x - 1, y]);
        if (x < mapData.width - 1) neighbours.Add(nodeData2D[x + 1, y]);
        if (y > 0) neighbours.Add(nodeData2D[x, y - 1]);
        if (y < mapData.height - 1) neighbours.Add(nodeData2D[x, y + 1]);
        node.Neighbours = neighbours;
        return neighbours;
    }

    public void resetNodeCosts() {
        foreach (Node node in nodeData2D) {
            node.gCost = 0;
            node.hCost = 0;
            node.parentNode = null;
        }
    }
    private void Start()
    {
        if (databaseManager == null)
        {
            databaseManager = FindObjectOfType<DatabaseManager>();
        }
        StartCoroutine(databaseManager.LoadMapDataIEnumerator((mapData) =>
        {
            this.mapData = mapData;
            Convert1DMapTo2DMap(mapData);
            DrawMap();
        }));
        StartCoroutine(databaseManager.FirebaseNodeMapInit());
    }
    
    
    void OnNodeCtrlLeftClicked(Node node) {
        switch (node.nodeType)
        {
            case NodeType.Walkable:
                UpdateNodeType(node.nodeIndex, NodeType.Block);
                break;
            case NodeType.Block:
                UpdateNodeType(node.nodeIndex, NodeType.Walkable);
                break;
            case NodeType.TemporaryBlock:
                UpdateNodeType(node.nodeIndex, NodeType.Walkable);
                break;
        }
    }

    void OnNodeAltLeftClicked(Node node) {
        switch (node.nodeType)
        {
            case NodeType.CrossNode:
                UpdateNodeType(node.nodeIndex, NodeType.Walkable);
                break;
            default:
                UpdateNodeType(node.nodeIndex, NodeType.CrossNode);
                break;
        }
    }
}

[System.Serializable]
public class MapData {
    public int width;
    public int height;
    public NodeType[] nodeMap;

    public MapData(int _width, int _height, NodeType[] _nodeMap) {
        width = _width;
        height = _height;
        nodeMap = _nodeMap;
    }
} 

[System.Serializable]
public class Placement{
    public int index;
    public int[] spot;
}