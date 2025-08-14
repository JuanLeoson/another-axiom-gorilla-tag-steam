using System;
using UnityEngine;
using UnityEngine.UI;

namespace OculusSampleFramework
{
	// Token: 0x02000D24 RID: 3364
	public class DistanceGrabberSample : MonoBehaviour
	{
		// Token: 0x170007F5 RID: 2037
		// (get) Token: 0x0600533F RID: 21311 RVA: 0x0019C53B File Offset: 0x0019A73B
		// (set) Token: 0x06005340 RID: 21312 RVA: 0x0019C544 File Offset: 0x0019A744
		public bool UseSpherecast
		{
			get
			{
				return this.useSpherecast;
			}
			set
			{
				this.useSpherecast = value;
				for (int i = 0; i < this.m_grabbers.Length; i++)
				{
					this.m_grabbers[i].UseSpherecast = this.useSpherecast;
				}
			}
		}

		// Token: 0x170007F6 RID: 2038
		// (get) Token: 0x06005341 RID: 21313 RVA: 0x0019C57E File Offset: 0x0019A77E
		// (set) Token: 0x06005342 RID: 21314 RVA: 0x0019C588 File Offset: 0x0019A788
		public bool AllowGrabThroughWalls
		{
			get
			{
				return this.allowGrabThroughWalls;
			}
			set
			{
				this.allowGrabThroughWalls = value;
				for (int i = 0; i < this.m_grabbers.Length; i++)
				{
					this.m_grabbers[i].m_preventGrabThroughWalls = !this.allowGrabThroughWalls;
				}
			}
		}

		// Token: 0x06005343 RID: 21315 RVA: 0x0019C5C8 File Offset: 0x0019A7C8
		private void Start()
		{
			DebugUIBuilder.instance.AddLabel("Distance Grab Sample", 0);
			DebugUIBuilder.instance.AddToggle("Use Spherecasting", new DebugUIBuilder.OnToggleValueChange(this.ToggleSphereCasting), this.useSpherecast, 0);
			DebugUIBuilder.instance.AddToggle("Grab Through Walls", new DebugUIBuilder.OnToggleValueChange(this.ToggleGrabThroughWalls), this.allowGrabThroughWalls, 0);
			DebugUIBuilder.instance.Show();
			float displayFrequency = OVRManager.display.displayFrequency;
			if (displayFrequency > 0.1f)
			{
				Debug.Log("Setting Time.fixedDeltaTime to: " + (1f / displayFrequency).ToString());
				Time.fixedDeltaTime = 1f / displayFrequency;
			}
		}

		// Token: 0x06005344 RID: 21316 RVA: 0x0019C673 File Offset: 0x0019A873
		public void ToggleSphereCasting(Toggle t)
		{
			this.UseSpherecast = !this.UseSpherecast;
		}

		// Token: 0x06005345 RID: 21317 RVA: 0x0019C684 File Offset: 0x0019A884
		public void ToggleGrabThroughWalls(Toggle t)
		{
			this.AllowGrabThroughWalls = !this.AllowGrabThroughWalls;
		}

		// Token: 0x04005C8C RID: 23692
		private bool useSpherecast;

		// Token: 0x04005C8D RID: 23693
		private bool allowGrabThroughWalls;

		// Token: 0x04005C8E RID: 23694
		[SerializeField]
		private DistanceGrabber[] m_grabbers;
	}
}
