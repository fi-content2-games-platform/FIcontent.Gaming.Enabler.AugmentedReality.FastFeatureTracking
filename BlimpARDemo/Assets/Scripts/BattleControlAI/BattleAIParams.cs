using UnityEngine;
using System;

[Serializable]
public class BattleAIParams
{
    public int FlockSize = 3;
    public int MaxShips = 3;
    public float ChangeTargetDelay = 30f;
    public GameObject ShipPrefab;
    public Transform PathPrefab;

    public Transform SpawnPoint;
    public float SpawnRadius = 200f;
    public float SpawnDelay = 3f;

    internal void TestParams()
    {
        if (!ShipPrefab)
            Debug.LogError("Please define the BattleAIParams.ShipPrefab in the inspector!");

        if (!SpawnPoint)
            Debug.LogError("Please define the BattleAIParams.SpawnPoint in the inspector!");
    }
}

