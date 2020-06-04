using UnityEngine;
using UnityEngine.UI;

namespace BattleShip.Game
{
    public class ClickablePointRenderer : MonoBehaviour
    {
        public Vector2 Position { get; set; }
        public Image Image { get; set; }
        public Button Button { get; set; }

        public ClickableGameBoard ClickableGameBoard { get; set; }

        private void Awake()
        {
            Button = GetComponent<Button>();
            Image = GetComponent<Image>();
        }

        private void Start()
        {
            Button.onClick.AddListener(SendHit);
        }

        private void SendHit()
        {
            ClickableGameBoard.SendHit(Position);
        }

        public void SetInteractable(bool value)
        {
            Button.interactable = value;
        }
    }
}
