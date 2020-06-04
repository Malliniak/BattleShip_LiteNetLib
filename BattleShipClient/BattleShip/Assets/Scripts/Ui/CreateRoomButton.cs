using BattleShip.Core;
using UnityEngine;

namespace BattleShip.Ui
{
    public class CreateRoomButton : MonoBehaviour
    {
        private RuntimeManager _manager;

        private void Awake()
        {
            _manager = FindObjectOfType<RuntimeManager>();
        }

        public void CreateRoomRequest()
        {
            _manager.NetHub.CreateRoomRequest();
        }
    }
}
