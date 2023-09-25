using MinerSimulator.Utils.Pathfinder;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.Carriage
{
    public class SuplyState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            float speed = Convert.ToSingle(stateParameters.Parameters[1]);
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            List<Vector3> travelPositions = stateParameters.Parameters[3] as List<Vector3>;
            int food = Convert.ToInt32(stateParameters.Parameters[4]);

            List<Action> behabiours = new List<Action>();

            behabiours.Add(() =>
            {
                if (travelPositions == null)
                {
                    stateParameters.Parameters[3] = PathFinder.FindPath(transform.position, Target.transform.position, PawnType.CARRIAGE);
                    travelPositions = stateParameters.Parameters[3] as List<Vector3>;
                }
                transform.position += Vector3.Normalize(travelPositions[0] - transform.position) * Time.deltaTime * speed;

                if (Vector3.Distance(transform.position, Target.transform.position) < 1.1f)
                {
                    Target.GetComponent<Mine>().SuplyFood(food);
                    stateParameters.Parameters[4] = 0;
                    Transition((int)Flags.OnNeedToReturnHome);
                }
                if (Vector3.Distance(transform.position, travelPositions[0]) < 0.1f)
                {
                    travelPositions.RemoveAt(0);
                    stateParameters.Parameters[3] = travelPositions;
                }

            });

            behabiours.Add(() => Debug.Log("SUPLY"));

            return behabiours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.Parameters[0] as Transform;
            GameObject Target = stateParameters.Parameters[2] as GameObject;
            List<Action> behaviours = new List<Action>();
            behaviours.Add(() =>
            {
                stateParameters.Parameters[3] = PathFinder.FindPath(transform.position, Target.transform.position, PawnType.CARRIAGE);
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
