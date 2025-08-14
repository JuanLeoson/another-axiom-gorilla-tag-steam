using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C06 RID: 3078
	public class BuilderRecycler : MonoBehaviour
	{
		// Token: 0x06004AFE RID: 19198 RVA: 0x0016C858 File Offset: 0x0016AA58
		private void Awake()
		{
			this.hasFans = (this.effectBehaviors.Count > 0 && this.bladeSoundPlayer != null && this.recycleParticles != null);
			this.hasPipes = (this.outputPipes.Count > 0);
		}

		// Token: 0x06004AFF RID: 19199 RVA: 0x0016C8AC File Offset: 0x0016AAAC
		private void Start()
		{
			if (this.hasPipes)
			{
				this.numPipes = Mathf.Min(this.outputPipes.Count, 3);
				this.props = new MaterialPropertyBlock();
				this.ResetOutputPipes();
				this.totalRecycledCost = new int[3];
				this.currentChainCost = new int[3];
				for (int i = 0; i < this.totalRecycledCost.Length; i++)
				{
					this.totalRecycledCost[i] = 0;
					this.currentChainCost[i] = 0;
				}
			}
			this.zoneRenderers.Clear();
			if (this.hasPipes)
			{
				this.zoneRenderers.AddRange(this.outputPipes);
			}
			if (this.hasFans)
			{
				foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
				{
					Renderer component = monoBehaviour.GetComponent<Renderer>();
					if (component != null)
					{
						this.zoneRenderers.Add(component);
					}
				}
			}
			this.inBuilderZone = true;
			ZoneManagement instance = ZoneManagement.instance;
			instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
			this.OnZoneChanged();
		}

		// Token: 0x06004B00 RID: 19200 RVA: 0x0016C9E0 File Offset: 0x0016ABE0
		private void OnDestroy()
		{
			if (ZoneManagement.instance != null)
			{
				ZoneManagement instance = ZoneManagement.instance;
				instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
			}
		}

		// Token: 0x06004B01 RID: 19201 RVA: 0x0016CA18 File Offset: 0x0016AC18
		private void OnZoneChanged()
		{
			bool flag = ZoneManagement.instance.IsZoneActive(GTZone.monkeBlocks);
			if (flag && !this.inBuilderZone)
			{
				using (List<Renderer>.Enumerator enumerator = this.zoneRenderers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Renderer renderer = enumerator.Current;
						renderer.enabled = true;
					}
					goto IL_8B;
				}
			}
			if (!flag && this.inBuilderZone)
			{
				foreach (Renderer renderer2 in this.zoneRenderers)
				{
					renderer2.enabled = false;
				}
			}
			IL_8B:
			this.inBuilderZone = flag;
		}

		// Token: 0x06004B02 RID: 19202 RVA: 0x0016CAD4 File Offset: 0x0016ACD4
		private void OnTriggerEnter(Collider other)
		{
			BuilderPiece builderPieceFromCollider = BuilderPiece.GetBuilderPieceFromCollider(other);
			if (builderPieceFromCollider == null)
			{
				return;
			}
			if (!builderPieceFromCollider.isBuiltIntoTable && !builderPieceFromCollider.isArmShelf)
			{
				this.table.RequestRecyclePiece(builderPieceFromCollider, true, this.recyclerID);
			}
		}

		// Token: 0x06004B03 RID: 19203 RVA: 0x0016CB18 File Offset: 0x0016AD18
		public void OnRecycleRequestedAtRecycler(BuilderPiece piece)
		{
			if (this.hasPipes)
			{
				this.AddPieceCost(piece.cost);
			}
			if (this.hasFans)
			{
				foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
				{
					monoBehaviour.enabled = true;
				}
				this.recycleParticles.SetActive(true);
				this.bladeSoundPlayer.Play();
				this.timeToStopBlades = (double)(Time.time + this.recycleEffectDuration);
				this.playingBladeEffect = true;
			}
		}

		// Token: 0x06004B04 RID: 19204 RVA: 0x0016CBB8 File Offset: 0x0016ADB8
		private void AddPieceCost(BuilderResources cost)
		{
			foreach (BuilderResourceQuantity builderResourceQuantity in cost.quantities)
			{
				if (builderResourceQuantity.type >= BuilderResourceType.Basic && builderResourceQuantity.type < BuilderResourceType.Count)
				{
					this.totalRecycledCost[(int)builderResourceQuantity.type] += builderResourceQuantity.count;
				}
			}
			if (!this.playingPipeEffect)
			{
				this.UpdatePipeLoop();
			}
		}

		// Token: 0x06004B05 RID: 19205 RVA: 0x0016CC40 File Offset: 0x0016AE40
		private Vector2 GetUVShiftOffset()
		{
			float y = Shader.GetGlobalVector(ShaderProps._Time).y;
			Vector4 vector = new Vector4(500f, 0f, 0f, 0f);
			Vector4 vector2 = vector / this.recycleEffectDuration;
			return new Vector2(-1f * (Mathf.Floor(y * vector2.x) * 1f / vector.x % 1f) * vector.x - vector.x + 165f, 0f);
		}

		// Token: 0x06004B06 RID: 19206 RVA: 0x0016CCCC File Offset: 0x0016AECC
		private void UpdatePipeLoop()
		{
			bool flag = false;
			for (int i = 0; i < this.numPipes; i++)
			{
				if (this.totalRecycledCost[i] > 0)
				{
					flag = true;
					this.outputPipes[i].GetPropertyBlock(this.props, 1);
					Vector4 value = new Vector4(500f, 0f, 0f, 0f) / this.recycleEffectDuration;
					Vector2 uvshiftOffset = this.GetUVShiftOffset();
					this.props.SetColor(ShaderProps._BaseColor, this.builderResourceColors.colors[i].color);
					this.props.SetVector(ShaderProps._UvShiftRate, value);
					this.props.SetVector(ShaderProps._UvShiftOffset, uvshiftOffset);
					this.outputPipes[i].SetPropertyBlock(this.props, 1);
					this.totalRecycledCost[i] = Mathf.Max(this.totalRecycledCost[i] - 1, 0);
				}
				else
				{
					this.outputPipes[i].GetPropertyBlock(this.props, 1);
					this.props.SetColor(ShaderProps._BaseColor, Color.black);
					this.outputPipes[i].SetPropertyBlock(this.props, 1);
				}
			}
			if (flag)
			{
				this.playingPipeEffect = true;
				this.timeToCheckPipes = (double)(Time.time + this.recycleEffectDuration);
				return;
			}
			this.playingPipeEffect = false;
		}

		// Token: 0x06004B07 RID: 19207 RVA: 0x0016CE30 File Offset: 0x0016B030
		private void ResetOutputPipes()
		{
			foreach (MeshRenderer meshRenderer in this.outputPipes)
			{
				meshRenderer.GetPropertyBlock(this.props, 1);
				this.props.SetColor(ShaderProps._BaseColor, Color.black);
				meshRenderer.SetPropertyBlock(this.props, 1);
			}
		}

		// Token: 0x06004B08 RID: 19208 RVA: 0x0016CEAC File Offset: 0x0016B0AC
		public void UpdateRecycler()
		{
			if (this.playingBladeEffect && (double)Time.time > this.timeToStopBlades)
			{
				if (this.hasFans)
				{
					foreach (MonoBehaviour monoBehaviour in this.effectBehaviors)
					{
						monoBehaviour.enabled = false;
					}
					this.recycleParticles.SetActive(false);
				}
				this.playingBladeEffect = false;
			}
			if (this.playingPipeEffect && (double)Time.time > this.timeToCheckPipes)
			{
				this.UpdatePipeLoop();
			}
		}

		// Token: 0x040053E5 RID: 21477
		public float recycleEffectDuration = 0.25f;

		// Token: 0x040053E6 RID: 21478
		private double timeToStopBlades = double.MinValue;

		// Token: 0x040053E7 RID: 21479
		private bool playingBladeEffect;

		// Token: 0x040053E8 RID: 21480
		private bool playingPipeEffect;

		// Token: 0x040053E9 RID: 21481
		private double timeToCheckPipes = double.MinValue;

		// Token: 0x040053EA RID: 21482
		public List<MonoBehaviour> effectBehaviors;

		// Token: 0x040053EB RID: 21483
		public GameObject recycleParticles;

		// Token: 0x040053EC RID: 21484
		public SoundBankPlayer bladeSoundPlayer;

		// Token: 0x040053ED RID: 21485
		public List<MeshRenderer> outputPipes;

		// Token: 0x040053EE RID: 21486
		public BuilderResourceColors builderResourceColors;

		// Token: 0x040053EF RID: 21487
		private bool hasFans;

		// Token: 0x040053F0 RID: 21488
		private bool hasPipes;

		// Token: 0x040053F1 RID: 21489
		private MaterialPropertyBlock props;

		// Token: 0x040053F2 RID: 21490
		private int[] totalRecycledCost;

		// Token: 0x040053F3 RID: 21491
		private int[] currentChainCost;

		// Token: 0x040053F4 RID: 21492
		private int numPipes;

		// Token: 0x040053F5 RID: 21493
		internal int recyclerID = -1;

		// Token: 0x040053F6 RID: 21494
		internal BuilderTable table;

		// Token: 0x040053F7 RID: 21495
		private List<Renderer> zoneRenderers = new List<Renderer>(10);

		// Token: 0x040053F8 RID: 21496
		private bool inBuilderZone;
	}
}
