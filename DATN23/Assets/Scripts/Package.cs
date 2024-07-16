using UnityEngine;

public class Package : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public PackageData packageData;
    
    void Start()
    {
    }

    public void DisableSpriteRenderer()
    {
        spriteRenderer.enabled = false;
    }

    public void EnableSpriteRenderer()
    {
        spriteRenderer.enabled = true;
    }
}

[System.Serializable]
public class PackageData
{
    public int PackageID;
    public int CurrentLocation;
    public int Destination;
    public PackageStatus PackageStatus;
}
