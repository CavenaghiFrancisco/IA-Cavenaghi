using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.Villager
{
    public class RetrieveState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            float speed = Convert.ToSingle(stateParameters.Parameters[1]);
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            float resources = Convert.ToSingle(stateParameters.Parameters[3]);
            GameObject home = stateParameters.Parameters[4] as GameObject;


            List<Action> behabiours = new List<Action>();

            behabiours.Add(() =>
            {
                transform.position += (home.transform.position - transform.position).normalized * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, home.transform.position) < 1.0f)
                {
                    resources = 0;
                    stateParameters.Parameters[3] = resources;
                    Transition((int)Flags.OnSeeTarget);
                }
            }
            );

            behabiours.Add(() => Debug.Log("RETRIEVE"));

            return behabiours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters stateParameters)
        {
            return new List<Action>();

        }

        public override List<Action> GetOnExitBehaviours(StateParameters stateParameters)
        {
            return new List<Action>();

        }

        public override void Transition(int flag)
        {
            SetFlag?.Invoke(flag);

        }
    }
}
