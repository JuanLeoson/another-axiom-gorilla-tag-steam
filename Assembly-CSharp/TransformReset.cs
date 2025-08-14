using System;
using UnityEngine;

// Token: 0x02000408 RID: 1032
public class TransformReset : MonoBehaviour
{
	// Token: 0x06001816 RID: 6166 RVA: 0x00080C00 File Offset: 0x0007EE00
	private void Awake()
	{
		Transform[] componentsInChildren = base.GetComponentsInChildren<Transform>();
		this.transformList = new TransformReset.OriginalGameObjectTransform[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.transformList[i] = new TransformReset.OriginalGameObjectTransform(componentsInChildren[i]);
		}
		this.ResetTransforms();
	}

	// Token: 0x06001817 RID: 6167 RVA: 0x00080C4C File Offset: 0x0007EE4C
	public void ReturnTransforms()
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.tempTransformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x06001818 RID: 6168 RVA: 0x00080C9C File Offset: 0x0007EE9C
	public void SetScale(float ratio)
	{
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.localScale *= ratio;
		}
	}

	// Token: 0x06001819 RID: 6169 RVA: 0x00080CE0 File Offset: 0x0007EEE0
	public void ResetTransforms()
	{
		this.tempTransformList = new TransformReset.OriginalGameObjectTransform[this.transformList.Length];
		for (int i = 0; i < this.transformList.Length; i++)
		{
			this.tempTransformList[i] = new TransformReset.OriginalGameObjectTransform(this.transformList[i].thisTransform);
		}
		foreach (TransformReset.OriginalGameObjectTransform originalGameObjectTransform in this.transformList)
		{
			originalGameObjectTransform.thisTransform.position = originalGameObjectTransform.thisPosition;
			originalGameObjectTransform.thisTransform.rotation = originalGameObjectTransform.thisRotation;
		}
	}

	// Token: 0x04001FFC RID: 8188
	private TransformReset.OriginalGameObjectTransform[] transformList;

	// Token: 0x04001FFD RID: 8189
	private TransformReset.OriginalGameObjectTransform[] tempTransformList;

	// Token: 0x02000409 RID: 1033
	private struct OriginalGameObjectTransform
	{
		// Token: 0x0600181B RID: 6171 RVA: 0x00080D78 File Offset: 0x0007EF78
		public OriginalGameObjectTransform(Transform constructionTransform)
		{
			this._thisTransform = constructionTransform;
			this._thisPosition = constructionTransform.position;
			this._thisRotation = constructionTransform.rotation;
		}

		// Token: 0x17000291 RID: 657
		// (get) Token: 0x0600181C RID: 6172 RVA: 0x00080D99 File Offset: 0x0007EF99
		// (set) Token: 0x0600181D RID: 6173 RVA: 0x00080DA1 File Offset: 0x0007EFA1
		public Transform thisTransform
		{
			get
			{
				return this._thisTransform;
			}
			set
			{
				this._thisTransform = value;
			}
		}

		// Token: 0x17000292 RID: 658
		// (get) Token: 0x0600181E RID: 6174 RVA: 0x00080DAA File Offset: 0x0007EFAA
		// (set) Token: 0x0600181F RID: 6175 RVA: 0x00080DB2 File Offset: 0x0007EFB2
		public Vector3 thisPosition
		{
			get
			{
				return this._thisPosition;
			}
			set
			{
				this._thisPosition = value;
			}
		}

		// Token: 0x17000293 RID: 659
		// (get) Token: 0x06001820 RID: 6176 RVA: 0x00080DBB File Offset: 0x0007EFBB
		// (set) Token: 0x06001821 RID: 6177 RVA: 0x00080DC3 File Offset: 0x0007EFC3
		public Quaternion thisRotation
		{
			get
			{
				return this._thisRotation;
			}
			set
			{
				this._thisRotation = value;
			}
		}

		// Token: 0x04001FFE RID: 8190
		private Transform _thisTransform;

		// Token: 0x04001FFF RID: 8191
		private Vector3 _thisPosition;

		// Token: 0x04002000 RID: 8192
		private Quaternion _thisRotation;
	}
}
