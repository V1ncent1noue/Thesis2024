using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [SerializeField] private GameObject carPrefab;
    [SerializeField] private PathFinding pathFinding;
    [SerializeField] private PathPooling pathPooling;
    [SerializeField] private DatabaseManager databaseManager;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private List<Car> cars = new List<Car>();
    [SerializeField] private List<CarData> carDataList = new List<CarData>();
    [SerializeField] private List<Car> PathRequestQueue = new List<Car>();
    

    void Start()
    {
        StartCoroutine(databaseManager.LoadCarDataIEnumerator("Car1", (carData) => {
            carDataList.Add(carData);
            CreateCar(carData);
        }));
        StartCoroutine(databaseManager.LoadCarDataIEnumerator("Car2", (carData) => {
            carDataList.Add(carData);
            CreateCar(carData);
        }));
    }
    private void CreateCar(CarData carData)
    {
        GameObject carGameObject = Instantiate(carPrefab, new Vector3(carData.CurrentPosition % mapManager.mapData.width, carData.CurrentPosition / mapManager.mapData.width, 0), Quaternion.identity);
        carGameObject.transform.SetParent(transform);
        Car car = carGameObject.GetComponent<Car>();
        car.carData = carData;
        car.OnCarLeftClicked += OnCarLeftClicked;
        car.OnCarMouseEnter += OnCarMouseEnter;
        car.OnCarMouseExit += OnCarMouseExit;
        car.transform.position = new Vector3(car.carData.CurrentPosition % mapManager.mapData.width, car.carData.CurrentPosition / mapManager.mapData.width, 0);
        StartCoroutine(databaseManager.LoadPackageDataIEnumerator(carData.PackageID, (packageData) => {
            car.package.packageData = packageData;
        }));
        cars.Add(car);
    }

    public void UpdatePathFinding(){
        foreach (Car car in cars)
        {
            car.carData.RoadPath = pathFinding.GetPath(car.carData.CurrentPosition, car.carData.Destination);
            car.carData.Road = pathFinding.GetRoad(car.carData.CurrentPosition, car.carData.Destination);
            databaseManager.UpdateCarRoadPath(car.carData.CarID, car.carData.RoadPath);
            databaseManager.UpdateCarRoad(car.carData.CarID, car.carData.Road);
        }
    }
    public void UpdateCarPositionInList(string carID, int currentPosition){
        foreach (Car car in cars)
        {
            if (car.carData.CarID == carID)
            {
                car.carData.CurrentPosition = currentPosition;
            }
        }
    }
    public void UpdateCarDestinationInList(string carID, int destination){
        foreach (Car car in cars)
        {
            if (car.carData.CarID == carID)
            {
                car.carData.Destination = destination;
            }
        }
    }
    public void UpdateCarPathRequestInList(string carID, bool pathRequest){
        foreach (Car car in cars)
        {
            if (car.carData.CarID == carID)
            {
                car.carData.PathRequest = pathRequest;
            }
        }
    }
    private void ReloadCarPosition(){
        foreach (Car car in cars)
        {
            car.transform.position = new Vector3(car.carData.CurrentPosition % mapManager.mapData.width, car.carData.CurrentPosition / mapManager.mapData.width, 0);
        }
    }

    private void CheckPathRequest(){
        foreach (Car car in cars)
        {
            if (car.carData.PathRequest)
            {
                car.carData.RoadPath = pathFinding.GetPath(car.carData.CurrentPosition, car.carData.Destination);
                car.carData.Road = pathFinding.GetRoad(car.carData.CurrentPosition, car.carData.Destination);
                databaseManager.UpdateCarRoadPath(car.carData.CarID, car.carData.RoadPath);
                databaseManager.UpdateCarRoad(car.carData.CarID, car.carData.Road);
                databaseManager.UpdateCarPathRequest(car.carData.CarID, false);
            }
        }
    }
    void OnCarLeftClicked(Car car)
    {
    }

    void OnCarMouseEnter(Car car)
    {
        pathPooling.displayPath(car.carData.RoadPath);
    }

    void OnCarMouseExit(Car car)
    {
        pathPooling.hidePath();
    }

    void FixedUpdate()
    {
        ReloadCarPosition();
        CheckPathRequest();
    }
}

[System.Serializable]
public class CarData {
    public string CarID;
    public int CurrentPosition;
    public CarStage carStage;
    public int Destination;
    public int PackageID;
    public string RoadPath;
    public string Road;
    public bool PathRequest;


    public CarData(string _CarID,int _currentPosition, CarStage _carStage, int _destination, int _packageID, string _roadPath, string _road, bool _pathRequest) {
        CarID = _CarID;
        CurrentPosition = _currentPosition;
        carStage = _carStage;
        Destination = _destination;
        PackageID = _packageID;
        RoadPath = _roadPath;
        Road = _road;
        PathRequest = _pathRequest;
    }
} 