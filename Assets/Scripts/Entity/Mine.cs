using MinerSimulator.Admins;
using MinerSimulator.Map;
using MinerSimulator.Utils.Voronoi;
using System;
using TMPro;
using UnityEngine;

namespace MinerSimulator.Entity
{
    public class Mine : MonoBehaviour
    {

        [SerializeField] private int amount = 20;
        [SerializeField] private int amountFood = 20;
        [SerializeField] private TMP_Text amountText = null;

        private bool worked = false;

        public static Action<bool,bool> OnMineDestroy;

        private bool isEmpty = false;

        public bool IsEmpty { get => isEmpty; }
        public bool Worked { get => worked; }

        public int Take(int substract)
        {
            worked = true;
            if (!isEmpty)
            {
                int money = amount - substract;

                if (money <= 0)
                {
                    substract += money;
                    SetEmpty();
                }

                SetAmount(amount - substract);

                return substract;
            }

            return 0;
        }

        public bool CanTakeFood()
        {
            if (amountFood > 0)
            {
                amountFood--;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SuplyFood(int foodSuply)
        {
            amountFood += foodSuply;
        }

        private void SetEmpty()
        {
            isEmpty = true;
            MapGenerator.Instance.MinesAvailable.Remove(this);
            Destroy(gameObject);
            OnMineDestroy?.Invoke(MapGenerator.Instance.MinesAvailable.Count > 0,VoronoiController.workdMines.Count > 0);
        }

        private void SetAmount(int amount)
        {
            if (amount < 0)
            {
                amount = 0;
            }
            this.amount = amount;
        }
    }

}
