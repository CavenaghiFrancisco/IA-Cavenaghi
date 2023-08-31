using System;
using System.Collections.Generic;
using UnityEngine;
namespace IA.FSM.Villager
{
    public class MineState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            float speed = Convert.ToSingle(stateParameters.Parameters[1]);
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            float resources = Convert.ToSingle(stateParameters.Parameters[3]);


            List<Action> behaviours = new List<Action>();
            behaviours.Add(() =>
            {
                resources += 1 * Time.deltaTime;
                stateParameters.Parameters[3] = resources;
                Debug.Log("MINE: " + stateParameters.Parameters[3]);
                if(resources >= 5)
                    Transition((int)Flags.OnHaveEnoughResources);
            });
            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters parameters)
        {
            return new List<Action>();
        }

        public override List<Action> GetOnExitBehaviours(StateParameters parameters)
        {
            return new List<Action>();
        }

        public override void Transition(int flag)
        {
            SetFlag?.Invoke(flag);

        }
    }
}
