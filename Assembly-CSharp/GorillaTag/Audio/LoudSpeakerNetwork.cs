using System;
using System.Collections.Generic;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x02000EF0 RID: 3824
	public class LoudSpeakerNetwork : MonoBehaviour
	{
		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x06005EDD RID: 24285 RVA: 0x001DE618 File Offset: 0x001DC818
		public AudioSource[] SpeakerSources
		{
			get
			{
				return this._speakerSources;
			}
		}

		// Token: 0x06005EDE RID: 24286 RVA: 0x001DE620 File Offset: 0x001DC820
		private void Awake()
		{
			if (this._speakerSources == null || this._speakerSources.Length == 0)
			{
				this._speakerSources = base.transform.GetComponentsInChildren<AudioSource>();
			}
			this._currentSpeakers = new List<Speaker>();
		}

		// Token: 0x06005EDF RID: 24287 RVA: 0x001DE650 File Offset: 0x001DC850
		private void Start()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer) && rigContainer.Voice != null)
			{
				GTSpeaker gtspeaker = (GTSpeaker)rigContainer.Voice.SpeakerInUse;
				if (gtspeaker != null)
				{
					gtspeaker.AddExternalAudioSources(this._speakerSources);
				}
			}
		}

		// Token: 0x06005EE0 RID: 24288 RVA: 0x001DE69B File Offset: 0x001DC89B
		private bool GetParentRigContainer(out RigContainer rigContainer)
		{
			if (this._rigContainer == null)
			{
				this._rigContainer = base.transform.GetComponentInParent<RigContainer>();
			}
			rigContainer = this._rigContainer;
			return rigContainer != null;
		}

		// Token: 0x06005EE1 RID: 24289 RVA: 0x001DE6CC File Offset: 0x001DC8CC
		private void OnEnable()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer))
			{
				rigContainer.AddLoudSpeakerNetwork(this);
			}
		}

		// Token: 0x06005EE2 RID: 24290 RVA: 0x001DE6EC File Offset: 0x001DC8EC
		private void OnDisable()
		{
			RigContainer rigContainer;
			if (this.GetParentRigContainer(out rigContainer))
			{
				rigContainer.RemoveLoudSpeakerNetwork(this);
			}
		}

		// Token: 0x06005EE3 RID: 24291 RVA: 0x001DE70A File Offset: 0x001DC90A
		public void AddSpeaker(Speaker speaker)
		{
			if (this._currentSpeakers.Contains(speaker))
			{
				return;
			}
			this._currentSpeakers.Add(speaker);
		}

		// Token: 0x06005EE4 RID: 24292 RVA: 0x001DE727 File Offset: 0x001DC927
		public void RemoveSpeaker(Speaker speaker)
		{
			this._currentSpeakers.Remove(speaker);
		}

		// Token: 0x06005EE5 RID: 24293 RVA: 0x001DE736 File Offset: 0x001DC936
		public void StartBroadcastSpeakerOutput(VRRig player)
		{
			GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(true, player.OwningNetPlayer.ActorNumber);
		}

		// Token: 0x06005EE6 RID: 24294 RVA: 0x001DE754 File Offset: 0x001DC954
		public void BroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
		{
			if (isLocal)
			{
				if (this._localRecorder == null)
				{
					this._localRecorder = (GTRecorder)NetworkSystem.Instance.LocalRecorder;
				}
				if (this._localRecorder != null)
				{
					this._localRecorder.DebugEchoMode = true;
					if (this.ReparentLocalSpeaker)
					{
						Transform transform = this._rigContainer.Voice.SpeakerInUse.transform;
						transform.transform.SetParent(base.transform, false);
						transform.localPosition = Vector3.zero;
					}
				}
				return;
			}
			using (List<Speaker>.Enumerator enumerator = this._currentSpeakers.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					GTSpeaker gtspeaker = (GTSpeaker)enumerator.Current;
					gtspeaker.ToggleAudioSource(true);
					gtspeaker.BroadcastExternal = true;
					RigContainer rigContainer;
					if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer))
					{
						Transform transform2 = rigContainer.Voice.SpeakerInUse.transform;
						transform2.SetParent(base.transform, false);
						transform2.localPosition = Vector3.zero;
					}
				}
			}
			this._currentSpeakerActor = actorNumber;
		}

		// Token: 0x06005EE7 RID: 24295 RVA: 0x001DE874 File Offset: 0x001DCA74
		public void StopBroadcastSpeakerOutput(VRRig player)
		{
			GorillaTagger.Instance.rigSerializer.BroadcastLoudSpeakerNetwork(false, player.OwningNetPlayer.ActorNumber);
		}

		// Token: 0x06005EE8 RID: 24296 RVA: 0x001DE894 File Offset: 0x001DCA94
		public void StopBroadcastLoudSpeakerNetwork(int actorNumber, bool isLocal = false)
		{
			if (isLocal)
			{
				if (this._localRecorder == null)
				{
					this._localRecorder = (GTRecorder)NetworkSystem.Instance.LocalRecorder;
				}
				if (this._localRecorder != null)
				{
					this._localRecorder.DebugEchoMode = false;
					RigContainer rigContainer;
					if (this.ReparentLocalSpeaker && this.GetParentRigContainer(out rigContainer))
					{
						Transform transform = rigContainer.Voice.SpeakerInUse.transform;
						transform.SetParent(rigContainer.SpeakerHead, false);
						transform.localPosition = Vector3.zero;
					}
				}
				return;
			}
			if (actorNumber == this._currentSpeakerActor)
			{
				using (List<Speaker>.Enumerator enumerator = this._currentSpeakers.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						GTSpeaker gtspeaker = (GTSpeaker)enumerator.Current;
						gtspeaker.ToggleAudioSource(false);
						gtspeaker.BroadcastExternal = false;
						RigContainer rigContainer2;
						if (VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(actorNumber), out rigContainer2))
						{
							Transform transform2 = rigContainer2.Voice.SpeakerInUse.transform;
							transform2.SetParent(rigContainer2.SpeakerHead, false);
							transform2.localPosition = Vector3.zero;
						}
					}
				}
				this._currentSpeakerActor = -1;
			}
		}

		// Token: 0x0400694D RID: 26957
		[SerializeField]
		private AudioSource[] _speakerSources;

		// Token: 0x0400694E RID: 26958
		[SerializeField]
		private List<Speaker> _currentSpeakers;

		// Token: 0x0400694F RID: 26959
		[SerializeField]
		private int _currentSpeakerActor = -1;

		// Token: 0x04006950 RID: 26960
		public bool ReparentLocalSpeaker = true;

		// Token: 0x04006951 RID: 26961
		private RigContainer _rigContainer;

		// Token: 0x04006952 RID: 26962
		private GTRecorder _localRecorder;
	}
}
