using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E23 RID: 3619
	public class GorillaRopeSwing : MonoBehaviour, IBuilderPieceComponent
	{
		// Token: 0x060059E3 RID: 23011 RVA: 0x001C5700 File Offset: 0x001C3900
		private void EdRecalculateId()
		{
			this.CalculateId(true);
		}

		// Token: 0x170008CF RID: 2255
		// (get) Token: 0x060059E4 RID: 23012 RVA: 0x001C5709 File Offset: 0x001C3909
		// (set) Token: 0x060059E5 RID: 23013 RVA: 0x001C5711 File Offset: 0x001C3911
		public bool isIdle { get; private set; }

		// Token: 0x170008D0 RID: 2256
		// (get) Token: 0x060059E6 RID: 23014 RVA: 0x001C571A File Offset: 0x001C391A
		// (set) Token: 0x060059E7 RID: 23015 RVA: 0x001C5722 File Offset: 0x001C3922
		public bool isFullyIdle { get; private set; }

		// Token: 0x170008D1 RID: 2257
		// (get) Token: 0x060059E8 RID: 23016 RVA: 0x001C572B File Offset: 0x001C392B
		public bool SupportsMovingAtRuntime
		{
			get
			{
				return this.supportMovingAtRuntime;
			}
		}

		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x060059E9 RID: 23017 RVA: 0x001C5733 File Offset: 0x001C3933
		public bool hasPlayers
		{
			get
			{
				return this.localPlayerOn || this.remotePlayers.Count > 0;
			}
		}

		// Token: 0x060059EA RID: 23018 RVA: 0x001C5750 File Offset: 0x001C3950
		protected virtual void Awake()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, false);
		}

		// Token: 0x060059EB RID: 23019 RVA: 0x001C57B3 File Offset: 0x001C39B3
		protected virtual void Start()
		{
			if (!this.useStaticId)
			{
				this.CalculateId(false);
			}
			RopeSwingManager.Register(this);
			this.started = true;
		}

		// Token: 0x060059EC RID: 23020 RVA: 0x001C57D1 File Offset: 0x001C39D1
		private void OnDestroy()
		{
			if (RopeSwingManager.instance != null)
			{
				RopeSwingManager.Unregister(this);
			}
		}

		// Token: 0x060059ED RID: 23021 RVA: 0x001C57E8 File Offset: 0x001C39E8
		protected virtual void OnEnable()
		{
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, true);
			VectorizedCustomRopeSimulation.Register(this);
			GorillaRopeSwingUpdateManager.RegisterRopeSwing(this);
		}

		// Token: 0x060059EE RID: 23022 RVA: 0x001C5857 File Offset: 0x001C3A57
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true, true);
			}
			VectorizedCustomRopeSimulation.Unregister(this);
			GorillaRopeSwingUpdateManager.UnregisterRopeSwing(this);
		}

		// Token: 0x060059EF RID: 23023 RVA: 0x001C5878 File Offset: 0x001C3A78
		internal void CalculateId(bool force = false)
		{
			Transform transform = base.transform;
			int staticHash = TransformUtils.GetScenePath(transform).GetStaticHash();
			int staticHash2 = base.GetType().Name.GetStaticHash();
			int num = StaticHash.Compute(staticHash, staticHash2);
			if (this.useStaticId)
			{
				if (string.IsNullOrEmpty(this.staticId) || force)
				{
					Vector3 position = transform.position;
					int i = StaticHash.Compute(position.x, position.y, position.z);
					int instanceID = transform.GetInstanceID();
					int num2 = StaticHash.Compute(num, i, instanceID);
					this.staticId = string.Format("#ID_{0:X8}", num2);
				}
				this.ropeId = this.staticId.GetStaticHash();
				return;
			}
			this.ropeId = (Application.isPlaying ? num : 0);
		}

		// Token: 0x060059F0 RID: 23024 RVA: 0x001C5934 File Offset: 0x001C3B34
		public void InvokeUpdate()
		{
			if (this.isIdle)
			{
				this.isFullyIdle = true;
			}
			if (!this.isIdle)
			{
				int num = -1;
				if (this.localPlayerOn)
				{
					num = this.localPlayerBoneIndex;
				}
				else if (this.remotePlayers.Count > 0)
				{
					num = this.remotePlayers.First<KeyValuePair<int, int>>().Value;
				}
				if (num >= 0 && VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, num).magnitude > 2f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.GTPlay();
				}
				if (this.localPlayerOn)
				{
					float num2 = MathUtils.Linear(this.velocityTracker.GetLatestVelocity(true).magnitude / this.scaleFactor, 0f, 10f, -0.07f, 0.5f);
					if (num2 > 0f)
					{
						GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num2, Time.deltaTime);
					}
				}
				Transform bone = this.GetBone(this.lastNodeCheckIndex);
				Vector3 nodeVelocity = VectorizedCustomRopeSimulation.instance.GetNodeVelocity(this, this.lastNodeCheckIndex);
				if (Physics.SphereCastNonAlloc(bone.position, 0.2f * this.scaleFactor, nodeVelocity.normalized, this.nodeHits, 0.4f * this.scaleFactor, this.wallLayerMask, QueryTriggerInteraction.Ignore) > 0)
				{
					this.SetVelocity(this.lastNodeCheckIndex, Vector3.zero, false, default(PhotonMessageInfoWrapped));
				}
				if (nodeVelocity.magnitude <= 0.35f)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true, false);
					this.potentialIdleTimer = 0f;
				}
				this.lastNodeCheckIndex++;
				if (this.lastNodeCheckIndex > this.nodes.Length)
				{
					this.lastNodeCheckIndex = 2;
				}
			}
			if (this.hasMonkeBlockParent && this.supportMovingAtRuntime)
			{
				base.transform.rotation = Quaternion.Euler(0f, base.transform.parent.rotation.eulerAngles.y, 0f);
			}
		}

		// Token: 0x060059F1 RID: 23025 RVA: 0x001C5B68 File Offset: 0x001C3D68
		private void SetIsIdle(bool idle, bool resetPos = false)
		{
			this.isIdle = idle;
			this.ropeCreakSFX.gameObject.SetActive(!idle);
			if (idle)
			{
				this.ToggleVelocityTracker(false, 0, default(Vector3));
				if (resetPos)
				{
					Vector3 vector = Vector3.zero;
					for (int i = 0; i < this.nodes.Length; i++)
					{
						this.nodes[i].transform.localRotation = Quaternion.identity;
						this.nodes[i].transform.localPosition = vector;
						vector += new Vector3(0f, -1f, 0f);
					}
					return;
				}
			}
			else
			{
				this.isFullyIdle = false;
			}
		}

		// Token: 0x060059F2 RID: 23026 RVA: 0x001C5C0D File Offset: 0x001C3E0D
		public Transform GetBone(int index)
		{
			if (index >= this.nodes.Length)
			{
				return this.nodes.Last<Transform>();
			}
			return this.nodes[index];
		}

		// Token: 0x060059F3 RID: 23027 RVA: 0x001C5C30 File Offset: 0x001C3E30
		public int GetBoneIndex(Transform r)
		{
			for (int i = 0; i < this.nodes.Length; i++)
			{
				if (this.nodes[i] == r)
				{
					return i;
				}
			}
			return this.nodes.Length - 1;
		}

		// Token: 0x060059F4 RID: 23028 RVA: 0x001C5C6C File Offset: 0x001C3E6C
		public void AttachLocalPlayer(XRNode xrNode, Transform grabbedBone, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(grabbedBone);
			this.localPlayerBoneIndex = boneIndex;
			velocity /= this.scaleFactor;
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = this.ropeId;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == XRNode.LeftHand);
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = false;
			}
			this.RefreshAllBonesMass();
			List<Vector3> list = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Transform transform in this.nodes)
				{
					list.Add(transform.position);
				}
			}
			velocity.y = 0f;
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2.5f))
			{
				RopeSwingManager.instance.SendSetVelocity_RPC(this.ropeId, boneIndex, velocity, true);
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 3)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.ToggleVelocityTracker(true, boneIndex, offset);
		}

		// Token: 0x060059F5 RID: 23029 RVA: 0x001C5E05 File Offset: 0x001C4005
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localPlayerBoneIndex = 0;
			this.RefreshAllBonesMass();
		}

		// Token: 0x060059F6 RID: 23030 RVA: 0x001C5E44 File Offset: 0x001C4044
		private void ToggleVelocityTracker(bool enable, int boneIndex = 0, Vector3 offset = default(Vector3))
		{
			if (enable)
			{
				this.velocityTracker.transform.SetParent(this.GetBone(boneIndex));
				this.velocityTracker.transform.localPosition = offset;
				this.velocityTracker.ResetState();
			}
			this.velocityTracker.gameObject.SetActive(enable);
			if (enable)
			{
				this.velocityTracker.Tick();
			}
		}

		// Token: 0x060059F7 RID: 23031 RVA: 0x001C5EA8 File Offset: 0x001C40A8
		private void RefreshAllBonesMass()
		{
			int num = 0;
			foreach (KeyValuePair<int, int> keyValuePair in this.remotePlayers)
			{
				if (keyValuePair.Value > num)
				{
					num = keyValuePair.Value;
				}
			}
			if (this.localPlayerBoneIndex > num)
			{
				num = this.localPlayerBoneIndex;
			}
			VectorizedCustomRopeSimulation.instance.SetMassForPlayers(this, this.hasPlayers, num);
		}

		// Token: 0x060059F8 RID: 23032 RVA: 0x001C5F2C File Offset: 0x001C412C
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Transform bone = this.GetBone(boneIndex);
			if (bone == null)
			{
				return false;
			}
			offsetTransform.SetParent(bone.transform);
			offsetTransform.localPosition = offset;
			offsetTransform.localRotation = Quaternion.identity;
			if (this.remotePlayers.ContainsKey(playerId))
			{
				Debug.LogError("already on the list!");
				return false;
			}
			this.remotePlayers.Add(playerId, boneIndex);
			this.RefreshAllBonesMass();
			return true;
		}

		// Token: 0x060059F9 RID: 23033 RVA: 0x001C5F99 File Offset: 0x001C4199
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
			this.RefreshAllBonesMass();
		}

		// Token: 0x060059FA RID: 23034 RVA: 0x001C5FB0 File Offset: 0x001C41B0
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope, PhotonMessageInfoWrapped info)
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			float num = 10000f;
			if (!velocity.IsValid(num))
			{
				return;
			}
			velocity.x = Mathf.Clamp(velocity.x, -100f, 100f);
			velocity.y = Mathf.Clamp(velocity.y, -100f, 100f);
			velocity.z = Mathf.Clamp(velocity.z, -100f, 100f);
			boneIndex = Mathf.Clamp(boneIndex, 0, this.nodes.Length);
			Transform bone = this.GetBone(boneIndex);
			if (!bone)
			{
				return;
			}
			if (info.Sender != null && !info.Sender.IsLocal)
			{
				VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(info.Sender);
				if (!vrrig || Vector3.Distance(bone.position, vrrig.transform.position) > 5f)
				{
					return;
				}
			}
			this.SetIsIdle(false, false);
			if (bone)
			{
				VectorizedCustomRopeSimulation.instance.SetVelocity(this, velocity, wholeRope, boneIndex);
			}
		}

		// Token: 0x060059FB RID: 23035 RVA: 0x001C60B8 File Offset: 0x001C42B8
		public void OnPieceCreate(int pieceType, int pieceId)
		{
			this.monkeBlockParent = base.GetComponentInParent<BuilderPiece>();
			this.hasMonkeBlockParent = (this.monkeBlockParent != null);
			int num = StaticHash.Compute(pieceType, pieceId);
			this.staticId = string.Format("#ID_{0:X8}", num);
			this.ropeId = this.staticId.GetStaticHash();
			GorillaRopeSwing gorillaRopeSwing;
			if (this.started && !RopeSwingManager.instance.TryGetRope(this.ropeId, out gorillaRopeSwing))
			{
				RopeSwingManager.Register(this);
			}
		}

		// Token: 0x060059FC RID: 23036 RVA: 0x001C6134 File Offset: 0x001C4334
		public void OnPieceDestroy()
		{
			RopeSwingManager.Unregister(this);
		}

		// Token: 0x060059FD RID: 23037 RVA: 0x001C613C File Offset: 0x001C433C
		public void OnPiecePlacementDeserialized()
		{
			VectorizedCustomRopeSimulation.Unregister(this);
			base.transform.rotation = Quaternion.identity;
			this.scaleFactor = (base.transform.lossyScale.x + base.transform.lossyScale.y + base.transform.lossyScale.z) / 3f;
			this.SetIsIdle(true, true);
			VectorizedCustomRopeSimulation.Register(this);
			if (this.monkeBlockParent != null)
			{
				this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
			}
		}

		// Token: 0x060059FE RID: 23038 RVA: 0x001C61C5 File Offset: 0x001C43C5
		public void OnPieceActivate()
		{
			if (this.monkeBlockParent != null)
			{
				this.supportMovingAtRuntime = this.IsAttachedToMovingPiece();
			}
		}

		// Token: 0x060059FF RID: 23039 RVA: 0x001C61E4 File Offset: 0x001C43E4
		private bool IsAttachedToMovingPiece()
		{
			return this.monkeBlockParent.attachIndex >= 0 && this.monkeBlockParent.attachIndex < this.monkeBlockParent.gridPlanes.Count && this.monkeBlockParent.gridPlanes[this.monkeBlockParent.attachIndex].GetMovingParentGrid() != null;
		}

		// Token: 0x06005A00 RID: 23040 RVA: 0x001C6244 File Offset: 0x001C4444
		public void OnPieceDeactivate()
		{
			this.supportMovingAtRuntime = false;
		}

		// Token: 0x040064AA RID: 25770
		public int ropeId;

		// Token: 0x040064AB RID: 25771
		public string staticId;

		// Token: 0x040064AC RID: 25772
		public bool useStaticId;

		// Token: 0x040064AD RID: 25773
		public const float ropeBitGenOffset = 1f;

		// Token: 0x040064AE RID: 25774
		[SerializeField]
		protected GameObject prefabRopeBit;

		// Token: 0x040064AF RID: 25775
		[SerializeField]
		private bool supportMovingAtRuntime;

		// Token: 0x040064B0 RID: 25776
		public Transform[] nodes = Array.Empty<Transform>();

		// Token: 0x040064B1 RID: 25777
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x040064B2 RID: 25778
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x040064B3 RID: 25779
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x040064B4 RID: 25780
		public GorillaVelocityTracker velocityTracker;

		// Token: 0x040064B5 RID: 25781
		private bool localPlayerOn;

		// Token: 0x040064B6 RID: 25782
		private int localPlayerBoneIndex;

		// Token: 0x040064B7 RID: 25783
		private XRNode localPlayerXRNode;

		// Token: 0x040064B8 RID: 25784
		private const float MAX_VELOCITY_FOR_IDLE = 0.5f;

		// Token: 0x040064B9 RID: 25785
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x040064BC RID: 25788
		private float potentialIdleTimer;

		// Token: 0x040064BD RID: 25789
		[SerializeField]
		protected int ropeLength = 8;

		// Token: 0x040064BE RID: 25790
		[SerializeField]
		private GorillaRopeSwingSettings settings;

		// Token: 0x040064BF RID: 25791
		private bool hasMonkeBlockParent;

		// Token: 0x040064C0 RID: 25792
		private BuilderPiece monkeBlockParent;

		// Token: 0x040064C1 RID: 25793
		[NonSerialized]
		public int ropeDataStartIndex;

		// Token: 0x040064C2 RID: 25794
		[NonSerialized]
		public int ropeDataIndexOffset;

		// Token: 0x040064C3 RID: 25795
		[SerializeField]
		private LayerMask wallLayerMask;

		// Token: 0x040064C4 RID: 25796
		private RaycastHit[] nodeHits = new RaycastHit[1];

		// Token: 0x040064C5 RID: 25797
		private float scaleFactor = 1f;

		// Token: 0x040064C6 RID: 25798
		private bool started;

		// Token: 0x040064C7 RID: 25799
		private int lastNodeCheckIndex = 2;
	}
}
