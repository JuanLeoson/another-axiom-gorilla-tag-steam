using System;
using AOT;
using Viveport.Internal;

namespace Viveport
{
	// Token: 0x02000B77 RID: 2935
	public class UserStats
	{
		// Token: 0x0600463D RID: 17981 RVA: 0x0015D464 File Offset: 0x0015B664
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void IsReadyIl2cppCallback(int errorCode)
		{
			UserStats.isReadyIl2cppCallback(errorCode);
		}

		// Token: 0x0600463E RID: 17982 RVA: 0x0015D474 File Offset: 0x0015B674
		public static int IsReady(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.isReadyIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.IsReady_64(new StatusCallback(UserStats.IsReadyIl2cppCallback));
			}
			return UserStats.IsReady(new StatusCallback(UserStats.IsReadyIl2cppCallback));
		}

		// Token: 0x0600463F RID: 17983 RVA: 0x0015D4E1 File Offset: 0x0015B6E1
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadStatsIl2cppCallback(int errorCode)
		{
			UserStats.downloadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06004640 RID: 17984 RVA: 0x0015D4F0 File Offset: 0x0015B6F0
		public static int DownloadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadStats_64(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
			}
			return UserStats.DownloadStats(new StatusCallback(UserStats.DownloadStatsIl2cppCallback));
		}

