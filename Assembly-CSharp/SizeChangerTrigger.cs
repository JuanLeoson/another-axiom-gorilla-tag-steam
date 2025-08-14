using System;
using UnityEngine;

// Token: 0x02000774 RID: 1908
public class SizeChangerTrigger : MonoBehaviour, IBuilderPieceComponent
{
	// Token: 0x14000061 RID: 97
	// (add) Token: 0x06002FD3 RID: 12243 RVA: 0x000FBE64 File Offset: 0x000FA064
	// (remove) Token: 0x06002FD4 RID: 12244 RVA: 0x000FBE9C File Offset: 0x000FA09C
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnEnter;

	// Token: 0x14000062 RID: 98
	// (add) Token: 0x06002FD5 RID: 12245 RVA: 0x000FBED4 File Offset: 0x000FA0D4
	// (remove) Token: 0x06002FD6 RID: 12246 RVA: 0x000FBF0C File Offset: 0x000FA10C
	public event SizeChangerTrigger.SizeChangerTriggerEvent OnExit;

	// Token: 0x06002FD7 RID: 12247 RVA: 0x000FBF41 File Offset: 0x000FA141
	private void Awake()
	{
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06002FD8 RID: 12248 RVA: 0x000FBF4F File Offset: 0x000FA14F
	public void OnTriggerEnter(Collider other)
	{
		if (this.OnEnter != null)
		{
			this.OnEnter(other);
		}
	}

	// Token: 0x06002FD9 RID: 12249 RVA: 0x000FBF65 File Offset: 0x000FA165
	public void OnTriggerExit(Collider other)
	{
		if (this.OnExit != null)
		{
			this.OnExit(other);
		}
	}

	// Token: 0x06002FDA RID: 12250 RVA: 0x000FBF7B File Offset: 0x000FA17B
	public Vector3 ClosestPoint(Vector3 position)
	{
		return this.myCollider.ClosestPoint(position);
	}

	// Token: 0x06002FDB RID: 12251 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPieceCreate(int pieceType, int pieceId)
	{
	}

	// Token: 0x06002FDC RID: 12252 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPieceDestroy()
	{
	}

	// Token: 0x06002FDD RID: 12253 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnPiecePlacementDeserialized()
	{
	}

	// Token: 0x06002FDE RID: 12254 RVA: 0x000FBF89 File Offset: 0x000FA189
	public void OnPieceActivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x06002FDF RID: 12255 RVA: 0x000FBF89 File Offset: 0x000FA189
	public void OnPieceDeactivate()
	{
		Debug.LogError("Size Trigger Pieces no longer work, need reimplementation");
	}

	// Token: 0x04003BED RID: 15341
	private Collider myCollider;

	// Token: 0x04003BF0 RID: 15344
	public bool builderEnterTrigger;

	// Token: 0x04003BF1 RID: 15345
	public bool builderExitOnEnterTrigger;

	// Token: 0x02000775 RID: 1909
	// (Invoke) Token: 0x06002FE2 RID: 12258
	public delegate void SizeChangerTriggerEvent(Collider other);
}
