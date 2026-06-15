using UnityEngine;

namespace Platformer3D
{
    [DefaultExecutionOrder(-100)]
    public class MovingPlatform : MonoBehaviour
    {
        public enum MoveMode
        {
            PingPong,
            Loop
        }

        [SerializeField] private Transform pointA;
        [SerializeField] private Transform pointB;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private MoveMode moveMode = MoveMode.PingPong;
        [SerializeField] private bool startAtPointB;

        private float progress;
        private int direction = 1;

        private void Start()
        {
            if (pointA == null || pointB == null)
            {
                Debug.LogWarning($"{name}: MovingPlatform requires Point A and Point B.", this);
                return;
            }

            progress = startAtPointB ? 1f : 0f;
            transform.position = Vector3.Lerp(pointA.position, pointB.position, progress);
        }

        private void FixedUpdate()
        {
            if (pointA == null || pointB == null)
            {
                return;
            }

            float distance = Vector3.Distance(pointA.position, pointB.position);
            if (distance > 0.001f)
            {
                progress += (moveSpeed * direction * Time.fixedDeltaTime) / distance;
            }

            if (moveMode == MoveMode.PingPong)
            {
                if (progress >= 1f)
                {
                    progress = 1f;
                    direction = -1;
                }
                else if (progress <= 0f)
                {
                    progress = 0f;
                    direction = 1;
                }
            }
            else if (progress >= 1f)
            {
                progress = 0f;
            }

            transform.position = Vector3.Lerp(pointA.position, pointB.position, progress);
        }

        private void OnDrawGizmosSelected()
        {
            if (pointA == null || pointB == null)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawSphere(pointA.position, 0.2f);
            Gizmos.DrawSphere(pointB.position, 0.2f);
        }
    }
}