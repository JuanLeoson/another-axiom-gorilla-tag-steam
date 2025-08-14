using System;
using GT_CustomMapSupportRuntime;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts.CustomMapSupport
{
	// Token: 0x02000C57 RID: 3159
	public class CMSTrigger : MonoBehaviour
	{
		// Token: 0x06004E28 RID: 20008 RVA: 0x001848F3 File Offset: 0x00182AF3
		public void OnEnable()
		{
			if (this.onEnableTriggerDelay > 0.0)
			{
				this.enabledTime = (double)Time.time;
			}
		}

		// Token: 0x06004E29 RID: 20009 RVA: 0x00184912 File Offset: 0x00182B12
		public byte GetID()
		{
			return this.id;
		}

		// Token: 0x06004E2A RID: 20010 RVA: 0x0018491C File Offset: 0x00182B1C
		public virtual void CopyTriggerSettings(TriggerSettings settings)
		{
			this.id = settings.triggerId;
			this.triggeredBy = settings.triggeredBy;
			float num = Math.Max(settings.validationDistance, 2f);
			this.validationDistanceSquared = num * num;
			if (this.triggeredBy == TriggerSource.None)
			{
				if (settings.triggeredByHead && !settings.triggeredByBody)
				{
					this.triggeredBy = TriggerSource.Head;
				}
				else if (settings.triggeredByBody && !settings.triggeredByHead)
				{
					this.triggeredBy = TriggerSource.Body;
				}
				else if (settings.triggeredByHands && !settings.triggeredByHead && !settings.triggeredByBody)
				{
					this.triggeredBy = TriggerSource.Hands;
				}
				else
				{
					this.triggeredBy = TriggerSource.HeadOrBody;
				}
			}
			TriggerSource triggerSource = this.triggeredBy;
			if (triggerSource != TriggerSource.Hands)
			{
				if (triggerSource - TriggerSource.Head <= 2)
				{
					base.gameObject.layer = UnityLayer.GorillaTrigger.ToLayerIndex();
				}
			}
			else
			{
				base.gameObject.layer = UnityLayer.GorillaInteractable.ToLayerIndex();
			}
			this.onEnableTriggerDelay = settings.onEnableTriggerDelay;
			this.generalRetriggerDelay = settings.generalRetriggerDelay;
			this.retriggerAfterDuration = settings.retriggerAfterDuration;
			if (Math.Abs(settings.retriggerDelay - 2f) > 0.001f && Math.Abs(settings.retriggerStayDuration - 2.0) < 0.001)
			{
				settings.retriggerStayDuration = (double)settings.retriggerDelay;
			}
			this.retriggerStayDuration = Math.Max(this.generalRetriggerDelay, settings.retriggerStayDuration);
			if (this.retriggerStayDuration <= 0.0)
			{
				this.retriggerAfterDuration = false;
			}
			this.numAllowedTriggers = settings.numAllowedTriggers;
			this.syncedToAllPlayers = settings.syncedToAllPlayers_private;
			if (this.syncedToAllPlayers)
			{
				CMSSerializer.RegisterTrigger(base.gameObject.scene.name, this);
			}
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].isTrigger = true;
			}
		}

		// Token: 0x06004E2B RID: 20011 RVA: 0x00184AED File Offset: 0x00182CED
		public void OnTriggerEnter(Collider triggeringCollider)
		{
			if (this.ValidateCollider(triggeringCollider) && this.CanTrigger())
			{
				this.OnTriggerActivation(triggeringCollider);
			}
		}

		// Token: 0x06004E2C RID: 20012 RVA: 0x00184B08 File Offset: 0x00182D08
		private void OnTriggerStay(Collider other)
		{
			if (!this.retriggerAfterDuration)
			{
				return;
			}
			if (this.ValidateCollider(other) && this.CanTrigger())
			{
				double num = (double)Time.time;
				if (NetworkSystem.Instance.InRoom)
				{
					num = PhotonNetwork.Time;
				}
				if (this.lastTriggerTime + this.retriggerStayDuration <= num)
				{
					this.OnTriggerActivation(other);
				}
			}
		}

		// Token: 0x06004E2D RID: 20013 RVA: 0x00184B60 File Offset: 0x00182D60
		private bool ValidateCollider(Collider other)
		{
			GameObject gameObject = other.gameObject;
			bool flag = gameObject == GorillaTagger.Instance.headCollider.gameObject && (this.triggeredBy == TriggerSource.Head || this.triggeredBy == TriggerSource.HeadOrBody);
			bool flag2;
			if (GorillaTagger.Instance.bodyCollider.enabled)
			{
				flag2 = (gameObject == GorillaTagger.Instance.bodyCollider.gameObject && (this.triggeredBy == TriggerSource.Body || this.triggeredBy == TriggerSource.HeadOrBody));
			}
			else
			{
				flag2 = (gameObject == VRRig.LocalRig.gameObject && (this.triggeredBy == TriggerSource.Body || this.triggeredBy == TriggerSource.HeadOrBody));
			}
			bool flag3 = (gameObject == GorillaTagger.Instance.leftHandTriggerCollider.gameObject || gameObject == GorillaTagger.Instance.rightHandTriggerCollider.gameObject) && this.triggeredBy == TriggerSource.Hands;
			return flag || flag2 || flag3;
		}

		// Token: 0x06004E2E RID: 20014 RVA: 0x00184C52 File Offset: 0x00182E52
		private void OnTriggerActivation(Collider activatingCollider)
		{
			if (this.syncedToAllPlayers)
			{
				CMSSerializer.RequestTrigger(this.id);
				return;
			}
			this.Trigger(-1.0, true, false);
		}

		// Token: 0x06004E2F RID: 20015 RVA: 0x00184C7C File Offset: 0x00182E7C
		public bool CanTrigger()
		{
			if (this.numAllowedTriggers > 0 && this.numTimesTriggered >= this.numAllowedTriggers)
			{
				return false;
			}
			if (this.onEnableTriggerDelay > 0.0 && (double)Time.time - this.enabledTime < this.onEnableTriggerDelay)
			{
				return false;
			}
			if (this.generalRetriggerDelay <= 0.0)
			{
				return true;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (PhotonNetwork.Time - this.lastTriggerTime < -1.0)
				{
					this.lastTriggerTime = -(4294967.295 - this.lastTriggerTime);
				}
				if (this.lastTriggerTime + this.generalRetriggerDelay <= PhotonNetwork.Time)
				{
					return true;
				}
			}
			else if (this.lastTriggerTime + this.generalRetriggerDelay <= (double)Time.time)
			{
				return true;
			}
			return false;
		}

		// Token: 0x06004E30 RID: 20016 RVA: 0x00184D48 File Offset: 0x00182F48
		public virtual void Trigger(double triggerTime = -1.0, bool originatedLocally = false, bool ignoreTriggerCount = false)
		{
			if (!ignoreTriggerCount)
			{
				if (this.numAllowedTriggers > 0 && this.numTimesTriggered >= this.numAllowedTriggers)
				{
					return;
				}
				this.numTimesTriggered += 1;
			}
			if (NetworkSystem.Instance.InRoom)
			{
				if (triggerTime < 0.0)
				{
					triggerTime = PhotonNetwork.Time;
				}
			}
			else if (originatedLocally)
			{
				triggerTime = (double)Time.time;
			}
			this.lastTriggerTime = triggerTime;
			if (this.numAllowedTriggers > 0 && this.numTimesTriggered >= this.numAllowedTriggers)
			{
				Collider[] components = base.gameObject.GetComponents<Collider>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = false;
				}
			}
		}

		// Token: 0x06004E31 RID: 20017 RVA: 0x00184DEC File Offset: 0x00182FEC
		public void ResetTrigger(bool onlyResetTriggerCount = false)
		{
			if (!onlyResetTriggerCount)
			{
				this.lastTriggerTime = -1.0;
			}
			this.numTimesTriggered = 0;
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = true;
			}
			CMSSerializer.ResetTrigger(this.id);
		}

		// Token: 0x06004E32 RID: 20018 RVA: 0x00184E40 File Offset: 0x00183040
		public void SetTriggerCount(byte value)
		{
			this.numTimesTriggered = Math.Min(value, this.numAllowedTriggers);
			if (this.numTimesTriggered >= this.numAllowedTriggers)
			{
				Collider[] components = base.gameObject.GetComponents<Collider>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = false;
				}
			}
		}

		// Token: 0x06004E33 RID: 20019 RVA: 0x00184E90 File Offset: 0x00183090
		public void SetLastTriggerTime(double value)
		{
			this.lastTriggerTime = value;
		}

		// Token: 0x04005714 RID: 22292
		public const byte INVALID_TRIGGER_ID = 255;

		// Token: 0x04005715 RID: 22293
		public const double MAX_PHOTON_SERVER_TIME = 4294967.295;

		// Token: 0x04005716 RID: 22294
		public const float MINIMUM_VALIDATION_DISTANCE = 2f;

		// Token: 0x04005717 RID: 22295
		public bool syncedToAllPlayers;

		// Token: 0x04005718 RID: 22296
		public float validationDistanceSquared;

		// Token: 0x04005719 RID: 22297
		public TriggerSource triggeredBy = TriggerSource.HeadOrBody;

		// Token: 0x0400571A RID: 22298
		public double onEnableTriggerDelay;

		// Token: 0x0400571B RID: 22299
		public double generalRetriggerDelay;

		// Token: 0x0400571C RID: 22300
		public bool retriggerAfterDuration;

		// Token: 0x0400571D RID: 22301
		public double retriggerStayDuration = 2.0;

		// Token: 0x0400571E RID: 22302
		public byte numAllowedTriggers;

		// Token: 0x0400571F RID: 22303
		private byte numTimesTriggered;

		// Token: 0x04005720 RID: 22304
		private double lastTriggerTime = -1.0;

		// Token: 0x04005721 RID: 22305
		private double enabledTime = -1.0;

		// Token: 0x04005722 RID: 22306
		public byte id = byte.MaxValue;
	}
}
