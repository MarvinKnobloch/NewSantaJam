using UnityEngine;
using UnityEngine.Events;

namespace Events
{
	[CreateAssetMenu(menuName = "Events/Float Event Channel")]
	public class FloatEventChannelSO : ScriptableObject
	{
		public UnityAction<float> OnEventRaised;

		public void RaiseEvent(float value)
		{
			OnEventRaised?.Invoke(value);
		}
	}
}