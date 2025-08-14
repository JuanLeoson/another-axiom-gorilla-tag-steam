using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C2D RID: 3117
	[NetworkBehaviourWeaved(13)]
	public class FlowersManager : NetworkComponent
	{
		// Token: 0x1700073B RID: 1851
		// (get) Token: 0x06004CB3 RID: 19635 RVA: 0x0017C501 File Offset: 0x0017A701
		// (set) Token: 0x06004CB4 RID: 19636 RVA: 0x0017C508 File Offset: 0x0017A708
		public static FlowersManager Instance { get; private set; }

		// Token: 0x06004CB5 RID: 19637 RVA: 0x0017C510 File Offset: 0x0017A710
		protected override void Awake()
		{
			base.Awake();
			FlowersManager.Instance = this;
			this.hitNotifiers = base.GetComponentsInChildren<SlingshotProjectileHitNotifier>();
			foreach (SlingshotProjectileHitNotifier slingshotProjectileHitNotifier in this.hitNotifiers)
			{
				if (slingshotProjectileHitNotifier != null)
				{
					slingshotProjectileHitNotifier.OnProjectileTriggerEnter += this.ProjectileHitReceiver;
				}
				else
				{
					Debug.LogError("Needs SlingshotProjectileHitNotifier added to this GameObject children");
				}
			}
			foreach (FlowersManager.FlowersInZone flowersInZone in this.sections)
			{
				foreach (GameObject gameObject in flowersInZone.sections)
				{
					this.sectionToZonesDict[gameObject] = flowersInZone.zone;
					Flower[] componentsInChildren = gameObject.GetComponentsInChildren<Flower>();
					this.allFlowers.AddRange(componentsInChildren);
					this.sectionToFlowersDict[gameObject] = componentsInChildren.ToList<Flower>();
				}
			}
		}

		// Token: 0x06004CB6 RID: 19638 RVA: 0x0017C634 File Offset: 0x0017A834
		private new void Start()
		{
			NetworkSystem.Instance.RegisterSceneNetworkItem(base.gameObject);
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
			if (base.IsMine)
			{
				foreach (Flower flower in this.allFlowers)
				{
					flower.UpdateFlowerState(Flower.FlowerState.Healthy, false, false);
				}
			}
		}

		// Token: 0x06004CB7 RID: 19639 RVA: 0x0017C6C8 File Offset: 0x0017A8C8
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			foreach (SlingshotProjectileHitNotifier slingshotProjectileHitNotifier in this.hitNotifiers)
			{
				if (slingshotProjectileHitNotifier != null)
				{
					slingshotProjectileHitNotifier.OnProjectileTriggerEnter -= this.ProjectileHitReceiver;
				}
			}
			FlowersManager.Instance = null;
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.HandleOnZoneChanged));
		}

		// Token: 0x06004CB8 RID: 19640 RVA: 0x0017C73B File Offset: 0x0017A93B
		private void ProjectileHitReceiver(SlingshotProjectile projectile, Collider collider)
		{
			if (!projectile.CompareTag("WaterBalloonProjectile"))
			{
				return;
			}
			this.WaterFlowers(collider);
		}

		// Token: 0x06004CB9 RID: 19641 RVA: 0x0017C754 File Offset: 0x0017A954
		private void WaterFlowers(Collider collider)
		{
			if (!base.IsMine)
			{
				return;
			}
			GameObject gameObject = collider.gameObject;
			if (gameObject == null)
			{
				Debug.LogError("Could not find any flowers section");
				return;
			}
			foreach (Flower flower in this.sectionToFlowersDict[gameObject])
			{
				flower.WaterFlower(true);
			}
		}

		// Token: 0x06004CBA RID: 19642 RVA: 0x0017C7D0 File Offset: 0x0017A9D0
		private void HandleOnZoneChanged()
		{
			foreach (KeyValuePair<GameObject, GTZone> keyValuePair in this.sectionToZonesDict)
			{
				bool enable = ZoneManagement.instance.IsZoneActive(keyValuePair.Value);
				foreach (Flower flower in this.sectionToFlowersDict[keyValuePair.Key])
				{
					flower.UpdateVisuals(enable);
				}
			}
		}

		// Token: 0x06004CBB RID: 19643 RVA: 0x0017C87C File Offset: 0x0017AA7C
		public int GetHealthyFlowersInZoneCount(GTZone zone)
		{
			int num = 0;
			foreach (KeyValuePair<GameObject, GTZone> keyValuePair in this.sectionToZonesDict)
			{
				if (keyValuePair.Value == zone)
				{
					using (List<Flower>.Enumerator enumerator2 = this.sectionToFlowersDict[keyValuePair.Key].GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							if (enumerator2.Current.GetCurrentState() == Flower.FlowerState.Healthy)
							{
								num++;
							}
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06004CBC RID: 19644 RVA: 0x0017C928 File Offset: 0x0017AB28
		protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			stream.SendNext(this.allFlowers.Count);
			for (int i = 0; i < this.allFlowers.Count; i++)
			{
				stream.SendNext(this.allFlowers[i].IsWatered);
				stream.SendNext(this.allFlowers[i].GetCurrentState());
			}
		}

		// Token: 0x06004CBD RID: 19645 RVA: 0x0017C9A8 File Offset: 0x0017ABA8
		protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != PhotonNetwork.MasterClient)
			{
				return;
			}
			int num = (int)stream.ReceiveNext();
			for (int i = 0; i < num; i++)
			{
				bool isWatered = (bool)stream.ReceiveNext();
				Flower.FlowerState currentState = this.allFlowers[i].GetCurrentState();
				Flower.FlowerState flowerState = (Flower.FlowerState)stream.ReceiveNext();
				if (currentState != flowerState)
				{
					this.allFlowers[i].UpdateFlowerState(flowerState, isWatered, true);
				}
			}
		}

		// Token: 0x1700073C RID: 1852
		// (get) Token: 0x06004CBE RID: 19646 RVA: 0x0017CA1B File Offset: 0x0017AC1B
		// (set) Token: 0x06004CBF RID: 19647 RVA: 0x0017CA45 File Offset: 0x0017AC45
		[Networked]
		[NetworkedWeaved(0, 13)]
		private unsafe FlowersDataStruct Data
		{
			get
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FlowersManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				return *(FlowersDataStruct*)(this.Ptr + 0);
			}
			set
			{
				if (this.Ptr == null)
				{
					throw new InvalidOperationException("Error when accessing FlowersManager.Data. Networked properties can only be accessed when Spawned() has been called.");
				}
				*(FlowersDataStruct*)(this.Ptr + 0) = value;
			}
		}

		// Token: 0x06004CC0 RID: 19648 RVA: 0x0017CA70 File Offset: 0x0017AC70
		public override void WriteDataFusion()
		{
			if (base.HasStateAuthority)
			{
				this.Data = new FlowersDataStruct(this.allFlowers);
			}
		}

		// Token: 0x06004CC1 RID: 19649 RVA: 0x0017CA8C File Offset: 0x0017AC8C
		public override void ReadDataFusion()
		{
			if (this.Data.FlowerCount > 0)
			{
				for (int i = 0; i < this.Data.FlowerCount; i++)
				{
					bool isWatered = this.Data.FlowerWateredData[i] == 1;
					Flower.FlowerState currentState = this.allFlowers[i].GetCurrentState();
					Flower.FlowerState flowerState = (Flower.FlowerState)this.Data.FlowerStateData[i];
					if (currentState != flowerState)
					{
						this.allFlowers[i].UpdateFlowerState(flowerState, isWatered, true);
					}
				}
			}
		}

		// Token: 0x06004CC2 RID: 19650 RVA: 0x0017CB24 File Offset: 0x0017AD24
		private void Update()
		{
			int num = this.flowerCheckIndex + 1;
			while (num < this.allFlowers.Count && num < this.flowerCheckIndex + this.flowersToCheck)
			{
				this.allFlowers[num].AnimCatch();
				num++;
			}
			this.flowerCheckIndex = ((this.flowerCheckIndex + this.flowersToCheck >= this.allFlowers.Count) ? 0 : (this.flowerCheckIndex + this.flowersToCheck));
		}

		// Token: 0x06004CC4 RID: 19652 RVA: 0x0017CBCF File Offset: 0x0017ADCF
		[WeaverGenerated]
		public override void CopyBackingFieldsToState(bool A_1)
		{
			base.CopyBackingFieldsToState(A_1);
			this.Data = this._Data;
		}

		// Token: 0x06004CC5 RID: 19653 RVA: 0x0017CBE7 File Offset: 0x0017ADE7
		[WeaverGenerated]
		public override void CopyStateToBackingFields()
		{
			base.CopyStateToBackingFields();
			this._Data = this.Data;
		}

		// Token: 0x040055D0 RID: 21968
		public List<FlowersManager.FlowersInZone> sections;

		// Token: 0x040055D1 RID: 21969
		public int flowersToCheck = 1;

		// Token: 0x040055D2 RID: 21970
		public int flowerCheckIndex;

		// Token: 0x040055D3 RID: 21971
		private readonly List<Flower> allFlowers = new List<Flower>();

		// Token: 0x040055D4 RID: 21972
		private SlingshotProjectileHitNotifier[] hitNotifiers;

		// Token: 0x040055D5 RID: 21973
		private readonly Dictionary<GameObject, List<Flower>> sectionToFlowersDict = new Dictionary<GameObject, List<Flower>>();

		// Token: 0x040055D6 RID: 21974
		private readonly Dictionary<GameObject, GTZone> sectionToZonesDict = new Dictionary<GameObject, GTZone>();

		// Token: 0x040055D7 RID: 21975
		private bool hasBeenSerialized;

		// Token: 0x040055D8 RID: 21976
		[WeaverGenerated]
		[DefaultForProperty("Data", 0, 13)]
		[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
		private FlowersDataStruct _Data;

		// Token: 0x02000C2E RID: 3118
		[Serializable]
		public class FlowersInZone
		{
			// Token: 0x040055D9 RID: 21977
			public GTZone zone;

			// Token: 0x040055DA RID: 21978
			public List<GameObject> sections;
		}
	}
}
