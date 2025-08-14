using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D0C RID: 3340
	public class HandsManager : MonoBehaviour
	{
		// Token: 0x170007C1 RID: 1985
		// (get) Token: 0x06005293 RID: 21139 RVA: 0x0019A474 File Offset: 0x00198674
		// (set) Token: 0x06005294 RID: 21140 RVA: 0x0019A47E File Offset: 0x0019867E
		public OVRHand RightHand
		{
			get
			{
				return this._hand[1];
			}
			private set
			{
				this._hand[1] = value;
			}
		}

		// Token: 0x170007C2 RID: 1986
		// (get) Token: 0x06005295 RID: 21141 RVA: 0x0019A489 File Offset: 0x00198689
		// (set) Token: 0x06005296 RID: 21142 RVA: 0x0019A493 File Offset: 0x00198693
		public OVRSkeleton RightHandSkeleton
		{
			get
			{
				return this._handSkeleton[1];
			}
			private set
			{
				this._handSkeleton[1] = value;
			}
		}

		// Token: 0x170007C3 RID: 1987
		// (get) Token: 0x06005297 RID: 21143 RVA: 0x0019A49E File Offset: 0x0019869E
		// (set) Token: 0x06005298 RID: 21144 RVA: 0x0019A4A8 File Offset: 0x001986A8
		public OVRSkeletonRenderer RightHandSkeletonRenderer
		{
			get
			{
				return this._handSkeletonRenderer[1];
			}
			private set
			{
				this._handSkeletonRenderer[1] = value;
			}
		}

		// Token: 0x170007C4 RID: 1988
		// (get) Token: 0x06005299 RID: 21145 RVA: 0x0019A4B3 File Offset: 0x001986B3
		// (set) Token: 0x0600529A RID: 21146 RVA: 0x0019A4BD File Offset: 0x001986BD
		public OVRMesh RightHandMesh
		{
			get
			{
				return this._handMesh[1];
			}
			private set
			{
				this._handMesh[1] = value;
			}
		}

		// Token: 0x170007C5 RID: 1989
		// (get) Token: 0x0600529B RID: 21147 RVA: 0x0019A4C8 File Offset: 0x001986C8
		// (set) Token: 0x0600529C RID: 21148 RVA: 0x0019A4D2 File Offset: 0x001986D2
		public OVRMeshRenderer RightHandMeshRenderer
		{
			get
			{
				return this._handMeshRenderer[1];
			}
			private set
			{
				this._handMeshRenderer[1] = value;
			}
		}

		// Token: 0x170007C6 RID: 1990
		// (get) Token: 0x0600529D RID: 21149 RVA: 0x0019A4DD File Offset: 0x001986DD
		// (set) Token: 0x0600529E RID: 21150 RVA: 0x0019A4E7 File Offset: 0x001986E7
		public OVRHand LeftHand
		{
			get
			{
				return this._hand[0];
			}
			private set
			{
				this._hand[0] = value;
			}
		}

		// Token: 0x170007C7 RID: 1991
		// (get) Token: 0x0600529F RID: 21151 RVA: 0x0019A4F2 File Offset: 0x001986F2
		// (set) Token: 0x060052A0 RID: 21152 RVA: 0x0019A4FC File Offset: 0x001986FC
		public OVRSkeleton LeftHandSkeleton
		{
			get
			{
				return this._handSkeleton[0];
			}
			private set
			{
				this._handSkeleton[0] = value;
			}
		}

		// Token: 0x170007C8 RID: 1992
		// (get) Token: 0x060052A1 RID: 21153 RVA: 0x0019A507 File Offset: 0x00198707
		// (set) Token: 0x060052A2 RID: 21154 RVA: 0x0019A511 File Offset: 0x00198711
		public OVRSkeletonRenderer LeftHandSkeletonRenderer
		{
			get
			{
				return this._handSkeletonRenderer[0];
			}
			private set
			{
				this._handSkeletonRenderer[0] = value;
			}
		}

		// Token: 0x170007C9 RID: 1993
		// (get) Token: 0x060052A3 RID: 21155 RVA: 0x0019A51C File Offset: 0x0019871C
		// (set) Token: 0x060052A4 RID: 21156 RVA: 0x0019A526 File Offset: 0x00198726
		public OVRMesh LeftHandMesh
		{
			get
			{
				return this._handMesh[0];
			}
			private set
			{
				this._handMesh[0] = value;
			}
		}

		// Token: 0x170007CA RID: 1994
		// (get) Token: 0x060052A5 RID: 21157 RVA: 0x0019A531 File Offset: 0x00198731
		// (set) Token: 0x060052A6 RID: 21158 RVA: 0x0019A53B File Offset: 0x0019873B
		public OVRMeshRenderer LeftHandMeshRenderer
		{
			get
			{
				return this._handMeshRenderer[0];
			}
			private set
			{
				this._handMeshRenderer[0] = value;
			}
		}

		// Token: 0x170007CB RID: 1995
		// (get) Token: 0x060052A7 RID: 21159 RVA: 0x0019A546 File Offset: 0x00198746
		// (set) Token: 0x060052A8 RID: 21160 RVA: 0x0019A54D File Offset: 0x0019874D
		public static HandsManager Instance { get; private set; }

		// Token: 0x060052A9 RID: 21161 RVA: 0x0019A558 File Offset: 0x00198758
		private void Awake()
		{
			if (HandsManager.Instance && HandsManager.Instance != this)
			{
				Object.Destroy(this);
				return;
			}
			HandsManager.Instance = this;
			this.LeftHand = this._leftHand.GetComponent<OVRHand>();
			this.LeftHandSkeleton = this._leftHand.GetComponent<OVRSkeleton>();
			this.LeftHandSkeletonRenderer = this._leftHand.GetComponent<OVRSkeletonRenderer>();
			this.LeftHandMesh = this._leftHand.GetComponent<OVRMesh>();
			this.LeftHandMeshRenderer = this._leftHand.GetComponent<OVRMeshRenderer>();
			this.RightHand = this._rightHand.GetComponent<OVRHand>();
			this.RightHandSkeleton = this._rightHand.GetComponent<OVRSkeleton>();
			this.RightHandSkeletonRenderer = this._rightHand.GetComponent<OVRSkeletonRenderer>();
			this.RightHandMesh = this._rightHand.GetComponent<OVRMesh>();
			this.RightHandMeshRenderer = this._rightHand.GetComponent<OVRMeshRenderer>();
			this._leftMeshRenderer = this.LeftHand.GetComponent<SkinnedMeshRenderer>();
			this._rightMeshRenderer = this.RightHand.GetComponent<SkinnedMeshRenderer>();
			base.StartCoroutine(this.FindSkeletonVisualGameObjects());
		}

		// Token: 0x060052AA RID: 21162 RVA: 0x0019A664 File Offset: 0x00198864
		private void Update()
		{
			HandsManager.HandsVisualMode visualMode = this.VisualMode;
			if (visualMode > HandsManager.HandsVisualMode.Skeleton)
			{
				if (visualMode != HandsManager.HandsVisualMode.Both)
				{
					this._currentHandAlpha = 1f;
				}
				else
				{
					this._currentHandAlpha = 0.6f;
				}
			}
			else
			{
				this._currentHandAlpha = 1f;
			}
			this._rightMeshRenderer.sharedMaterial.SetFloat(this.HandAlphaId, this._currentHandAlpha);
			this._leftMeshRenderer.sharedMaterial.SetFloat(this.HandAlphaId, this._currentHandAlpha);
		}

		// Token: 0x060052AB RID: 21163 RVA: 0x0019A6DF File Offset: 0x001988DF
		private IEnumerator FindSkeletonVisualGameObjects()
		{
			while (!this._leftSkeletonVisual || !this._rightSkeletonVisual)
			{
				if (!this._leftSkeletonVisual)
				{
					Transform transform = this.LeftHand.transform.Find("SkeletonRenderer");
					if (transform)
					{
						this._leftSkeletonVisual = transform.gameObject;
					}
				}
				if (!this._rightSkeletonVisual)
				{
					Transform transform2 = this.RightHand.transform.Find("SkeletonRenderer");
					if (transform2)
					{
						this._rightSkeletonVisual = transform2.gameObject;
					}
				}
				yield return null;
			}
			this.SetToCurrentVisualMode();
			yield break;
		}

		// Token: 0x060052AC RID: 21164 RVA: 0x0019A6EE File Offset: 0x001988EE
		public void SwitchVisualization()
		{
			if (!this._leftSkeletonVisual || !this._rightSkeletonVisual)
			{
				return;
			}
			this.VisualMode = (this.VisualMode + 1) % (HandsManager.HandsVisualMode)3;
			this.SetToCurrentVisualMode();
		}

		// Token: 0x060052AD RID: 21165 RVA: 0x0019A724 File Offset: 0x00198924
		private void SetToCurrentVisualMode()
		{
			switch (this.VisualMode)
			{
			case HandsManager.HandsVisualMode.Mesh:
				this.RightHandMeshRenderer.enabled = true;
				this._rightMeshRenderer.enabled = true;
				this._rightSkeletonVisual.gameObject.SetActive(false);
				this.LeftHandMeshRenderer.enabled = true;
				this._leftMeshRenderer.enabled = true;
				this._leftSkeletonVisual.gameObject.SetActive(false);
				return;
			case HandsManager.HandsVisualMode.Skeleton:
				this.RightHandMeshRenderer.enabled = false;
				this._rightMeshRenderer.enabled = false;
				this._rightSkeletonVisual.gameObject.SetActive(true);
				this.LeftHandMeshRenderer.enabled = false;
				this._leftMeshRenderer.enabled = false;
				this._leftSkeletonVisual.gameObject.SetActive(true);
				return;
			case HandsManager.HandsVisualMode.Both:
				this.RightHandMeshRenderer.enabled = true;
				this._rightMeshRenderer.enabled = true;
				this._rightSkeletonVisual.gameObject.SetActive(true);
				this.LeftHandMeshRenderer.enabled = true;
				this._leftMeshRenderer.enabled = true;
				this._leftSkeletonVisual.gameObject.SetActive(true);
				return;
			default:
				return;
			}
		}

		// Token: 0x060052AE RID: 21166 RVA: 0x0019A844 File Offset: 0x00198A44
		public static List<OVRBoneCapsule> GetCapsulesPerBone(OVRSkeleton skeleton, OVRSkeleton.BoneId boneId)
		{
			List<OVRBoneCapsule> list = new List<OVRBoneCapsule>();
			IList<OVRBoneCapsule> capsules = skeleton.Capsules;
			for (int i = 0; i < capsules.Count; i++)
			{
				if (capsules[i].BoneIndex == (short)boneId)
				{
					list.Add(capsules[i]);
				}
			}
			return list;
		}

		// Token: 0x060052AF RID: 21167 RVA: 0x0019A890 File Offset: 0x00198A90
		public bool IsInitialized()
		{
			return this.LeftHandSkeleton && this.LeftHandSkeleton.IsInitialized && this.RightHandSkeleton && this.RightHandSkeleton.IsInitialized && this.LeftHandMesh && this.LeftHandMesh.IsInitialized && this.RightHandMesh && this.RightHandMesh.IsInitialized;
		}

		// Token: 0x04005C02 RID: 23554
		private const string SKELETON_VISUALIZER_NAME = "SkeletonRenderer";

		// Token: 0x04005C03 RID: 23555
		[SerializeField]
		private GameObject _leftHand;

		// Token: 0x04005C04 RID: 23556
		[SerializeField]
		private GameObject _rightHand;

		// Token: 0x04005C05 RID: 23557
		public HandsManager.HandsVisualMode VisualMode;

		// Token: 0x04005C06 RID: 23558
		private OVRHand[] _hand = new OVRHand[2];

		// Token: 0x04005C07 RID: 23559
		private OVRSkeleton[] _handSkeleton = new OVRSkeleton[2];

		// Token: 0x04005C08 RID: 23560
		private OVRSkeletonRenderer[] _handSkeletonRenderer = new OVRSkeletonRenderer[2];

		// Token: 0x04005C09 RID: 23561
		private OVRMesh[] _handMesh = new OVRMesh[2];

		// Token: 0x04005C0A RID: 23562
		private OVRMeshRenderer[] _handMeshRenderer = new OVRMeshRenderer[2];

		// Token: 0x04005C0B RID: 23563
		private SkinnedMeshRenderer _leftMeshRenderer;

		// Token: 0x04005C0C RID: 23564
		private SkinnedMeshRenderer _rightMeshRenderer;

		// Token: 0x04005C0D RID: 23565
		private GameObject _leftSkeletonVisual;

		// Token: 0x04005C0E RID: 23566
		private GameObject _rightSkeletonVisual;

		// Token: 0x04005C0F RID: 23567
		private float _currentHandAlpha = 1f;

		// Token: 0x04005C10 RID: 23568
		private int HandAlphaId = Shader.PropertyToID("_HandAlpha");

		// Token: 0x02000D0D RID: 3341
		public enum HandsVisualMode
		{
			// Token: 0x04005C13 RID: 23571
			Mesh,
			// Token: 0x04005C14 RID: 23572
			Skeleton,
			// Token: 0x04005C15 RID: 23573
			Both
		}
	}
}
