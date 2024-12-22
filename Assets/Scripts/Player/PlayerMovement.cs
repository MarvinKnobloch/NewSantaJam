using UnityEngine;
using Santa;

public class PlayerMovement
{
    public PlayerController player;

    const string idleState = "Idle";
    const string runState = "Run";
    const string jumpState = "Jump";
    const string dashState = "Dash";
    public const string fallState = "Midair";
    const string landState = "Landing";

    private float landTimer = 0;

    public void Movement()
    {
        var currentVelocity = player.velocity;

        Vector3 applied;
        applied = player.transform.rotation * player.moveVector * Time.fixedDeltaTime * player.moveSpeed;

        player.velocity = Vector3.Lerp(currentVelocity, applied, Time.fixedDeltaTime * 15f);
        player.velocity.y = currentVelocity.y;
        if (player.externalForce != Vector3.zero)
        {
            player.velocity += player.externalForce;
            player.externalForce = Vector3.zero;
        }

        var wasGrounded = player.IsGrounded;

        Vector3 movement = player.playerCollision.CollideAndSlide();
        player.speed = new Vector2(movement.x, movement.z).magnitude;

        if (wasGrounded && !player.IsGrounded)
        {
            if (player.state == PlayerController.States.GroundState) player.SwitchGroundToAir();

            player.groundedEventChannel.RaiseEvent(player.IsGrounded);
        }

        if (player.IsGrounded && !wasGrounded)
        {
            if (player.state != PlayerController.States.DashState)
            {
                player.SwitchToGroundState();
                player.ChangeAnimationState(landState);
                landTimer = 1f;
            }

            var fallDist = (player.fallStartHeight - player.transform.position.y);
            player.fallStartHeight = float.MinValue;
            if (fallDist > 1) player.onLanding?.Invoke(fallDist);

            player.groundedEventChannel.RaiseEvent(player.IsGrounded);
        }
        if (player.IsGrounded && landTimer > 0)
        {
            landTimer -= Time.fixedDeltaTime;
        }

        player.animator.SetFloat("Forward", Vector3.Dot(player.transform.forward, player.velocity));
        player.rig.MovePosition(player.rig.position + movement);
    }

    public void RotatePlayer()
    {
        if (!player.enabled) return;
        player.cameraRotation.y += player.lookVector.x * GameManager.Settings.mouseSensitivityX;
        player.cameraRotation.x -= player.lookVector.y * GameManager.Settings.mouseSensitivityY;
        player.cameraRotation.x = Mathf.Clamp(player.cameraRotation.x, player.minCameraAngle, player.maxCameraAngle);
        player.cameraRotation.z = 0;

        player.rig.rotation = Quaternion.Euler(0, player.cameraRotation.y, 0);
        player.lookVector = Vector2.zero;
    }
    public void GroundMovement()
    {
        player.velocity.y = -player.stickToGroundForce;

        if (player.currentstate == landState && landTimer > 0) return;
        if (player.moveVector != Vector3.zero) player.ChangeAnimationState(runState);
        else player.ChangeAnimationState(idleState);
    }
    public void GroundToAir()
    {
        player.groundToAirTimer += Time.fixedDeltaTime;

        PlayerFall();

        if (player.velocity.y > 0) player.ChangeAnimationState(jumpState);
        else player.ChangeAnimationState(fallState);

        if(player.groundToAirTimer > player.offGroundJumpDelay)
        {
            player.SwitchToAirState();
        }

    }
    public void AirMovement()
    {
        if (player.performNormalJump) Jump();
        else if (player.performDoubleJump) Jump();
        else
        {
            PlayerFall();
        }

        if (player.fallStartHeight == float.MinValue && player.velocity.y < 0)
        {
            player.fallStartHeight = player.transform.position.y;
        }

        if (player.velocity.y > 0) player.ChangeAnimationState(jumpState);
        else player.ChangeAnimationState(fallState);
    }
    private void Jump()
    {
        if (player.controls.Player.Jump.IsPressed())
        {
            if (player.jumpPressTime < player.jumpMaxPressTime)
            {
                player.velocity.y = player.jumpStrength - (player.jumpPressTime * player.jumpStrengthDecay);
                player.jumpPressTime += Time.fixedDeltaTime;
            }
            else JumpReset();
        }
        else
        {
            PlayerFall();
            JumpReset();
        }
    }
    private void PlayerFall()
    {
        if (player.velocity.y > player.maxFallSpeed)
        {
            player.velocity.y += Physics.gravity.y * Time.fixedDeltaTime * player.gravityFactor;
        }
        else player.velocity.y = player.maxFallSpeed;
    }
    private void JumpReset()
    {
        if (player.performNormalJump) player.performNormalJump = false;
        else if (player.performDoubleJump) player.performDoubleJump = false;
    }
    public void Dash()
    {
        var currentVelocity = player.velocity;
        Vector3 applied;

        applied = player.transform.forward * player.dashStrength * Time.fixedDeltaTime;


        player.velocity = Vector3.Lerp(currentVelocity, applied, Time.fixedDeltaTime * 15f);
        player.velocity.y = 0;

        Vector3 movement = player.playerCollision.CollideAndSlide();
        player.speed = new Vector2(movement.x, movement.z).magnitude;

        player.rig.MovePosition(player.rig.position + movement);

        player.dashTimer += Time.deltaTime;
        if(player.dashTimer > player.dashLength)
        {
            RaycastHit groundCheck;
            Vector3 pos = player.rig.position;

            float radius = player.playerCollider.radius - player.skinWidth;
            float height = player.playerCollider.height / 2f - player.playerCollider.radius;
            Vector3 p1 = pos + player.playerCollider.center - Vector3.up * height;
            Vector3 p2 = pos + player.playerCollider.center + Vector3.up * height;

            if (Physics.CapsuleCast(p1, p2, radius * 0.5f, Vector3.down, out groundCheck, 1, player.currentLayers, QueryTriggerInteraction.Ignore))
            {
                player.SwitchToGroundState();
            }
            else
            {
                player.SwitchToAirState();
            }
        }
    }
    public void HoldWallGrab()
    {
        if (player.controls.Player.WallGrab.IsPressed() == false)
        {
            player.SwitchToAirState();
        }
    }
}
