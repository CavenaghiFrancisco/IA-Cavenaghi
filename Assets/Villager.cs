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
        OnHaveEnoughResources
    }
}

namespace IA.FSM.Villager
{
    public class Villager : MonoBehaviour
    {
        public GameObject Target;
        public GameObject Home;

        private float speed = 5;

        public float resourcesCollected = 0;

        private FSM fsm;

        private List<Vector3> travelPositions;

        StateParameters collectParameters;
        StateParameters mineAndRetrieveParameters;

        private void Awake()
        {
            collectParameters = new StateParameters();
            mineAndRetrieveParameters = new StateParameters();

            fsm = new FSM(Enum.GetValues(typeof(States)).Length, Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)States.Collect, (int)Flags.OnNearTarget, (int)States.Mine);

            fsm.SetRelation((int)States.Mine, (int)Flags.OnHaveEnoughResources, (int)States.Retrieve);

            fsm.SetRelation((int)States.Retrieve, (int)Flags.OnSeeTarget, (int)States.Collect);

            collectParameters.Parameters = new object[4] { gameObject.transform, speed, Target, travelPositions };
            fsm.AddState<CollectState>((int)States.Collect,
                collectParameters,collectParameters);

            mineAndRetrieveParameters.Parameters = new object[6] { gameObject.transform, speed, Target, resourcesCollected, Home, travelPositions };
            fsm.AddState<MineState>((int)States.Mine,
                mineAndRetrieveParameters,mineAndRetrieveParameters);

            fsm.AddState<RetrieveState>((int)States.Retrieve,
                mineAndRetrieveParameters, mineAndRetrieveParameters);

            fsm.SetCurrentStateForced((int)States.Collect);
        }

        public void Update()
        {
            fsm.Update();
        }
    }
}