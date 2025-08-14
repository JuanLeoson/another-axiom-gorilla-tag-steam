using System;
using UnityEngine;

namespace GorillaTagScripts.ObstacleCourse
{
	// Token: 0x02000C76 RID: 3190
	public class ObstacleCourse : MonoBehaviour
	{
		// Token: 0x17000768 RID: 1896
		// (get) Token: 0x06004EE6 RID: 20198 RVA: 0x00188CB4 File Offset: 0x00186EB4
		// (set) Token: 0x06004EE7 RID: 20199 RVA: 0x00188CBC File Offset: 0x00186EBC
		public int winnerActorNumber { get; private set; }

		// Token: 0x06004EE8 RID: 20200 RVA: 0x00188CC8 File Offset: 0x00186EC8
		private void Awake()
		{
			this.numPlayersOnCourse = 0;
			for (int i = 0; i < this.zoneTriggers.Length; i++)
			{
				ObstacleCourseZoneTrigger obstacleCourseZoneTrigger = this.zoneTriggers[i];
				if (!(obstacleCourseZoneTrigger == null))
				{
					obstacleCourseZoneTrigger.OnPlayerTriggerEnter += this.OnPlayerEnterZone;
					obstacleCourseZoneTrigger.OnPlayerTriggerExit += this.OnPlayerExitZone;
				}
			}
			this.TappableBell.OnTapped += this.OnEndLineTrigger;
		}

		// Token: 0x06004EE9 RID: 20201 RVA: 0x00188D3C File Offset: 0x00186F3C
		private void OnDestroy()
		{
			for (int i = 0; i < this.zoneTriggers.Length; i++)
			{
				ObstacleCourseZoneTrigger obstacleCourseZoneTrigger = this.zoneTriggers[i];
				if (!(obstacleCourseZoneTrigger == null))
				{
					obstacleCourseZoneTrigger.OnPlayerTriggerEnter -= this.OnPlayerEnterZone;
					obstacleCourseZoneTrigger.OnPlayerTriggerExit -= this.OnPlayerExitZone;
				}
			}
			this.TappableBell.OnTapped -= this.OnEndLineTrigger;
		}

		// Token: 0x06004EEA RID: 20202 RVA: 0x00188DA9 File Offset: 0x00186FA9
		private void Start()
		{
			this.RestartTimer(false);
		}

		// Token: 0x06004EEB RID: 20203 RVA: 0x00188DB4 File Offset: 0x00186FB4
		public void InvokeUpdate()
		{
			foreach (ZoneBasedObject zoneBasedObject in this.zoneBasedVisuals)
			{
				if (zoneBasedObject != null)
				{
					zoneBasedObject.gameObject.SetActive(zoneBasedObject.IsLocalPlayerInZone());
				}
			}
			if (NetworkSystem.Instance.InRoom && ObstacleCourseManager.Instance.IsMine && this.currentState == ObstacleCourse.RaceState.Finished && Time.time - this.startTime >= this.cooldownTime)
			{
				this.RestartTimer(true);
			}
		}

		// Token: 0x06004EEC RID: 20204 RVA: 0x00188E30 File Offset: 0x00187030
		public void OnPlayerEnterZone(Collider other)
		{
			if (ObstacleCourseManager.Instance.IsMine)
			{
				this.numPlayersOnCourse++;
			}
		}

		// Token: 0x06004EED RID: 20205 RVA: 0x00188E4C File Offset: 0x0018704C
		public void OnPlayerExitZone(Collider other)
		{
			if (ObstacleCourseManager.Instance.IsMine)
			{
				this.numPlayersOnCourse--;
			}
		}

		// Token: 0x06004EEE RID: 20206 RVA: 0x00188E68 File Offset: 0x00187068
		private void RestartTimer(bool playFx = true)
		{
			this.UpdateState(ObstacleCourse.RaceState.Started, playFx);
		}

		// Token: 0x06004EEF RID: 20207 RVA: 0x00188E72 File Offset: 0x00187072
		private void EndRace()
		{
			this.UpdateState(ObstacleCourse.RaceState.Finished, true);
			this.startTime = Time.time;
		}

		// Token: 0x06004EF0 RID: 20208 RVA: 0x00188E88 File Offset: 0x00187088
		public void PlayWinningEffects()
		{
			if (this.confettiParticle)
			{
				this.confettiParticle.Play();
			}
			if (this.bannerRenderer)
			{
				UberShaderProperty baseColor = UberShader.BaseColor;
				Material material = this.bannerRenderer.material;
				RigContainer rigContainer = this.winnerRig;
				baseColor.SetValue<Color?>(material, (rigContainer != null) ? new Color?(rigContainer.Rig.playerColor) : null);
			}
			this.audioSource.GTPlay();
		}

