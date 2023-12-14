using IA.FSM.Entities.Carriage;
using IA.FSM.Entities.Villager;
using MinerSimulator.Map;
using MinerSimulator.Utils.Voronoi;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UnityEngine;

namespace MinerSimulator.Admins
{
    public class VillagerAdmin : MonoBehaviour
    {
        public ConcurrentBag<Villager> Villagers { get; private set; } = new ConcurrentBag<Villager>();
        public ConcurrentBag<Carriage> Carriages { get; private set; } = new ConcurrentBag<Carriage>();

        public GameObject VillagerPrefab;
        public GameObject CarriagePrefab;
        public int VillagerQuantity = 2;
        public static int CarriageQuantity = 1;

        private bool emergency = false;

        public bool Emergency => emergency;

        public static Action OnEmergencyCalled;

        private static VillagerAdmin instance;

        public static VillagerAdmin Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<VillagerAdmin>();

                return instance;
            }
        }

        private void Start()
        {
            InitializeEntities(VillagerPrefab, Villagers, VillagerQuantity, VillagerQuantity);
            InitializeEntities(CarriagePrefab, Carriages, CarriageQuantity, CarriageQuantity);

            Parallel.ForEach(Villagers, new ParallelOptions { MaxDegreeOfParallelism = 6}, villager =>
            {
                villager.Update();
            });

            Parallel.ForEach(Carriages, new ParallelOptions { MaxDegreeOfParallelism = 6}, carriage =>
            {
                carriage.Update();
            });
        }

        private void InitializeEntities<T>(GameObject prefab, ConcurrentBag<T> entities, int quantity, int initialQuantity)
            where T : IA.FSM.Entities.Entity
        {
            for (int i = 0; i < quantity; i++)
            {
                GameObject entityAux = Instantiate(prefab, transform.position, Quaternion.identity);
                T entity = entityAux.GetComponent<T>();
                entities.Add(entity);
                entityAux.AddComponent<VoronoiController>().SetVoronoi(MapGenerator.Instance.MinesAvailable);
                entity.Home = this.gameObject;
                entity.Speed = UnityEngine.Random.Range(1, 2);
                if (typeof(T) == typeof(Carriage))
                {
                    entity.Speed = 6;
                }
            }

            quantity -= initialQuantity;
            initialQuantity -= quantity;

            if(typeof(T) == typeof(Carriage))
            {
                CarriageQuantity--;
            }
        }

        public void SetEmergency(bool isEmergency)
        {
            emergency = !isEmergency;
            OnEmergencyCalled?.Invoke();
        }
    }
}
