using UnityEngine;
using Events;
using UnityEngine.Playables;

namespace Santa
{
    public class PlayableDialogBehaviour : PlayableBehaviour
    {
        public Dialog dialog = null;
        public int page = 0;
        public DialogEventChannelSO dialogEventChannel;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            dialogEventChannel.RaiseEvent(dialog, page);
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            dialogEventChannel.RaiseEvent(null);
        }
    }
}