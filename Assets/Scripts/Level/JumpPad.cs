using UnityEngine;

namespace Platformer3D
{
    [RequireComponent(typeof(Collider))]
    public class JumpPad : MonoBehaviour
    {
        [SerializeField] private float jumpBoostForce = 14f;

        private void Reset()
        {
            Collider collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out PlayerController player))
            {
                return;
            }

            player.ApplyJumpBoost(jumpBoostForce);
        }
    }
}