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
        public GameObject Target;
        public GameObject Home;

        private float speed = 7;

        public float food = 0;

        private FSM fsm;

        private List<Vector3> travelPositions;

        StateParameters returnParameters;
        StateParameters suplyParameters;

        private void Awake()
        {
            returnParameters = new StateParameters();
            suplyParameters = new StateParameters();

            fsm = new FSM(Enum.GetValues(typeof(States)).Length, Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)States.Return, (int)Flags.OnSuplyMode, (int)States.Suply);

            fsm.SetRelation((int)States.Suply, (int)Flags.OnNeedToReturnHome, (int)States.Return);

            returnParameters.Parameters = new object[5] { gameObject.transform, speed, Target, travelPositions, food };
            fsm.AddState<SuplyState>((int)States.Suply,
                suplyParameters, suplyParameters);

            suplyParameters.Parameters = new object[6] { gameObject.transform, speed, Target, food, Home, travelPositions };
            fsm.AddState<ReturnState>((int)States.Return,
                returnParameters, returnParameters);

            fsm.SetCurrentStateForced((int)States.Suply);
        }

        public void Update()
        {
            fsm.Update();
        }
    }
}