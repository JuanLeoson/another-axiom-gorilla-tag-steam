using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E8E RID: 3726
	[Serializable]
	internal class ExpectedUsersDecayTimer : TickSystemTimerAbstract
	{
		// Token: 0x06005D57 RID: 23895 RVA: 0x001D7C28 File Offset: 0x001D5E28
		public override void OnTimedEvent()
		{
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.IsMasterClient)
			{
				int num = 0;
				if (PhotonNetwork.CurrentRoom.ExpectedUsers != null && PhotonNetwork.CurrentRoom.ExpectedUsers.Length != 0)
				{
					foreach (string key in PhotonNetwork.CurrentRoom.ExpectedUsers)
					{
						float num2;
						if (this.expectedUsers.TryGetValue(key, out num2))
						{
							if (num2 + this.decayTime < Time.time)
							{
								num++;
							}
						}
						else
						{
							this.expectedUsers.Add(key, Time.time);
						}
					}
					if (num >= PhotonNetwork.CurrentRoom.ExpectedUsers.Length && num != 0)
					{
						PhotonNetwork.CurrentRoom.ClearExpectedUsers();
						this.expectedUsers.Clear();
					}
				}
			}
		}

		// Token: 0x06005D58 RID: 23896 RVA: 0x001D7CEF File Offset: 0x001D5EEF
		public override void Stop()
		{
			base.Stop();
			this.expectedUsers.Clear();
		}

		// Token: 0x0400675A RID: 26458
		public float decayTime = 15f;

		// Token: 0x0400675B RID: 26459
		private Dictionary<string, float> expectedUsers = new Dictionary<string, float>(10);
	}
}
