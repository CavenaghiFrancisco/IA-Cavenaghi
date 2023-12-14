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

        [SerializeField] private int amount = 27;
        [SerializeField] private int amountFood = 27;
        [SerializeField] private TMP_Text amountText = null;
        

        private bool worked = false;

        public static Action<bool, bool> OnMineDestroy;

        private int mined = 0;
        private bool isEmpty = false;

        public bool IsEmpty { get => isEmpty; }
        public bool Worked { get => worked; }
        public int Mined { get => mined; set => mined = value; }

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

                mined += substract;
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
            return false;
        }

        public bool CanContinue()
        {
            return mined < 15;
        }

        public void SuplyFood(int foodSuply)
        {
            amountFood += foodSuply;
        }

        public int GetMinedResources()
        {
            int aux = mined;
            mined = 0;
            return aux;
        }

        private void SetEmpty()
        {
            isEmpty = true;
            MapGenerator.Instance.MinesAvailable.Remove(this);
            Destroy(gameObject);
            OnMineDestroy?.Invoke(MapGenerator.Instance.MinesAvailable.Count > 0, VoronoiController.workdMines.Count > 0);
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
