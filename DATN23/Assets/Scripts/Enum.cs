using UnityEngine;

public enum PackageStatus
{
    Waiting,
    Loading,
    Moving,
    Unloading,
    Delivered
}

public enum NodeType
{
    Walkable,
    CrossNode,
    TemporaryBlock,
    Block
}

public enum CarStage
{
    Idle,
    Moving,
    Loading,
    HeadingBack
}

