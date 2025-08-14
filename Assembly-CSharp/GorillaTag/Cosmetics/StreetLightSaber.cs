using System;
using System.Collections.Generic;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02000F63 RID: 3939
	public class StreetLightSaber : MonoBehaviour
	{
		// Token: 0x17000963 RID: 2403
		// (get) Token: 0x0600618B RID: 24971 RVA: 0x001F0447 File Offset: 0x001EE647
		private StreetLightSaber.State CurrentState
		{
			get
			{
				return StreetLightSaber.values[this.currentIndex];
			}
		}

		// Token: 0x0600618C RID: 24972 RVA: 0x001F0458 File Offset: 0x001EE658
		private void Awake()
		{
			foreach (StreetLightSaber.StaffStates staffStates in this.allStates)
			{
				this.allStatesDict[staffStates.state] = staffStates;
			}
			this.currentIndex = 0;
			this.autoSwitchEnabledTime = 0f;
			this.hashId = Shader.PropertyToID(this.shaderColorProperty);
			Material[] sharedMaterials = this.meshRenderer.sharedMaterials;
			this.instancedMaterial = new Material(sharedMaterials[this.materialIndex]);
			sharedMaterials[this.materialIndex] = this.instancedMaterial;
			this.meshRenderer.sharedMaterials = sharedMaterials;
		}

		// Token: 0x0600618D RID: 24973 RVA: 0x001F04EC File Offset: 0x001EE6EC
		private void Update()
		{
			if (this.autoSwitch && Time.time - this.autoSwitchEnabledTime > this.autoSwitchTimer)
			{
				this.UpdateStateAuto();
			}
		}

		// Token: 0x0600618E RID: 24974 RVA: 0x001F0510 File Offset: 0x001EE710
		private void OnDestroy()
		{
			this.allStatesDict.Clear();
		}

		// Token: 0x0600618F RID: 24975 RVA: 0x001F051D File Offset: 0x001EE71D
		private void OnEnable()
		{
			this.ForceSwitchTo(StreetLightSaber.State.Off);
		}

		// Token: 0x06006190 RID: 24976 RVA: 0x001F0528 File Offset: 0x001EE728
		public void UpdateStateManual()
		{
			int newIndex = (this.currentIndex + 1) % StreetLightSaber.values.Length;
			this.SwitchState(newIndex);
		}

		// Token: 0x06006191 RID: 24977 RVA: 0x001F0550 File Offset: 0x001EE750
		private void UpdateStateAuto()
		{
			StreetLightSaber.State value = (this.CurrentState == StreetLightSaber.State.Green) ? StreetLightSaber.State.Red : StreetLightSaber.State.Green;
			int newIndex = Array.IndexOf<StreetLightSaber.State>(StreetLightSaber.values, value);
			this.SwitchState(newIndex);
			this.autoSwitchEnabledTime = Time.time;
		}

		// Token: 0x06006192 RID: 24978 RVA: 0x001F0589 File Offset: 0x001EE789
		public void EnableAutoSwitch(bool enable)
		{
			this.autoSwitch = enable;
		}

		// Token: 0x06006193 RID: 24979 RVA: 0x001F051D File Offset: 0x001EE71D
		public void ResetStaff()
		{
			this.ForceSwitchTo(StreetLightSaber.State.Off);
		}

		// Token: 0x06006194 RID: 24980 RVA: 0x001F0594 File Offset: 0x001EE794
		public void HitReceived(Vector3 contact)
		{
			if (this.velocityTracker != null && this.velocityTracker.GetLatestVelocity(true).magnitude >= this.minHitVelocityThreshold)
			{
				StreetLightSaber.StaffStates staffStates = this.allStatesDict[this.CurrentState];
				if (staffStates == null)
				{
					return;
				}
				staffStates.OnSuccessfulHit.Invoke(contact);
			}
		}

		// Token: 0x06006195 RID: 24981 RVA: 0x001F05EC File Offset: 0x001EE7EC
		private void SwitchState(int newIndex)
		{
			if (newIndex == this.currentIndex)
			{
				return;
			}
			StreetLightSaber.State currentState = this.CurrentState;
			StreetLightSaber.State key = StreetLightSaber.values[newIndex];
			StreetLightSaber.StaffStates staffStates;
			if (this.allStatesDict.TryGetValue(currentState, out staffStates))
			{
				UnityEvent onExitState = staffStates.onExitState;
				if (onExitState != null)
				{
					onExitState.Invoke();
				}
			}
			this.currentIndex = newIndex;
			StreetLightSaber.StaffStates staffStates2;
			if (this.allStatesDict.TryGetValue(key, out staffStates2))
			{
				UnityEvent onEnterState = staffStates2.onEnterState;
				if (onEnterState != null)
				{
					onEnterState.Invoke();
				}
				if (this.trailRenderer != null)
				{
					this.trailRenderer.startColor = staffStates2.color;
				}
				if (this.meshRenderer != null)
				{
					this.instancedMaterial.SetColor(this.hashId, staffStates2.color);
				}
			}
		}

		// Token: 0x06006196 RID: 24982 RVA: 0x001F06A0 File Offset: 0x001EE8A0
		private void ForceSwitchTo(StreetLightSaber.State targetState)
		{
			int num = Array.IndexOf<StreetLightSaber.State>(StreetLightSaber.values, targetState);
			if (num >= 0)
			{
				this.SwitchState(num);
			}
		}

		// Token: 0x04006DB3 RID: 28083
		[SerializeField]
		private float autoSwitchTimer = 5f;

		// Token: 0x04006DB4 RID: 28084
		[SerializeField]
		private TrailRenderer trailRenderer;

		// Token: 0x04006DB5 RID: 28085
		[SerializeField]
		private Renderer meshRenderer;

		// Token: 0x04006DB6 RID: 28086
		[SerializeField]
		private string shaderColorProperty;

		// Token: 0x04006DB7 RID: 28087
		[SerializeField]
		private int materialIndex;

		// Token: 0x04006DB8 RID: 28088
		[SerializeField]
		private GorillaVelocityTracker velocityTracker;

		// Token: 0x04006DB9 RID: 28089
		[SerializeField]
		private float minHitVelocityThreshold;

		// Token: 0x04006DBA RID: 28090
		private static readonly StreetLightSaber.State[] values = (StreetLightSaber.State[])Enum.GetValues(typeof(StreetLightSaber.State));

		// Token: 0x04006DBB RID: 28091
		[Space]
		[Header("Staff State Settings")]
		public StreetLightSaber.StaffStates[] allStates = new StreetLightSaber.StaffStates[0];

		// Token: 0x04006DBC RID: 28092
		private int currentIndex;

		// Token: 0x04006DBD RID: 28093
		private Dictionary<StreetLightSaber.State, StreetLightSaber.StaffStates> allStatesDict = new Dictionary<StreetLightSaber.State, StreetLightSaber.StaffStates>();

		// Token: 0x04006DBE RID: 28094
		private bool autoSwitch;

		// Token: 0x04006DBF RID: 28095
		private float autoSwitchEnabledTime;

		// Token: 0x04006DC0 RID: 28096
		private int hashId;

		// Token: 0x04006DC1 RID: 28097
		private Material instancedMaterial;

		// Token: 0x02000F64 RID: 3940
		[Serializable]
		public class StaffStates
		{
			// Token: 0x04006DC2 RID: 28098
			public StreetLightSaber.State state;

			// Token: 0x04006DC3 RID: 28099
			public Color color;

			// Token: 0x04006DC4 RID: 28100
			public UnityEvent onEnterState;

			// Token: 0x04006DC5 RID: 28101
			public UnityEvent onExitState;

			// Token: 0x04006DC6 RID: 28102
			public UnityEvent<Vector3> OnSuccessfulHit;
		}

		// Token: 0x02000F65 RID: 3941
		public enum State
		{
			// Token: 0x04006DC8 RID: 28104
			Off,
			// Token: 0x04006DC9 RID: 28105
			Green,
			// Token: 0x04006DCA RID: 28106
			Red
		}
	}
}
