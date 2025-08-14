using System;
using UnityEngine;

// Token: 0x020000C6 RID: 198
public class MetroBlimp : MonoBehaviour
{
	// Token: 0x060004F0 RID: 1264 RVA: 0x0001CD93 File Offset: 0x0001AF93
	private void Awake()
	{
		this._startLocalHeight = base.transform.localPosition.y;
	}

	// Token: 0x060004F1 RID: 1265 RVA: 0x0001CDAC File Offset: 0x0001AFAC
	public void Tick()
	{
		bool flag = Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f < 0.0001f;
		int num = Mathf.CeilToInt(this._numHandsOnBlimp / 2f);
		if (this._numHandsOnBlimp == 0f)
		{
			this._topStayTime = 0f;
			if (flag)
			{
				this.blimpRenderer.material.DisableKeyword("_INNER_GLOW");
			}
		}
		else
		{
			this._topStayTime += Time.deltaTime;
			if (flag)
			{
				this.blimpRenderer.material.EnableKeyword("_INNER_GLOW");
			}
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 vector = localPosition;
		float y = vector.y;
		float num2 = this._startLocalHeight + this.descendOffset;
		float deltaTime = Time.deltaTime;
		if (num > 0)
		{
			if (y > num2)
			{
				vector += Vector3.down * (this.descendSpeed * (float)num * deltaTime);
			}
		}
		else if (y < this._startLocalHeight)
		{
			vector += Vector3.up * (this.ascendSpeed * deltaTime);
		}
		base.transform.localPosition = Vector3.Slerp(localPosition, vector, 0.5f);
	}

	// Token: 0x060004F2 RID: 1266 RVA: 0x0001CEDB File Offset: 0x0001B0DB
	private static bool IsPlayerHand(Collider c)
	{
		return c.gameObject.IsOnLayer(UnityLayer.GorillaHand);
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x0001CEEA File Offset: 0x0001B0EA
	private void OnTriggerEnter(Collider other)
	{
		if (MetroBlimp.IsPlayerHand(other))
		{
			this._numHandsOnBlimp += 1f;
		}
	}

	// Token: 0x060004F4 RID: 1268 RVA: 0x0001CF06 File Offset: 0x0001B106
	private void OnTriggerExit(Collider other)
	{
		if (MetroBlimp.IsPlayerHand(other))
		{
			this._numHandsOnBlimp -= 1f;
		}
	}

	// Token: 0x040005DD RID: 1501
	public MetroSpotlight spotLightLeft;

	// Token: 0x040005DE RID: 1502
	public MetroSpotlight spotLightRight;

	// Token: 0x040005DF RID: 1503
	[Space]
	public BoxCollider topCollider;

	// Token: 0x040005E0 RID: 1504
	public Material blimpMaterial;

	// Token: 0x040005E1 RID: 1505
	public Renderer blimpRenderer;

	// Token: 0x040005E2 RID: 1506
	[Space]
	public float ascendSpeed = 1f;

	// Token: 0x040005E3 RID: 1507
	public float descendSpeed = 0.5f;

	// Token: 0x040005E4 RID: 1508
	public float descendOffset = -24.1f;

	// Token: 0x040005E5 RID: 1509
	public float descendReactionTime = 3f;

	// Token: 0x040005E6 RID: 1510
	[Space]
	[NonSerialized]
	private float _startLocalHeight;

	// Token: 0x040005E7 RID: 1511
	[NonSerialized]
	private float _topStayTime;

	// Token: 0x040005E8 RID: 1512
	[NonSerialized]
	private float _numHandsOnBlimp;

	// Token: 0x040005E9 RID: 1513
	[NonSerialized]
	private bool _lowering;

	// Token: 0x040005EA RID: 1514
	private const string _INNER_GLOW = "_INNER_GLOW";
}
