using DRZ.BlimpArDemo.BattleControlAI;

using UnityEngine;

public class Faction
{
    public Stations Stations;
    public Fighters Fighters;

    protected float stationsPerc;

    public Faction()
    {
        stationsPerc = 0.75f;// (float)stationParams.MaxShips / (float)(stationParams.MaxShips + squadronParams.MaxShips);
    }

    public Faction(BattleAIParams stationParams, BattleAIParams squadronParams)
        : base()
    {
        Stations = new Stations(stationParams, true);
        Fighters = new Fighters(squadronParams, false);
    }

    /// <summary>
    /// Updates the number battle ships to keep it balanced
    /// until a faction is set as defeated
    /// </summary>
    internal void Update()
    {
        Stations.Update();
        Fighters.Update();

    }

    /// <summary>
    /// Gives a random target between the percentage of stations and squadrons
    /// </summary>
    /// <returns>the target</returns>
    public Transform GetTarget()
    {
        //targeting is balanced between stations and squadrons
        if (Random.Range(0f, 1f) > stationsPerc)
        {
            if (Stations.Count > 0)
                return Stations[Random.Range(0, Stations.Count - 1)].transform;
        }

        if (Fighters.Count == 0)
            return null;
        else
            return Fighters[Random.Range(0, Fighters.Count - 1)].transform;

    }

    internal void SetDefeated(bool p)
    {
        Fighters.SetDefeated(p);
        Stations.SetDefeated(p);
    }
}