		// Token: 0x06004641 RID: 17985 RVA: 0x0015D560 File Offset: 0x0015B760
		public static int GetStat(string name, int defaultValue)
		{
			int result = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref result);
			}
			else
			{
				UserStats.GetStat(name, ref result);
			}
			return result;
		}

		// Token: 0x06004642 RID: 17986 RVA: 0x0015D58C File Offset: 0x0015B78C
		public static float GetStat(string name, float defaultValue)
		{
			float result = defaultValue;
			if (IntPtr.Size == 8)
			{
				UserStats.GetStat_64(name, ref result);
			}
			else
			{
				UserStats.GetStat(name, ref result);
			}
			return result;
		}

		// Token: 0x06004643 RID: 17987 RVA: 0x0015D5B8 File Offset: 0x0015B7B8
		public static void SetStat(string name, int value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06004644 RID: 17988 RVA: 0x0015D5D3 File Offset: 0x0015B7D3
		public static void SetStat(string name, float value)
		{
			if (IntPtr.Size == 8)
			{
				UserStats.SetStat_64(name, value);
				return;
			}
			UserStats.SetStat(name, value);
		}

		// Token: 0x06004645 RID: 17989 RVA: 0x0015D5EE File Offset: 0x0015B7EE
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadStatsIl2cppCallback(int errorCode)
		{
			UserStats.uploadStatsIl2cppCallback(errorCode);
		}

		// Token: 0x06004646 RID: 17990 RVA: 0x0015D5FC File Offset: 0x0015B7FC
		public static int UploadStats(StatusCallback callback)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadStatsIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadStats_64(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
			}
			return UserStats.UploadStats(new StatusCallback(UserStats.UploadStatsIl2cppCallback));
		}

		// Token: 0x06004647 RID: 17991 RVA: 0x0015D66C File Offset: 0x0015B86C
		public static bool GetAchievement(string pchName)
		{
			int num = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievement_64(pchName, ref num);
			}
			else
			{
				UserStats.GetAchievement(pchName, ref num);
			}
			return num == 1;
		}

		// Token: 0x06004648 RID: 17992 RVA: 0x0015D69C File Offset: 0x0015B89C
		public static int GetAchievementUnlockTime(string pchName)
		{
			int result = 0;
			if (IntPtr.Size == 8)
			{
				UserStats.GetAchievementUnlockTime_64(pchName, ref result);
			}
			else
			{
				UserStats.GetAchievementUnlockTime(pchName, ref result);
			}
			return result;
		}

		// Token: 0x06004649 RID: 17993 RVA: 0x0015D6C8 File Offset: 0x0015B8C8
		public static string GetAchievementIcon(string pchName)
		{
			return "";
		}

		// Token: 0x0600464A RID: 17994 RVA: 0x0015D6C8 File Offset: 0x0015B8C8
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr)
		{
			return "";
		}

		// Token: 0x0600464B RID: 17995 RVA: 0x0015D6C8 File Offset: 0x0015B8C8
		public static string GetAchievementDisplayAttribute(string pchName, UserStats.AchievementDisplayAttribute attr, Locale locale)
		{
			return "";
		}

		// Token: 0x0600464C RID: 17996 RVA: 0x0015D6CF File Offset: 0x0015B8CF
		public static int SetAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.SetAchievement_64(pchName);
			}
			return UserStats.SetAchievement(pchName);
		}

		// Token: 0x0600464D RID: 17997 RVA: 0x0015D6E6 File Offset: 0x0015B8E6
		public static int ClearAchievement(string pchName)
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.ClearAchievement_64(pchName);
			}
			return UserStats.ClearAchievement(pchName);
		}

		// Token: 0x0600464E RID: 17998 RVA: 0x0015D6FD File Offset: 0x0015B8FD
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void DownloadLeaderboardScoresIl2cppCallback(int errorCode)
		{
			UserStats.downloadLeaderboardScoresIl2cppCallback(errorCode);
		}

		// Token: 0x0600464F RID: 17999 RVA: 0x0015D70C File Offset: 0x0015B90C
		public static int DownloadLeaderboardScores(StatusCallback callback, string pchLeaderboardName, UserStats.LeaderBoardRequestType eLeaderboardDataRequest, UserStats.LeaderBoardTimeRange eLeaderboardDataTimeRange, int nRangeStart, int nRangeEnd)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.downloadLeaderboardScoresIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.DownloadLeaderboardScores_64(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
			}
			return UserStats.DownloadLeaderboardScores(new StatusCallback(UserStats.DownloadLeaderboardScoresIl2cppCallback), pchLeaderboardName, (ELeaderboardDataRequest)eLeaderboardDataRequest, (ELeaderboardDataTimeRange)eLeaderboardDataTimeRange, nRangeStart, nRangeEnd);
		}

		// Token: 0x06004650 RID: 18000 RVA: 0x0015D787 File Offset: 0x0015B987
		[MonoPInvokeCallback(typeof(StatusCallback))]
		private static void UploadLeaderboardScoreIl2cppCallback(int errorCode)
		{
			UserStats.uploadLeaderboardScoreIl2cppCallback(errorCode);
		}

		// Token: 0x06004651 RID: 18001 RVA: 0x0015D794 File Offset: 0x0015B994
		public static int UploadLeaderboardScore(StatusCallback callback, string pchLeaderboardName, int nScore)
		{
			if (callback == null)
			{
				throw new InvalidOperationException("callback == null");
			}
			UserStats.uploadLeaderboardScoreIl2cppCallback = new StatusCallback(callback.Invoke);
			Api.InternalStatusCallbacks.Add(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback));
			if (IntPtr.Size == 8)
			{
				return UserStats.UploadLeaderboardScore_64(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
			}
			return UserStats.UploadLeaderboardScore(new StatusCallback(UserStats.UploadLeaderboardScoreIl2cppCallback), pchLeaderboardName, nScore);
		}

		// Token: 0x06004652 RID: 18002 RVA: 0x0015D808 File Offset: 0x0015BA08
		public static Leaderboard GetLeaderboardScore(int index)
		{
			LeaderboardEntry_t leaderboardEntry_t;
			leaderboardEntry_t.m_nGlobalRank = 0;
			leaderboardEntry_t.m_nScore = 0;
			leaderboardEntry_t.m_pUserName = "";
			if (IntPtr.Size == 8)
			{
				UserStats.GetLeaderboardScore_64(index, ref leaderboardEntry_t);
			}
			else
			{
				UserStats.GetLeaderboardScore(index, ref leaderboardEntry_t);
			}
			return new Leaderboard
			{
				Rank = leaderboardEntry_t.m_nGlobalRank,
				Score = leaderboardEntry_t.m_nScore,
				UserName = leaderboardEntry_t.m_pUserName
			};
		}

		// Token: 0x06004653 RID: 18003 RVA: 0x0015D876 File Offset: 0x0015BA76
		public static int GetLeaderboardScoreCount()
		{
			if (IntPtr.Size == 8)
			{
				return UserStats.GetLeaderboardScoreCount_64();
			}
			return UserStats.GetLeaderboardScoreCount();
		}

		// Token: 0x06004654 RID: 18004 RVA: 0x0015D88B File Offset: 0x0015BA8B
		public static UserStats.LeaderBoardSortMethod GetLeaderboardSortMethod()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod_64();
			}
			return (UserStats.LeaderBoardSortMethod)UserStats.GetLeaderboardSortMethod();
		}

		// Token: 0x06004655 RID: 18005 RVA: 0x0015D8A0 File Offset: 0x0015BAA0
		public static UserStats.LeaderBoardDiaplayType GetLeaderboardDisplayType()
		{
			if (IntPtr.Size == 8)
			{
				return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType_64();
			}
			return (UserStats.LeaderBoardDiaplayType)UserStats.GetLeaderboardDisplayType();
		}

		// Token: 0x0400511A RID: 20762
		private static StatusCallback isReadyIl2cppCallback;

		// Token: 0x0400511B RID: 20763
		private static StatusCallback downloadStatsIl2cppCallback;

		// Token: 0x0400511C RID: 20764
		private static StatusCallback uploadStatsIl2cppCallback;

		// Token: 0x0400511D RID: 20765
		private static StatusCallback downloadLeaderboardScoresIl2cppCallback;

		// Token: 0x0400511E RID: 20766
		private static StatusCallback uploadLeaderboardScoreIl2cppCallback;

		// Token: 0x02000B78 RID: 2936
		public enum LeaderBoardRequestType
		{
			// Token: 0x04005120 RID: 20768
			GlobalData,
			// Token: 0x04005121 RID: 20769
			GlobalDataAroundUser,
			// Token: 0x04005122 RID: 20770
			LocalData,
			// Token: 0x04005123 RID: 20771
			LocalDataAroundUser
		}

		// Token: 0x02000B79 RID: 2937
		public enum LeaderBoardTimeRange
		{
			// Token: 0x04005125 RID: 20773
			AllTime,
			// Token: 0x04005126 RID: 20774
			Daily,
			// Token: 0x04005127 RID: 20775
			Weekly,
			// Token: 0x04005128 RID: 20776
			Monthly
		}

		// Token: 0x02000B7A RID: 2938
		public enum LeaderBoardSortMethod
		{
			// Token: 0x0400512A RID: 20778
			None,
			// Token: 0x0400512B RID: 20779
			Ascending,
			// Token: 0x0400512C RID: 20780
			Descending
		}

		// Token: 0x02000B7B RID: 2939
		public enum LeaderBoardDiaplayType
		{
			// Token: 0x0400512E RID: 20782
			None,
			// Token: 0x0400512F RID: 20783
			Numeric,
			// Token: 0x04005130 RID: 20784
			TimeSeconds,
			// Token: 0x04005131 RID: 20785
			TimeMilliSeconds
		}

		// Token: 0x02000B7C RID: 2940
		public enum LeaderBoardScoreMethod
		{
			// Token: 0x04005133 RID: 20787
			None,
			// Token: 0x04005134 RID: 20788
			KeepBest,
			// Token: 0x04005135 RID: 20789
			ForceUpdate
		}

		// Token: 0x02000B7D RID: 2941
		public enum AchievementDisplayAttribute
		{
			// Token: 0x04005137 RID: 20791
			Name,
			// Token: 0x04005138 RID: 20792
			Desc,
			// Token: 0x04005139 RID: 20793
			Hidden
		}
	}
}
