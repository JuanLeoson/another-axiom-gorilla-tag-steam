using System;

namespace GorillaNetworking
{
	// Token: 0x02000D77 RID: 3447
	public class CustomMapNetworkJoinTrigger : GorillaNetworkJoinTrigger
	{
		// Token: 0x060055F1 RID: 22001 RVA: 0x001AB044 File Offset: 0x001A9244
		public override string GetFullDesiredGameModeString()
		{
			return string.Concat(new string[]
			{
				this.networkZone,
				GorillaComputer.instance.currentQueue,
				CustomMapLoader.LoadedMapModId.ToString(),
				"_",
				CustomMapLoader.LoadedMapModFileId.ToString(),
				base.GetDesiredGameType()
			});
		}

		// Token: 0x060055F2 RID: 22002 RVA: 0x001AB0A5 File Offset: 0x001A92A5
		public override byte GetRoomSize()
		{
			return CustomMapLoader.GetRoomSizeForCurrentlyLoadedMap();
		}
	}
}
