using System;
using BattleShip.Core;
using TMPro;
using UnityEngine;

namespace BattleShip.Ui
{
    public class MainScreenTitle : MonoBehaviour
    {
        private RuntimeManager _manager;
        private TextMeshProUGUI _textMesh;
        
        private void Awake()
        {
            _textMesh = GetComponent<TextMeshProUGUI>();
            _manager = FindObjectOfType<RuntimeManager>();
        }

        private void Start()
        {
            _manager.Player.PlayerNameChanged += UpdateTitle;
        }

        private void UpdateTitle()
        {
            _textMesh.text = $"Hello, {_manager.Player.Name}";
        }
    }
}
