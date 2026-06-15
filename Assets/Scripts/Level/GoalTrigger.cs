using UnityEngine;

namespace Platformer3D
{
    [RequireComponent(typeof(Collider))]
    public class GoalTrigger : MonoBehaviour
    {
        [SerializeField] private int bonusScore = 100;

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

            if (bonusScore > 0)
            {
                GameManager.Instance.AddScore(bonusScore);
            }

            GameManager.Instance.TriggerVictory();
        }
    }
}