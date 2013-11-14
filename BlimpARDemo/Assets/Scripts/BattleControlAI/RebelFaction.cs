using DRZ.BlimpArDemo.BattleControlAI;
using UnityEngine;

public class RebelFaction : Faction
{
    public RebelFaction (BattleAIParams stationParams, BattleAIParams squadronParams) : base()
    {
        Stations = new Stations(stationParams, false);
        Fighters = new Fighters(squadronParams, false);

        //stationsPerc = (float)stationParams.MaxShips / (float)(stationParams.MaxShips + squadronParams.MaxShips);                
    }
}

