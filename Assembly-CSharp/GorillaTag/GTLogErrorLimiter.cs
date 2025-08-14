using System;
using System.Runtime.CompilerServices;
using System.Text;
using Cysharp.Text;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02000E64 RID: 3684
	public class GTLogErrorLimiter
	{
		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x06005C3D RID: 23613 RVA: 0x001D047F File Offset: 0x001CE67F
		// (set) Token: 0x06005C3E RID: 23614 RVA: 0x001D0487 File Offset: 0x001CE687
		public string baseMessage
		{
			get
			{
				return this._baseMessage;
			}
			set
			{
				this._baseMessage = (value ?? "__NULL__");
			}
		}

		// Token: 0x06005C3F RID: 23615 RVA: 0x001D0499 File Offset: 0x001CE699
		public GTLogErrorLimiter(string baseMessage, int countdown = 10, string occurrencesJoinString = "\n- ")
		{
			this.baseMessage = baseMessage;
			this.countdown = countdown;
			this.sb = ZString.CreateStringBuilder();
			this.sb.Append(this.baseMessage);
			this.occurrencesJoinString = occurrencesJoinString;
		}

		// Token: 0x06005C40 RID: 23616 RVA: 0x001D04D4 File Offset: 0x001CE6D4
		public void Log(string subMessage = "", Object context = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.countdown < 0)
			{
				return;
			}
			if (this.countdown == 0)
			{
				this.sb.Insert(0, "!!!! THIS MESSAGE HAS REACHED MAX SPAM COUNT AND WILL NO LONGER BE LOGGED !!!!\n");
			}
			this.sb.Append(subMessage ?? "__NULL__");
			this.sb.Append("\n\nError origin - Caller: ");
			this.sb.Append(caller ?? "__NULL__");
			this.sb.Append(", Line: ");
			this.sb.Append(line);
			this.sb.Append("File: ");
			this.sb.Append(sourceFilePath ?? "__NULL__");
			Debug.LogError(this.sb.ToString(), context);
			this.sb.Clear();
			this.sb.Append(this.baseMessage);
			this.countdown--;
			this.occurrenceCount = 0;
		}

		// Token: 0x06005C41 RID: 23617 RVA: 0x001D05C9 File Offset: 0x001CE7C9
		public void Log(Object obj, Object context = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (!obj)
			{
				this.Log("__NULL__", context, caller, sourceFilePath, line);
				return;
			}
			this.Log(obj.ToString(), null, "Log", "C:\\Users\\root\\GT\\Assets\\GorillaTag\\Shared\\Scripts\\MonkeFX\\GTLogErrorLimiter.cs", 137);
		}

		// Token: 0x06005C42 RID: 23618 RVA: 0x001D0601 File Offset: 0x001CE801
		public void AddOccurrence(string s)
		{
			this.occurrenceCount++;
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Append(s);
		}

		// Token: 0x06005C43 RID: 23619 RVA: 0x001D0637 File Offset: 0x001CE837
		public void AddOccurrence(StringBuilder stringBuilder)
		{
			this.occurrenceCount++;
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Append<StringBuilder>(stringBuilder);
		}

		// Token: 0x06005C44 RID: 23620 RVA: 0x001D0670 File Offset: 0x001CE870
		public void AddOccurence(GameObject gObj)
		{
			this.occurrenceCount++;
			if (gObj == null)
			{
				this.AddOccurrence("__NULL__");
				return;
			}
			this.sb.Append(this.occurrencesJoinString ?? "\n- ");
			this.sb.Q(gObj.GetPath());
		}

		// Token: 0x06005C45 RID: 23621 RVA: 0x001D06CC File Offset: 0x001CE8CC
		public void LogOccurrences(Component component = null, Object obj = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.occurrenceCount <= 0)
			{
				return;
			}
			this.sb.Insert(0, string.Format("Occurred {0} times: ", this.occurrenceCount));
			this.Log("\"" + component.GetComponentPath(int.MaxValue) + "\"", obj, caller, sourceFilePath, line);
		}

		// Token: 0x06005C46 RID: 23622 RVA: 0x001D072C File Offset: 0x001CE92C
		public void LogOccurrences(Utf16ValueStringBuilder subMessage, Object obj = null, [CallerMemberName] string caller = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int line = 0)
		{
			if (this.occurrenceCount <= 0)
			{
				return;
			}
			this.sb.Insert(0, string.Format("Occurred {0} times: ", this.occurrenceCount));
			this.sb.Append<Utf16ValueStringBuilder>(subMessage);
			this.Log("", obj, caller, sourceFilePath, line);
		}

		// Token: 0x040065E0 RID: 26080
		private const string __NULL__ = "__NULL__";

		// Token: 0x040065E1 RID: 26081
		public int countdown;

		// Token: 0x040065E2 RID: 26082
		public int occurrenceCount;

		// Token: 0x040065E3 RID: 26083
		public string occurrencesJoinString;

		// Token: 0x040065E4 RID: 26084
		private string _baseMessage;

		// Token: 0x040065E5 RID: 26085
		public Utf16ValueStringBuilder sb;

		// Token: 0x040065E6 RID: 26086
		private const string k_lastMsgHeader = "!!!! THIS MESSAGE HAS REACHED MAX SPAM COUNT AND WILL NO LONGER BE LOGGED !!!!\n";
	}
}
