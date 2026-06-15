using UnityEngine;
using UnityEngine.InputSystem;

namespace Platformer3D
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private float jumpForce = 8f;
        [SerializeField] private float gravity = -20f;
        [SerializeField] private Transform cameraTransform;

        [Header("Ground Check")]
        [SerializeField] private float groundCheckRadiusScale = 0.9f;
        [SerializeField] private float groundCheckExtraDistance = 0.15f;
        [SerializeField] private LayerMask groundMask = ~0;

        [Header("Fall Detection")]
        [SerializeField] private float fallDeathY = -10f;

        private CharacterController controller;
        private Vector3 velocity;
        private bool jumpRequested;
        private bool controlsEnabled = true;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            controller.minMoveDistance = 0f;
            controller.skinWidth = 0.08f;

            if (TryGetComponent<CapsuleCollider>(out CapsuleCollider capsuleCollider))
            {
                capsuleCollider.enabled = false;
            }

            AlignVisualMeshToController();
        }

        private void OnEnable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
            }
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
            }

            DetachFromMovingPlatform();
        }

        private void Start()
        {
            if (cameraTransform == null && Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }

            if (GameManager.Instance != null)
            {
                HandleGameStateChanged(GameManager.Instance.CurrentState);
            }
        }

        private void Update()
        {
            if (!CanControl())
            {
                return;
            }

            if (ReadJumpPressed())
            {
                jumpRequested = true;
            }

            CheckFallDeath();
        }

        private void FixedUpdate()
        {
            if (!CanControl())
            {
                return;
            }

            HandleMovement();
        }

        private bool CanControl()
        {
            if (!controlsEnabled)
            {
                return false;
            }

            return GameManager.Instance == null || GameManager.Instance.IsPlaying;
        }

        private void HandleMovement()
        {
            bool isGrounded = CheckGrounded();

            if (isGrounded)
            {
                if (velocity.y < 0f)
                {
                    velocity.y = -2f;
                }

                if (jumpRequested)
                {
                    DetachFromMovingPlatform();
                    velocity.y = jumpForce;
                }
            }
            else
            {
                DetachFromMovingPlatform();
            }

            jumpRequested = false;

            Vector2 moveInput = ReadMoveInput();
            Vector3 moveDirection = GetCameraRelativeDirection(moveInput);

            velocity.y += gravity * Time.fixedDeltaTime;

            Vector3 motion = moveDirection * moveSpeed;
            motion.y = velocity.y;
            controller.Move(motion * Time.fixedDeltaTime);
        }

        private bool CheckGrounded()
        {
            if (controller.isGrounded)
            {
                return true;
            }

            Vector3 sphereCenter = GetFeetSphereCenter();
            float radius = controller.radius * groundCheckRadiusScale;
            float checkDistance = groundCheckExtraDistance + controller.skinWidth;

            return Physics.SphereCast(
                sphereCenter,
                radius,
                Vector3.down,
                out _,
                checkDistance,
                groundMask,
                QueryTriggerInteraction.Ignore);
        }

        private Vector3 GetFeetSphereCenter()
        {
            float bottomY = transform.position.y + controller.center.y - (controller.height * 0.5f);
            return new Vector3(
                transform.position.x + controller.center.x,
                bottomY + controller.radius,
                transform.position.z + controller.center.z);
        }

        private void AlignVisualMeshToController()
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshRenderer == null || meshFilter == null)
            {
                return;
            }

            GameObject visual = new GameObject("PlayerVisual");
            visual.transform.SetParent(transform, false);
            visual.transform.localPosition = controller.center;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;

            MeshFilter visualFilter = visual.AddComponent<MeshFilter>();
            visualFilter.sharedMesh = meshFilter.sharedMesh;

            MeshRenderer visualRenderer = visual.AddComponent<MeshRenderer>();
            visualRenderer.sharedMaterials = meshRenderer.sharedMaterials;

            Destroy(meshFilter);
            Destroy(meshRenderer);
        }

        private Vector2 ReadMoveInput()
        {
            Vector2 input = Vector2.zero;
            Keyboard keyboard = Keyboard.current;

            if (keyboard != null)
            {
                if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                {
                    input.y += 1f;
                }

                if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                {
                    input.y -= 1f;
                }

                if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                {
                    input.x -= 1f;
                }

                if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                {
                    input.x += 1f;
                }
            }
            else
            {
                input.x = Input.GetAxisRaw("Horizontal");
                input.y = Input.GetAxisRaw("Vertical");
            }

            return input.sqrMagnitude > 1f ? input.normalized : input;
        }

        private bool ReadJumpPressed()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null)
            {
                return keyboard.spaceKey.wasPressedThisFrame;
            }

            return Input.GetButtonDown("Jump");
        }

        private Vector3 GetCameraRelativeDirection(Vector2 input)
        {
            if (input.sqrMagnitude <= 0f)
            {
                return Vector3.zero;
            }

            if (cameraTransform == null)
            {
                return new Vector3(input.x, 0f, input.y).normalized;
            }

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            return (forward * input.y + right * input.x).normalized;
        }

        private void CheckFallDeath()
        {
            if (GameManager.Instance == null)
            {
                return;
            }

            if (transform.position.y < fallDeathY)
            {
                GameManager.Instance.TriggerGameOver(GameOverReason.Fall);
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y < 0.6f)
            {
                return;
            }

            if (hit.collider.TryGetComponent(out MovingPlatform _))
            {
                AttachToMovingPlatform(hit.collider.transform);
            }
        }

        private void AttachToMovingPlatform(Transform platformTransform)
        {
            if (transform.parent == platformTransform)
            {
                return;
            }

            transform.SetParent(platformTransform, true);
        }

        private void DetachFromMovingPlatform()
        {
            if (transform.parent == null)
            {
                return;
            }

            if (transform.parent.GetComponent<MovingPlatform>() != null)
            {
                transform.SetParent(null, true);
            }
        }

        private void HandleGameStateChanged(GameState state)
        {
            controlsEnabled = state == GameState.Playing;
        }

        public void ApplyJumpBoost(float boostForce)
        {
            DetachFromMovingPlatform();
            velocity.y = boostForce;
            jumpRequested = false;
        }

        public void ClearPlatformReference()
        {
            DetachFromMovingPlatform();
        }

        private void OnDrawGizmosSelected()
        {
            if (controller == null)
            {
                controller = GetComponent<CharacterController>();
            }

            if (controller == null)
            {
                return;
            }

            Gizmos.color = Color.green;
            Vector3 center = GetFeetSphereCenter();
            float radius = controller.radius * groundCheckRadiusScale;
            Gizmos.DrawWireSphere(center, radius);
            Gizmos.DrawLine(center, center + Vector3.down * (groundCheckExtraDistance + controller.skinWidth));
        }
    }
}