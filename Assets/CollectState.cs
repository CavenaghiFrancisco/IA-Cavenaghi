using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.Villager
{
    public class CollectState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            float speed = Convert.ToSingle(stateParameters.Parameters[1]);
            GameObject Target = stateParameters.Parameters[2] as GameObject;

            List<Action> behabiours = new List<Action>();

            behabiours.Add(() =>
            {
                transform.position += Vector3.Normalize(Target.transform.position - transform.position) * Time.deltaTime * speed;

                if (Vector3.Distance(transform.position, Target.transform.position) < 2.0f)
                {
                    Transition((int)Flags.OnNearTarget);
                }
            });

            behabiours.Add(() => Debug.Log("COLLECT"));

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
