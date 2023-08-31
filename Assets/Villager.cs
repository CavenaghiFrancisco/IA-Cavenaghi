using System;
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

        StateParameters collectParameters;
        StateParameters mineAndRetrieveParameters;

        private void Start()
        {
            collectParameters = new StateParameters();
            mineAndRetrieveParameters = new StateParameters();

            fsm = new FSM(Enum.GetValues(typeof(States)).Length, Enum.GetValues(typeof(Flags)).Length);

            fsm.SetRelation((int)States.Collect, (int)Flags.OnNearTarget, (int)States.Mine);

            fsm.SetRelation((int)States.Mine, (int)Flags.OnHaveEnoughResources, (int)States.Retrieve);

            fsm.SetRelation((int)States.Retrieve, (int)Flags.OnSeeTarget, (int)States.Collect);

            collectParameters.Parameters = new object[3] { gameObject.transform, speed, Target };
            fsm.AddState<CollectState>((int)States.Collect,
                collectParameters);

            mineAndRetrieveParameters.Parameters = new object[5] { gameObject.transform, speed, Target, resourcesCollected, Home };
            fsm.AddState<MineState>((int)States.Mine,
                mineAndRetrieveParameters);

            fsm.AddState<RetrieveState>((int)States.Retrieve,
                mineAndRetrieveParameters);

            fsm.SetCurrentStateForced((int)States.Collect);
        }

        public void Update()
        {
            fsm.Update();
        }
    }
}