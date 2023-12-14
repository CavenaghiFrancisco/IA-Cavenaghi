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
        OnSupplyMode
    }

    public class Carriage : Entity
    {
        private int food = 0;

        protected override void Start()
        {
            InitializeFSM();
            SubscribeToEvents();
            SetInitialState();
        }

        private void InitializeFSM()
        {
            voronoiCalculator = GetComponent<VoronoiController>();
            allParameters = new StateParameters();

            fsm = new FSM(Enum.GetValues(typeof(States)).Length, Enum.GetValues(typeof(Flags)).Length);
            ConfigureFSMRelations();
            ConfigureFSMStates();
        }

        private void ConfigureFSMRelations()
        {
            fsm.SetRelation((int)States.Return, (int)Flags.OnSupplyMode, (int)States.Suply);
            fsm.SetRelation((int)States.Suply, (int)Flags.OnNeedToReturnHome, (int)States.Return);
        }

        private void ConfigureFSMStates()
        {
            allParameters.Parameters = new object[7] { gameObject.transform, Speed, Target, food, Home, travelPositions, voronoiCalculator };

            fsm.AddState<SuplyState>((int)States.Suply, allParameters, allParameters);
            fsm.AddState<ReturnState>((int)States.Return, allParameters, allParameters);
        }

        private void SetInitialState()
        {
            fsm.SetCurrentStateForced((int)States.Suply);
        }

        protected override int GetReturnState()
        {
            return (int)States.Return;
        }
    }
}
