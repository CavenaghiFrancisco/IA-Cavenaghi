using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MinerSimulator.Map
{
    public class Personalizator : MonoBehaviour
    {
        public TMP_InputField sizeXField;
        public TMP_InputField sizeYField;
        public TMP_InputField betweenSpaceField;
        public TMP_InputField minesField;
        public Button bttn;

        public MapGenerator map;

        private int sizeX;
        private int sizeY;
        private int betweenSpace;
        private int mines;

        private void Start()
        {
            bttn.onClick.AddListener(() =>
            {
                map.CreateMap(sizeX, sizeY, betweenSpace, mines);
                gameObject.SetActive(false);
            });
        }

        private void Update()
        {
            Check();
        }

        public void Check()
        {
            if (sizeXField != null && sizeYField != null && betweenSpaceField != null && minesField != null)
            {
                if (int.TryParse(sizeXField.text, out sizeX) &&
                    int.TryParse(sizeYField.text, out sizeY) &&
                    int.TryParse(betweenSpaceField.text, out betweenSpace) &&
                    int.TryParse(minesField.text, out mines))
                {
                    bttn.interactable = true;
                }
                else
                {
                    bttn.interactable = false;
                }
            }
        }
    }
}
