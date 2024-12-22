using UnityEngine;

namespace Santa
{
    public class BubbleScaler : MonoBehaviour, IActivate
    {
        public Bubble bubble;
        public float timePerStep = 3f;

        public float[] scalePoints;

        private int scaleIndex = 0;

        private float scaleTimer;

        public void Activate()
        {
            if (CanBeTriggered())
            {
                scaleIndex++;
                scaleTimer = timePerStep;  
            }
        }

        private void Update()
        {
            if (scaleIndex == 0) return;
            if (scaleTimer > 0) scaleTimer -= Time.deltaTime;

            float scale = Mathf.Lerp(scalePoints[scaleIndex - 1], scalePoints[scaleIndex], (timePerStep - scaleTimer) / timePerStep);
            bubble.transform.localScale = Vector3.one * scale;
        }

        public bool CanBeTriggered()
        {
            return scaleIndex < scalePoints.Length - 1;
        }

        public void Deactivate()
        {}

        public void SetRequirement()
        {}
    }
}
