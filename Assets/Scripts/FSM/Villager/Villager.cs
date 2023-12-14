using IA.FSM.States;
using IA.FSM.States.Villager;
using MinerSimulator.Admins;
using MinerSimulator.Entity;
using MinerSimulator.Map;
using MinerSimulator.Utils.Voronoi;
using System;
using UnityEngine;

namespace IA.FSM.Entities.Villager
{
    internal enum States
    {
        Collect,
        Mine,
        Retrieve
    }

    internal enum Flags
    {
        OnSeeTarget,
        OnNearTarget,
        OnHaveEnoughResources,
        OnMineDestroyed,
        OnEmergency
    }
}

namespace IA.FSM.Entities.Villager
{
    public class Villager : Entity
    {
        private int mined = 0;
        private float resourcesCollected = 0;

        protected override void Start()
        {
            InitializeVoronoi();
            InitializeFSM();
            SubscribeToEvents();
            SetInitialState();
        }

        private void InitializeVoronoi()
        {
            voronoiCalculator = GetComponent<VoronoiController>();
        }

        private void InitializeFSM()
        {
            fsm = new FSM(Enum.GetValues(typeof(States)).Length, Enum.GetValues(typeof(Flags)).Length);
            ConfigureFSMRelations();
            ConfigureFSMStates();
        }

        private void ConfigureFSMRelations()
        {
            fsm.SetRelation((int)States.Collect, (int)Flags.OnNearTarget, (int)States.Mine);
            fsm.SetRelation((int)States.Mine, (int)Flags.OnHaveEnoughResources, (int)States.Retrieve);
            fsm.SetRelation((int)States.Retrieve, (int)Flags.OnSeeTarget, (int)States.Collect);
            fsm.SetRelation((int)States.Mine, (int)Flags.OnMineDestroyed, (int)States.Collect);
            fsm.SetRelation((int)States.Collect, (int)Flags.OnEmergency, (int)States.Retrieve);
        }

        private void ConfigureFSMStates()
        {
            allParameters = new StateParameters { Parameters = new object[8] { gameObject.transform, Speed, Target, resourcesCollected, Home, travelPositions, mined, voronoiCalculator } };

            fsm.AddState<MineState>((int)States.Mine, allParameters, allParameters);
            fsm.AddState<RetrieveState>((int)States.Retrieve, allParameters, allParameters);
            fsm.AddState<CollectState>((int)States.Collect, allParameters, allParameters);
        }

        private void SetInitialState()
        {
            fsm.SetCurrentStateForced((int)States.Collect);
        }

        public new void Update()
        {
            if (!(IsCloseToHome() && MapGenerator.Instance.MinesAvailable.Count < 0))
                fsm.Update();
        }

        private bool IsCloseToHome()
        {
            return Vector3.Distance(transform.position, Home.transform.position) < 1;
        }

        protected override int GetReturnState()
        {
            return (int)States.Retrieve;
        }
    }
}
