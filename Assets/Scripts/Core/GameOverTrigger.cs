using UnityEngine;

namespace DropStack.Core
{
    public class GameOverTrigger : MonoBehaviour
    {
        private GameManager gameManager;
        public bool HasPieceInZone { get; private set; }

        public void Initialize(GameManager manager)
        {
            gameManager = manager;
        }

        public void ClearZone()
        {
            HasPieceInZone = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<GamePiece>() == null)
            {
                return;
            }
            HasPieceInZone = true;
            gameManager.OnPieceEnteredGameOverZone();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<GamePiece>() == null)
            {
                return;
            }
            HasPieceInZone = false;
        }
    }
}
