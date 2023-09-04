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
            List<Vector3> travelPositions = stateParameters.Parameters[5] as List<Vector3>;


            List<Action> behabiours = new List<Action>();

            behabiours.Add(() =>
            {
                transform.position += Vector3.Normalize(travelPositions[0] - transform.position) * Time.deltaTime * speed;

                if (Vector3.Distance(transform.position, home.transform.position) < 1.1f)
                {
                    resources = 0;
                    stateParameters.Parameters[3] = resources;
                    Transition((int)Flags.OnSeeTarget);
                }
                if (Vector3.Distance(transform.position, travelPositions[0]) < 0.1f)
                {
                    travelPositions.RemoveAt(0);
                    stateParameters.Parameters[5] = travelPositions;
                }
            }
            );

            behabiours.Add(() => Debug.Log("RETRIEVE"));

            return behabiours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters stateParameters)
        {
            GameObject home = stateParameters.Parameters[4] as GameObject;
            Transform transform = stateParameters.Parameters[0] as Transform;
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            List<Action> behaviours = new List<Action>();
            behaviours.Add(() =>
            {
                stateParameters.Parameters[5] = PathFinder.FindPath(transform.position, home.transform.position);
            });
            return behaviours;
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
