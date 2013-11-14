using UnityEngine;
using System.Collections.Generic;

namespace DRZ.BlimpArDemo.BattleControlAI
{
    public class Stations : ShipList<Station>
    {
        protected override Transform container
        {
            get { return Containers.Stations; }
        }

        public Stations(BattleAIParams shipListParams, bool init)
            : base(shipListParams, init)
        {                        
           
        }
    }
}