		// Token: 0x06004EF1 RID: 20209 RVA: 0x00188EFE File Offset: 0x001870FE
		public void OnEndLineTrigger(VRRig rig)
		{
			if (ObstacleCourseManager.Instance.IsMine && this.currentState == ObstacleCourse.RaceState.Started)
			{
				this.winnerActorNumber = rig.creator.ActorNumber;
				this.winnerRig = rig.rigContainer;
				this.EndRace();
			}
		}

		// Token: 0x06004EF2 RID: 20210 RVA: 0x00188F37 File Offset: 0x00187137
		public void Deserialize(int _winnerActorNumber, ObstacleCourse.RaceState _currentState)
		{
			if (!ObstacleCourseManager.Instance.IsMine)
			{
				this.winnerActorNumber = _winnerActorNumber;
				VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.GetPlayer(this.winnerActorNumber), out this.winnerRig);
				this.UpdateState(_currentState, true);
			}
		}

		// Token: 0x06004EF3 RID: 20211 RVA: 0x00188F78 File Offset: 0x00187178
		private void UpdateState(ObstacleCourse.RaceState state, bool playFX = true)
		{
			this.currentState = state;
			WinnerScoreboard winnerScoreboard = this.scoreboard;
			RigContainer rigContainer = this.winnerRig;
			winnerScoreboard.UpdateBoard((rigContainer != null) ? rigContainer.Rig.playerNameVisible : null, this.currentState);
			if (this.currentState == ObstacleCourse.RaceState.Finished)
			{
				this.PlayWinningEffects();
			}
			else if (this.currentState == ObstacleCourse.RaceState.Started && this.bannerRenderer)
			{
				UberShader.BaseColor.SetValue<Color>(this.bannerRenderer.material, Color.white);
			}
			this.UpdateStartingGate();
		}

		// Token: 0x06004EF4 RID: 20212 RVA: 0x00188FFC File Offset: 0x001871FC
		private void UpdateStartingGate()
		{
			if (this.currentState == ObstacleCourse.RaceState.Finished)
			{
				this.leftGate.transform.RotateAround(this.leftGate.transform.position, Vector3.up, 90f);
				this.rightGate.transform.RotateAround(this.rightGate.transform.position, Vector3.up, -90f);
				return;
			}
			if (this.currentState == ObstacleCourse.RaceState.Started)
			{
				this.leftGate.transform.RotateAround(this.leftGate.transform.position, Vector3.up, -90f);
				this.rightGate.transform.RotateAround(this.rightGate.transform.position, Vector3.up, 90f);
			}
		}

		// Token: 0x040057F0 RID: 22512
		public WinnerScoreboard scoreboard;

		// Token: 0x040057F2 RID: 22514
		private RigContainer winnerRig;

		// Token: 0x040057F3 RID: 22515
		public ObstacleCourseZoneTrigger[] zoneTriggers;

		// Token: 0x040057F4 RID: 22516
		[HideInInspector]
		public ObstacleCourse.RaceState currentState;

		// Token: 0x040057F5 RID: 22517
		[SerializeField]
		private ParticleSystem confettiParticle;

		// Token: 0x040057F6 RID: 22518
		[SerializeField]
		private Renderer bannerRenderer;

		// Token: 0x040057F7 RID: 22519
		[SerializeField]
		private TappableBell TappableBell;

		// Token: 0x040057F8 RID: 22520
		[SerializeField]
		private AudioSource audioSource;

		// Token: 0x040057F9 RID: 22521
		[SerializeField]
		private float cooldownTime = 20f;

		// Token: 0x040057FA RID: 22522
		[SerializeField]
		private ZoneBasedObject[] zoneBasedVisuals;

		// Token: 0x040057FB RID: 22523
		public GameObject leftGate;

		// Token: 0x040057FC RID: 22524
		public GameObject rightGate;

		// Token: 0x040057FD RID: 22525
		private int numPlayersOnCourse;

		// Token: 0x040057FE RID: 22526
		private float startTime;

		// Token: 0x02000C77 RID: 3191
		public enum RaceState
		{
			// Token: 0x04005800 RID: 22528
			Started,
			// Token: 0x04005801 RID: 22529
			Waiting,
			// Token: 0x04005802 RID: 22530
			Finished
		}
	}
}
