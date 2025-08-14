using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000BEA RID: 3050
	public class Mole : Tappable
	{
		// Token: 0x14000084 RID: 132
		// (add) Token: 0x06004A08 RID: 18952 RVA: 0x00167D0C File Offset: 0x00165F0C
		// (remove) Token: 0x06004A09 RID: 18953 RVA: 0x00167D44 File Offset: 0x00165F44
		public event Mole.MoleTapEvent OnTapped;

		// Token: 0x17000720 RID: 1824
		// (get) Token: 0x06004A0A RID: 18954 RVA: 0x00167D79 File Offset: 0x00165F79
		// (set) Token: 0x06004A0B RID: 18955 RVA: 0x00167D81 File Offset: 0x00165F81
		public bool IsLeftSideMole { get; set; }

		// Token: 0x06004A0C RID: 18956 RVA: 0x00167D8C File Offset: 0x00165F8C
		private void Awake()
		{
			this.currentState = Mole.MoleState.Hidden;
			Vector3 position = base.transform.position;
			this.origin = (this.target = position);
			this.visiblePosition = new Vector3(position.x, position.y + this.positionOffset, position.z);
			this.hiddenPosition = new Vector3(position.x, position.y - this.positionOffset, position.z);
			this.travelTime = this.normalTravelTime;
			this.animCurve = (this.normalAnimCurve = AnimationCurves.EaseInOutQuad);
			this.hitAnimCurve = AnimationCurves.EaseOutBack;
			for (int i = 0; i < this.moleTypes.Length; i++)
			{
				if (this.moleTypes[i].isHazard)
				{
					this.hazardMoles.Add(i);
				}
				else
				{
					this.safeMoles.Add(i);
				}
			}
			this.randomMolePickedIndex = -1;
		}

		// Token: 0x06004A0D RID: 18957 RVA: 0x00167E74 File Offset: 0x00166074
		public void InvokeUpdate()
		{
			if (this.currentState == Mole.MoleState.Ready)
			{
				return;
			}
			switch (this.currentState)
			{
			case Mole.MoleState.Reset:
			case Mole.MoleState.Hidden:
				this.currentState = Mole.MoleState.Ready;
				break;
			case Mole.MoleState.TransitionToVisible:
			case Mole.MoleState.TransitionToHidden:
			{
				float num = this.animCurve.Evaluate(Mathf.Clamp01((Time.time - this.animStartTime) / this.travelTime));
				base.transform.position = Vector3.Lerp(this.origin, this.target, num);
				if (num >= 1f)
				{
					this.currentState++;
				}
				break;
			}
			}
			if (Time.time - this.currentTime >= this.showMoleDuration && this.currentState > Mole.MoleState.Ready && this.currentState < Mole.MoleState.TransitionToHidden)
			{
				this.HideMole(false);
			}
		}

		// Token: 0x06004A0E RID: 18958 RVA: 0x00167F3F File Offset: 0x0016613F
		public bool CanPickMole()
		{
			return this.currentState == Mole.MoleState.Ready;
		}

		// Token: 0x06004A0F RID: 18959 RVA: 0x00167F4C File Offset: 0x0016614C
		public void ShowMole(float _showMoleDuration, int randomMoleTypeIndex)
		{
			if (randomMoleTypeIndex >= this.moleTypes.Length || randomMoleTypeIndex < 0)
			{
				return;
			}
			this.randomMolePickedIndex = randomMoleTypeIndex;
			for (int i = 0; i < this.moleTypes.Length; i++)
			{
				this.moleTypes[i].gameObject.SetActive(i == randomMoleTypeIndex);
				if (this.moleTypes[i].monkeMoleDefaultMaterial != null)
				{
					this.moleTypes[i].MeshRenderer.material = this.moleTypes[i].monkeMoleDefaultMaterial;
				}
			}
			this.showMoleDuration = _showMoleDuration;
			this.origin = base.transform.position;
			this.target = this.visiblePosition;
			this.animCurve = this.normalAnimCurve;
			this.currentState = Mole.MoleState.TransitionToVisible;
			this.animStartTime = (this.currentTime = Time.time);
			this.travelTime = this.normalTravelTime;
		}

		// Token: 0x06004A10 RID: 18960 RVA: 0x00168024 File Offset: 0x00166224
		public void HideMole(bool isHit = false)
		{
			if (this.currentState < Mole.MoleState.TransitionToVisible || this.currentState > Mole.MoleState.Visible)
			{
				return;
			}
			this.origin = base.transform.position;
			this.target = this.hiddenPosition;
			this.animCurve = (isHit ? this.hitAnimCurve : this.normalAnimCurve);
			this.animStartTime = Time.time;
			this.travelTime = (isHit ? this.hitTravelTime : this.normalTravelTime);
			this.currentState = Mole.MoleState.TransitionToHidden;
		}

		// Token: 0x06004A11 RID: 18961 RVA: 0x001680A4 File Offset: 0x001662A4
		public bool CanTap()
		{
			Mole.MoleState moleState = this.currentState;
			return moleState == Mole.MoleState.TransitionToVisible || moleState == Mole.MoleState.Visible;
		}

		// Token: 0x06004A12 RID: 18962 RVA: 0x001680C9 File Offset: 0x001662C9
		public override bool CanTap(bool isLeftHand)
		{
			return this.CanTap();
		}

		// Token: 0x06004A13 RID: 18963 RVA: 0x001680D4 File Offset: 0x001662D4
		public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
		{
			if (!this.CanTap())
			{
				return;
			}
			bool flag = info.Sender.ActorNumber == NetworkSystem.Instance.LocalPlayerID;
			bool isLeft = flag && GorillaTagger.Instance.lastLeftTap >= GorillaTagger.Instance.lastRightTap;
			MoleTypes moleTypes = null;
			if (this.randomMolePickedIndex >= 0 && this.randomMolePickedIndex < this.moleTypes.Length)
			{
				moleTypes = this.moleTypes[this.randomMolePickedIndex];
			}
			if (moleTypes != null)
			{
				Mole.MoleTapEvent onTapped = this.OnTapped;
				if (onTapped == null)
				{
					return;
				}
				onTapped(moleTypes, base.transform.position, flag, isLeft);
			}
		}

		// Token: 0x06004A14 RID: 18964 RVA: 0x00168173 File Offset: 0x00166373
		public void ResetPosition()
		{
			base.transform.position = this.hiddenPosition;
			this.currentState = Mole.MoleState.Reset;
		}

		// Token: 0x06004A15 RID: 18965 RVA: 0x0016818D File Offset: 0x0016638D
		public int GetMoleTypeIndex(bool useHazardMole)
		{
			if (!useHazardMole)
			{
				return this.safeMoles[Random.Range(0, this.safeMoles.Count)];
			}
			return this.hazardMoles[Random.Range(0, this.hazardMoles.Count)];
		}

		// Token: 0x040052D0 RID: 21200
		public float positionOffset = 0.2f;

		// Token: 0x040052D1 RID: 21201
		public MoleTypes[] moleTypes;

		// Token: 0x040052D2 RID: 21202
		private float showMoleDuration;

		// Token: 0x040052D3 RID: 21203
		private Vector3 visiblePosition;

		// Token: 0x040052D4 RID: 21204
		private Vector3 hiddenPosition;

		// Token: 0x040052D5 RID: 21205
		private float currentTime;

		// Token: 0x040052D6 RID: 21206
		private float animStartTime;

		// Token: 0x040052D7 RID: 21207
		private float travelTime;

		// Token: 0x040052D8 RID: 21208
		private float normalTravelTime = 0.3f;

		// Token: 0x040052D9 RID: 21209
		private float hitTravelTime = 0.2f;

		// Token: 0x040052DA RID: 21210
		private AnimationCurve animCurve;

		// Token: 0x040052DB RID: 21211
		private AnimationCurve normalAnimCurve;

		// Token: 0x040052DC RID: 21212
		private AnimationCurve hitAnimCurve;

		// Token: 0x040052DD RID: 21213
		private Mole.MoleState currentState;

		// Token: 0x040052DE RID: 21214
		private Vector3 origin;

		// Token: 0x040052DF RID: 21215
		private Vector3 target;

		// Token: 0x040052E0 RID: 21216
		private int randomMolePickedIndex;

		// Token: 0x040052E2 RID: 21218
		public CallLimiter rpcCooldown;

		// Token: 0x040052E3 RID: 21219
		private int moleScore;

		// Token: 0x040052E4 RID: 21220
		private List<int> safeMoles = new List<int>();

		// Token: 0x040052E5 RID: 21221
		private List<int> hazardMoles = new List<int>();

		// Token: 0x02000BEB RID: 3051
		// (Invoke) Token: 0x06004A18 RID: 18968
		public delegate void MoleTapEvent(MoleTypes moleType, Vector3 position, bool isLocalTap, bool isLeft);

		// Token: 0x02000BEC RID: 3052
		public enum MoleState
		{
			// Token: 0x040052E8 RID: 21224
			Reset,
			// Token: 0x040052E9 RID: 21225
			Ready,
			// Token: 0x040052EA RID: 21226
			TransitionToVisible,
			// Token: 0x040052EB RID: 21227
			Visible,
			// Token: 0x040052EC RID: 21228
			TransitionToHidden,
			// Token: 0x040052ED RID: 21229
			Hidden
		}
	}
}
