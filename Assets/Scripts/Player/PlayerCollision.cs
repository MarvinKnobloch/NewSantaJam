using UnityEngine;
using Santa;

public class PlayerCollision

{
    public PlayerController player;

    public Vector3 CollideAndSlide()
    {
        Vector3 velHorizontal = player.Vec2D(player.velocity);
        Vector3 velVertical = new Vector3(0, player.velocity.y - player.stepHeight, 0);

        player.rig.position += Vector3.up * player.stepHeight;
        var movement = CollideAndSlide(player.rig.position, velHorizontal, velHorizontal.normalized, false, 0);

        movement += CollideAndSlide(player.rig.position + movement, velVertical, Vector3.zero, true, 0);
        return movement;
    }

    private Vector3 CollideAndSlide(Vector3 pos, Vector3 vel, Vector3 origDir, bool gravityPass, int depth)
    {
        if (depth >= 3) return Vector3.zero;

        float dist = vel.magnitude + player.skinWidth;

        RaycastHit hit;
        RaycastHit groundCheck;

        float radius = player.playerCollider.radius - player.skinWidth;
        float height = player.playerCollider.height / 2f - player.playerCollider.radius;
        Vector3 p1 = pos + player.playerCollider.center - Vector3.up * height;
        Vector3 p2 = pos + player.playerCollider.center + Vector3.up * height;

        if (Physics.CapsuleCast(p1, p2, radius * 0.5f, vel.normalized, out groundCheck, dist, player.groundLayers, QueryTriggerInteraction.Ignore))
        {
            if (player.state != PlayerController.States.DashState)
            {
                player.SwitchToGroundState();
            }
        }

        if (Physics.CapsuleCast(p1, p2, radius, vel.normalized, out hit, dist, player.groundLayers, QueryTriggerInteraction.Ignore))
        {
            Vector3 snapToSurface = vel.normalized * (hit.distance - player.skinWidth);
            Vector3 leftOver = vel - snapToSurface;
            float angle = Vector3.Angle(hit.normal, Vector3.up);

            if (snapToSurface.magnitude <= player.skinWidth)
            {
                snapToSurface = Vector3.zero;
            }

            if (angle < player.slopeLimit)
            {
                // Rampe
                if (gravityPass)
                {
                    if (player.IsGrounded && snapToSurface.y < -1f) snapToSurface.y = -1f;

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
                if (player.IsGrounded && !gravityPass)
                {
                    if (hit.collider.gameObject.TryGetComponent(out ITrigger other))
                    {
                        other.Trigger(player, TriggerCommand.Toggle);
                    }

                    var wallNormal = player.Vec2D(hit.normal).normalized;
                    float scale = 1 - Vector3.Dot(wallNormal, -origDir);
                    leftOver = Vector3.ProjectOnPlane(player.Vec2D(leftOver), wallNormal) * scale;
                }
                else
                {
                    if (player.IsGrounded && player.velocity.y < 0) { player.velocity.y = 0; leftOver.y = 0; }
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
            if(player.state == PlayerController.States.GroundState) player.SwitchGroundToAir();
        }

        return vel;
    }

}
