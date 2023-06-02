using Code.Scripts.FireSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.FireSystem
{
    public class UI_controller_fire_system : MonoBehaviour
    {
        [SerializeField] private FireSystem fs;
        [SerializeField] private Button resetButton;
        [SerializeField] private Slider windSlider;
        [SerializeField] private TMP_InputField wind_x;
        [SerializeField] private TMP_InputField wind_z;

        // Start is called before the first frame update
        void Start()
        {
            resetButton.onClick.AddListener(() => fs.ResetFires());
            windSlider.onValueChanged.AddListener((f) => fs.windDamageMultiplier = f);
            wind_x.onValueChanged.AddListener((s) => fs.windDirection.x = float.Parse(s));
            wind_z.onValueChanged.AddListener((s) => fs.windDirection.z = float.Parse(s));
        }
    }
}
