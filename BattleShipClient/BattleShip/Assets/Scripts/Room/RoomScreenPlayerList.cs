using System.Collections.Generic;
using BattleShip.Core;
using BattleShip.Core.Players;
using TMPro;
using UnityEngine;

namespace BattleShip.Room
{
    public class RoomScreenPlayerList : MonoBehaviour
    {
        private RuntimeManager _manager;
        [SerializeField]
        private List<TextMeshProUGUI> _textMeshs = new List<TextMeshProUGUI>();

        private void Awake()
        {
            _manager = FindObjectOfType<RuntimeManager>();
        }

        private void Start()
        {
            if (_manager != null)
            {
                _manager.Room.RoomUpdated += RoomDataUpdate;
                _manager.Room.RoomReadyUpdated += RoomReadyUpdate;
            }
            RoomDataUpdate();
        }

        private void RoomReadyUpdate(Player obj)
        {
            for (int i = 0; i < _manager.Room.Players.Count; i++)
            {
                if(_manager.Room.Players[i].Id != obj.Id) continue;
                if(obj.IsReady) _textMeshs[i].color = Color.green;
                if(!obj.IsReady) _textMeshs[i].color = Color.white;
            }
        }

        private void RoomDataUpdate()
        {
            Debug.Log($"{_textMeshs.Count} <- textmesh count");
            Debug.Log($"{_manager.Room.Players.Count} <- players count");
            for (int i = 0; i < _manager.Room.Players.Count; i++)
            {
                _textMeshs[i].text = _manager.Room.Players[i].Name;
            }
        }
    }
}
