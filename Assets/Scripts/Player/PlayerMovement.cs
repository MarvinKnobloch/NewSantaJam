using UnityEngine;
using Santa;

public class PlayerMovement
{
    public PlayerController player;

    public void Movement()
    {
        var currentVelocity = player.velocity;
        Vector3 applied;

        applied = player.transform.rotation * player.moveVector * Time.fixedDeltaTime * player.moveSpeed;


        player.velocity = Vector3.Lerp(currentVelocity, applied, Time.fixedDeltaTime * 15f);
        player.velocity.y = currentVelocity.y;

        var wasGrounded = player.IsGrounded;

        Vector3 movement = player.playerCollision.CollideAndSlide();
        player.speed = new Vector2(movement.x, movement.z).magnitude;

        if (player.IsGrounded && !wasGrounded)
        {
            var fallDist = (player.fallStartHeight - player.transform.position.y);
            player.fallStartHeight = float.MinValue;
            if (fallDist > 1) player.onLanding?.Invoke(fallDist);
        }

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
        player.jumpFrameTimer = player.offGroundJumpDelay;
    }
    public void AirMovement()
    {
        player.jumpFrameTimer -= Time.deltaTime;

        if (player.controls.Player.Jump.IsPressed() && player.jumpPressTime < player.jumpMaxPressTime)
        {
            player.velocity.y = player.jumpStrength;
            player.jumpPressTime += Time.fixedDeltaTime;

        }
        else
        {
            if (player.velocity.y > player.maxFallSpeed)
            {
                player.velocity.y += Physics.gravity.y * Time.fixedDeltaTime * player.gravityFactor;
            }
            else player.velocity.y = player.maxFallSpeed;
        }

        if (player.fallStartHeight == float.MinValue && player.velocity.y < 0)
        {
            player.fallStartHeight = player.transform.position.y;
        }
    }
}
