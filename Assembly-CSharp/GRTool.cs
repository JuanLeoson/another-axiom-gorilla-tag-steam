using System;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000673 RID: 1651
public class GRTool : MonoBehaviour, IGameEntitySerialize, IGameEntityComponent
{
	// Token: 0x14000050 RID: 80
	// (add) Token: 0x0600285B RID: 10331 RVA: 0x000D9510 File Offset: 0x000D7710
	// (remove) Token: 0x0600285C RID: 10332 RVA: 0x000D9548 File Offset: 0x000D7748
	public event GRTool.EnergyChangeEvent OnEnergyChange;

	// Token: 0x0600285D RID: 10333 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Awake()
	{
	}

	// Token: 0x0600285E RID: 10334 RVA: 0x000D957D File Offset: 0x000D777D
	private void Start()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		this.RefreshMeters();
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x000D959F File Offset: 0x000D779F
	public void OnEntityInit()
	{
		this.energy = this.GetEnergyStart();
	}

	// Token: 0x06002860 RID: 10336 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002861 RID: 10337 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnEntityStateChange(long prevState, long nextState)
	{
	}

	// Token: 0x06002862 RID: 10338 RVA: 0x000D95AD File Offset: 0x000D77AD
	public int GetEnergyMax()
	{
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyMax);
	}

	// Token: 0x06002863 RID: 10339 RVA: 0x000D95BB File Offset: 0x000D77BB
	public int GetEnergyUseCost()
	{
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyUseCost);
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x000D95C9 File Offset: 0x000D77C9
	public int GetEnergyStart()
	{
		return this.attributes.CalculateFinalValueForAttribute(GRAttributeType.EnergyStart);
	}

	// Token: 0x06002865 RID: 10341 RVA: 0x000D95D7 File Offset: 0x000D77D7
	private void OnEnable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
	}

	// Token: 0x06002866 RID: 10342 RVA: 0x000D9600 File Offset: 0x000D7800
	private void OnDisable()
	{
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.GrabbedByPlayer));
	}

	// Token: 0x06002867 RID: 10343 RVA: 0x000D9629 File Offset: 0x000D7829
	public void RefillEnergy(int count, GameEntityId chargingEntityId)
	{
		this.SetEnergyInternal(this.energy + count, chargingEntityId);
	}

	// Token: 0x06002868 RID: 10344 RVA: 0x000D963A File Offset: 0x000D783A
	public void RefillEnergy()
	{
		this.SetEnergyInternal(this.GetEnergyMax(), GameEntityId.Invalid);
	}

	// Token: 0x06002869 RID: 10345 RVA: 0x000D964D File Offset: 0x000D784D
	public void UseEnergy()
	{
		this.SetEnergyInternal(this.energy - this.GetEnergyUseCost(), GameEntityId.Invalid);
	}

	// Token: 0x0600286A RID: 10346 RVA: 0x000D9667 File Offset: 0x000D7867
	public bool HasEnoughEnergy()
	{
		return this.energy >= this.GetEnergyUseCost();
	}

	// Token: 0x0600286B RID: 10347 RVA: 0x000D967A File Offset: 0x000D787A
	public void SetEnergy(int newEnergy)
	{
		this.SetEnergyInternal(newEnergy, GameEntityId.Invalid);
	}

	// Token: 0x0600286C RID: 10348 RVA: 0x000D9688 File Offset: 0x000D7888
	public bool IsEnergyFull()
	{
		return this.energy >= this.GetEnergyMax();
	}

	// Token: 0x0600286D RID: 10349 RVA: 0x000D969C File Offset: 0x000D789C
	private void SetEnergyInternal(int value, GameEntityId chargingEntityId)
	{
		int num = this.energy;
		this.energy = Mathf.Clamp(value, 0, this.GetEnergyMax());
		int energyChange = this.energy - num;
		GRTool.EnergyChangeEvent onEnergyChange = this.OnEnergyChange;
		if (onEnergyChange != null)
		{
			onEnergyChange(this, energyChange, chargingEntityId);
		}
		this.RefreshMeters();
	}

	// Token: 0x0600286E RID: 10350 RVA: 0x000D96E8 File Offset: 0x000D78E8
	public void RefreshMeters()
	{
		for (int i = 0; i < this.energyMeters.Count; i++)
		{
			this.energyMeters[i].Refresh();
		}
	}

	// Token: 0x0600286F RID: 10351 RVA: 0x000D971C File Offset: 0x000D791C
	public void UpgradeTool(string upgradeID)
	{
		for (int i = 0; i < this.upgrades.Count; i++)
		{
			if (this.upgrades[i].Id == upgradeID)
			{
				this.ClearUpgradeSlot(this.upgrades[i].Slot);
				for (int j = 0; j < this.upgrades[i].VisibleItem.Count; j++)
				{
					this.upgrades[i].VisibleItem[j].SetActive(true);
				}
				for (int k = 0; k < this.upgradeSlots[this.upgrades[i].Slot].DefaultVisibleItems.Count; k++)
				{
					this.upgradeSlots[this.upgrades[i].Slot].DefaultVisibleItems[k].SetActive(false);
				}
				foreach (GRBonusEntry entry in this.upgrades[i].bonusEffects)
				{
					this.attributes.AddBonus(entry);
				}
				this.upgradeSlots[this.upgrades[i].Slot].installedItem = this.upgrades[i];
			}
		}
	}

	// Token: 0x06002870 RID: 10352 RVA: 0x000D989C File Offset: 0x000D7A9C
	public void ClearUpgradeSlot(int slot)
	{
		if (this.upgradeSlots[slot].installedItem != null)
		{
			for (int i = 0; i < this.upgradeSlots[slot].installedItem.VisibleItem.Count; i++)
			{
				this.upgradeSlots[slot].installedItem.VisibleItem[i].SetActive(false);
			}
			foreach (GRBonusEntry entry in this.upgradeSlots[slot].installedItem.bonusEffects)
			{
				this.attributes.RemoveBonus(entry);
			}
			for (int j = 0; j < this.upgradeSlots[slot].DefaultVisibleItems.Count; j++)
			{
				this.upgradeSlots[slot].DefaultVisibleItems[j].SetActive(true);
			}
		}
	}

	// Token: 0x06002871 RID: 10353 RVA: 0x000D99A0 File Offset: 0x000D7BA0
	public void OnGameEntitySerialize(BinaryWriter writer)
	{
		writer.Write(this.upgradeSlots.Count);
		for (int i = 0; i < this.upgradeSlots.Count; i++)
		{
			if (this.upgradeSlots[i] != null)
			{
				if (this.upgradeSlots[i].installedItem != null)
				{
					writer.Write(this.upgradeSlots[i].installedItem.Id);
				}
				else
				{
					writer.Write("");
				}
			}
			else
			{
				writer.Write("");
			}
		}
		writer.Write(this.energy);
	}

	// Token: 0x06002872 RID: 10354 RVA: 0x000D9A38 File Offset: 0x000D7C38
	public void OnGameEntityDeserialize(BinaryReader reader)
	{
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string upgradeID = reader.ReadString();
			this.UpgradeTool(upgradeID);
		}
		int num2 = reader.ReadInt32();
		this.SetEnergy(num2);
	}

	// Token: 0x06002873 RID: 10355 RVA: 0x000D9A74 File Offset: 0x000D7C74
	public void GrabbedByPlayer()
	{
		if (this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
		{
			GRPlayer grplayer = GRPlayer.Get(this.gameEntity.heldByActorNumber);
			if (grplayer)
			{
				grplayer.GrabbedItem(this.gameEntity.id, base.gameObject.name);
			}
		}
	}

	// Token: 0x040033D6 RID: 13270
	public GRAttributes attributes;

	// Token: 0x040033D7 RID: 13271
	public List<GRTool.Upgrade> upgrades;

	// Token: 0x040033D8 RID: 13272
	public List<GRTool.UpgradeSlot> upgradeSlots = new List<GRTool.UpgradeSlot>();

	// Token: 0x040033D9 RID: 13273
	public List<GRMeterEnergy> energyMeters;

	// Token: 0x040033DA RID: 13274
	public List<Transform> upgradeParts;

	// Token: 0x040033DB RID: 13275
	public GameEntity gameEntity;

	// Token: 0x040033DC RID: 13276
	public string ToolResearchID;

	// Token: 0x040033DD RID: 13277
	[ReadOnly]
	public int energy;

	// Token: 0x02000674 RID: 1652
	[Serializable]
	public class Upgrade
	{
		// Token: 0x040033DF RID: 13279
		public string Id;

		// Token: 0x040033E0 RID: 13280
		public int Slot;

		// Token: 0x040033E1 RID: 13281
		public List<GameObject> VisibleItem;

		// Token: 0x040033E2 RID: 13282
		public List<GRBonusEntry> bonusEffects;
	}

	// Token: 0x02000675 RID: 1653
	[Serializable]
	public class UpgradeSlot
	{
		// Token: 0x040033E3 RID: 13283
		public List<GameObject> DefaultVisibleItems;

		// Token: 0x040033E4 RID: 13284
		[NonSerialized]
		public GRTool.Upgrade installedItem;
	}

	// Token: 0x02000676 RID: 1654
	// (Invoke) Token: 0x06002878 RID: 10360
	public delegate void EnergyChangeEvent(GRTool tool, int energyChange, GameEntityId chargingEntityId);
}
