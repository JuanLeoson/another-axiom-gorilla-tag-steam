using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004EC RID: 1260
public class AtticHider : MonoBehaviour
{
	// Token: 0x06001E99 RID: 7833 RVA: 0x000A1C8A File Offset: 0x0009FE8A
	private void Start()
	{
		this.OnZoneChanged();
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Combine(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001E9A RID: 7834 RVA: 0x000A1CB8 File Offset: 0x0009FEB8
	private void OnDestroy()
	{
		ZoneManagement instance = ZoneManagement.instance;
		instance.onZoneChanged = (Action)Delegate.Remove(instance.onZoneChanged, new Action(this.OnZoneChanged));
	}

	// Token: 0x06001E9B RID: 7835 RVA: 0x000A1CE0 File Offset: 0x0009FEE0
	private void OnZoneChanged()
	{
		if (this.AtticRenderer == null)
		{
			return;
		}
		if (ZoneManagement.instance.IsZoneActive(GTZone.attic))
		{
			if (this._coroutine != null)
			{
				base.StopCoroutine(this._coroutine);
				this._coroutine = null;
			}
			this._coroutine = base.StartCoroutine(this.WaitForAtticLoad());
			return;
		}
		if (this._coroutine != null)
		{
			base.StopCoroutine(this._coroutine);
			this._coroutine = null;
		}
		this.AtticRenderer.enabled = true;
	}

	// Token: 0x06001E9C RID: 7836 RVA: 0x000A1D5F File Offset: 0x0009FF5F
	private IEnumerator WaitForAtticLoad()
	{
		while (!ZoneManagement.instance.IsSceneLoaded(GTZone.attic))
		{
			yield return new WaitForSeconds(0.2f);
		}
		yield return null;
		this.AtticRenderer.enabled = false;
		this._coroutine = null;
		yield break;
	}

	// Token: 0x04002746 RID: 10054
	[SerializeField]
	private MeshRenderer AtticRenderer;

	// Token: 0x04002747 RID: 10055
	private Coroutine _coroutine;
}
