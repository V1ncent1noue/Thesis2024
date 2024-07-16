using UnityEngine;
using System;
using Firebase.Database;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;


public class DatabaseManager : MonoBehaviour
{
    [SerializeField] private CarManager carManager;
    [SerializeField] private MapManager MapManager;
    private DatabaseReference reference;
    void Awake()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(FirebaseCarInit());
    }

    public IEnumerator FirebaseCarInit()
    {
        var CarInit = reference.Child("CarData").GetValueAsync();
        yield return new WaitUntil(() => CarInit.IsCompleted);

        if (CarInit.Exception != null)
        {
            Debug.LogError(CarInit.Exception);
        }
        else
        {
            DataSnapshot snapshot = CarInit.Result;
            foreach (DataSnapshot carSnapshot in snapshot.Children)
            {
                string carKey = carSnapshot.Key;
                reference.Child("CarData").Child(carKey).Child("Destination").ValueChanged += HandleDestinationChanged;
                reference.Child("CarData").Child(carKey).Child("CurrentPosition").ValueChanged += HandleCurrentPositionChanged;
                reference.Child("CarData").Child(carKey).Child("PathRequest").ValueChanged += HandlePathRequestChanged;
            }
        }
    }
    public IEnumerator FirebaseNodeMapInit()
    {
        var NodeMapInit = reference.Child("MapData/nodeMap").GetValueAsync();
        yield return new WaitUntil(() => NodeMapInit.IsCompleted);

        if (NodeMapInit.Exception != null)
        {
            Debug.LogError(NodeMapInit.Exception);
        }
        else
        {
            DataSnapshot snapshot = NodeMapInit.Result;
            foreach (DataSnapshot nodeSnapshot in snapshot.Children)
            {
                string nodeKey = nodeSnapshot.Key;
                reference.Child("MapData/nodeMap").Child(nodeKey).ValueChanged += HandleNodeChange;
            }
        }
    }
    #region MapData
    private void HandleNodeChange(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Get the key of the node whose data changed
        string nodeKey = args.Snapshot.Reference.Key;
        MapManager.UpdateNodeType(int.Parse(nodeKey), (NodeType)int.Parse(args.Snapshot.Value.ToString()));
        // Do something with the data in args.Snapshot
    }
    public void SaveMapData(MapData mapData)
    {
        string json = JsonUtility.ToJson(mapData);
        reference.Child("MapData").SetRawJsonValueAsync(json);
    }

    public void UpdateNodeType(int Index, NodeType nodeType)
    {
        reference.Child("MapData").Child("nodeMap").Child(Index.ToString()).SetValueAsync((int)nodeType);
    }

    public IEnumerator LoadMapDataIEnumerator(Action<MapData> callback)
    {
        if (reference == null)
        {
            // Handle the error here, e.g., by initializing reference or throwing an error.
            throw new Exception("reference is null");
        }
        else
        {

            var task = reference.Child("MapData").GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }
            else
            {
                DataSnapshot snapshot = task.Result;
                string json = snapshot.GetRawJsonValue();
                MapData mapData = JsonUtility.FromJson<MapData>(json);
                callback(mapData);
            }
        }

    }

    public IEnumerator LoadPlacementIEnumerator(Action<Placement> callback)
    {
        var task = reference.Child("PlacementData").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            string json = snapshot.GetRawJsonValue();
            Placement placement = JsonUtility.FromJson<Placement>(json);
            callback(placement);
        }
    }
    #endregion

    #region CarData
    void HandleDestinationChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Get the key of the car whose data changed
        string carKey = args.Snapshot.Reference.Parent.Key;
        Debug.Log("Destination of car " + carKey + " changed to: " + args.Snapshot.Value.ToString());
        // Do something with the data in args.Snapshot
        carManager.UpdateCarDestinationInList(carKey, int.Parse(args.Snapshot.Value.ToString()));
    }

    void HandleCurrentPositionChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Get the key of the car whose data changed
        string carKey = args.Snapshot.Reference.Parent.Key;
        Debug.Log("Current position of car " + carKey + " changed to: " + args.Snapshot.Value.ToString());
        // Do something with the data in args.Snapshot
        carManager.UpdateCarPositionInList(carKey, int.Parse(args.Snapshot.Value.ToString()));
    }

    void HandlePathRequestChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Get the key of the car whose data changed
        string carKey = args.Snapshot.Reference.Parent.Key;
        Debug.Log("Path request of car " + carKey + " changed to: " + args.Snapshot.Value.ToString());
        // Do something with the data in args.Snapshot
        carManager.UpdateCarPathRequestInList(carKey, (bool)args.Snapshot.Value);
    }
    public void SaveCarData(CarData carData)
    {
        string json = JsonUtility.ToJson(carData);
        reference.Child("CarData").Child(carData.CarID).SetRawJsonValueAsync(json);
    }
    public void SaveAllCarData(List<CarData> carDataList)
    {
        foreach (CarData carData in carDataList)
        {
            string json = JsonUtility.ToJson(carData);
            reference.Child("CarData").Child(carData.CarID).SetRawJsonValueAsync(json);
        }
    }
    public void UpdateCarRoadPath(string carID, string roadPath)
    {
        reference.Child("CarData").Child(carID).Child("RoadPath").SetValueAsync(roadPath);
    }
    public void UpdateCarRoad(string carID, string path)
    {
        reference.Child("CarData").Child(carID).Child("Road").SetValueAsync(path);
    }
    public void UpdateCarPathRequest(string carID, bool pathRequest)
    {
        reference.Child("CarData").Child(carID).Child("PathRequest").SetValueAsync(pathRequest);
    }
    public IEnumerator GetDestinationFromPackageID(int packageID, Action<int> callback)
    {
        var task = reference.Child("PackageData").Child(packageID.ToString()).Child("Destination").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            int destination = int.Parse(snapshot.Value.ToString());
            callback(destination);
        }
    }
    public IEnumerator LoadCarDataIEnumerator(string carID, Action<CarData> callback)
    {
        var task = reference.Child("CarData").Child(carID).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            string json = snapshot.GetRawJsonValue();
            CarData carData = JsonUtility.FromJson<CarData>(json);
            callback(carData);
        }
    }
    public IEnumerator LoadAllCarDataIEnumerator(Action<List<CarData>> callback)
    {
        var task = reference.Child("CarData").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            List<CarData> carDataList = new List<CarData>();
            foreach (DataSnapshot carSnapshot in snapshot.Children)
            {
                string json = carSnapshot.GetRawJsonValue();
                CarData carData = JsonUtility.FromJson<CarData>(json);
                carDataList.Add(carData);
            }
            callback(carDataList);
        }
    }

    #endregion

    #region PackageData

    public void SavePackageData(PackageData packageData)
    {
        string json = JsonUtility.ToJson(packageData);
        reference.Child("PackageData").Child(packageData.PackageID.ToString()).SetRawJsonValueAsync(json);
    }

    public void UpdatePackageData(int packageID, int currentLocation, int destination)
    {
        reference.Child("PackageData").Child(packageID.ToString()).Child("CurrentLocation").SetValueAsync(currentLocation);
        reference.Child("PackageData").Child(packageID.ToString()).Child("Destination").SetValueAsync(destination);
    }

    public void UpdatePackageStatus(int packageID, PackageStatus packageStatus)
    {
        reference.Child("PackageData").Child(packageID.ToString()).Child("PackageStatus").SetValueAsync((int)packageStatus);
    }
    public IEnumerator LoadPackageDataIEnumerator(int packageID, Action<PackageData> callback)
    {
        var task = reference.Child("PackageData").Child(packageID.ToString()).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError(task.Exception);
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            string json = snapshot.GetRawJsonValue();
            PackageData packageData = JsonUtility.FromJson<PackageData>(json);
            callback(packageData);
        }
    }

    public void UpdateDestination(int packageID, int destination)
    {
        reference.Child("PackageData").Child(packageID.ToString()).Child("Destination").SetValueAsync(destination);
    }

    #endregion
}


