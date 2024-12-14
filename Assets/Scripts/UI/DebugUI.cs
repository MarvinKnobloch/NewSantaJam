using Events;
using Santa;
using TMPro;
using UnityEngine;

public class DebugUI : MonoBehaviour
{
    public TextMeshProUGUI playerStateText;
    public TextMeshProUGUI playerGroundedText;

    [Space]
    public IntEventChannelSO stateChannel;
    public BoolEventChannelSO groundedEventChannel;

    private void OnEnable()
    {
        stateChannel.OnEventRaised += UpdatePlayerState;
        groundedEventChannel.OnEventRaised += UpdateGroundedState;
    }

    private void OnDisable()
    {
        stateChannel.OnEventRaised -= UpdatePlayerState;
        groundedEventChannel.OnEventRaised -= UpdateGroundedState;
    }

    // Update is called once per frame
    void UpdatePlayerState(int state)
    {
        playerStateText.text = PlayerStateToString((PlayerController.States) state);
    }

    void UpdateGroundedState(bool state)
    {
        playerGroundedText.text = state ? "true" : "false";
    }

    string PlayerStateToString(PlayerController.States state)
    {
        switch(state)
        {
            case PlayerController.States.GroundState: return "GroundState";
            case PlayerController.States.GroundToAir: return "GroundToAir";
            case PlayerController.States.AirState: return "AirState";
            case PlayerController.States.DashState: return "DashState";
            case PlayerController.States.WallGrab: return "WallGrab";

            default: return "Unknown";
        }
    }
}
