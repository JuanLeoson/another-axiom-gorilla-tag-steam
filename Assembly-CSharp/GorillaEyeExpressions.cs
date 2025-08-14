using System;
using UnityEngine;

// Token: 0x020006C4 RID: 1732
public class GorillaEyeExpressions : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06002AD2 RID: 10962 RVA: 0x000E3C1D File Offset: 0x000E1E1D
	private void Awake()
	{
		this.loudness = base.GetComponent<GorillaSpeakerLoudness>();
	}

	// Token: 0x06002AD3 RID: 10963 RVA: 0x000E3C2B File Offset: 0x000E1E2B
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.timeLastUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x06002AD4 RID: 10964 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06002AD5 RID: 10965 RVA: 0x000E3C4A File Offset: 0x000E1E4A
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.timeLastUpdated;
		this.timeLastUpdated = Time.time;
		this.CheckEyeEffects();
		this.UpdateEyeExpression();
	}

	// Token: 0x06002AD6 RID: 10966 RVA: 0x000E3C78 File Offset: 0x000E1E78
	private void CheckEyeEffects()
	{
		if (this.loudness == null)
		{
			this.loudness = base.GetComponent<GorillaSpeakerLoudness>();
		}
		if (this.loudness.IsSpeaking && this.loudness.Loudness > this.screamVolume)
		{
			this.overrideDuration = this.screamDuration;
			this.overrideUV = this.ScreamUV;
			return;
		}
		if (this.overrideDuration > 0f)
		{
			this.overrideDuration -= this.deltaTime;
			if (this.overrideDuration <= 0f)
			{
				this.overrideUV = this.BaseUV;
			}
		}
	}

	// Token: 0x06002AD7 RID: 10967 RVA: 0x000E3D14 File Offset: 0x000E1F14
	private void UpdateEyeExpression()
	{
		this.targetFace.GetComponent<Renderer>().material.SetVector(this._BaseMap_ST, new Vector4(0.5f, 1f, this.overrideUV.x, this.overrideUV.y));
	}

	// Token: 0x0400365D RID: 13917
	public GameObject targetFace;

	// Token: 0x0400365E RID: 13918
	[Space]
	[SerializeField]
	private float screamVolume = 0.2f;

	// Token: 0x0400365F RID: 13919
	[SerializeField]
	private float screamDuration = 0.5f;

	// Token: 0x04003660 RID: 13920
	[SerializeField]
	private Vector2 ScreamUV = new Vector2(0.8f, 0f);

	// Token: 0x04003661 RID: 13921
	private Vector2 BaseUV = Vector3.zero;

	// Token: 0x04003662 RID: 13922
	private GorillaSpeakerLoudness loudness;

	// Token: 0x04003663 RID: 13923
	private float overrideDuration;

	// Token: 0x04003664 RID: 13924
	private Vector2 overrideUV;

	// Token: 0x04003665 RID: 13925
	private float timeLastUpdated;

	// Token: 0x04003666 RID: 13926
	private float deltaTime;

	// Token: 0x04003667 RID: 13927
	private ShaderHashId _BaseMap_ST = "_BaseMap_ST";
}
