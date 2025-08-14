using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class DeployedChild : MonoBehaviour
{
	// Token: 0x060009FF RID: 2559 RVA: 0x00036D2C File Offset: 0x00034F2C
	public void Deploy(DeployableObject parent, Vector3 launchPos, Quaternion launchRot, Vector3 releaseVel, bool isRemote = false)
	{
		this._parent = parent;
		this._parent.DeployChild();
		Transform transform = base.transform;
		transform.position = launchPos;
		transform.rotation = launchRot;
		transform.localScale = this._parent.transform.lossyScale;
		this._rigidbody.velocity = releaseVel;
		this._isRemote = isRemote;
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x00036D89 File Offset: 0x00034F89
	public void ReturnToParent(float delay)
	{
		if (delay > 0f)
		{
			base.StartCoroutine(this.ReturnToParentDelayed(delay));
			return;
		}
		if (this._parent != null)
		{
			this._parent.ReturnChild();
		}
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x00036DBB File Offset: 0x00034FBB
	private IEnumerator ReturnToParentDelayed(float delay)
	{
		float start = Time.time;
		while (Time.time < start + delay)
		{
			yield return null;
		}
		if (this._parent != null)
		{
			this._parent.ReturnChild();
		}
		yield break;
	}

	// Token: 0x04000C0B RID: 3083
	[SerializeField]
	private Rigidbody _rigidbody;

	// Token: 0x04000C0C RID: 3084
	[SerializeReference]
	private DeployableObject _parent;

	// Token: 0x04000C0D RID: 3085
	private bool _isRemote;
}
