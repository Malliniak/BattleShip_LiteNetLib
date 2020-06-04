using System;
using BattleShip.Core;
using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BattleShip.Ui
{
    public class MainScreenInput : MonoBehaviour
    {
        private TMP_InputField _input;
        private RuntimeManager _manager;
        private Button _button;

        private void Awake()
        {
            _manager = FindObjectOfType<RuntimeManager>();
            _input = GetComponentInChildren<TMP_InputField>();
            _button = GetComponentInChildren<Button>();
        }

        private void Start()
        {
            _button.interactable = false;

            _input.onEndEdit.AddListener(delegate
            {
                Debug.Log(_input.text);
                if (_input.text == "") return;
                _button.interactable = true;
            });
        }

        public void ChangePlayerName()
        {
            _manager.NetHub.NameChangeRequest(_input.text);
            Reset();
        }

        public void JoinRoom()
        {
            _manager.NetHub.JoinRoomRequest(_input.text);
            Reset();
        }
        
        private void Reset()
        {
            _button.interactable = false;
            _input.text = "";
        }
    }
}
