using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02000E2B RID: 3627
	public class OldGorillaRopeSwing : MonoBehaviourPun
	{
		// Token: 0x170008D5 RID: 2261
		// (get) Token: 0x06005A26 RID: 23078 RVA: 0x001C6C18 File Offset: 0x001C4E18
		// (set) Token: 0x06005A27 RID: 23079 RVA: 0x001C6C20 File Offset: 0x001C4E20
		public bool isIdle { get; private set; }

		// Token: 0x06005A28 RID: 23080 RVA: 0x001C6C29 File Offset: 0x001C4E29
		private void Awake()
		{
			this.SetIsIdle(true);
		}

		// Token: 0x06005A29 RID: 23081 RVA: 0x001C6C32 File Offset: 0x001C4E32
		private void OnDisable()
		{
			if (!this.isIdle)
			{
				this.SetIsIdle(true);
			}
		}

		// Token: 0x06005A2A RID: 23082 RVA: 0x001C6C44 File Offset: 0x001C4E44
		private void Update()
		{
			if (this.localPlayerOn && this.localGrabbedRigid)
			{
				float magnitude = this.localGrabbedRigid.velocity.magnitude;
				if (magnitude > 2.5f && !this.ropeCreakSFX.isPlaying && Mathf.RoundToInt(Time.time) % 5 == 0)
				{
					this.ropeCreakSFX.GTPlay();
				}
				float num = MathUtils.Linear(magnitude, 0f, 10f, -0.07f, 0.5f);
				if (num > 0f)
				{
					GorillaTagger.Instance.DoVibration(this.localPlayerXRNode, num, Time.deltaTime);
				}
			}
			if (!this.isIdle)
			{
				if (!this.localPlayerOn && this.remotePlayers.Count == 0)
				{
					foreach (Rigidbody rigidbody in this.bones)
					{
						float magnitude2 = rigidbody.velocity.magnitude;
						float num2 = Time.deltaTime * this.settings.frictionWhenNotHeld;
						if (num2 < magnitude2 - 0.1f)
						{
							rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, Vector3.zero, num2);
						}
					}
				}
				bool flag = false;
				for (int j = 0; j < this.bones.Length; j++)
				{
					if (this.bones[j].velocity.magnitude > 0.1f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					this.potentialIdleTimer += Time.deltaTime;
				}
				else
				{
					this.potentialIdleTimer = 0f;
				}
				if (this.potentialIdleTimer >= 2f)
				{
					this.SetIsIdle(true);
					this.potentialIdleTimer = 0f;
				}
			}
		}

		// Token: 0x06005A2B RID: 23083 RVA: 0x001C6DF0 File Offset: 0x001C4FF0
		private void SetIsIdle(bool idle)
		{
			this.isIdle = idle;
			this.ToggleIsKinematic(idle);
			if (idle)
			{
				for (int i = 0; i < this.bones.Length; i++)
				{
					this.bones[i].velocity = Vector3.zero;
					this.bones[i].angularVelocity = Vector3.zero;
					this.bones[i].transform.localRotation = Quaternion.identity;
				}
			}
		}

		// Token: 0x06005A2C RID: 23084 RVA: 0x001C6E5C File Offset: 0x001C505C
		private void ToggleIsKinematic(bool kinematic)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				this.bones[i].isKinematic = kinematic;
				if (kinematic)
				{
					this.bones[i].interpolation = RigidbodyInterpolation.None;
				}
				else
				{
					this.bones[i].interpolation = RigidbodyInterpolation.Interpolate;
				}
			}
		}

		// Token: 0x06005A2D RID: 23085 RVA: 0x001C6EAB File Offset: 0x001C50AB
		public Rigidbody GetBone(int index)
		{
			if (index >= this.bones.Length)
			{
				return this.bones.Last<Rigidbody>();
			}
			return this.bones[index];
		}

		// Token: 0x06005A2E RID: 23086 RVA: 0x001C6ECC File Offset: 0x001C50CC
		public int GetBoneIndex(Rigidbody r)
		{
			for (int i = 0; i < this.bones.Length; i++)
			{
				if (this.bones[i] == r)
				{
					return i;
				}
			}
			return this.bones.Length - 1;
		}

		// Token: 0x06005A2F RID: 23087 RVA: 0x001C6F08 File Offset: 0x001C5108
		public void AttachLocalPlayer(XRNode xrNode, Rigidbody rigid, Vector3 offset, Vector3 velocity)
		{
			int boneIndex = this.GetBoneIndex(rigid);
			velocity *= this.settings.inheritVelocityMultiplier;
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = base.photonView.ViewID;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeBoneIndex = boneIndex;
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == XRNode.LeftHand);
				GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			}
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			if (this.remotePlayers.Count <= 0)
			{
				foreach (Rigidbody rigidbody in this.bones)
				{
					list.Add(rigidbody.transform.localEulerAngles);
					list2.Add(rigidbody.velocity);
				}
			}
			if (Time.time - this.lastGrabTime > 1f && (this.remotePlayers.Count == 0 || velocity.magnitude > 2f))
			{
				this.SetVelocity_RPC(boneIndex, velocity, true, list.ToArray(), list2.ToArray());
			}
			this.lastGrabTime = Time.time;
			this.ropeCreakSFX.transform.parent = this.GetBone(Math.Max(0, boneIndex - 2)).transform;
			this.ropeCreakSFX.transform.localPosition = Vector3.zero;
			this.localPlayerOn = true;
			this.localPlayerXRNode = xrNode;
			this.localGrabbedRigid = rigid;
		}

		// Token: 0x06005A30 RID: 23088 RVA: 0x001C708B File Offset: 0x001C528B
		public void DetachLocalPlayer()
		{
			if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
			{
				GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
			}
			this.localPlayerOn = false;
			this.localGrabbedRigid = null;
		}

		// Token: 0x06005A31 RID: 23089 RVA: 0x001C70C4 File Offset: 0x001C52C4
		public bool AttachRemotePlayer(int playerId, int boneIndex, Transform offsetTransform, Vector3 offset)
		{
			Rigidbody bone = this.GetBone(boneIndex);
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
			return true;
		}

		// Token: 0x06005A32 RID: 23090 RVA: 0x001C712B File Offset: 0x001C532B
		public void DetachRemotePlayer(int playerId)
		{
			this.remotePlayers.Remove(playerId);
		}

		// Token: 0x06005A33 RID: 23091 RVA: 0x001C713C File Offset: 0x001C533C
		public void SetVelocity_RPC(int boneIndex, Vector3 velocity, bool wholeRope = true, Vector3[] ropeRotations = null, Vector3[] ropeVelocities = null)
		{
			if (NetworkSystem.Instance.InRoom)
			{
				base.photonView.RPC("SetVelocity", RpcTarget.All, new object[]
				{
					boneIndex,
					velocity,
					wholeRope,
					ropeRotations,
					ropeVelocities
				});
				return;
			}
			this.SetVelocity(boneIndex, velocity, wholeRope, ropeRotations, ropeVelocities);
		}

		// Token: 0x06005A34 RID: 23092 RVA: 0x001C71A0 File Offset: 0x001C53A0
		[PunRPC]
		public void SetVelocity(int boneIndex, Vector3 velocity, bool wholeRope = true, Vector3[] ropeRotations = null, Vector3[] ropeVelocities = null)
		{
			this.SetIsIdle(false);
			if (ropeRotations != null && ropeVelocities != null && ropeRotations.Length != 0)
			{
				this.ToggleIsKinematic(true);
				for (int i = 0; i < ropeRotations.Length; i++)
				{
					if (i != 0)
					{
						this.bones[i].transform.localRotation = Quaternion.Euler(ropeRotations[i]);
						this.bones[i].velocity = ropeVelocities[i];
					}
				}
				this.ToggleIsKinematic(false);
			}
			Rigidbody bone = this.GetBone(boneIndex);
			if (bone)
			{
				if (wholeRope)
				{
					int num = 0;
					float maxLength = Mathf.Min(velocity.magnitude, 15f);
					foreach (Rigidbody rigidbody in this.bones)
					{
						Vector3 vector = velocity / (float)boneIndex * (float)num;
						vector = Vector3.ClampMagnitude(vector, maxLength);
						rigidbody.velocity = vector;
						num++;
					}
					return;
				}
				bone.velocity = velocity;
			}
		}

		// Token: 0x040064F3 RID: 25843
		public const float kPlayerMass = 0.8f;

		// Token: 0x040064F4 RID: 25844
		public const float ropeBitGenOffset = 1f;

		// Token: 0x040064F5 RID: 25845
		public const float MAX_ROPE_SPEED = 15f;

		// Token: 0x040064F6 RID: 25846
		[SerializeField]
		private GameObject prefabRopeBit;

		// Token: 0x040064F7 RID: 25847
		public Rigidbody[] bones = Array.Empty<Rigidbody>();

		// Token: 0x040064F8 RID: 25848
		private Dictionary<int, int> remotePlayers = new Dictionary<int, int>();

		// Token: 0x040064F9 RID: 25849
		[NonSerialized]
		public float lastGrabTime;

		// Token: 0x040064FA RID: 25850
		[SerializeField]
		private AudioSource ropeCreakSFX;

		// Token: 0x040064FB RID: 25851
		private bool localPlayerOn;

		// Token: 0x040064FC RID: 25852
		private XRNode localPlayerXRNode;

		// Token: 0x040064FD RID: 25853
		private Rigidbody localGrabbedRigid;

		// Token: 0x040064FE RID: 25854
		private const float MAX_VELOCITY_FOR_IDLE = 0.1f;

		// Token: 0x040064FF RID: 25855
		private const float TIME_FOR_IDLE = 2f;

		// Token: 0x04006501 RID: 25857
		private float potentialIdleTimer;

		// Token: 0x04006502 RID: 25858
		[Header("Config")]
		[SerializeField]
		private int ropeLength = 8;

		// Token: 0x04006503 RID: 25859
		[SerializeField]
		private GorillaRopeSwingSettings settings;
	}
}
