using IA.FSM.Entities.Carriage;
using MinerSimulator.Admins;
using MinerSimulator.Utils.Pathfinder;
using MinerSimulator.Utils.Voronoi;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA.FSM.States.Carriage
{
    public class ReturnState : State
    {
        public override List<Action> GetBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.GetTransform(0);
            float speed = stateParameters.GetFloat(1);
            int food = stateParameters.GetInt(3);
            GameObject home = stateParameters.GetGameObject(4);
            List<Vector3> travelPositions = stateParameters.GetVectorList(5);
            VoronoiController voronoi = stateParameters.GetVoronoi(6);

            List<Action> behaviours = new List<Action>();

            behaviours.Add(() =>
            {
                UpdateTravelPositions(stateParameters, transform, speed, home, voronoi);

                if (CanSupply(food, voronoi) && !VillagerAdmin.Instance.Emergency)
                {
                    Transition((int)Flags.OnSupplyMode);
                }

                HandleReturnToHome(stateParameters, transform, home, voronoi);
            });

            behaviours.Add(() => Debug.Log("RETURN"));

            return behaviours;
        }

        public override List<Action> GetOnEnterBehaviours(StateParameters stateParameters)
        {
            Transform transform = stateParameters.GetTransform(0);
            GameObject home = stateParameters.GetGameObject(4);
            List<Vector3> travelPositions = stateParameters.GetVectorList(5);

            List<Action> behaviours = new List<Action>();
            behaviours.Add(() => SetInitialTravelPositions(stateParameters, transform, home));
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

        private void UpdateTravelPositions(StateParameters stateParameters, Transform transform, float speed, GameObject home, VoronoiController voronoi)
        {
            List<Vector3> travelPositions = stateParameters.GetVectorList(5);

            if (travelPositions.Count <= 0 || travelPositions == null)
            {
                travelPositions = PathFinder.FindPath(transform.position, home.transform.position, PawnType.CARRIAGE);
                stateParameters.SetVectorList(5, travelPositions);
            }

            if (travelPositions.Count > 0)
            {
                transform.position += Vector3.Normalize(travelPositions[0] - transform.position) * Time.deltaTime * speed;
            }
        }

        private bool CanSupply(float food, VoronoiController voronoi)
        {
            return food > 0 && VoronoiController.workdMines.Count > 0 && !VillagerAdmin.Instance.Emergency;
        }

        private void HandleReturnToHome(StateParameters stateParameters, Transform transform, GameObject home, VoronoiController voronoi)
        {
            int food = stateParameters.GetInt(3);
            int resourcesCollected = stateParameters.GetInt(7);
            List<Vector3> travelPositions = stateParameters.GetVectorList(5);

            if (Vector3.Distance(transform.position, home.transform.position) < 1.1f)
            {
                food = 10;
                resourcesCollected = 0;
                stateParameters.SetInt(3, food);
                stateParameters.SetInt(7, resourcesCollected);

                if (VoronoiController.workdMines.Count <= 0 || VillagerAdmin.Instance.Emergency)
                {
                    if (VillagerAdmin.Instance.Emergency)
                        transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                    return;
                }

                transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;

                if (VoronoiController.workdMines.Count > 0)
                    Transition((int)Flags.OnSupplyMode);
            }

            if (travelPositions.Count > 0 && Vector3.Distance(transform.position, travelPositions[0]) < 0.3f)
            {
                travelPositions.RemoveAt(0);
                stateParameters.SetVectorList(5, travelPositions);
            }
        }

        private void SetInitialTravelPositions(StateParameters stateParameters, Transform transform, GameObject home)
        {
            List<Vector3> travelPositions = PathFinder.FindPath(transform.position, home.transform.position, PawnType.CARRIAGE);
            stateParameters.SetVectorList(5, travelPositions);
        }
    }
}
