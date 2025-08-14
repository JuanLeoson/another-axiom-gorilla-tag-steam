using System;
using System.Collections.Generic;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200053C RID: 1340
public class BuilderPiecePrivatePlot : MonoBehaviour
{
	// Token: 0x060020D3 RID: 8403 RVA: 0x000B0206 File Offset: 0x000AE406
	private void Awake()
	{
		this.Init();
	}

	// Token: 0x060020D4 RID: 8404 RVA: 0x000B0210 File Offset: 0x000AE410
	private void Init()
	{
		if (this.initDone)
		{
			return;
		}
		this.materialProps = new MaterialPropertyBlock();
		this.usedResources = new int[3];
		for (int i = 0; i < this.usedResources.Length; i++)
		{
			this.usedResources[i] = 0;
		}
		this.tempResourceCount = new int[3];
		this.piece = base.GetComponent<BuilderPiece>();
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
		this.piecesToCount = new Queue<BuilderPiece>(1024);
		this.initDone = true;
		this.privatePlotIndex = -1;
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x000B0298 File Offset: 0x000AE498
	private void Start()
	{
		if (this.piece != null && this.piece.GetTable() != null)
		{
			BuilderTable table = this.piece.GetTable();
			this.doesLocalPlayerOwnAPlot = table.DoesPlayerOwnPlot(PhotonNetwork.LocalPlayer.ActorNumber);
			table.OnLocalPlayerClaimedPlot.AddListener(new UnityAction<bool>(this.OnLocalPlayerClaimedPlot));
			this.UpdateVisuals();
			foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
			{
				builderResourceMeter.table = this.piece.GetTable();
			}
		}
		this.buildArea.gameObject.SetActive(true);
		this.buildArea.enabled = true;
		this.buildAreaBounds = this.buildArea.bounds;
		this.buildArea.gameObject.SetActive(false);
		this.buildArea.enabled = false;
		this.zoneRenderers.Clear();
		this.zoneRenderers.Add(this.tmpLabel.GetComponent<Renderer>());
		foreach (BuilderResourceMeter builderResourceMeter2 in this.resourceMeters)
		{
			this.zoneRenderers.AddRange(builderResourceMeter2.GetComponentsInChildren<Renderer>());
		}
		this.zoneRenderers.AddRange(this.borderMeshes);
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
		this.inBuilderZone = true;
		this.OnZoneChanged();
	}

	// Token: 0x060020D6 RID: 8406 RVA: 0x000B0454 File Offset: 0x000AE654
	private void OnDestroy()
	{
		if (this.piece != null && this.piece.GetTable() != null)
		{
			this.piece.GetTable().OnLocalPlayerClaimedPlot.RemoveListener(new UnityAction<bool>(this.OnLocalPlayerClaimedPlot));
		}
		if (ZoneManagement.instance != null)
		{
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
		}
	}

	// Token: 0x060020D7 RID: 8407 RVA: 0x000B04D8 File Offset: 0x000AE6D8
	private void OnZoneChanged()
	{
		bool flag = ZoneManagement.instance.IsZoneActive(this.piece.GetTable().tableZone);
		this.inBuilderZone = flag;
	}

	// Token: 0x060020D8 RID: 8408 RVA: 0x000B0507 File Offset: 0x000AE707
	private void OnLocalPlayerClaimedPlot(bool claim)
	{
		this.doesLocalPlayerOwnAPlot = claim;
		this.UpdateVisuals();
	}

	// Token: 0x060020D9 RID: 8409 RVA: 0x000B0518 File Offset: 0x000AE718
	public void UpdatePlot()
	{
		if (BuilderPieceInteractor.instance == null || BuilderPieceInteractor.instance.heldChainLength == null || BuilderPieceInteractor.instance.heldChainLength.Length < 2)
		{
			return;
		}
		if (!PhotonNetwork.InRoom)
		{
			return;
		}
		if (!this.initDone)
		{
			this.Init();
		}
		if ((this.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && this.owningPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber) || (this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && !this.doesLocalPlayerOwnAPlot))
		{
			BuilderPiece parentPiece = BuilderPieceInteractor.instance.prevPotentialPlacement[0].parentPiece;
			BuilderPiece parentPiece2 = BuilderPieceInteractor.instance.prevPotentialPlacement[1].parentPiece;
			bool flag = false;
			if (parentPiece == null && this.leftPotentialParent != null)
			{
				this.isLeftOverPlot = false;
				this.leftPotentialParent = null;
				flag = true;
			}
			else if ((this.leftPotentialParent == null && parentPiece != null) || (parentPiece != null && !parentPiece.Equals(this.leftPotentialParent)))
			{
				BuilderPiece attachedBuiltInPiece = parentPiece.GetAttachedBuiltInPiece();
				this.isLeftOverPlot = (attachedBuiltInPiece != null && attachedBuiltInPiece.Equals(this.piece));
				this.leftPotentialParent = parentPiece;
				flag = true;
			}
			if (parentPiece2 == null && this.rightPotentialParent != null)
			{
				this.isRightOverPlot = false;
				this.rightPotentialParent = null;
				flag = true;
			}
			else if ((this.rightPotentialParent == null && parentPiece2 != null) || (parentPiece2 != null && !parentPiece2.Equals(this.rightPotentialParent)))
			{
				BuilderPiece attachedBuiltInPiece2 = parentPiece2.GetAttachedBuiltInPiece();
				this.isRightOverPlot = (attachedBuiltInPiece2 != null && attachedBuiltInPiece2.Equals(this.piece));
				this.rightPotentialParent = parentPiece2;
				flag = true;
			}
			if (flag)
			{
				this.UpdateVisuals();
			}
		}
		else if (this.isRightOverPlot || this.isLeftOverPlot)
		{
			this.isRightOverPlot = false;
			this.isLeftOverPlot = false;
			this.UpdateVisuals();
		}
		foreach (BuilderResourceMeter builderResourceMeter in this.resourceMeters)
		{
			builderResourceMeter.UpdateMeterFill();
		}
	}

	// Token: 0x060020DA RID: 8410 RVA: 0x000B0750 File Offset: 0x000AE950
	public void RecountPlotCost()
	{
		this.Init();
		this.piece.GetChainCost(this.usedResources);
		this.UpdateVisuals();
	}

	// Token: 0x060020DB RID: 8411 RVA: 0x000B076F File Offset: 0x000AE96F
	public void OnPieceAttachedToPlot(BuilderPiece attachPiece)
	{
		this.AddChainResourcesToCount(attachPiece, true);
		this.UpdateVisuals();
	}

	// Token: 0x060020DC RID: 8412 RVA: 0x000B077F File Offset: 0x000AE97F
	public void OnPieceDetachedFromPlot(BuilderPiece detachPiece)
	{
		this.AddChainResourcesToCount(detachPiece, false);
		this.UpdateVisuals();
	}

	// Token: 0x060020DD RID: 8413 RVA: 0x000B078F File Offset: 0x000AE98F
	public void ChangeAttachedPieceCount(int delta)
	{
		this.attachedPieceCount += delta;
		this.UpdateVisuals();
	}

	// Token: 0x060020DE RID: 8414 RVA: 0x000B07A8 File Offset: 0x000AE9A8
	public void AddChainResourcesToCount(BuilderPiece chain, bool attach)
	{
		if (chain == null)
		{
			return;
		}
		this.piecesToCount.Clear();
		for (int i = 0; i < this.tempResourceCount.Length; i++)
		{
			this.tempResourceCount[i] = 0;
		}
		this.piecesToCount.Enqueue(chain);
		this.AddPieceCostToArray(chain, this.tempResourceCount);
		bool flag = false;
		while (this.piecesToCount.Count > 0 && !flag)
		{
			BuilderPiece builderPiece = this.piecesToCount.Dequeue().firstChildPiece;
			while (builderPiece != null)
			{
				this.piecesToCount.Enqueue(builderPiece);
				if (!this.AddPieceCostToArray(builderPiece, this.tempResourceCount))
				{
					Debug.LogWarning("Builder plot placing pieces over limits");
					flag = true;
					break;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
		for (int j = 0; j < this.usedResources.Length; j++)
		{
			if (attach)
			{
				this.usedResources[j] += this.tempResourceCount[j];
			}
			else
			{
				this.usedResources[j] -= this.tempResourceCount[j];
			}
		}
	}

	// Token: 0x060020DF RID: 8415 RVA: 0x000B08A9 File Offset: 0x000AEAA9
	public void ClaimPlotForPlayerNumber(int player)
	{
		this.owningPlayerActorNumber = player;
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Occupied);
	}

	// Token: 0x060020E0 RID: 8416 RVA: 0x000B08B9 File Offset: 0x000AEAB9
	public int GetOwnerActorNumber()
	{
		return this.owningPlayerActorNumber;
	}

	// Token: 0x060020E1 RID: 8417 RVA: 0x000B08C4 File Offset: 0x000AEAC4
	public void ClearPlot()
	{
		this.Init();
		this.attachedPieceCount = 0;
		for (int i = 0; i < this.usedResources.Length; i++)
		{
			this.usedResources[i] = 0;
		}
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
	}

	// Token: 0x060020E2 RID: 8418 RVA: 0x000B0901 File Offset: 0x000AEB01
	public void FreePlot()
	{
		this.SetPlotState(BuilderPiecePrivatePlot.PlotState.Vacant);
	}

	// Token: 0x060020E3 RID: 8419 RVA: 0x000B090A File Offset: 0x000AEB0A
	public bool IsPlotClaimed()
	{
		return this.plotState > BuilderPiecePrivatePlot.PlotState.Vacant;
	}

	// Token: 0x060020E4 RID: 8420 RVA: 0x000B0918 File Offset: 0x000AEB18
	public bool IsChainUnderCapacity(BuilderPiece chain)
	{
		if (chain == null)
		{
			return true;
		}
		this.piecesToCount.Clear();
		for (int i = 0; i < this.tempResourceCount.Length; i++)
		{
			this.tempResourceCount[i] = this.usedResources[i];
		}
		this.piecesToCount.Enqueue(chain);
		if (!this.AddPieceCostToArray(chain, this.tempResourceCount))
		{
			return false;
		}
		while (this.piecesToCount.Count > 0)
		{
			BuilderPiece builderPiece = this.piecesToCount.Dequeue().firstChildPiece;
			while (builderPiece != null)
			{
				this.piecesToCount.Enqueue(builderPiece);
				if (!this.AddPieceCostToArray(builderPiece, this.tempResourceCount))
				{
					return false;
				}
				builderPiece = builderPiece.nextSiblingPiece;
			}
		}
		return true;
	}

	// Token: 0x060020E5 RID: 8421 RVA: 0x000B09CC File Offset: 0x000AEBCC
	public bool AddPieceCostToArray(BuilderPiece addedPiece, int[] array)
	{
		if (addedPiece == null)
		{
			return true;
		}
		if (addedPiece.cost != null)
		{
			foreach (BuilderResourceQuantity builderResourceQuantity in addedPiece.cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					array[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
					if (array[(int)builderResourceQuantity.type] > this.piece.GetTable().GetPrivateResourceLimitForType((int)builderResourceQuantity.type))
					{
						return false;
					}
				}
			}
			return true;
		}
		return true;
	}

	// Token: 0x060020E6 RID: 8422 RVA: 0x000B0A88 File Offset: 0x000AEC88
	public bool CanPlayerAttachToPlot(int actorNumber)
	{
		return (this.plotState == BuilderPiecePrivatePlot.PlotState.Occupied && this.owningPlayerActorNumber == actorNumber) || (this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant && !this.piece.GetTable().DoesPlayerOwnPlot(actorNumber));
	}

	// Token: 0x060020E7 RID: 8423 RVA: 0x000B0ABC File Offset: 0x000AECBC
	public bool CanPlayerGrabFromPlot(int actorNumber, Vector3 worldPosition)
	{
		if (this.owningPlayerActorNumber == actorNumber || this.plotState == BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			return true;
		}
		int pieceId;
		if (this.piece.GetTable().plotOwners.TryGetValue(actorNumber, out pieceId))
		{
			BuilderPiece builderPiece = this.piece.GetTable().GetPiece(pieceId);
			BuilderPiecePrivatePlot builderPiecePrivatePlot;
			if (builderPiece != null && builderPiece.TryGetPlotComponent(out builderPiecePrivatePlot))
			{
				return builderPiecePrivatePlot.IsLocationWithinPlotExtents(worldPosition);
			}
		}
		return false;
	}

	// Token: 0x060020E8 RID: 8424 RVA: 0x000B0B24 File Offset: 0x000AED24
	private void SetPlotState(BuilderPiecePrivatePlot.PlotState newState)
	{
		this.plotState = newState;
		BuilderPiecePrivatePlot.PlotState plotState = this.plotState;
		if (plotState != BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			if (plotState == BuilderPiecePrivatePlot.PlotState.Occupied)
			{
				if (this.tmpLabel != null && NetworkSystem.Instance != null)
				{
					string text = string.Empty;
					NetPlayer player = NetworkSystem.Instance.GetPlayer(this.owningPlayerActorNumber);
					RigContainer rigContainer;
					if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
					{
						text = rigContainer.Rig.playerNameVisible;
					}
					if (string.IsNullOrEmpty(text) && !this.tmpLabel.text.Equals("OCCUPIED"))
					{
						this.tmpLabel.text = "OCCUPIED";
					}
					else if (!this.tmpLabel.text.Equals(text))
					{
						this.tmpLabel.text = text;
					}
				}
				else if (this.tmpLabel != null && !this.tmpLabel.text.Equals("OCCUPIED"))
				{
					this.tmpLabel.text = "OCCUPIED";
				}
			}
		}
		else
		{
			this.owningPlayerActorNumber = -1;
			if (this.tmpLabel != null && !this.tmpLabel.text.Equals(string.Empty))
			{
				this.tmpLabel.text = string.Empty;
			}
		}
		this.UpdateVisuals();
	}

	// Token: 0x060020E9 RID: 8425 RVA: 0x000B0C78 File Offset: 0x000AEE78
	public bool IsLocationWithinPlotExtents(Vector3 worldPosition)
	{
		if (!this.buildAreaBounds.Contains(worldPosition))
		{
			return false;
		}
		Vector3 vector = this.buildArea.transform.InverseTransformPoint(worldPosition);
		Vector3 vector2 = this.buildArea.center + this.buildArea.size / 2f;
		Vector3 vector3 = this.buildArea.center - this.buildArea.size / 2f;
		return vector.x >= vector3.x && vector.x <= vector2.x && vector.y >= vector3.y && vector.y <= vector2.y && vector.z >= vector3.z && vector.z <= vector2.z;
	}

	// Token: 0x060020EA RID: 8426 RVA: 0x000B0D4C File Offset: 0x000AEF4C
	public void OnAvailableResourceChange()
	{
		this.UpdateVisuals();
	}

	// Token: 0x060020EB RID: 8427 RVA: 0x000B0D54 File Offset: 0x000AEF54
	private void UpdateVisuals()
	{
		if (this.usedResources == null || this.piece.GetTable() == null)
		{
			return;
		}
		BuilderPiecePrivatePlot.PlotState plotState = this.plotState;
		if (plotState != BuilderPiecePrivatePlot.PlotState.Vacant)
		{
			if (plotState != BuilderPiecePrivatePlot.PlotState.Occupied)
			{
				return;
			}
			if (this.owningPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				this.UpdateVisualsForOwner();
				return;
			}
			this.SetBorderColor(this.placementDisallowedColor);
			int num = 0;
			while (num < this.resourceMeters.Count && num < 3)
			{
				int privateResourceLimitForType = this.piece.GetTable().GetPrivateResourceLimitForType(num);
				if (privateResourceLimitForType != 0)
				{
					this.resourceMeters[num].SetNormalizedFillTarget((float)(privateResourceLimitForType - this.usedResources[num]) / (float)privateResourceLimitForType);
				}
				num++;
			}
		}
		else
		{
			if (!this.doesLocalPlayerOwnAPlot)
			{
				this.UpdateVisualsForOwner();
				return;
			}
			this.SetBorderColor(this.placementDisallowedColor);
			for (int i = 0; i < this.resourceMeters.Count; i++)
			{
				if (i >= 3)
				{
					return;
				}
				int privateResourceLimitForType2 = this.piece.GetTable().GetPrivateResourceLimitForType(i);
				if (privateResourceLimitForType2 != 0)
				{
					this.resourceMeters[i].SetNormalizedFillTarget((float)(privateResourceLimitForType2 - this.usedResources[i]) / (float)privateResourceLimitForType2);
				}
			}
			return;
		}
	}

	// Token: 0x060020EC RID: 8428 RVA: 0x000B0E70 File Offset: 0x000AF070
	private void UpdateVisualsForOwner()
	{
		bool flag = true;
		if (this.usedResources == null)
		{
			return;
		}
		if (BuilderPieceInteractor.instance == null || BuilderPieceInteractor.instance.heldChainCost == null)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			int num = this.usedResources[i];
			if (this.isLeftOverPlot)
			{
				num += BuilderPieceInteractor.instance.heldChainCost[0][i];
			}
			if (this.isRightOverPlot)
			{
				num += BuilderPieceInteractor.instance.heldChainCost[1][i];
			}
			int privateResourceLimitForType = this.piece.GetTable().GetPrivateResourceLimitForType(i);
			if (num < privateResourceLimitForType)
			{
				flag = false;
			}
			if (privateResourceLimitForType != 0 && this.resourceMeters.Count > i)
			{
				this.resourceMeters[i].SetNormalizedFillTarget((float)(privateResourceLimitForType - num) / (float)privateResourceLimitForType);
			}
		}
		if (flag)
		{
			this.SetBorderColor(this.placementDisallowedColor);
			return;
		}
		this.SetBorderColor(this.placementAllowedColor);
	}

	// Token: 0x060020ED RID: 8429 RVA: 0x000B0F5C File Offset: 0x000AF15C
	private void SetBorderColor(Color color)
	{
		this.borderMeshes[0].GetPropertyBlock(this.materialProps);
		this.materialProps.SetColor(ShaderProps._BaseColor, color);
		foreach (MeshRenderer meshRenderer in this.borderMeshes)
		{
			meshRenderer.SetPropertyBlock(this.materialProps);
		}
	}

	// Token: 0x040029C4 RID: 10692
	[SerializeField]
	private Color placementAllowedColor;

	// Token: 0x040029C5 RID: 10693
	[SerializeField]
	private Color placementDisallowedColor;

	// Token: 0x040029C6 RID: 10694
	[SerializeField]
	private Color overCapacityColor;

	// Token: 0x040029C7 RID: 10695
	public List<MeshRenderer> borderMeshes;

	// Token: 0x040029C8 RID: 10696
	public BoxCollider buildArea;

	// Token: 0x040029C9 RID: 10697
	[SerializeField]
	private TMP_Text tmpLabel;

	// Token: 0x040029CA RID: 10698
	[SerializeField]
	private List<BuilderResourceMeter> resourceMeters;

	// Token: 0x040029CB RID: 10699
	[NonSerialized]
	public int[] usedResources;

	// Token: 0x040029CC RID: 10700
	[NonSerialized]
	public int[] tempResourceCount;

	// Token: 0x040029CD RID: 10701
	[SerializeField]
	private GameObject plotClaimedFX;

	// Token: 0x040029CE RID: 10702
	private BuilderPiece leftPotentialParent;

	// Token: 0x040029CF RID: 10703
	private BuilderPiece rightPotentialParent;

	// Token: 0x040029D0 RID: 10704
	private bool isLeftOverPlot;

	// Token: 0x040029D1 RID: 10705
	private bool isRightOverPlot;

	// Token: 0x040029D2 RID: 10706
	private Bounds buildAreaBounds;

	// Token: 0x040029D3 RID: 10707
	[HideInInspector]
	public BuilderPiece piece;

	// Token: 0x040029D4 RID: 10708
	private int owningPlayerActorNumber;

	// Token: 0x040029D5 RID: 10709
	private int attachedPieceCount;

	// Token: 0x040029D6 RID: 10710
	[HideInInspector]
	public int privatePlotIndex;

	// Token: 0x040029D7 RID: 10711
	[HideInInspector]
	public BuilderPiecePrivatePlot.PlotState plotState;

	// Token: 0x040029D8 RID: 10712
	private bool doesLocalPlayerOwnAPlot;

	// Token: 0x040029D9 RID: 10713
	private Queue<BuilderPiece> piecesToCount;

	// Token: 0x040029DA RID: 10714
	private bool initDone;

	// Token: 0x040029DB RID: 10715
	private MaterialPropertyBlock materialProps;

	// Token: 0x040029DC RID: 10716
	private List<Renderer> zoneRenderers = new List<Renderer>(12);

	// Token: 0x040029DD RID: 10717
	private bool inBuilderZone;

	// Token: 0x0200053D RID: 1341
	public enum PlotState
	{
		// Token: 0x040029DF RID: 10719
		Vacant,
		// Token: 0x040029E0 RID: 10720
		Occupied
	}
}
