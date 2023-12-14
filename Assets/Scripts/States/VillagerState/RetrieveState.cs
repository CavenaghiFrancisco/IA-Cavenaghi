using IA.FSM.Entities.Villager;
using MinerSimulator.Admins;
using MinerSimulator.Map;
using MinerSimulator.Utils.Pathfinder;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.States.Villager
{
    public class RetrieveState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.GetTransform(0);
            float speed = stateParameters.GetFloat(1);
            GameObject home = stateParameters.GetGameObject(4);
            float resources = stateParameters.GetFloat(3);
            List<Vector3> travelPositions = stateParameters.GetVectorList(5);

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                if (VillagerAdmin.Instance.Emergency)
                {
                    HandleEmergencyBehaviour(stateParameters, transform, resources, home, travelPositions);
                }
                else
                {
                    HandleNormalBehaviour(stateParameters, transform, speed, resources, home, travelPositions);
                }
            });

            behaviours.Add(() => Debug.Log("RETRIEVE"));

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters stateParameters)
        {
            GameObject home = stateParameters.GetGameObject(4);
            Transform transform = stateParameters.GetTransform(0);
            stateParameters.SetVectorList(5,PathFinder.FindPath(transform.position, home.transform.position, PawnType.VILLAGER));

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

        private void HandleEmergencyBehaviour(StateParameters stateParameters, Transform transform, float resources, GameObject home, List<Vector3> travelPositions)
        {
            if (Vector3.Distance(transform.position, home.transform.position) < 1.1f)
            {
                transform.GetComponent<MeshRenderer>().enabled = false;
                resources = 0;
                stateParameters.SetFloat(3, resources);
                return;
            }

            if (Vector3.Distance(transform.position, home.transform.position) > 1.1f)
            {
                travelPositions = stateParameters.GetVectorList(5);
                if (travelPositions.Count > 0)
                    transform.position += Vector3.Normalize(travelPositions[0] - transform.position) * Time.deltaTime * stateParameters.GetFloat(1);
            }

            if (travelPositions.Count > 0 && Vector3.Distance(transform.position, travelPositions[0]) < 0.3f)
            {
                travelPositions.RemoveAt(0);
                stateParameters.SetVectorList(5, travelPositions);
            }
        }

        private void HandleNormalBehaviour(StateParameters stateParameters, Transform transform, float speed, float resources, GameObject home, List<Vector3> travelPositions)
        {
            transform.GetComponent<MeshRenderer>().enabled = true;
            travelPositions = stateParameters.GetVectorList(5);

            if (Vector3.Distance(transform.position, home.transform.position) < 1.1f)
            {
                if (MapGenerator.Instance.MinesAvailable.Count <= 0)
                    return;
                resources = 0;
                stateParameters.SetFloat(3, resources);
                Transition((int)Flags.OnSeeTarget);
            }

            if (travelPositions.Count > 0 && Vector3.Distance(transform.position, travelPositions[0]) < 0.3f)
            {
                travelPositions.RemoveAt(0);
                stateParameters.SetVectorList(5, travelPositions);
            }

            if (Vector3.Distance(transform.position, home.transform.position) > 1.1f)
            {
                if (resources < 15)
                {
                    Transition((int)Flags.OnSeeTarget);
                }

                if (travelPositions.Count > 0)
                    transform.position += Vector3.Normalize(travelPositions[0] - transform.position) * Time.deltaTime * speed;
            }
        }
    }
}
