using System;
using System.Runtime.InteropServices;

namespace Viveport.Internal
{
	// Token: 0x02000B9F RID: 2975
	internal class ArcadeLeaderboard
	{
		// Token: 0x06004738 RID: 18232 RVA: 0x0015FAA7 File Offset: 0x0015DCA7
		static ArcadeLeaderboard()
		{
			Api.LoadLibraryManually("viveport_api");
		}

		// Token: 0x06004739 RID: 18233
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_IsReady")]
		internal static extern void IsReady(StatusCallback IsReadyCallback);

		// Token: 0x0600473A RID: 18234
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_IsReady")]
		internal static extern void IsReady_64(StatusCallback IsReadyCallback);

		// Token: 0x0600473B RID: 18235
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_DownloadLeaderboardScores")]
		internal static extern void DownloadLeaderboardScores(StatusCallback downloadLeaderboardScoresCB, string pchLeaderboardName, ELeaderboardDataTimeRange eLeaderboardDataTimeRange, int nCount);

		// Token: 0x0600473C RID: 18236
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_DownloadLeaderboardScores")]
		internal static extern void DownloadLeaderboardScores_64(StatusCallback downloadLeaderboardScoresCB, string pchLeaderboardName, ELeaderboardDataTimeRange eLeaderboardDataTimeRange, int nCount);

		// Token: 0x0600473D RID: 18237
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_UploadLeaderboardScore")]
		internal static extern void UploadLeaderboardScore(StatusCallback uploadLeaderboardScoreCB, string pchLeaderboardName, string pchUserName, int nScore);

		// Token: 0x0600473E RID: 18238
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_UploadLeaderboardScore")]
		internal static extern void UploadLeaderboardScore_64(StatusCallback uploadLeaderboardScoreCB, string pchLeaderboardName, string pchUserName, int nScore);

		// Token: 0x0600473F RID: 18239
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScore")]
		internal static extern void GetLeaderboardScore(int index, ref LeaderboardEntry_t pLeaderboardEntry);

		// Token: 0x06004740 RID: 18240
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScore")]
		internal static extern void GetLeaderboardScore_64(int index, ref LeaderboardEntry_t pLeaderboardEntry);

		// Token: 0x06004741 RID: 18241
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScoreCount")]
		internal static extern int GetLeaderboardScoreCount();

		// Token: 0x06004742 RID: 18242
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardScoreCount")]
		internal static extern int GetLeaderboardScoreCount_64();

		// Token: 0x06004743 RID: 18243
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserRank")]
		internal static extern int GetLeaderboardUserRank();

		// Token: 0x06004744 RID: 18244
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserRank")]
		internal static extern int GetLeaderboardUserRank_64();

		// Token: 0x06004745 RID: 18245
		[DllImport("viveport_api", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserScore")]
		internal static extern int GetLeaderboardUserScore();

		// Token: 0x06004746 RID: 18246
		[DllImport("viveport_api64", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, EntryPoint = "IViveportArcadeLeaderboard_GetLeaderboardUserScore")]
		internal static extern int GetLeaderboardUserScore_64();
	}
}
