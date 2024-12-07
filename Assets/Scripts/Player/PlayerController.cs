using UnityEngine;
using UnityEngine.InputSystem;

namespace Santa
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed;
        public float runSpeed;
        public float jumpStrength = 0.5f;
        public int offGroundJumpFrames = 4;

        [Range(0, 90)] public float slopeLimit = 42f;

        [Header("Physik")]
        public bool useGravity = true;
        public float gravityFactor = 0.3f;
        public float stickToGroundForce = 10f;
        public float skinWidth = 0.01f;
        public float stepHeight = 0.2f;

        [Range(0f, 1f)] public float groundedDist = 0.03f;
        public LayerMask groundLayers = Physics.AllLayers;

        [Space]
        public Transform cameraAnker;

        #region Properties
        public bool IsGrounded { get; private set; }
        public bool OnSlope { get; private set; }
        public Vector3 Velocity { get => velocity; }
        public float Speed { get => speed; }
        #endregion

        // Input Controls
        private Controls controls;
        private new CapsuleCollider collider;
        private Rigidbody rig;

        private Vector3 moveVector = Vector3.zero;
        private Vector2 lookVector = Vector2.zero;
        private float rotationY = 0;

        private Vector3 velocity;
        private float speed;
        [System.NonSerialized] public Vector3 cameraRotation;
        private float fallStartHeight = float.MinValue;
        private int jumpFrameTimer;

        // Funktions Delegaten für den Spieler
        public System.Action onJump;
        public System.Action onUse;
        public System.Action onCast;
        public System.Action onCancel;
        public System.Action onLight;
        public System.Action<float> onLanding;

        void Awake()
        {
            controls = KeybindManager.inputActions;
            collider = GetComponent<CapsuleCollider>();
            rig = GetComponent<Rigidbody>();
            rig.useGravity = useGravity;
            rig.isKinematic = true;
            rig.freezeRotation = true;
        }

        private void FixedUpdate()
        {
            RotatePlayer();
            MoveKinematic();
        }

        private void OnEnable()
        {
            controls.Player.Enable();
            EnableMovementInputs(true);
            EnableActionInputs(true);

            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2, Screen.height / 2));
            cameraRotation = transform.rotation.eulerAngles;
            moveVector = Vector3.zero;
            velocity = Vector3.zero;
        }

        private void OnDisable()
        {
            controls.Player.Disable();
            EnableMovementInputs(false);
            EnableActionInputs(false);
        }

        public void EnableMovementInputs(bool enabled)
        {
            if (enabled && this.enabled)
            {
                controls.Player.Movement.performed += ReadMovement;
                controls.Player.Rotate.performed += ReadRotation;
                controls.Player.Jump.performed += OnJump;
            }
            else
            {
                controls.Player.Movement.performed -= ReadMovement;
                controls.Player.Rotate.performed -= ReadRotation;
                controls.Player.Jump.performed -= OnJump;
            }
        }

        public void EnableActionInputs(bool enabled)
        {
            if (enabled && this.enabled)
            {
                controls.Player.Use.performed += OnUse;
            }
            else
            {
                controls.Player.Use.performed -= OnUse;
            }
        }

        public void SetTransform(Vector3 position, float rotation)
        {
            transform.position = position;
            cameraRotation.y = rotation;
        }

        private void ReadMovement(InputAction.CallbackContext ctx)
        {
            var vec2 = ctx.ReadValue<Vector2>();
            moveVector = new Vector3(vec2.x, 0, vec2.y);
        }

        private void ReadRotation(InputAction.CallbackContext ctx)
        {
            rotationY = ctx.ReadValue<float>();
        }

        public void MoveKinematic()
        {
            if (!IsGrounded)
            {
                if (jumpFrameTimer >= 0) jumpFrameTimer -= 1;
                if (controls.Player.Jump.IsPressed() || jumpFrameTimer > 0)
                {
                    velocity.y += Physics.gravity.y * Time.fixedDeltaTime * gravityFactor * 0.5f;
                }
                else
                {
                    velocity.y += Physics.gravity.y * Time.fixedDeltaTime * gravityFactor;
                }
                if (fallStartHeight == float.MinValue && velocity.y < 0)
                {
                    fallStartHeight = transform.position.y;
                }
            }
            else
            {
                velocity.y = -stickToGroundForce;
                jumpFrameTimer = offGroundJumpFrames;
            }

            var currentVelocity = velocity;
            Vector3 applied;
            if (!controls.Player.Run.IsPressed())
            {
                applied = transform.rotation * moveVector * Time.fixedDeltaTime * runSpeed;
            }
            else
            {
                applied = transform.rotation * moveVector * Time.fixedDeltaTime * moveSpeed;
            }
            velocity = Vector3.Lerp(currentVelocity, applied, Time.fixedDeltaTime * 15f);
            velocity.y = currentVelocity.y;

            var wasGrounded = IsGrounded;

            Vector3 movement = CollideAndSlide();
            speed = new Vector2(movement.x, movement.z).magnitude;

            if (IsGrounded && !wasGrounded)
            {
                var fallDist = (fallStartHeight - transform.position.y);
                fallStartHeight = float.MinValue;
                if (fallDist > 1) onLanding?.Invoke(fallDist);
            }

            rig.MovePosition(rig.position + movement);
        }

        public void RotatePlayer()
        {
            if (!enabled) return;
            cameraRotation.y += lookVector.x * GameManager.Settings.mouseSensitivityX;
            cameraRotation.x -= lookVector.y * GameManager.Settings.mouseSensitivityY;
            cameraRotation.x = Mathf.Clamp(cameraRotation.x, -90, 90);
            cameraRotation.z = 0;

            rig.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
            lookVector = Vector2.zero;
        }

        public bool IsRunning()
        {
            return !controls.Player.Run.IsPressed() && !Mathf.Approximately(moveVector.x + moveVector.z, 0) && Mathf.Abs(speed) > 0.01;
        }

        private Vector3 CollideAndSlide()
        {
            Vector3 velHorizontal = Vec2D(velocity);
            Vector3 velVertical = new Vector3(0, velocity.y - stepHeight, 0);

            rig.position += Vector3.up * stepHeight;
            var movement = CollideAndSlide(rig.position, velHorizontal, velHorizontal.normalized, false, 0);

            IsGrounded = true;
            movement += CollideAndSlide(rig.position + movement, velVertical, Vector3.zero, true, 0);
            return movement;
        }

        private Vector3 CollideAndSlide(Vector3 pos, Vector3 vel, Vector3 origDir, bool gravityPass, int depth)
        {
            if (depth >= 3) return Vector3.zero;

            float dist = vel.magnitude + skinWidth;

            RaycastHit hit;

            float radius = collider.radius - skinWidth;
            float height = collider.height / 2f - collider.radius;
            Vector3 p1 = pos + collider.center - Vector3.up * height;
            Vector3 p2 = pos + collider.center + Vector3.up * height;
            if (Physics.CapsuleCast(p1, p2, radius, vel.normalized, out hit, dist, groundLayers, QueryTriggerInteraction.Ignore))
            {
                Vector3 snapToSurface = vel.normalized * (hit.distance - skinWidth);
                Vector3 leftOver = vel - snapToSurface;
                float angle = Vector3.Angle(hit.normal, Vector3.up);

                if (snapToSurface.magnitude <= skinWidth)
                {
                    snapToSurface = Vector3.zero;
                }

                if (angle < slopeLimit)
                {
                    // Rampe
                    if (gravityPass)
                    {
                        // Snippet für Events, die mit dem Boden zu tun haben. Einbrechen des Bodens, etc.
                        /*if (hit.collider.gameObject.TryGetComponent(out IOnStep other))
                        {
                            other.OnStep(rig.mass);
                        }*/
                        return snapToSurface;
                    }
                    leftOver = Vector3.ProjectOnPlane(leftOver, hit.normal);
                }
                else
                {
                    // Wand
                    if (IsGrounded && !gravityPass)
                    {
                        var wallNormal = Vec2D(hit.normal).normalized;
                        float scale = 1 - Vector3.Dot(wallNormal, -origDir);
                        leftOver = Vector3.ProjectOnPlane(Vec2D(leftOver), wallNormal) * scale;
                    }
                    else
                    {
                        leftOver = Vector3.ProjectOnPlane(leftOver, hit.normal);
                    }
                }

                if (depth == 0 && hit.rigidbody != null)
                {
                    hit.rigidbody.AddForceAtPosition(vel * 10, hit.point, ForceMode.Impulse);
                }

                return snapToSurface + CollideAndSlide(pos + snapToSurface, leftOver, origDir, gravityPass, depth + 1);
            }
            else if (gravityPass)
            {
                IsGrounded = false;
            }

            return vel;
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            var pressed = ctx.ReadValueAsButton();
            if (pressed && jumpFrameTimer > 0)
            {
                onJump?.Invoke();
                velocity.y = jumpStrength;
                IsGrounded = false;
                jumpFrameTimer = -1;
            }
        }

        private void OnUse(InputAction.CallbackContext ctx)
        {
            var pressed = ctx.ReadValueAsButton();
            if (pressed)
            {
                onUse.Invoke();
            }
        }

        private void OnCast(InputAction.CallbackContext ctx)
        {
            var pressed = ctx.ReadValueAsButton();
            if (pressed)
            {
                onCast?.Invoke();
            }
        }

        private void OnCancel(InputAction.CallbackContext ctx)
        {
            var pressed = ctx.ReadValueAsButton();
            if (pressed)
            {
                onCancel?.Invoke();
            }
        }

        private void OnLight(InputAction.CallbackContext ctx)
        {
            var pressed = ctx.ReadValueAsButton();
            if (pressed)
            {
                onLight?.Invoke();
            }
        }

        private Vector3 Vec2D(Vector3 vec)
        {
            return new Vector3(vec.x, 0, vec.z);
        }
    }
}