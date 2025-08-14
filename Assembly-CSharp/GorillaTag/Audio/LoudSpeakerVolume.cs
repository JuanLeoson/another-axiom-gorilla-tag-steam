using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000EF2 RID: 3826
	public class LoudSpeakerVolume : MonoBehaviour
	{
		// Token: 0x06005EEE RID: 24302 RVA: 0x001DEA98 File Offset: 0x001DCC98
		public void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("GorillaPlayer"))
			{
				VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
				if (component != null && component.creator != null)
				{
					if (component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId)
					{
						this._trigger.OnPlayerEnter(component);
						return;
					}
				}
				else
				{
					Debug.LogWarning("LoudSpeakerNetworkVolume :: OnTriggerEnter no colliding rig found!");
				}
			}
		}

		// Token: 0x06005EEF RID: 24303 RVA: 0x001DEB08 File Offset: 0x001DCD08
		public void OnTriggerExit(Collider other)
		{
			VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
			if (component != null && component.creator != null)
			{
				if (component.creator.UserId == NetworkSystem.Instance.LocalPlayer.UserId)
				{
					this._trigger.OnPlayerExit(component);
					return;
				}
			}
			else
			{
				Debug.LogWarning("LoudSpeakerNetworkVolume :: OnTriggerExit no colliding rig found!");
			}
		}

		// Token: 0x04006956 RID: 26966
		[SerializeField]
		private LoudSpeakerTrigger _trigger;
	}
}
