using Santa;
using UnityEngine;
using UnityEngine.Events;

namespace Events
{
	/// <summary>
	/// Dialog Event
	/// </summary>
	[CreateAssetMenu(menuName = "Events/Dialog Event Channel")]
	public class DialogEventChannelSO : ScriptableObject
	{
		public UnityAction<Dialog, int> OnEventRaised;

		public void RaiseEvent(Dialog value, int page = 0)
		{
			OnEventRaised?.Invoke(value, page);
		}
	}
}