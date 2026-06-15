using UnityEngine;

namespace Platformer3D
{
    [RequireComponent(typeof(Collider))]
    public class DeathZone : MonoBehaviour
    {
        private void Reset()
        {
            Collider collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                return;
            }

            if (GameManager.Instance == null || !GameManager.Instance.IsPlaying)
            {
                return;
            }

            GameManager.Instance.TriggerGameOver(GameOverReason.Fall);
        }
    }
}