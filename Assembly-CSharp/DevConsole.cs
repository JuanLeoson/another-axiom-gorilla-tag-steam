using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001D4 RID: 468
public class DevConsole : MonoBehaviour, IDebugObject
{
	// Token: 0x17000125 RID: 293
	// (get) Token: 0x06000B87 RID: 2951 RVA: 0x00040063 File Offset: 0x0003E263
	public static DevConsole instance
	{
		get
		{
			if (DevConsole._instance == null)
			{
				DevConsole._instance = Object.FindObjectOfType<DevConsole>();
			}
			return DevConsole._instance;
		}
	}

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06000B88 RID: 2952 RVA: 0x00040081 File Offset: 0x0003E281
	public static List<DevConsole.LogEntry> logEntries
	{
		get
		{
			return DevConsole.instance._logEntries;
		}
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x00040090 File Offset: 0x0003E290
	public void OnDestroyDebugObject()
	{
		Debug.Log("Destroying debug instances now");
		foreach (DevConsoleInstance devConsoleInstance in this.instances)
		{
			Object.DestroyImmediate(devConsoleInstance.gameObject);
		}
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x00020127 File Offset: 0x0001E327
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000E2F RID: 3631
	private static DevConsole _instance;

	// Token: 0x04000E30 RID: 3632
	[SerializeField]
	private AudioClip errorSound;

	// Token: 0x04000E31 RID: 3633
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000E32 RID: 3634
	[SerializeField]
	private float maxHeight;

	// Token: 0x04000E33 RID: 3635
	public static readonly string[] tracebackScrubbing = new string[]
	{
		"ExitGames.Client.Photon",
		"Photon.Realtime.LoadBalancingClient",
		"Photon.Pun.PhotonHandler"
	};

	// Token: 0x04000E34 RID: 3636
	private const int kLogEntriesCapacityIncrementAmount = 1024;

	// Token: 0x04000E35 RID: 3637
	[SerializeReference]
	[SerializeField]
	private readonly List<DevConsole.LogEntry> _logEntries = new List<DevConsole.LogEntry>(1024);

	// Token: 0x04000E36 RID: 3638
	public int targetLogIndex = -1;

	// Token: 0x04000E37 RID: 3639
	public int currentLogIndex;

	// Token: 0x04000E38 RID: 3640
	public bool isMuted;

	// Token: 0x04000E39 RID: 3641
	public float currentZoomLevel = 1f;

	// Token: 0x04000E3A RID: 3642
	public List<GameObject> disableWhileActive;

	// Token: 0x04000E3B RID: 3643
	public List<GameObject> enableWhileActive;

	// Token: 0x04000E3C RID: 3644
	public int expandAmount = 20;

	// Token: 0x04000E3D RID: 3645
	public int expandedMessageIndex = -1;

	// Token: 0x04000E3E RID: 3646
	public bool canExpand = true;

	// Token: 0x04000E3F RID: 3647
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x04000E40 RID: 3648
	public float lineStartHeight;

	// Token: 0x04000E41 RID: 3649
	public float textStartHeight;

	// Token: 0x04000E42 RID: 3650
	public float lineStartTextWidth;

	// Token: 0x04000E43 RID: 3651
	public double textScale = 0.5;

	// Token: 0x04000E44 RID: 3652
	public List<DevConsoleInstance> instances;

	// Token: 0x020001D5 RID: 469
	[Serializable]
	public class LogEntry
	{
		// Token: 0x17000127 RID: 295
		// (get) Token: 0x06000B8D RID: 2957 RVA: 0x0004017A File Offset: 0x0003E37A
		public string Message
		{
			get
			{
				if (this.repeatCount > 1)
				{
					return string.Format("({0}) {1}", this.repeatCount, this._Message);
				}
				return this._Message;
			}
		}

		// Token: 0x06000B8E RID: 2958 RVA: 0x000401A8 File Offset: 0x0003E3A8
		public LogEntry(string message, LogType type, string trace)
		{
			this._Message = message;
			this.Type = type;
			this.Trace = trace;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array = trace.Split("\n".ToCharArray(), StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				string line = array[i];
				if (!DevConsole.tracebackScrubbing.Any((string scrubString) => line.Contains(scrubString)))
				{
					stringBuilder.AppendLine(line);
				}
			}
			this.Trace = stringBuilder.ToString();
			DevConsole.LogEntry.TotalIndex++;
			this.index = DevConsole.LogEntry.TotalIndex;
		}

		// Token: 0x04000E45 RID: 3653
		private static int TotalIndex;

		// Token: 0x04000E46 RID: 3654
		[SerializeReference]
		[SerializeField]
		public readonly string _Message;

		// Token: 0x04000E47 RID: 3655
		[SerializeField]
		[SerializeReference]
		public readonly LogType Type;

		// Token: 0x04000E48 RID: 3656
		public readonly string Trace;

		// Token: 0x04000E49 RID: 3657
		public bool forwarded;

		// Token: 0x04000E4A RID: 3658
		public int repeatCount = 1;

		// Token: 0x04000E4B RID: 3659
		public bool filtered;

		// Token: 0x04000E4C RID: 3660
		public int index;
	}

	// Token: 0x020001D7 RID: 471
	[Serializable]
	public class DisplayedLogLine
	{
		// Token: 0x17000128 RID: 296
		// (get) Token: 0x06000B91 RID: 2961 RVA: 0x00040262 File Offset: 0x0003E462
		// (set) Token: 0x06000B92 RID: 2962 RVA: 0x0004026A File Offset: 0x0003E46A
		public Type data { get; set; }

		// Token: 0x06000B93 RID: 2963 RVA: 0x00040274 File Offset: 0x0003E474
		public DisplayedLogLine(GameObject obj)
		{
			this.lineText = obj.GetComponentInChildren<Text>();
			this.buttons = obj.GetComponentsInChildren<GorillaDevButton>();
			this.transform = obj.GetComponent<RectTransform>();
			this.backdrop = obj.GetComponentInChildren<SpriteRenderer>();
			foreach (GorillaDevButton gorillaDevButton in this.buttons)
			{
				if (gorillaDevButton.Type == DevButtonType.LineExpand)
				{
					this.maximizeButton = gorillaDevButton;
				}
				if (gorillaDevButton.Type == DevButtonType.LineForward)
				{
					this.forwardButton = gorillaDevButton;
				}
			}
		}

		// Token: 0x04000E4E RID: 3662
		public GorillaDevButton[] buttons;

		// Token: 0x04000E4F RID: 3663
		public Text lineText;

		// Token: 0x04000E50 RID: 3664
		public RectTransform transform;

		// Token: 0x04000E51 RID: 3665
		public int targetMessage;

		// Token: 0x04000E52 RID: 3666
		public GorillaDevButton maximizeButton;

		// Token: 0x04000E53 RID: 3667
		public GorillaDevButton forwardButton;

		// Token: 0x04000E54 RID: 3668
		public SpriteRenderer backdrop;

		// Token: 0x04000E55 RID: 3669
		private bool expanded;

		// Token: 0x04000E56 RID: 3670
		public DevInspector inspector;
	}

	// Token: 0x020001D8 RID: 472
	[Serializable]
	public class MessagePayload
	{
		// Token: 0x06000B94 RID: 2964 RVA: 0x000402F0 File Offset: 0x0003E4F0
		public static List<DevConsole.MessagePayload> GeneratePayloads(string username, List<DevConsole.LogEntry> entries)
		{
			List<DevConsole.MessagePayload> list = new List<DevConsole.MessagePayload>();
			List<DevConsole.MessagePayload.Block> list2 = new List<DevConsole.MessagePayload.Block>();
			entries.Sort((DevConsole.LogEntry e1, DevConsole.LogEntry e2) => e1.index.CompareTo(e2.index));
			string text = "";
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block("User `" + username + "` Forwarded some errors"));
			foreach (DevConsole.LogEntry logEntry in entries)
			{
				string[] array = logEntry.Trace.Split("\n".ToCharArray());
				string text2 = "";
				foreach (string str in array)
				{
					text2 = text2 + "    " + str + "\n";
				}
				string text3 = string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
				if (text.Length + text3.Length > 3000)
				{
					text += "```";
					list2.Add(new DevConsole.MessagePayload.Block(text));
					list.Add(new DevConsole.MessagePayload
					{
						blocks = list2.ToArray()
					});
					list2 = new List<DevConsole.MessagePayload.Block>();
					text = "```";
				}
				text += string.Format("({0}) {1}\n{2}\n", logEntry.Type, logEntry.Message, text2);
			}
			text += "```";
			list2.Add(new DevConsole.MessagePayload.Block(text));
			list.Add(new DevConsole.MessagePayload
			{
				blocks = list2.ToArray()
			});
			return list;
		}

		// Token: 0x04000E58 RID: 3672
		public DevConsole.MessagePayload.Block[] blocks;

		// Token: 0x020001D9 RID: 473
		[Serializable]
		public class Block
		{
			// Token: 0x06000B96 RID: 2966 RVA: 0x000404C0 File Offset: 0x0003E6C0
			public Block(string markdownText)
			{
				this.text = new DevConsole.MessagePayload.TextBlock
				{
					text = markdownText,
					type = "mrkdwn"
				};
				this.type = "section";
			}

			// Token: 0x04000E59 RID: 3673
			public string type;

			// Token: 0x04000E5A RID: 3674
			public DevConsole.MessagePayload.TextBlock text;
		}

		// Token: 0x020001DA RID: 474
		[Serializable]
		public class TextBlock
		{
			// Token: 0x04000E5B RID: 3675
			public string type;

			// Token: 0x04000E5C RID: 3676
			public string text;
		}
	}
}
