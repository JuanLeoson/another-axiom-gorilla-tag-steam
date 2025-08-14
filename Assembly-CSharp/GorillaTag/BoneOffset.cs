using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E4E RID: 3662
	[Serializable]
	public struct BoneOffset
	{
		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x06005BF9 RID: 23545 RVA: 0x001CFD04 File Offset: 0x001CDF04
		public Vector3 pos
		{
			get
			{
				return this.offset.pos;
			}
		}

		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x06005BFA RID: 23546 RVA: 0x001CFD11 File Offset: 0x001CDF11
		public Quaternion rot
		{
			get
			{
				return this.offset.rot;
			}
		}

		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06005BFB RID: 23547 RVA: 0x001CFD1E File Offset: 0x001CDF1E
		public Vector3 scale
		{
			get
			{
				return this.offset.scale;
			}
		}

		// Token: 0x06005BFC RID: 23548 RVA: 0x001CFD2B File Offset: 0x001CDF2B
		public BoneOffset(GTHardCodedBones.EBone bone)
		{
			this.bone = bone;
			this.offset = XformOffset.Identity;
		}

		// Token: 0x06005BFD RID: 23549 RVA: 0x001CFD44 File Offset: 0x001CDF44
		public BoneOffset(GTHardCodedBones.EBone bone, XformOffset offset)
		{
			this.bone = bone;
			this.offset = offset;
		}

		// Token: 0x06005BFE RID: 23550 RVA: 0x001CFD59 File Offset: 0x001CDF59
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot);
		}

		// Token: 0x06005BFF RID: 23551 RVA: 0x001CFD74 File Offset: 0x001CDF74
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles);
		}

		// Token: 0x06005C00 RID: 23552 RVA: 0x001CFD8F File Offset: 0x001CDF8F
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Quaternion rot, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rot, scale);
		}

		// Token: 0x06005C01 RID: 23553 RVA: 0x001CFDAC File Offset: 0x001CDFAC
		public BoneOffset(GTHardCodedBones.EBone bone, Vector3 pos, Vector3 rotAngles, Vector3 scale)
		{
			this.bone = bone;
			this.offset = new XformOffset(pos, rotAngles, scale);
		}

		// Token: 0x040065C6 RID: 26054
		public GTHardCodedBones.SturdyEBone bone;

		// Token: 0x040065C7 RID: 26055
		public XformOffset offset;

		// Token: 0x040065C8 RID: 26056
		public static readonly BoneOffset Identity = new BoneOffset
		{
			bone = GTHardCodedBones.EBone.None,
			offset = XformOffset.Identity
		};
	}
}
