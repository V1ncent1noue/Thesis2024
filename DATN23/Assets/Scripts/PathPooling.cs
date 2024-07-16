using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPooling : MonoBehaviour
{
    [SerializeField] private GameObject pathPrefab;
    [SerializeField] private MapManager mapManager;
    private List<GameObject> pathPool = new List<GameObject>();
    private Vector2Int poolSize;
    public string pathData;

    private void Start() {
        poolSize = new Vector2Int(26, 12);
        for (int y = 0; y < poolSize.y; y++) {
            for (int x = 0; x < poolSize.x; x++) {
                GameObject path = Instantiate(pathPrefab, new Vector3(x, y, 0), Quaternion.identity);
                path.name = $"{x}_{y}";
                path.transform.SetParent(transform);
                pathPool.Add(path);
                path.SetActive(false);
            }
        }
    }

    public void displayPath(string pathArray) {
        string[] pathArraySplit = pathArray.Split('_');
        for (int i = 0; i < pathArraySplit.Length - 1; i++) {
            if(pathArraySplit[i] != "-1") {
            int pathIndex = int.Parse(pathArraySplit[i]);
            pathPool[pathIndex].SetActive(true);
            }
        }
    }

    public void hidePath() {
        foreach (GameObject path in pathPool) {
            path.SetActive(false);
        }
    }
}
