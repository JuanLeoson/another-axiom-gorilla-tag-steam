using System;
using emotitron.Compression;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E73 RID: 3699
	public class DrinkableHoldable : TransferrableObject
	{
		// Token: 0x06005C75 RID: 23669 RVA: 0x001D1594 File Offset: 0x001CF794
		internal override void OnEnable()
		{
			base.OnEnable();
			base.enabled = (this.containerLiquid != null);
			this.itemState = (TransferrableObject.ItemStates)DrinkableHoldable.PackValues(this.sipSoundCooldown, this.containerLiquid.fillAmount, this.coolingDown);
			this.myByteArray = new byte[32];
		}

		// Token: 0x06005C76 RID: 23670 RVA: 0x001D15E8 File Offset: 0x001CF7E8
		protected override void LateUpdateLocal()
		{
			if (!this.containerLiquid.isActiveAndEnabled || !GorillaParent.hasInstance || !GorillaComputer.hasInstance)
			{
				base.LateUpdateLocal();
				return;
			}
			float num = (float)((GorillaComputer.instance.startupMillis + (long)Time.realtimeSinceStartup * 1000L) % 259200000L) / 1000f;
			if (Mathf.Abs(num - this.lastTimeSipSoundPlayed) > 129600f)
			{
				this.lastTimeSipSoundPlayed = num;
			}
			float num2 = this.sipRadius * this.sipRadius;
			bool flag = (GorillaTagger.Instance.offlineVRRig.head.rigTarget.transform.TransformPoint(this.headToMouthOffset) - this.containerLiquid.cupTopWorldPos).sqrMagnitude < num2;
			if (!flag)
			{
				foreach (VRRig vrrig in GorillaParent.instance.vrrigs)
				{
					if (!vrrig.isOfflineVRRig)
					{
						if (flag || vrrig.head == null)
						{
							break;
						}
						if (vrrig.head.rigTarget == null)
						{
							break;
						}
						flag = ((vrrig.head.rigTarget.transform.TransformPoint(this.headToMouthOffset) - this.containerLiquid.cupTopWorldPos).sqrMagnitude < num2);
					}
				}
			}
			if (flag)
			{
				this.containerLiquid.fillAmount = Mathf.Clamp01(this.containerLiquid.fillAmount - this.sipRate * Time.deltaTime);
				if (num > this.lastTimeSipSoundPlayed + this.sipSoundCooldown)
				{
					if (!this.wasSipping)
					{
						this.lastTimeSipSoundPlayed = num;
						this.coolingDown = true;
					}
				}
				else
				{
					this.coolingDown = false;
				}
			}
			this.wasSipping = flag;
			this.itemState = (TransferrableObject.ItemStates)DrinkableHoldable.PackValues(this.lastTimeSipSoundPlayed, this.containerLiquid.fillAmount, this.coolingDown);
			base.LateUpdateLocal();
		}

		// Token: 0x06005C77 RID: 23671 RVA: 0x001D17E4 File Offset: 0x001CF9E4
		protected override void LateUpdateReplicated()
		{
			base.LateUpdateReplicated();
			int itemState = (int)this.itemState;
			this.UnpackValuesNonstatic(itemState, out this.lastTimeSipSoundPlayed, out this.containerLiquid.fillAmount, out this.coolingDown);
		}

		// Token: 0x06005C78 RID: 23672 RVA: 0x001D181D File Offset: 0x001CFA1D
		protected override void LateUpdateShared()
		{
			base.LateUpdateShared();
			if (this.coolingDown && !this.wasCoolingDown)
			{
				this.sipSoundBankPlayer.Play();
			}
			this.wasCoolingDown = this.coolingDown;
		}

		// Token: 0x06005C79 RID: 23673 RVA: 0x001D184C File Offset: 0x001CFA4C
		private static int PackValues(float cooldownStartTime, float fillAmount, bool coolingDown)
		{
			byte[] array = new byte[32];
			int num = 0;
			array.WriteBool(coolingDown, ref num);
			array.Write((ulong)((double)cooldownStartTime * 100.0), ref num, 25);
			array.Write((ulong)((double)fillAmount * 63.0), ref num, 6);
			return BitConverter.ToInt32(array, 0);
		}

		// Token: 0x06005C7A RID: 23674 RVA: 0x001D18A0 File Offset: 0x001CFAA0
		private void UnpackValuesNonstatic(in int packed, out float cooldownStartTime, out float fillAmount, out bool coolingDown)
		{
			DrinkableHoldable.GetBytes(packed, ref this.myByteArray);
			int num = 0;
			coolingDown = this.myByteArray.ReadBool(ref num);
			cooldownStartTime = (float)(this.myByteArray.Read(ref num, 25) / 100.0);
			fillAmount = this.myByteArray.Read(ref num, 6) / 63f;
		}

		// Token: 0x06005C7B RID: 23675 RVA: 0x001D1904 File Offset: 0x001CFB04
		public static void GetBytes(int value, ref byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				bytes[i] = (byte)(value >> 8 * i & 255);
			}
		}

		// Token: 0x06005C7C RID: 23676 RVA: 0x001D1934 File Offset: 0x001CFB34
		private static void UnpackValuesStatic(in int packed, out float cooldownStartTime, out float fillAmount, out bool coolingDown)
		{
			byte[] bytes = BitConverter.GetBytes(packed);
			int num = 0;
			coolingDown = bytes.ReadBool(ref num);
			cooldownStartTime = (float)(bytes.Read(ref num, 25) / 100.0);
			fillAmount = bytes.Read(ref num, 6) / 63f;
		}

		// Token: 0x04006624 RID: 26148
		[AssignInCorePrefab]
		public ContainerLiquid containerLiquid;

		// Token: 0x04006625 RID: 26149
		[AssignInCorePrefab]
		[SoundBankInfo]
		public SoundBankPlayer sipSoundBankPlayer;

		// Token: 0x04006626 RID: 26150
		[AssignInCorePrefab]
		public float sipRate = 0.1f;

		// Token: 0x04006627 RID: 26151
		[AssignInCorePrefab]
		public float sipSoundCooldown = 0.5f;

		// Token: 0x04006628 RID: 26152
		[AssignInCorePrefab]
		public Vector3 headToMouthOffset = new Vector3(0f, 0.0208f, 0.171f);

		// Token: 0x04006629 RID: 26153
		[AssignInCorePrefab]
		public float sipRadius = 0.15f;

		// Token: 0x0400662A RID: 26154
		private float lastTimeSipSoundPlayed;

		// Token: 0x0400662B RID: 26155
		private bool wasSipping;

		// Token: 0x0400662C RID: 26156
		private bool coolingDown;

		// Token: 0x0400662D RID: 26157
		private bool wasCoolingDown;

		// Token: 0x0400662E RID: 26158
		private byte[] myByteArray;
	}
}
