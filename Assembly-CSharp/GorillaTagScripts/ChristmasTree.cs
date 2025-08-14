using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000C23 RID: 3107
	[NetworkBehaviourWeaved(1)]
	public class ChristmasTree : NetworkComponent
	{
		// Token: 0x06004C64 RID: 19556 RVA: 0x0017AF48 File Offset: 0x00179148
		protected override void Awake()
		{
			base.Awake();
			foreach (AttachPoint attachPoint in this.hangers.GetComponentsInChildren<AttachPoint>())
			{
				this.attachPointsList.Add(attachPoint);
				AttachPoint attachPoint2 = attachPoint;
				attachPoint2.onHookedChanged = (UnityAction)Delegate.Combine(attachPoint2.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.lightRenderers = this.lights.GetComponentsInChildren<MeshRenderer>();
			MeshRenderer[] array = this.lightRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].material = this.lightsOffMaterial;
			}
			this.wasActive = false;
			this.isActive = false;
		}

		// Token: 0x06004C65 RID: 19557 RVA: 0x0017AFE9 File Offset: 0x001791E9
		private void Update()
		{
			if (this.spinTheTop && this.topOrnament)
			{
				this.topOrnament.transform.Rotate(0f, this.spinSpeed * Time.deltaTime, 0f, Space.World);
			}
		}

		// Token: 0x06004C66 RID: 19558 RVA: 0x0017B028 File Offset: 0x00179228
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (AttachPoint attachPoint in this.attachPointsList)
			{
				attachPoint.onHookedChanged = (UnityAction)Delegate.Remove(attachPoint.onHookedChanged, new UnityAction(this.UpdateHangers));
			}
			this.attachPointsList.Clear();
		}

		// Token: 0x06004C67 RID: 19559 RVA: 0x0017B0A8 File Offset: 0x001792A8
		private void UpdateHangers()
		{
			if (this.attachPointsList.Count == 0)
			{
				return;
			}
			using (List<AttachPoint>.Enumerator enumerator = this.attachPointsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsHooked())
					{
						if (base.IsMine)
						{
							this.updateLight(true);
						}
						return;
					}
				}
			}
			if (base.IsMine)
			{
				this.updateLight(false);
			}
		}

		// Token: 0x06004C68 RID: 19560 RVA: 0x0017B128 File Offset: 0x00179328
		private void updateLight(bool enable)
		{
			this.isActive = enable;
			for (int i = 0; i < this.lightRenderers.Length; i++)
			{
				this.lightRenderers[i].material = (enable ? this.lightsOnMaterials[i % this.lightsOnMaterials.Length] : this.lightsOffMaterial);
			}
			this.spinTheTop = enable;
		}

		// Token: 0x17000737 RID: 1847
		// (get) Token: 0x06004C69 RID: 19561 RVA: 0x0017B17F File Offset: 0x0017937F
		// (set) Token: 0x06004C6A RID: 19562 RVA: 0x0017B1A9 File Offset: 0x001793A9
		[Networked]
		[NetworkedWeaved(0, 1)]
		private unsafe NetworkBool Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ChristmasTree.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(NetworkBool*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing ChristmasTree.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(NetworkBool*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06004C6B RID: 19563 RVA: 0x0017B1D4 File Offset: 0x001793D4
		public override void WriteDataFusion()
		{
			this.Data = this.isActive;
		}

		// Token: 0x06004C6C RID: 19564 RVA: 0x0017B1E7 File Offset: 0x001793E7
		public override void ReadDataFusion()
		{
			this.wasActive = this.isActive;
			this.isActive = this.Data;
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x06004C6D RID: 19565 RVA: 0x0017B220 File Offset: 0x00179420
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			stream.SendNext(this.isActive);
		}

		// Token: 0x06004C6E RID: 19566 RVA: 0x0017B244 File Offset: 0x00179444
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (!info.Sender.IsMasterClient)
			{
				return;
			}
			this.wasActive = this.isActive;
			this.isActive = (bool)stream.ReceiveNext();
			if (this.wasActive != this.isActive)
			{
				this.updateLight(this.isActive);
			}
		}

		// Token: 0x06004C70 RID: 19568 RVA: 0x0017B2B4 File Offset: 0x001794B4
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06004C71 RID: 19569 RVA: 0x0017B2CC File Offset: 0x001794CC
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x04005584 RID: 21892
		public GameObject hangers;

		// Token: 0x04005585 RID: 21893
		public GameObject lights;

		// Token: 0x04005586 RID: 21894
		public GameObject topOrnament;

		// Token: 0x04005587 RID: 21895
		public float spinSpeed = 60f;

		// Token: 0x04005588 RID: 21896
		private readonly List<AttachPoint> attachPointsList = new List<AttachPoint>();

		// Token: 0x04005589 RID: 21897
		private MeshRenderer[] lightRenderers;

		// Token: 0x0400558A RID: 21898
		private bool wasActive;

		// Token: 0x0400558B RID: 21899
		private bool isActive;

		// Token: 0x0400558C RID: 21900
		private bool spinTheTop;

		// Token: 0x0400558D RID: 21901
		[SerializeField]
		private Material lightsOffMaterial;

		// Token: 0x0400558E RID: 21902
		[SerializeField]
		private Material[] lightsOnMaterials;

		// Token: 0x0400558F RID: 21903
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 1)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private NetworkBool _Data;
	}
}
