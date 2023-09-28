using IA.FSM.Carriage;
using IA.FSM.Villager;
using MinerSimulator.Utils.Voronoi;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace MinerSimulator.Admins
{
    public class VillagerAdmin : MonoBehaviour
    {
        public System.Collections.Concurrent.ConcurrentBag<Villager> villagers = new System.Collections.Concurrent.ConcurrentBag<Villager>();
        public System.Collections.Concurrent.ConcurrentBag<Carriage> carriages = new System.Collections.Concurrent.ConcurrentBag<Carriage>();

        public GameObject villagerPrefab;
        public GameObject carriagePrefab;
        public int villagerQuantity = 3;
        public static int carriageQuantity = 1;

        static bool emergency = false;

        public static bool Emergency { get => emergency; }

        public static Action OnEmergencyCalled;

        private void Start()
        {
            for(int i = 0; i < villagerQuantity; i++) 
            {
                GameObject villagerAux = Instantiate(villagerPrefab, transform.position, Quaternion.identity);
                villagers.Add(villagerAux.GetComponent<Villager>());
                villagerAux.AddComponent<VoronoiController>().SetVoronoi(AdminOfGame.GetMap().MinesAvailable);
                villagerAux.GetComponent<Villager>().Home = this.gameObject;
                villagerAux.GetComponent<Villager>().Speed = UnityEngine.Random.Range(1,4);
            }

            for (int i = 0; i < carriageQuantity; i++)
            {
                GameObject carriageAux = Instantiate(carriagePrefab, transform.position, Quaternion.identity);
                carriageAux.AddComponent<Carriage>();
                carriages.Add(carriageAux.GetComponent<Carriage>());
                carriageAux.AddComponent<VoronoiController>();
                carriageAux.GetComponent<Carriage>().Home = gameObject;
                carriageAux.GetComponent<Carriage>().Speed = UnityEngine.Random.Range(7, 10);
                carriageQuantity--;
                i--;
            }

            ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = 32 };

            Parallel.ForEach(villagers, options, currentItem =>
            {
                currentItem.Update();
            });

            Parallel.ForEach(carriages, options, currentItem =>
            {
                currentItem.Update();
            });
        }

        public static void SetEmergency(bool isEmergency)
        {
            emergency = !isEmergency;
            OnEmergencyCalled();
        }
    }
}
