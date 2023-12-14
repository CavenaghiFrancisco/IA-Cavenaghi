using IA.FSM.States;
using MinerSimulator.Admins;
using MinerSimulator.Entity;
using MinerSimulator.Map;
using MinerSimulator.Utils.Voronoi;
using System;
using UnityEngine;

namespace IA.FSM.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        protected FSM fsm;
        protected StateParameters allParameters;
        protected VoronoiController voronoiCalculator;

        private float speed;
        private int resourcesCollected = 0;
        private Transform target;
        private GameObject home;
        protected Vector3[] travelPositions;

        public Transform Target { get => target; set => target = value; }
        public GameObject Home { get => home; set => home = value; }
        public float Speed { get => speed; set => speed = value; }
        public int ResourcesCollected { get => resourcesCollected; set => resourcesCollected = value; }

        protected abstract void Start();

        public void Update()
        {
            fsm?.Update();
        }

        protected virtual void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        protected void SubscribeToEvents()
        {
            Mine.OnMineDestroy += HandleMineDestroy;
            VillagerAdmin.OnEmergencyCalled += HandleEmergencyCalled;
        }

        protected void UnsubscribeFromEvents()
        {
            Mine.OnMineDestroy -= HandleMineDestroy;
            VillagerAdmin.OnEmergencyCalled -= HandleEmergencyCalled;
        }

        protected void HandleMineDestroy(bool areMines, bool areWorkedMines)
        {
            if (!areWorkedMines)
                fsm.SetCurrentStateForced(GetReturnState());
            voronoiCalculator.SetVoronoi(MapGenerator.Instance.MinesAvailable);
        }

        protected void HandleEmergencyCalled()
        {
            fsm.SetCurrentStateForced(GetReturnState());
        }

        protected abstract int GetReturnState();
    }
}
