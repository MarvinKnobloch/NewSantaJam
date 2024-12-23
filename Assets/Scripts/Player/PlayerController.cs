using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Events;

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
        [SerializeField] private LayerMask groundLayers = Physics.AllLayers;
        [SerializeField] private LayerMask bubbleGroundLayers = Physics.AllLayers;

        [NonSerialized] public LayerMask currentLayers;

        //animation;
        [NonSerialized] public Animator animator;
        public string currentstate;
        const string idlestate = "Idle";
        const string dashState = "Dash";

        #region Properties
        public bool IsGrounded { get; set; }
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

        [NonSerialized] public Vector3 velocity;
        [NonSerialized] public float speed;
        [NonSerialized] public Vector3 cameraRotation;
        [NonSerialized] public float fallStartHeight = float.MinValue;

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
        [NonSerialized] public Vector3 externalForce;

        //Wall
        public bool performedWallGrab;
        public bool canPerformWallGrab;

        //Reset
        private bool resetPerformed;

        // Funktions Delegaten f�r den Spieler
        public System.Action onJump;
        public System.Action onUse;
        public System.Action<float> onLanding;

        private bool toogleAbilities;

        private readonly PlayerMovement playerMovement = new PlayerMovement();
        [NonSerialized] public readonly PlayerCollision playerCollision = new PlayerCollision();

        public IntEventChannelSO stateEventChannel;
        public BoolEventChannelSO groundedEventChannel;

        public States state;
        public enum States
        {
            GroundState,
            GroundToAir,
            AirState,
            DashState,
            WallGrab,
            Empty,
        }
        void Awake()
        {
            controls = Keybindinputmanager.Controls;
            playerCollider = GetComponent<CapsuleCollider>();
            rig = GetComponent<Rigidbody>();
            rig.useGravity = useGravity;
            rig.isKinematic = true;
            rig.freezeRotation = true;
            currentLayers = groundLayers;

            animator = GetComponentInChildren<Animator>();
            currentstate = idlestate;

            playerMovement.player = this;
            playerCollision.player = this;

            state = States.AirState;
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
            Cursor.lockState = CursorLockMode.Confined;
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
                case States.WallGrab:
                    playerMovement.RotatePlayer();
                    playerMovement.HoldWallGrab();
                    break;
            }
            UpdateBubble();
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
                controls.Player.Reset.performed += OnReset;
                //controls.Menu.CollectableReset.performed += OnCollectableReset;
            }
            else
            {
                controls.Player.Movement.performed -= ReadMovement;
                controls.Player.Rotate.performed -= ReadRotation;
                controls.Player.Jump.performed -= OnJump;
                controls.Player.Dash.performed -= OnDash;
                controls.Menu.CheatMode.performed -= OnCheat;
                controls.Player.Reset.performed -= OnReset;
                //controls.Menu.CollectableReset.performed -= OnCollectableReset;
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

        public void ChangeAnimationState(string newstate, float time = 0.15f)
        {
            if (currentstate == newstate) return;

            Debug.Log("Change Animation State: " + newstate);
            animator.CrossFadeInFixedTime(newstate, time);
            currentstate = newstate;
        }
        private void LateUpdate()
        {
            // TWS: Die simpelste Implementierung, die mir spontan einf�llt
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
                        Jump();
                        break;
                    case States.GroundToAir:
                        Jump();
                        break;
                    case States.AirState:
                        CheckDoubleJump();
                        break;
                    case States.WallGrab:
                        CheckDoubleJump();
                        break;
                }
            }
        }
        private void Jump()
        {
            performNormalJump = true;
            StartJump();

            AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.jump);
        }
        private void CheckDoubleJump()
        {
            if (PlayerPrefs.GetInt("DoubleJumpUnlock") == 0) return;

            if (!canDoubleJump) return;
            if (performNormalJump) return;

            canDoubleJump = false;
            performDoubleJump = true;
            StartJump();

            AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.doubleJump);
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
                    ChangeAnimationState(dashState);
                    StartDash();
                }
            }
        }

        private void OnUse(InputAction.CallbackContext ctx)
        {
            var pressed = ctx.ReadValueAsButton();
            if (pressed)
            {
                if (GameManager.Instance.playerUI.messageBox.activeSelf) GameManager.Instance.playerUI.CloseMessageBox();
                else
                {
                    onUse.Invoke();
                }
            }
        }
        private void OnReset(InputAction.CallbackContext ctx)
        {
            var pressed = ctx.ReadValueAsButton();
            if (pressed && !resetPerformed)
            {
                resetPerformed = true;
                Player.Instance.die();
            }
        }

        public Vector3 Vec2D(Vector3 vec)
        {
            return new Vector3(vec.x, 0, vec.z);
        }

        public void SwitchToGroundState()
        {
            velocity.y = 0;

            performedWallGrab = false;
            canDash = true;
            canDoubleJump = true;
            state = States.GroundState;
            stateEventChannel.RaiseEvent((int) state);
        }
        public void SwitchGroundToAir()
        {
            groundToAirTimer = 0;
            state = States.GroundToAir;
            stateEventChannel.RaiseEvent((int)state);
        }
        public void SwitchToAirState()
        {
            state = States.AirState;
            stateEventChannel.RaiseEvent((int)state);
        }
        public void SwitchToWallGrab()
        {
            performedWallGrab = true;
            performNormalJump = false;
            performDoubleJump = false;
            canDoubleJump = true;

            velocity = Vector3.zero;
            state = States.WallGrab;
            stateEventChannel.RaiseEvent((int)state);
        }
        public void StartDash()
        {
            performNormalJump = false;
            if (performDoubleJump) performDoubleJump = false;
            velocity = Vector3.zero;
            dashTimer = 0;
            canDash = false;

            state = States.DashState;
            stateEventChannel.RaiseEvent((int)state);

            AudioController.Instance.PlaySoundOneshot((int)AudioController.Sounds.dash);
        }

        public void ResetAbilities()
        {
            canDoubleJump = true;
            canDash = true;
        }

        void UpdateBubble()
        {
            if (!BubbleController.Instance) return;

            var inverted = false;
            var myDimensionLayer = inverted ? Layers.Dimension_2 : Layers.Dimension_1;
            var otherDimensionLayer = inverted ? Layers.Dimension_1 : Layers.Dimension_2;

            playerCollider.excludeLayers = Layers.Mask(otherDimensionLayer);
            currentLayers = groundLayers;
            for (int i = 0; i < BubbleController.COUNT; i++)
            {
                if (Vector3.Distance(transform.position, BubbleController.Instance.positions[i]) < BubbleController.Instance.radien[i])
                {
                    Debug.Log("in Bubble");
                    playerCollider.excludeLayers = Layers.Mask(myDimensionLayer);
                    currentLayers = bubbleGroundLayers;
                    return;
                }
            }
        }
        private void OnCheat(InputAction.CallbackContext ctx)
        {
            var pressed = ctx.ReadValueAsButton();
            if (pressed)
            {
#if UNITY_EDITOR
                if (toogleAbilities)
                {
                    PlayerPrefs.SetInt("DoubleJumpUnlock", 1);
                    PlayerPrefs.SetInt("DashUnlock", 1);
                    PlayerPrefs.SetInt("WallGrabUnlock", 1);
                }
                else
                {
                    PlayerPrefs.SetInt("DoubleJumpUnlock", 0);
                    PlayerPrefs.SetInt("DashUnlock", 0);
                    PlayerPrefs.SetInt("WallGrabUnlock", 0);
                }
                toogleAbilities = !toogleAbilities;
#endif
            }
        }
        //private static void OnCollectableReset(InputAction.CallbackContext ctx)
        //{
        //    var pressed = ctx.ReadValueAsButton();
        //    if (pressed)
        //    {
        //        for (int i = 0; i < 15; i++)
        //        {
        //            PlayerPrefs.SetInt("Collectable" + i, 0);
        //        }
        //    }
        //}
    }
}