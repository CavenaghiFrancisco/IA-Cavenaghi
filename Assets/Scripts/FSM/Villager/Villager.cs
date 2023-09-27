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
        public GameObject Target;
        public GameObject Home;
        public VoronoiController voronoiCalculator;
        public int mined = 0;

        public float speed = 5;

        public float resourcesCollected = 0;

        private FSM fsm;

        private List<Vector3> travelPositions;

        StateParameters allParameters;

        private void Start()
        {
            Mine.OnMineDestroy += (bool areMines) =>
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
            Mine.OnMineDestroy -= (bool areMines) =>
            {
                voronoiCalculator.SetVoronoi(AdminOfGame.GetMap().MinesAvailable);
                fsm.SetCurrentStateForced((int)States.Retrieve);
            };

            VillagerAdmin.OnEmergencyCalled -= () => fsm.SetCurrentStateForced((int)States.Retrieve);
        }
    }
}