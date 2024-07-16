using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//ui
using UnityEngine.UI; 

public class ItemResultCell : MonoBehaviour
{
    [SerializeField] private Text itemID;
    [SerializeField] private Text itemStatus;
    [SerializeField] private Text itemPlacement;
    [SerializeField] private Text itemCurrentLocation;

    public void SetItemID(string id)
    {
        itemID.text = "ID:" + id;
    }
    public void SetItemStatus(string status)
    {
        itemStatus.text = "Status:" + status;
    }
    public void SetItemPlacement(string placement)
    {
        itemPlacement.text = "Placement:" + placement;
    }
    public void SetItemCurrentLocation(string location)
    {
        itemCurrentLocation.text = "Current Location:" + location;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
