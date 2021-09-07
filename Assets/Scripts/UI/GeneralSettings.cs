using UnityEngine;
using UnityEngine.UI;

namespace ThePackt
{
    public class GeneralSettings : MonoBehaviour
    {
        [SerializeField] private InputField _timeInputField;
        [SerializeField] private InputField _playersInputField;
        [SerializeField] private Toggle _fogToggle;
        [SerializeField] private Toggle _damageToggle;
        private CharacterSelectionData _charSelectdata;

        private void Start()
        {
            _charSelectdata = CharacterSelectionData.Instance;

            _timeInputField.text = _charSelectdata.GetTimeDuration().ToString();
            _playersInputField.text = _charSelectdata.GetPlayersNumber().ToString();
            _fogToggle.SetIsOnWithoutNotify(_charSelectdata.GetFogEnabled());
            _damageToggle.SetIsOnWithoutNotify(_charSelectdata.GetDamageEnabled());
        }

        public void SetFogPreference()
        {
            Debug.Log("UII setting fog " + _charSelectdata.GetFogEnabled());

            _charSelectdata.SwitchFogEnabled();

            Debug.Log("UII set fog " + _charSelectdata.GetFogEnabled());
        }

        public void SetDamagePreference()
        {
            Debug.Log("UII setting damage " + _charSelectdata.GetDamageEnabled());

            _charSelectdata.SwitchDamageEnabled();

            Debug.Log("UII set damage " + _charSelectdata.GetDamageEnabled());
        }

        public void SetTimePreference()
        {
            int value;
            if (_timeInputField.text != "-" && int.TryParse(_timeInputField.text, out value))
            {
                if (value < 60)
                {
                    value = 60;
                    _timeInputField.text = "60";
                }
                else if (value > 3599)
                {
                    value = 3599;
                    _timeInputField.text = "3599";
                }
            }
            else
            {
                value = 360;
                _timeInputField.text = "360";
            }

            Debug.Log("UII setting time " + value);

            _charSelectdata.SetTimeDuration(value);
        }

        public void SetPlayersPreference()
        {
            int value;
            if (_playersInputField.text != "-" && int.TryParse(_playersInputField.text, out value))
            {
                if (value < 1)
                {
                    value = 1;
                    _playersInputField.text = "1";
                }
                else if (value > 6)
                {
                    value = 6;
                    _playersInputField.text = "6";
                }
            }
            else
            {
                value = 6;
                _playersInputField.text = "6";
            }

            Debug.Log("UII setting players " + value);

            _charSelectdata.SetPlayersNumber(value);
        }
    }
}
