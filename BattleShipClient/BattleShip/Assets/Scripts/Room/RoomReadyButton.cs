using BattleShip.Core;
using UnityEngine;
using UnityEngine.UI;

namespace BattleShip.Room
{
    public class RoomReadyButton : MonoBehaviour
    {
        private Button _button;
        private RuntimeManager _runtimeManager;

        private void Awake()
        {
            _runtimeManager = FindObjectOfType<RuntimeManager>();
            _button = GetComponent<Button>();

            _button.onClick.AddListener(() => _runtimeManager.NetHub.PlayerRoomStateRequest(_runtimeManager.Player.Id,
                _runtimeManager.Room.RoomId, !_runtimeManager.Player.IsReady));
        }
    }
}
