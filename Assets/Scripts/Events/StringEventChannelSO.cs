using UnityEngine;
using UnityEngine.Events;

namespace Events
{
	/// <summary>
	/// This class is used for Events that have one String Argument.
	/// </summary>
	[CreateAssetMenu(menuName = "Events/String Event Channel")]
	public class StringEventChannelSO : ScriptableObject
	{
		public UnityAction<string> OnEventRaised;

		public void RaiseEvent(string value)
		{
			OnEventRaised?.Invoke(value);
		}
	}
}