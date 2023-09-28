using IA.FSM.States;
using IA.FSM.States.Carriage;
using MinerSimulator.Admins;
using MinerSimulator.Entity;
using MinerSimulator.Utils.Voronoi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.Carriage
{
    internal enum States
    {
        Suply,
        Return
    }

    internal enum Flags
    {
        OnNeedToReturnHome,
        OnSuplyMode
    }
}

namespace IA.FSM.Carriage
{
    public class Carriage : MonoBehaviour
    {
        private GameObject target;
        private GameObject home;
        private VoronoiController voronoiCalculator;

        private float speed;

        private float food = 0;

        private FSM fsm;

        private List<Vector3> travelPositions;

        StateParameters allParameters;

        public GameObject Target { get => target; set => target = value; }
        public GameObject Home { get => home; set => home = value; }
        public float Speed { get => speed; set => speed = value; }

        private void Start()
        {
            Mine.OnMineDestroy += (bool areMines,bool areWorkedMines) =>
            {
                if (!areWorkedMines)
                    fsm.SetCurrentStateForced((int)States.Return);
            };
            voronoiCalculator = GetComponent<VoronoiController>();
            allParameters = new StateParameters();

            fsm = new FSM(Enum.GetValues(typeof(States)).Length, Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)States.Return, (int)Flags.OnSuplyMode, (int)States.Suply);

            fsm.SetRelation((int)States.Suply, (int)Flags.OnNeedToReturnHome, (int)States.Return);

            allParameters.Parameters = new object[7] { gameObject.transform, speed, Target, food, Home, travelPositions, voronoiCalculator };
            fsm.AddState<SuplyState>((int)States.Suply,
                allParameters, allParameters);

            fsm.AddState<ReturnState>((int)States.Return,
                allParameters, allParameters);

            fsm.SetCurrentStateForced((int)States.Suply);

            VillagerAdmin.OnEmergencyCalled += () => fsm.SetCurrentStateForced((int)States.Return);
        }

        public void Update()
        {
            fsm.Update();
        }

        private void OnDestroy()
        {
            VillagerAdmin.OnEmergencyCalled -= () => fsm.SetCurrentStateForced((int)States.Return);
        }
    }
}