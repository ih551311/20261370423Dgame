using UnityEngine;

namespace Platformer3D
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -8f);
        [SerializeField] private float followSmooth = 8f;
        [SerializeField] private float lookSmooth = 10f;

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSmooth * Time.deltaTime);

            Vector3 lookTarget = target.position + Vector3.up * 1.5f;
            Quaternion desiredRotation = Quaternion.LookRotation(lookTarget - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, lookSmooth * Time.deltaTime);
        }
    }
}