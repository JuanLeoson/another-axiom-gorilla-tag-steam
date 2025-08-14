using System;
using ExitGames.Client.Photon;
using Photon.Voice;
using Photon.Voice.Unity;
using POpusCodec.Enums;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02000D44 RID: 3396
	[CreateAssetMenu(fileName = "VoiceSettings", menuName = "Gorilla Tag/VoiceSettings")]
	public class SO_NetworkVoiceSettings : ScriptableObject
	{
		// Token: 0x04005D90 RID: 23952
		[Header("Voice settings")]
		public bool AutoConnectAndJoin = true;

		// Token: 0x04005D91 RID: 23953
		public bool AutoLeaveAndDisconnect = true;

		// Token: 0x04005D92 RID: 23954
		public bool WorkInOfflineMode = true;

		// Token: 0x04005D93 RID: 23955
		public DebugLevel LogLevel = DebugLevel.ERROR;

		// Token: 0x04005D94 RID: 23956
		public DebugLevel GlobalRecordersLogLevel = DebugLevel.INFO;

		// Token: 0x04005D95 RID: 23957
		public DebugLevel GlobalSpeakersLogLevel = DebugLevel.INFO;

		// Token: 0x04005D96 RID: 23958
		public bool CreateSpeakerIfNotFound;

		// Token: 0x04005D97 RID: 23959
		public int UpdateInterval = 50;

		// Token: 0x04005D98 RID: 23960
		public bool SupportLogger;

		// Token: 0x04005D99 RID: 23961
		public int BackgroundTimeout = 60000;

		// Token: 0x04005D9A RID: 23962
		[Header("Recorder Settings")]
		public bool RecordOnlyWhenEnabled;

		// Token: 0x04005D9B RID: 23963
		public bool RecordOnlyWhenJoined = true;

		// Token: 0x04005D9C RID: 23964
		public bool StopRecordingWhenPaused;

		// Token: 0x04005D9D RID: 23965
		public bool TransmitEnabled = true;

		// Token: 0x04005D9E RID: 23966
		public bool AutoStart = true;

		// Token: 0x04005D9F RID: 23967
		public bool Encrypt;

		// Token: 0x04005DA0 RID: 23968
		public byte InterestGroup;

		// Token: 0x04005DA1 RID: 23969
		public bool DebugEcho;

		// Token: 0x04005DA2 RID: 23970
		public bool ReliableMode;

		// Token: 0x04005DA3 RID: 23971
		[Header("Recorder Codec Parameters")]
		public OpusCodec.FrameDuration FrameDuration = OpusCodec.FrameDuration.Frame60ms;

		// Token: 0x04005DA4 RID: 23972
		public SamplingRate SamplingRate = SamplingRate.Sampling16000;

		// Token: 0x04005DA5 RID: 23973
		[Range(6000f, 510000f)]
		public int Bitrate = 20000;

		// Token: 0x04005DA6 RID: 23974
		[Header("Recorder Audio Source Settings")]
		public Recorder.InputSourceType InputSourceType;

		// Token: 0x04005DA7 RID: 23975
		public Recorder.MicType MicrophoneType;

		// Token: 0x04005DA8 RID: 23976
		public bool UseFallback = true;

		// Token: 0x04005DA9 RID: 23977
		public bool Detect = true;

		// Token: 0x04005DAA RID: 23978
		[Range(0f, 1f)]
		public float Threshold = 0.07f;

		// Token: 0x04005DAB RID: 23979
		public int Delay = 500;
	}
}
