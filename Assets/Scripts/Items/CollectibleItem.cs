using UnityEngine;

namespace Platformer3D
{
    [RequireComponent(typeof(Collider))]
    public class CollectibleItem : MonoBehaviour
    {
        [SerializeField] private int scoreValue = 10;
        [SerializeField] private float rotateSpeed = 90f;
        [SerializeField] private float bobHeight = 0.25f;
        [SerializeField] private float bobSpeed = 2f;

        private Vector3 startPosition;

        private void Reset()
        {
            Collider collider = GetComponent<Collider>();
            collider.isTrigger = true;
        }

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);
            float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = startPosition + Vector3.up * bobOffset;
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

            GameManager.Instance.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}