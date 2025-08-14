using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F0A RID: 3850
	public class TalkingSkullWearableHelper : MonoBehaviour, IGorillaSliceableSimple
	{
		// Token: 0x06005F56 RID: 24406 RVA: 0x001E16C0 File Offset: 0x001DF8C0
		public void Awake()
		{
			this._materialPropertyBlock = new MaterialPropertyBlock();
		}

		// Token: 0x06005F57 RID: 24407 RVA: 0x001E16D0 File Offset: 0x001DF8D0
		private void Start()
		{
			this._helpers = new List<TalkingSkullHelper>();
			base.transform.root.GetComponentsInChildren<TalkingSkullHelper>(this._helpers);
			this._helpers.RemoveAll((TalkingSkullHelper helper) => helper.TalkingCosmeticType != this.TalkingCosmeticType);
			VRRig componentInParent = base.GetComponentInParent<VRRig>();
			this._speakerHeadCollider = ((componentInParent != null) ? componentInParent.rigContainer.HeadCollider : null);
			this._headDistanceSqr = this.HeadDistance * this.HeadDistance;
		}

		// Token: 0x06005F58 RID: 24408 RVA: 0x001E1746 File Offset: 0x001DF946
		public void OnEnable()
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
			this.SetJewelColor(this.JewelColorOff, this.EmissiveColorOff);
		}

		// Token: 0x06005F59 RID: 24409 RVA: 0x00010F78 File Offset: 0x0000F178
		public void OnDisable()
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		}

		// Token: 0x06005F5A RID: 24410 RVA: 0x001E1764 File Offset: 0x001DF964
		public void SliceUpdate()
		{
			this._deltaTime = Time.time - this._timeLastUpdated;
			this._timeLastUpdated = Time.time;
			if (this._speakerHeadCollider == null)
			{
				return;
			}
			this._toggleCooldown -= Time.deltaTime;
			if (this._toggleCooldown <= 0f)
			{
				if ((base.transform.position - this._speakerHeadCollider.transform.position).sqrMagnitude < this._headDistanceSqr)
				{
					this.ToggleBand(true);
					return;
				}
				this.ToggleBand(false);
			}
		}

		// Token: 0x06005F5B RID: 24411 RVA: 0x001E17FC File Offset: 0x001DF9FC
		private void ToggleBand(bool toggle)
		{
			if (toggle)
			{
				for (int i = 0; i < this._helpers.Count; i++)
				{
					this._helpers[i].CanTalk(true);
				}
				if (this._meshRenderer)
				{
					this.SetJewelColor(this.JewelColorOn, this.EmissiveColorOn);
				}
			}
			else
			{
				for (int j = 0; j < this._helpers.Count; j++)
				{
					this._helpers[j].CanTalk(false);
				}
				if (this._meshRenderer)
				{
					this.SetJewelColor(this.JewelColorOff, this.EmissiveColorOff);
				}
			}
			this._toggleCooldown = this.ToggleCooldown;
		}

		// Token: 0x06005F5C RID: 24412 RVA: 0x001E18A8 File Offset: 0x001DFAA8
		private void SetJewelColor(Color jewelColor, Color emissiveColor)
		{
			this._materialPropertyBlock.SetColor("_BaseColor", jewelColor);
			this._materialPropertyBlock.SetColor("_EmissionColor", emissiveColor);
			this._meshRenderer.SetPropertyBlock(this._materialPropertyBlock, 0);
		}

		// Token: 0x040069FF RID: 27135
		public TalkingCosmeticType TalkingCosmeticType;

		// Token: 0x04006A00 RID: 27136
		public float HeadDistance = 0.5f;

		// Token: 0x04006A01 RID: 27137
		public float ToggleCooldown = 0.5f;

		// Token: 0x04006A02 RID: 27138
		public Color JewelColorOff = Color.black;

		// Token: 0x04006A03 RID: 27139
		public Color JewelColorOn = Color.white;

		// Token: 0x04006A04 RID: 27140
		public Color EmissiveColorOff = Color.white;

		// Token: 0x04006A05 RID: 27141
		public Color EmissiveColorOn = Color.white;

		// Token: 0x04006A06 RID: 27142
		[SerializeField]
		private List<TalkingSkullHelper> _helpers;

		// Token: 0x04006A07 RID: 27143
		[SerializeField]
		private Collider _speakerHeadCollider;

		// Token: 0x04006A08 RID: 27144
		[SerializeField]
		private MeshRenderer _meshRenderer;

		// Token: 0x04006A09 RID: 27145
		private float _deltaTime;

		// Token: 0x04006A0A RID: 27146
		private float _timeLastUpdated;

		// Token: 0x04006A0B RID: 27147
		private float _headDistanceSqr = 1f;

		// Token: 0x04006A0C RID: 27148
		private float _toggleCooldown;

		// Token: 0x04006A0D RID: 27149
		private MaterialPropertyBlock _materialPropertyBlock;
	}
}
