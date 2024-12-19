using Events;
using UnityEngine;
using UnityEngine.Playables;

namespace Santa
{
    public class PlayableDialogAsset : PlayableAsset
    {
        public Dialog dialog;
        public int page;
        public DialogEventChannelSO dialogEventChannel;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<PlayableDialogBehaviour>.Create(graph);

            var behaviour = playable.GetBehaviour();
            behaviour.dialog = dialog;
            behaviour.page = page;
            behaviour.dialogEventChannel = dialogEventChannel;

            return playable;
        }
    }
}