using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DRZ.BlimpArDemo.BattleControlAI;

public class BattleAI : MonoBehaviour
{    
    public Transform emperrorStationTarget;
    public BattleAIParams rebelStationParams, rebelSquadronParams;
    public BattleAIParams emperrorStationParams, emperrorSquadronParams;

    public RebelFaction rebels;
    public Faction emperror;

    void Awake()
    {
        rebelStationParams.TestParams();
        rebelSquadronParams.TestParams();
        emperrorStationParams.TestParams();
        emperrorSquadronParams.TestParams();
    }

    void Start()
    {
        GameEventManager.GameStartEvent += InitFactions;
        GameEventManager.GameOverEvent += DestroyFactions;       
    }

    /// <summary>
    /// Create the factions
    /// </summary>
    private void InitFactions()
    {
        //init stations
        rebels = new RebelFaction(rebelStationParams, rebelSquadronParams);
        emperror = new Faction(emperrorStationParams, emperrorSquadronParams);

        emperror.Fighters.SetDefeated(true);
    }
    private void DestroyFactions()
    {
        emperror.SetDefeated(true);
        rebels.SetDefeated(true);

        foreach (var g in rebels.Fighters)
            Destroy(g.gameObject);

        foreach (var g in emperror.Fighters)
            Destroy(g.gameObject);

        foreach (var g in rebels.Stations)
            Destroy(g.gameObject);

        foreach (var g in emperror.Stations)
            Destroy(g.gameObject);



    }

    /// <summary>
    /// updates the lists and assigns targets
    /// </summary>
    void Update()
    {
        if (rebels != null && emperror != null)
        {
            rebels.Update();

            emperror.Update();

            //AssignTargetsChain();
            AssignTargetsBalancedRandom();
        }
    }



    /// <summary>
    /// Chain targeting: 
    ///     rebels > empstations 
    ///     emperror > rebels
    /// </summary>
    private void AssignTargetsChain()
    {
        for (int i = 0; i < rebels.Fighters.Count; i++)
        {
            if (rebels.Fighters[i].target) continue;
            if (emperror.Stations.Count == 0) continue;
            rebels.Fighters[i].SetTarget(emperror.Stations[Random.Range(0, emperror.Stations.Count - 1)].transform);
        }

        foreach (var f in emperror.Fighters)
        {

            if (f.NewTarget)
            {
                f.SetTarget(rebels.Fighters[Random.Range(0, rebels.Fighters.Count - 1)].transform, emperror.Fighters.aiParams.ChangeTargetDelay);
            }
        }
    }

    /// <summary>
    /// Assisgns the targets between stations and squadrons
    /// </summary>
    private void AssignTargetsBalancedRandom()
    {
        // set rebel targets
        foreach (var f in rebels.Fighters)
        {
            if (f.NewTarget)
            {
                f.SetTarget(emperror.GetTarget(), rebels.Fighters.aiParams.ChangeTargetDelay);
            }
        }

        // emperror targets
        foreach (var f in emperror.Fighters)
        {
            if (f.NewTarget)
            {
                f.SetTarget(rebels.GetTarget(), emperror.Fighters.aiParams.ChangeTargetDelay);
            }
        }
    }

    #region Scripted Behaviours

    /// <summary>
    /// Emperror ships attack
    /// </summary>
    [ContextMenu("Attack Target")]
    public void EmperrorAttackTarget()
    {
        foreach (var s in emperror.Stations)
        {
            if (s.currentPathTargetIndex == 0) s.currentPathTargetIndex++;
           StartCoroutine(SetTarget((EmperrorStation)s));
        }
    }

    IEnumerator SetTarget(EmperrorStation s)
    {
        yield return new WaitForSeconds(Random.Range(0f, 2f));

        s.target = emperrorStationTarget ? emperrorStationTarget : null;
    }

    #endregion
}


