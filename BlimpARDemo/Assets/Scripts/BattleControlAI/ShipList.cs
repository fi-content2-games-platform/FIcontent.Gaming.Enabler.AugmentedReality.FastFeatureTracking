using UnityEngine;
using System.Collections.Generic;

namespace DRZ.BlimpArDemo.BattleControlAI
{
    public abstract class ShipList<T> : List<T> where T : MonoBehaviour
    {
        private float nextSpawnTime;

        private bool defeated = true;

        public BattleAIParams aiParams;                         // stores the ship list parameters

        protected abstract Transform container { get; }         // gets the default container where instances are stored

        public ShipList(BattleAIParams shipListParams, bool init)
        {
            this.aiParams = shipListParams;

            if (init)
            {
                /*
                var dir = aiParams.SpawnPoint.forward;
                var pos = aiParams.SpawnPoint.position;
                float spacing = 500f;

                for (int i = 0; i < aiParams.MaxShips / 2; i++)
                {
                    pos += dir * spacing;

                    if (i * 2 + 1 == aiParams.MaxShips)
                        Spawn(pos);
                    else
                    {
                        var pos1 = new Vector3(pos.x - spacing, pos.y, pos.z - spacing);
                        var pos2 = new Vector3(pos.x + spacing, pos.y, pos.z + spacing);

                        Spawn(pos1);
                        Spawn(pos2);
                    }
                }
                /**/

                //for (int i = 0; i < aiParams.MaxShips; i++)
                //  Spawn(aiParams.SpawnPoint.position);
            }
        }

        /// <summary>
        /// Sets as defeated, no more ships will spawn in this list
        /// </summary>
        public void SetDefeated(bool defeated)
        {
            this.defeated = defeated;
        }

        /// <summary>
        /// Spawns the ships at the specified point
        /// </summary>
        private void Spawn(Vector3 pos)
        {
            var s = GameObject.Instantiate(aiParams.ShipPrefab, pos, aiParams.SpawnPoint.rotation) as GameObject;
            s.transform.parent = container;
            s.SendMessage("SetPath", aiParams.PathPrefab, SendMessageOptions.DontRequireReceiver);
            var bc = s.GetComponent<BoidController>();
            if (bc)
            {
                bc.flockSize = aiParams.FlockSize;
            }
            this.Add(s.GetComponent<T>());
        }
        /// <summary>
        /// Spawns the ships around the specified spawnPoint
        /// </summary>
        private void Spawn()
        {
            Vector3 pos = Random.onUnitSphere * aiParams.SpawnRadius + aiParams.SpawnPoint.position;

            Spawn(pos);
        }



        /// <summary>
        /// Keeps the number of ships in the scene constant
        /// </summary>
        internal void Update()
        {
            this.CleanUp();
            this.Respawn();
        }

        /// <summary>
        /// Cleans the list
        /// </summary>
        private void CleanUp()
        {
            for (int i = this.Count - 1; i >= 0; i--)
                if (!this[i])
                    this.RemoveAt(i);
        }

        /// <summary>
        /// Keeps the stations spawning until the side is defeated
        /// </summary>
        private void Respawn()
        {
            if (!defeated && this.Count < aiParams.MaxShips)
                if (Time.time > nextSpawnTime)
                {
                    nextSpawnTime = Time.time + aiParams.SpawnDelay;
                    Spawn();
                }
        }
    }
}
