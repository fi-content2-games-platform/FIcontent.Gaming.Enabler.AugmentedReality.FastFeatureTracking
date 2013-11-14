using UnityEngine;


namespace DRZ.BlimpArDemo.BattleControlAI
{
    public class Fighters : ShipList<BoidController>
    {        
        protected override Transform container
        {
            get { return Containers.Fighters; }
        }

        public Fighters(BattleAIParams shipListParams, bool init)
            : base(shipListParams, init)
        {

        }
    }
}

