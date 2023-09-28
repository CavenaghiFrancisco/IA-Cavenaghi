using IA.FSM.States;
using IA.FSM.States.Villager;
using MinerSimulator.Admins;
using MinerSimulator.Entity;
using MinerSimulator.Utils.Voronoi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.Villager
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

namespace IA.FSM.Villager
{
    public class Villager : MonoBehaviour
    {
        private GameObject target;
        private GameObject home;
        private VoronoiController voronoiCalculator;
        private int mined = 0;

        private float speed = 5;

        private float resourcesCollected = 0;

        private FSM fsm;

        private List<Vector3> travelPositions;

        StateParameters allParameters;

        public GameObject Home { get => home; set => home = value; }
        public GameObject Target { get => target; set => target = value; }
        public float Speed { get => speed; set => speed = value; }

        private void Start()
        {
            Mine.OnMineDestroy += (bool areMines, bool areWorkedMines) =>
            {
                voronoiCalculator.SetVoronoi(AdminOfGame.GetMap().MinesAvailable);
                if(!areMines)
                    fsm.SetCurrentStateForced((int)States.Retrieve);
            };
            VillagerAdmin.OnEmergencyCalled += () => fsm.SetCurrentStateForced((int)States.Retrieve);
            voronoiCalculator = GetComponent<VoronoiController>();
            allParameters = new StateParameters();

            fsm = new FSM(Enum.GetValues(typeof(States)).Length, Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)States.Collect, (int)Flags.OnNearTarget, (int)States.Mine);

            fsm.SetRelation((int)States.Mine, (int)Flags.OnHaveEnoughResources, (int)States.Retrieve);

            fsm.SetRelation((int)States.Retrieve, (int)Flags.OnSeeTarget, (int)States.Collect);

            fsm.SetRelation((int)States.Mine, (int)Flags.OnMineDestroyed, (int)States.Collect);

            fsm.SetRelation((int)States.Collect, (int)Flags.OnEmergency, (int)States.Retrieve);



            allParameters.Parameters = new object[8] { gameObject.transform, speed, Target, resourcesCollected, Home, travelPositions, mined, voronoiCalculator };
            fsm.AddState<MineState>((int)States.Mine,
                allParameters,allParameters);

            fsm.AddState<RetrieveState>((int)States.Retrieve,
                allParameters, allParameters);

            fsm.AddState<CollectState>((int)States.Collect,
                allParameters, allParameters);

            fsm.SetCurrentStateForced((int)States.Collect);
        }

        public void Update()
        {
            if(!(Vector3.Distance(transform.position,Home.transform.position) < 1 && AdminOfGame.GetMap().MinesAvailable.Count < 0))
            fsm.Update();
        }

        private void OnDestroy()
        {
            Mine.OnMineDestroy -= (bool areMines, bool areWorkedMines) =>
            {
                voronoiCalculator.SetVoronoi(AdminOfGame.GetMap().MinesAvailable);
                fsm.SetCurrentStateForced((int)States.Retrieve);
            };

            VillagerAdmin.OnEmergencyCalled -= () => fsm.SetCurrentStateForced((int)States.Retrieve);
        }
    }
}