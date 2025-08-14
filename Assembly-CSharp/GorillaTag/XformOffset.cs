using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaTag
{
	// Token: 0x02000E53 RID: 3667
	[Serializable]
	public struct XformOffset
	{
		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x06005C0A RID: 23562 RVA: 0x001CFE09 File Offset: 0x001CE009
		// (set) Token: 0x06005C0B RID: 23563 RVA: 0x001CFE11 File Offset: 0x001CE011
		[Tooltip("The rotation of the cosmetic relative to the parent bone.")]
		public Quaternion rot
		{
			get
			{
				return this._rotQuat;
			}
			set
			{
				this._rotQuat = value;
			}
		}

		// Token: 0x06005C0C RID: 23564 RVA: 0x001CFE1A File Offset: 0x001CE01A
		public XformOffset(int thisIsAnUnusedDummyValue)
		{
			this.pos = Vector3.zero;
			this._rotQuat = Quaternion.identity;
			this._rotEulerAngles = Vector3.zero;
			this.scale = Vector3.one;
		}

		// Token: 0x06005C0D RID: 23565 RVA: 0x001CFE48 File Offset: 0x001CE048
		public XformOffset(Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.pos = pos;
			this._rotQuat = rot;
			this._rotEulerAngles = rot.eulerAngles;
			this.scale = scale;
		}

		// Token: 0x06005C0E RID: 23566 RVA: 0x001CFE6C File Offset: 0x001CE06C
		public XformOffset(Vector3 pos, Vector3 rot, Vector3 scale)
		{
			this.pos = pos;
			this._rotQuat = Quaternion.Euler(rot);
			this._rotEulerAngles = rot;
			this.scale = scale;
		}

		// Token: 0x06005C0F RID: 23567 RVA: 0x001CFE8F File Offset: 0x001CE08F
		public XformOffset(Vector3 pos, Quaternion rot)
		{
			this.pos = pos;
			this._rotQuat = rot;
			this._rotEulerAngles = rot.eulerAngles;
			this.scale = Vector3.one;
		}

		// Token: 0x06005C10 RID: 23568 RVA: 0x001CFEB7 File Offset: 0x001CE0B7
		public XformOffset(Vector3 pos, Vector3 rot)
		{
			this.pos = pos;
			this._rotQuat = Quaternion.Euler(rot);
			this._rotEulerAngles = rot;
			this.scale = Vector3.one;
		}

		// Token: 0x06005C11 RID: 23569 RVA: 0x001CFEE0 File Offset: 0x001CE0E0
		public XformOffset(Transform boneXform, Transform cosmeticXform)
		{
			this.pos = boneXform.InverseTransformPoint(cosmeticXform.position);
			this._rotQuat = Quaternion.Inverse(boneXform.rotation) * cosmeticXform.rotation;
			this._rotEulerAngles = this._rotQuat.eulerAngles;
			Vector3 lossyScale = boneXform.lossyScale;
			Vector3 lossyScale2 = cosmeticXform.lossyScale;
			this.scale = new Vector3(lossyScale2.x / lossyScale.x, lossyScale2.y / lossyScale.y, lossyScale2.z / lossyScale.z);
		}

		// Token: 0x06005C12 RID: 23570 RVA: 0x001CFF6C File Offset: 0x001CE16C
		public XformOffset(Matrix4x4 matrix)
		{
			this.pos = matrix.GetPosition();
			this.scale = matrix.lossyScale;
			if (Vector3.Dot(Vector3.Cross(matrix.GetColumn(0), matrix.GetColumn(1)), matrix.GetColumn(2)) < 0f)
			{
				this.scale = -this.scale;
			}
			Matrix4x4 matrix4x = matrix;
			matrix4x.SetColumn(0, matrix4x.GetColumn(0) / this.scale.x);
			matrix4x.SetColumn(1, matrix4x.GetColumn(1) / this.scale.y);
			matrix4x.SetColumn(2, matrix4x.GetColumn(2) / this.scale.z);
			this._rotQuat = Quaternion.LookRotation(matrix4x.GetColumn(2), matrix4x.GetColumn(1));
			this._rotEulerAngles = this._rotQuat.eulerAngles;
		}

		// Token: 0x06005C13 RID: 23571 RVA: 0x001D0074 File Offset: 0x001CE274
		public bool Approx(XformOffset other)
		{
			return this.pos.Approx(other.pos, 1E-05f) && this._rotQuat.Approx(other._rotQuat, 1E-06f) && this.scale.Approx(other.scale, 1E-05f);
		}

		// Token: 0x040065D1 RID: 26065
		[Tooltip("The position of the cosmetic relative to the parent bone.")]
		public Vector3 pos;

		// Token: 0x040065D2 RID: 26066
		[FormerlySerializedAs("_edRotQuat")]
		[FormerlySerializedAs("rot")]
		[HideInInspector]
		[SerializeField]
		private Quaternion _rotQuat;

		// Token: 0x040065D3 RID: 26067
		[FormerlySerializedAs("_edRotEulerAngles")]
		[FormerlySerializedAs("_edRotEuler")]
		[HideInInspector]
		[SerializeField]
		private Vector3 _rotEulerAngles;

		// Token: 0x040065D4 RID: 26068
		[Tooltip("The scale of the cosmetic relative to the parent bone.")]
		public Vector3 scale;

		// Token: 0x040065D5 RID: 26069
		public static readonly XformOffset Identity = new XformOffset
		{
			_rotQuat = Quaternion.identity,
			scale = Vector3.one
		};
	}
}
