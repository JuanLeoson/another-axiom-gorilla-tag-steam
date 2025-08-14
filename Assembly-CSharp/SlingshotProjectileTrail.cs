using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020003EA RID: 1002
public class SlingshotProjectileTrail : MonoBehaviour
{
	// Token: 0x06001786 RID: 6022 RVA: 0x0007EF20 File Offset: 0x0007D120
	private void Awake()
	{
		this.initialWidthMultiplier = this.trailRenderer.widthMultiplier;
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x0007EF34 File Offset: 0x0007D134
	public void AttachTrail(GameObject obj, bool blueTeam, bool redTeam)
	{
		this.followObject = obj;
		this.followXform = this.followObject.transform;
		Transform transform = base.transform;
		transform.position = this.followXform.position;
		this.initialScale = transform.localScale.x;
		transform.localScale = this.followXform.localScale;
		this.trailRenderer.widthMultiplier = this.initialWidthMultiplier * this.followXform.localScale.x;
		this.trailRenderer.Clear();
		if (blueTeam)
		{
			this.SetColor(this.blueColor);
		}
		else if (redTeam)
		{
			this.SetColor(this.orangeColor);
		}
		else
		{
			this.SetColor(this.defaultColor);
		}
		this.timeToDie = -1f;
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x0007EFFC File Offset: 0x0007D1FC
	protected void LateUpdate()
	{
		if (this.followObject.IsNull())
		{
			ObjectPools.instance.Destroy(base.gameObject);
			return;
		}
		base.gameObject.transform.position = this.followXform.position;
		if (!this.followObject.activeSelf && this.timeToDie < 0f)
		{
			this.timeToDie = Time.time + this.trailRenderer.time;
		}
		if (this.timeToDie > 0f && Time.time > this.timeToDie)
		{
			base.transform.localScale = Vector3.one * this.initialScale;
			ObjectPools.instance.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x0007F0B8 File Offset: 0x0007D2B8
	public void SetColor(Color color)
	{
		TrailRenderer trailRenderer = this.trailRenderer;
		this.trailRenderer.endColor = color;
		trailRenderer.startColor = color;
	}

	// Token: 0x04001F61 RID: 8033
	public TrailRenderer trailRenderer;

	// Token: 0x04001F62 RID: 8034
	public Color defaultColor = Color.white;

	// Token: 0x04001F63 RID: 8035
	public Color orangeColor = new Color(1f, 0.5f, 0f, 1f);

	// Token: 0x04001F64 RID: 8036
	public Color blueColor = new Color(0f, 0.72f, 1f, 1f);

	// Token: 0x04001F65 RID: 8037
	private GameObject followObject;

	// Token: 0x04001F66 RID: 8038
	private Transform followXform;

	// Token: 0x04001F67 RID: 8039
	private float timeToDie = -1f;

	// Token: 0x04001F68 RID: 8040
	private float initialScale;

	// Token: 0x04001F69 RID: 8041
	private float initialWidthMultiplier;
}
