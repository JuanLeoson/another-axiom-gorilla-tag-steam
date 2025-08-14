using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F46 RID: 3910
	public class GlowBugsInJar : MonoBehaviour
	{
		// Token: 0x060060D6 RID: 24790 RVA: 0x001ED808 File Offset: 0x001EBA08
		private void OnEnable()
		{
			this.shakeStarted = false;
			this.UpdateGlow(0f);
			if (this._events == null)
			{
				this._events = base.gameObject.GetOrAddComponent<RubberDuckEvents>();
				NetPlayer netPlayer = (this.transferrableObject.myOnlineRig != null) ? this.transferrableObject.myOnlineRig.creator : ((this.transferrableObject.myRig != null) ? (this.transferrableObject.myRig.creator ?? NetworkSystem.Instance.LocalPlayer) : null);
				if (netPlayer != null)
				{
					this._events.Init(netPlayer);
				}
			}
			if (this._events != null)
			{
				this._events.Activate += this.OnShakeEvent;
			}
		}

		// Token: 0x060060D7 RID: 24791 RVA: 0x001ED8E0 File Offset: 0x001EBAE0
		private void OnDisable()
		{
			if (this._events != null)
			{
				this._events.Activate -= this.OnShakeEvent;
				this._events.Dispose();
				this._events = null;
			}
		}

		// Token: 0x060060D8 RID: 24792 RVA: 0x001ED930 File Offset: 0x001EBB30
		private void OnShakeEvent(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
		{
			if (sender != target)
			{
				return;
			}
			GorillaNot.IncrementRPCCall(info, "OnShakeEvent");
			if (!this.callLimiter.CheckCallTime(Time.time))
			{
				return;
			}
			if (args != null && args.Length == 1)
			{
				object obj = args[0];
				if (obj is bool)
				{
					bool flag = (bool)obj;
					if (flag)
					{
						this.ShakeStartLocal();
						return;
					}
					this.ShakeEndLocal();
					return;
				}
			}
		}

		// Token: 0x060060D9 RID: 24793 RVA: 0x001ED990 File Offset: 0x001EBB90
		public void HandleOnShakeStart()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					true
				});
			}
			this.ShakeStartLocal();
		}

		// Token: 0x060060DA RID: 24794 RVA: 0x001ED9EA File Offset: 0x001EBBEA
		private void ShakeStartLocal()
		{
			this.currentGlowAmount = 0f;
			this.shakeStarted = true;
			this.shakeTimer = 0f;
		}

		// Token: 0x060060DB RID: 24795 RVA: 0x001EDA0C File Offset: 0x001EBC0C
		public void HandleOnShakeEnd()
		{
			if (PhotonNetwork.InRoom && this._events != null && this._events.Activate != null)
			{
				this._events.Activate.RaiseOthers(new object[]
				{
					false
				});
			}
			this.ShakeEndLocal();
		}

		// Token: 0x060060DC RID: 24796 RVA: 0x001EDA66 File Offset: 0x001EBC66
		private void ShakeEndLocal()
		{
			this.shakeStarted = false;
			this.shakeTimer = 0f;
		}

		// Token: 0x060060DD RID: 24797 RVA: 0x001EDA7C File Offset: 0x001EBC7C
		public void Update()
		{
			if (this.shakeStarted)
			{
				this.shakeTimer += 1f;
				if (this.shakeTimer >= this.glowUpdateInterval && this.currentGlowAmount < 1f)
				{
					this.currentGlowAmount += this.glowIncreaseStepAmount;
					this.UpdateGlow(this.currentGlowAmount);
					this.shakeTimer = 0f;
					return;
				}
			}
			else
			{
				this.shakeTimer += 1f;
				if (this.shakeTimer >= this.glowUpdateInterval && this.currentGlowAmount > 0f)
				{
					this.currentGlowAmount -= this.glowDecreaseStepAmount;
					this.UpdateGlow(this.currentGlowAmount);
					this.shakeTimer = 0f;
				}
			}
		}

		// Token: 0x060060DE RID: 24798 RVA: 0x001EDB48 File Offset: 0x001EBD48
		private void UpdateGlow(float value)
		{
			if (this.renderers.Length != 0)
			{
				for (int i = 0; i < this.renderers.Length; i++)
				{
					Material material = this.renderers[i].material;
					Color color = material.GetColor(this.shaderProperty);
					color.a = value;
					material.SetColor(this.shaderProperty, color);
					material.EnableKeyword("_EMISSION");
				}
			}
		}

		// Token: 0x04006CD3 RID: 27859
		[SerializeField]
		private TransferrableObject transferrableObject;

		// Token: 0x04006CD4 RID: 27860
		[Space]
		[Tooltip("Time interval - every X seconds update the glow value")]
		[SerializeField]
		private float glowUpdateInterval = 2f;

		// Token: 0x04006CD5 RID: 27861
		[Tooltip("step increment - increase the glow value one step for N amount")]
		[SerializeField]
		private float glowIncreaseStepAmount = 0.1f;

		// Token: 0x04006CD6 RID: 27862
		[Tooltip("step decrement - decrease the glow value one step for N amount")]
		[SerializeField]
		private float glowDecreaseStepAmount = 0.2f;

		// Token: 0x04006CD7 RID: 27863
		[Space]
		[SerializeField]
		private string shaderProperty = "_EmissionColor";

		// Token: 0x04006CD8 RID: 27864
		[SerializeField]
		private Renderer[] renderers;

		// Token: 0x04006CD9 RID: 27865
		private bool shakeStarted = true;

		// Token: 0x04006CDA RID: 27866
		private static int EmissionColor;

		// Token: 0x04006CDB RID: 27867
		private float currentGlowAmount;

		// Token: 0x04006CDC RID: 27868
		private float shakeTimer;

		// Token: 0x04006CDD RID: 27869
		private RubberDuckEvents _events;

		// Token: 0x04006CDE RID: 27870
		private CallLimiter callLimiter = new CallLimiter(10, 2f, 0.5f);
	}
}
