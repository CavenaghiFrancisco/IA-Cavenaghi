using IA.FSM.States;
using IA.FSM.States.Carriage;
using MinerSimulator.Admins;
using MinerSimulator.Entity;
using MinerSimulator.Utils.Voronoi;
using System;

namespace IA.FSM.Entities.Carriage
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

namespace IA.FSM.Entities.Carriage
{
    public class Carriage : Entity
    {
        private float food = 0;

        protected override void Start()
        {
            Mine.OnMineDestroy += (bool areMines, bool areWorkedMines) =>
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

        private void OnDestroy()
        {
            VillagerAdmin.OnEmergencyCalled -= () => fsm.SetCurrentStateForced((int)States.Return);
        }
    }
}