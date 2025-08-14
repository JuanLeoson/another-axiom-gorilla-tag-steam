using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D1A RID: 3354
	public class FingerTipPokeToolView : MonoBehaviour, InteractableToolView
	{
		// Token: 0x170007DC RID: 2012
		// (get) Token: 0x060052F6 RID: 21238 RVA: 0x0019B794 File Offset: 0x00199994
		// (set) Token: 0x060052F7 RID: 21239 RVA: 0x0019B79C File Offset: 0x0019999C
		public InteractableTool InteractableTool { get; set; }

		// Token: 0x170007DD RID: 2013
		// (get) Token: 0x060052F8 RID: 21240 RVA: 0x0019B7A5 File Offset: 0x001999A5
		// (set) Token: 0x060052F9 RID: 21241 RVA: 0x0019B7B2 File Offset: 0x001999B2
		public bool EnableState
		{
			get
			{
				return this._sphereMeshRenderer.enabled;
			}
			set
			{
				this._sphereMeshRenderer.enabled = value;
			}
		}

		// Token: 0x170007DE RID: 2014
		// (get) Token: 0x060052FA RID: 21242 RVA: 0x0019B7C0 File Offset: 0x001999C0
		// (set) Token: 0x060052FB RID: 21243 RVA: 0x0019B7C8 File Offset: 0x001999C8
		public bool ToolActivateState { get; set; }

		// Token: 0x170007DF RID: 2015
		// (get) Token: 0x060052FC RID: 21244 RVA: 0x0019B7D1 File Offset: 0x001999D1
		// (set) Token: 0x060052FD RID: 21245 RVA: 0x0019B7D9 File Offset: 0x001999D9
		public float SphereRadius { get; private set; }

		// Token: 0x060052FE RID: 21246 RVA: 0x0019B7E2 File Offset: 0x001999E2
		private void Awake()
		{
			this.SphereRadius = this._sphereMeshRenderer.transform.localScale.z * 0.5f;
		}

		// Token: 0x060052FF RID: 21247 RVA: 0x000023F5 File Offset: 0x000005F5
		public void SetFocusedInteractable(Interactable interactable)
		{
		}

		// Token: 0x04005C50 RID: 23632
		[SerializeField]
		private MeshRenderer _sphereMeshRenderer;
	}
}
