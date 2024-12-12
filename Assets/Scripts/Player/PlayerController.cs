using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Santa
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        public float moveSpeed;
        public float jumpStrength = 0.5f;
        public float jumpMaxPressTime;
        public float jumpStrengthDecay = 0.1f;
        public float offGroundJumpDelay = 0.05f;
        [Range(0, 90)] public float slopeLimit = 42f;

        public GameObject playerCam;
        public float maxCameraAngle = 75f;
        public float minCameraAngle = -75f;

        [Header("Physik")]
        public bool useGravity = true;
        public float gravityFactor = 0.3f;
        public float stickToGroundForce = 2f;
        public float skinWidth = 0.01f;
        public float stepHeight = 0.2f;

        [Header("Dash")]
        public float dashStrength;
        public float dashLength;
        [NonSerialized] public bool canDash;

        [Range(0f, 1f)] public float groundedDist = 0.03f;
        public LayerMask groundLayers = Physics.AllLayers;

        #region Properties
        public bool IsGrounded { get; private set; }
        public bool OnSlope { get; private set; }
        public Vector3 Velocity { get => velocity; }
        public float Speed { get => speed; }
        #endregion

        // Input Controls
        [NonSerialized] public Controls controls;
        [NonSerialized] public CapsuleCollider playerCollider;
        [NonSerialized] public Rigidbody rig;

        [NonSerialized] public Vector3 moveVector = Vector3.zero;
        [NonSerialized] public Vector2 lookVector = Vector2.zero;
        private float rotationY = 0;

        [NonSerialized] public Vector3 velocity;
        [NonSerialized] public float speed;
        [NonSerialized] public Vector3 cameraRotation;
        [NonSerialized] public float fallStartHeight = float.MinValue;
        //[NonSerialized] public float jumpFrameTimer;
        [NonSerialized] public float maxFallSpeed = -2f;
        [NonSerialized] public float jumpPressTime;
        [NonSerialized] public float groundToAirTimer;
        public bool performNormalJump;
        public bool canDoubleJump;
        public bool performDoubleJump;
        [NonSerialized] public float dashTimer;

        //Platform
        public bool isOnPlatform;
        [NonSerialized] public Platform movingPlatform;

        // Funktions Delegaten für den Spieler
        public System.Action onJump;
        public System.Action onUse;
        public System.Action<float> onLanding;

        private bool toogleAbilities;

        private PlayerMovement playerMovement = new PlayerMovement();
        [NonSerialized] public PlayerCollision playerCollision = new PlayerCollision();

        public States state;
        public enum States
        {
            GroundState,
            GroundToAir,
            AirState,
            DashState,
            Empty,
        }
        void Awake()
        {
            controls = Keybindinputmanager.inputActions;
            playerCollider = GetComponent<CapsuleCollider>();
            rig = GetComponent<Rigidbody>();
            rig.useGravity = useGravity;
            rig.isKinematic = true;
            rig.freezeRotation = true;

            playerMovement.player = this;
            playerCollision.player = this;
        }


        private void OnEnable()
        {
            controls.Player.Enable();
            EnableMovementInputs(true);
            EnableActionInputs(true);

            Mouse.current.WarpCursorPosition(new Vector2(Screen.width / 2, Screen.height / 2));
            cameraRotation.y = transform.rotation.eulerAngles.y;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            controls.Player.Disable();
            EnableMovementInputs(false);
            EnableActionInputs(false);
        }
        private void FixedUpdate()
        {
            switch (state)
            {
                case States.GroundState:
                    playerMovement.RotatePlayer();
                    playerMovement.GroundMovement();        
                    playerMovement.Movement();
                    break;
                case States.GroundToAir:
                    playerMovement.RotatePlayer();
                    playerMovement.GroundToAir();
                    playerMovement.Movement();
                    break;
                case States.AirState:
                    playerMovement.RotatePlayer();
                    playerMovement.AirMovement();
                    playerMovement.Movement();
                    break;
                case States.DashState:
                    playerMovement.Dash();
                    break;
            }
        }

        public void EnableMovementInputs(bool enabled)
        {
            if (enabled && this.enabled)
            {
                controls.Player.Movement.performed += ReadMovement;
                controls.Player.Rotate.performed += ReadRotation;
                controls.Player.Jump.performed += OnJump;
                controls.Player.Dash.performed += OnDash;
                controls.Menu.CheatMode.performed += OnCheat;
            }
            else
            {
                controls.Player.Movement.performed -= ReadMovement;
                controls.Player.Rotate.performed -= ReadRotation;
                controls.Player.Jump.performed -= OnJump;
                controls.Player.Dash.performed -= OnDash;
                controls.Menu.CheatMode.performed -= OnCheat;
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
            lookVector = ctx.ReadValue<Vector2>();
        }

        public void ResetCameraMouseDelta()
        {
            lookVector = Vector2.zero;
            cameraRotation = GameManager.Camera.transform.eulerAngles;
        }

        private void LateUpdate()
        {
            // TWS: Die simpelste Implementierung, die mir spontan einfällt
            playerCam.transform.eulerAngles = cameraRotation;
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            var pressed = ctx.ReadValueAsButton();
            if (pressed)
            {
                switch (state)
                {
                    case States.GroundState:
                        performNormalJump = true;
                        StartJump();
                        break;
                    case States.GroundToAir:
                        performNormalJump = true;
                        StartJump();
                        break;
                    case States.AirState:
                        if (PlayerPrefs.GetInt("DoubleJumpUnlock") == 0) return;

                        if (canDoubleJump == false) return;
                        if (performNormalJump) return;

                        canDoubleJump = false;
                        performDoubleJump = true;
                        StartJump();
                        break;
                }
            }
        }
        private void StartJump()
        {
            onJump?.Invoke();
            velocity.y = jumpStrength;
            SwitchToAirState();
            jumpPressTime = 0;
        }
        private void OnDash(InputAction.CallbackContext ctx)
        {
            if (PlayerPrefs.GetInt("DashUnlock") == 0) return;

            var pressed = ctx.ReadValueAsButton();
            if (pressed)
            {
                if (state == States.GroundState) return;

                if (canDash)
                {
                    StartDash();
                }
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

        public Vector3 Vec2D(Vector3 vec)
        {
            return new Vector3(vec.x, 0, vec.z);
        }
        public void SwitchToGroundState()
        {
            canDash = true;
            canDoubleJump = true;
            IsGrounded = true;
            state = States.GroundState;
        }
        public void SwitchGroundToAir()
        {
            groundToAirTimer = 0;
            IsGrounded = false;
            state = States.GroundToAir;
        }
        public void SwitchToAirState()
        {
            IsGrounded = false;
            state = States.AirState;
        }
        public void StartDash()
        {
            performNormalJump = false;
            if (performDoubleJump) performDoubleJump = false;
            IsGrounded = false;
            velocity = Vector3.zero;
            dashTimer = 0;
            canDash = false;

            state = States.DashState;
        }
        private void OnCheat(InputAction.CallbackContext ctx)
        {
            var pressed = ctx.ReadValueAsButton();
            if (pressed)
            {
                if (toogleAbilities)
                {
                    PlayerPrefs.SetInt("DoubleJumpUnlock", 1);
                    PlayerPrefs.SetInt("DashUnlock", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("DoubleJumpUnlock", 0);
                    PlayerPrefs.SetInt("DashUnlock", 0);
                }
                toogleAbilities = !toogleAbilities;
            }
        }
    }
}