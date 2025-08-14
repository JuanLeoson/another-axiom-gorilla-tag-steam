using System;
using UnityEngine;

// Token: 0x02000348 RID: 840
public static class NativeVideoPlayer
{
	// Token: 0x17000235 RID: 565
	// (get) Token: 0x060013FE RID: 5118 RVA: 0x0006AD88 File Offset: 0x00068F88
	private static IntPtr VideoPlayerClass
	{
		get
		{
			if (NativeVideoPlayer._VideoPlayerClass == null)
			{
				try
				{
					IntPtr intPtr = AndroidJNI.FindClass("com/oculus/videoplayer/NativeVideoPlayer");
					if (intPtr != IntPtr.Zero)
					{
						NativeVideoPlayer._VideoPlayerClass = new IntPtr?(AndroidJNI.NewGlobalRef(intPtr));
						AndroidJNI.DeleteLocalRef(intPtr);
					}
					else
					{
						Debug.LogError("Failed to find NativeVideoPlayer class");
						NativeVideoPlayer._VideoPlayerClass = new IntPtr?(IntPtr.Zero);
					}
				}
				catch (Exception exception)
				{
					Debug.LogError("Failed to find NativeVideoPlayer class");
					Debug.LogException(exception);
					NativeVideoPlayer._VideoPlayerClass = new IntPtr?(IntPtr.Zero);
				}
			}
			return NativeVideoPlayer._VideoPlayerClass.GetValueOrDefault();
		}
	}

	// Token: 0x17000236 RID: 566
	// (get) Token: 0x060013FF RID: 5119 RVA: 0x0006AE28 File Offset: 0x00069028
	private static IntPtr Activity
	{
		get
		{
			if (NativeVideoPlayer._Activity == null)
			{
				try
				{
					IntPtr intPtr = AndroidJNI.FindClass("com/unity3d/player/UnityPlayer");
					IntPtr staticFieldID = AndroidJNI.GetStaticFieldID(intPtr, "currentActivity", "Landroid/app/Activity;");
					IntPtr staticObjectField = AndroidJNI.GetStaticObjectField(intPtr, staticFieldID);
					NativeVideoPlayer._Activity = new IntPtr?(AndroidJNI.NewGlobalRef(staticObjectField));
					AndroidJNI.DeleteLocalRef(staticObjectField);
					AndroidJNI.DeleteLocalRef(intPtr);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					NativeVideoPlayer._Activity = new IntPtr?(IntPtr.Zero);
				}
			}
			return NativeVideoPlayer._Activity.GetValueOrDefault();
		}
	}

	// Token: 0x17000237 RID: 567
	// (get) Token: 0x06001400 RID: 5120 RVA: 0x00002076 File Offset: 0x00000276
	public static bool IsAvailable
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000238 RID: 568
	// (get) Token: 0x06001401 RID: 5121 RVA: 0x0006AEB4 File Offset: 0x000690B4
	public static bool IsPlaying
	{
		get
		{
			if (NativeVideoPlayer.getIsPlayingMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getIsPlayingMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getIsPlaying", "()Z");
			}
			return AndroidJNI.CallStaticBooleanMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getIsPlayingMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x17000239 RID: 569
	// (get) Token: 0x06001402 RID: 5122 RVA: 0x0006AEF4 File Offset: 0x000690F4
	public static NativeVideoPlayer.PlabackState CurrentPlaybackState
	{
		get
		{
			if (NativeVideoPlayer.getCurrentPlaybackStateMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getCurrentPlaybackStateMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getCurrentPlaybackState", "()I");
			}
			return (NativeVideoPlayer.PlabackState)AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getCurrentPlaybackStateMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700023A RID: 570
	// (get) Token: 0x06001403 RID: 5123 RVA: 0x0006AF34 File Offset: 0x00069134
	public static long Duration
	{
		get
		{
			if (NativeVideoPlayer.getDurationMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getDurationMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getDuration", "()J");
			}
			return AndroidJNI.CallStaticLongMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getDurationMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700023B RID: 571
	// (get) Token: 0x06001404 RID: 5124 RVA: 0x0006AF74 File Offset: 0x00069174
	public static NativeVideoPlayer.StereoMode VideoStereoMode
	{
		get
		{
			if (NativeVideoPlayer.getStereoModeMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getStereoModeMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getStereoMode", "()I");
			}
			return (NativeVideoPlayer.StereoMode)AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getStereoModeMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700023C RID: 572
	// (get) Token: 0x06001405 RID: 5125 RVA: 0x0006AFB4 File Offset: 0x000691B4
	public static int VideoWidth
	{
		get
		{
			if (NativeVideoPlayer.getWidthMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getWidthMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getWidth", "()I");
			}
			return AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getWidthMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700023D RID: 573
	// (get) Token: 0x06001406 RID: 5126 RVA: 0x0006AFF4 File Offset: 0x000691F4
	public static int VideoHeight
	{
		get
		{
			if (NativeVideoPlayer.getHeightMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getHeightMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getHeight", "()I");
			}
			return AndroidJNI.CallStaticIntMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getHeightMethodId, NativeVideoPlayer.EmptyParams);
		}
	}

	// Token: 0x1700023E RID: 574
	// (get) Token: 0x06001407 RID: 5127 RVA: 0x0006B034 File Offset: 0x00069234
	// (set) Token: 0x06001408 RID: 5128 RVA: 0x0006B074 File Offset: 0x00069274
	public static long PlaybackPosition
	{
		get
		{
			if (NativeVideoPlayer.getPlaybackPositionMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.getPlaybackPositionMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "getPlaybackPosition", "()J");
			}
			return AndroidJNI.CallStaticLongMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.getPlaybackPositionMethodId, NativeVideoPlayer.EmptyParams);
		}
		set
		{
			if (NativeVideoPlayer.setPlaybackPositionMethodId == IntPtr.Zero)
			{
				NativeVideoPlayer.setPlaybackPositionMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setPlaybackPosition", "(J)V");
				NativeVideoPlayer.setPlaybackPositionParams = new jvalue[1];
			}
			NativeVideoPlayer.setPlaybackPositionParams[0].j = value;
			AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setPlaybackPositionMethodId, NativeVideoPlayer.setPlaybackPositionParams);
		}
	}

	// Token: 0x06001409 RID: 5129 RVA: 0x0006B0DC File Offset: 0x000692DC
	public static void PlayVideo(string path, string drmLicenseUrl, IntPtr surfaceObj)
	{
		if (NativeVideoPlayer.playVideoMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.playVideoMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "playVideo", "(Landroid/content/Context;Ljava/lang/String;Ljava/lang/String;Landroid/view/Surface;)V");
			NativeVideoPlayer.playVideoParams = new jvalue[4];
		}
		IntPtr intPtr = AndroidJNI.NewStringUTF(path);
		IntPtr intPtr2 = AndroidJNI.NewStringUTF(drmLicenseUrl);
		NativeVideoPlayer.playVideoParams[0].l = NativeVideoPlayer.Activity;
		NativeVideoPlayer.playVideoParams[1].l = intPtr;
		NativeVideoPlayer.playVideoParams[2].l = intPtr2;
		NativeVideoPlayer.playVideoParams[3].l = surfaceObj;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.playVideoMethodId, NativeVideoPlayer.playVideoParams);
		AndroidJNI.DeleteLocalRef(intPtr);
		AndroidJNI.DeleteLocalRef(intPtr2);
	}

	// Token: 0x0600140A RID: 5130 RVA: 0x0006B194 File Offset: 0x00069394
	public static void Stop()
	{
		if (NativeVideoPlayer.stopMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.stopMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "stop", "()V");
		}
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.stopMethodId, NativeVideoPlayer.EmptyParams);
	}

	// Token: 0x0600140B RID: 5131 RVA: 0x0006B1D4 File Offset: 0x000693D4
	public static void Play()
	{
		if (NativeVideoPlayer.resumeMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.resumeMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "resume", "()V");
		}
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.resumeMethodId, NativeVideoPlayer.EmptyParams);
	}

	// Token: 0x0600140C RID: 5132 RVA: 0x0006B214 File Offset: 0x00069414
	public static void Pause()
	{
		if (NativeVideoPlayer.pauseMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.pauseMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "pause", "()V");
		}
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.pauseMethodId, NativeVideoPlayer.EmptyParams);
	}

	// Token: 0x0600140D RID: 5133 RVA: 0x0006B254 File Offset: 0x00069454
	public static void SetPlaybackSpeed(float speed)
	{
		if (NativeVideoPlayer.setPlaybackSpeedMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.setPlaybackSpeedMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setPlaybackSpeed", "(F)V");
			NativeVideoPlayer.setPlaybackSpeedParams = new jvalue[1];
		}
		NativeVideoPlayer.setPlaybackSpeedParams[0].f = speed;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setPlaybackSpeedMethodId, NativeVideoPlayer.setPlaybackSpeedParams);
	}

	// Token: 0x0600140E RID: 5134 RVA: 0x0006B2BC File Offset: 0x000694BC
	public static void SetLooping(bool looping)
	{
		if (NativeVideoPlayer.setLoopingMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.setLoopingMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setLooping", "(Z)V");
			NativeVideoPlayer.setLoopingParams = new jvalue[1];
		}
		NativeVideoPlayer.setLoopingParams[0].z = looping;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setLoopingMethodId, NativeVideoPlayer.setLoopingParams);
	}

	// Token: 0x0600140F RID: 5135 RVA: 0x0006B324 File Offset: 0x00069524
	public static void SetListenerRotation(Quaternion rotation)
	{
		if (NativeVideoPlayer.setListenerRotationQuaternionMethodId == IntPtr.Zero)
		{
			NativeVideoPlayer.setListenerRotationQuaternionMethodId = AndroidJNI.GetStaticMethodID(NativeVideoPlayer.VideoPlayerClass, "setListenerRotationQuaternion", "(FFFF)V");
			NativeVideoPlayer.setListenerRotationQuaternionParams = new jvalue[4];
		}
		NativeVideoPlayer.setListenerRotationQuaternionParams[0].f = rotation.x;
		NativeVideoPlayer.setListenerRotationQuaternionParams[1].f = rotation.y;
		NativeVideoPlayer.setListenerRotationQuaternionParams[2].f = rotation.z;
		NativeVideoPlayer.setListenerRotationQuaternionParams[3].f = rotation.w;
		AndroidJNI.CallStaticVoidMethod(NativeVideoPlayer.VideoPlayerClass, NativeVideoPlayer.setListenerRotationQuaternionMethodId, NativeVideoPlayer.setListenerRotationQuaternionParams);
	}

	// Token: 0x04001B68 RID: 7016
	private static IntPtr? _Activity;

	// Token: 0x04001B69 RID: 7017
	private static IntPtr? _VideoPlayerClass;

	// Token: 0x04001B6A RID: 7018
	private static readonly jvalue[] EmptyParams = new jvalue[0];

	// Token: 0x04001B6B RID: 7019
	private static IntPtr getIsPlayingMethodId;

	// Token: 0x04001B6C RID: 7020
	private static IntPtr getCurrentPlaybackStateMethodId;

	// Token: 0x04001B6D RID: 7021
	private static IntPtr getDurationMethodId;

	// Token: 0x04001B6E RID: 7022
	private static IntPtr getStereoModeMethodId;

	// Token: 0x04001B6F RID: 7023
	private static IntPtr getWidthMethodId;

	// Token: 0x04001B70 RID: 7024
	private static IntPtr getHeightMethodId;

	// Token: 0x04001B71 RID: 7025
	private static IntPtr getPlaybackPositionMethodId;

	// Token: 0x04001B72 RID: 7026
	private static IntPtr setPlaybackPositionMethodId;

	// Token: 0x04001B73 RID: 7027
	private static jvalue[] setPlaybackPositionParams;

	// Token: 0x04001B74 RID: 7028
	private static IntPtr playVideoMethodId;

	// Token: 0x04001B75 RID: 7029
	private static jvalue[] playVideoParams;

	// Token: 0x04001B76 RID: 7030
	private static IntPtr stopMethodId;

	// Token: 0x04001B77 RID: 7031
	private static IntPtr resumeMethodId;

	// Token: 0x04001B78 RID: 7032
	private static IntPtr pauseMethodId;

	// Token: 0x04001B79 RID: 7033
	private static IntPtr setPlaybackSpeedMethodId;

	// Token: 0x04001B7A RID: 7034
	private static jvalue[] setPlaybackSpeedParams;

	// Token: 0x04001B7B RID: 7035
	private static IntPtr setLoopingMethodId;

	// Token: 0x04001B7C RID: 7036
	private static jvalue[] setLoopingParams;

	// Token: 0x04001B7D RID: 7037
	private static IntPtr setListenerRotationQuaternionMethodId;

	// Token: 0x04001B7E RID: 7038
	private static jvalue[] setListenerRotationQuaternionParams;

	// Token: 0x02000349 RID: 841
	public enum PlabackState
	{
		// Token: 0x04001B80 RID: 7040
		Idle = 1,
		// Token: 0x04001B81 RID: 7041
		Preparing,
		// Token: 0x04001B82 RID: 7042
		Buffering,
		// Token: 0x04001B83 RID: 7043
		Ready,
		// Token: 0x04001B84 RID: 7044
		Ended
	}

	// Token: 0x0200034A RID: 842
	public enum StereoMode
	{
		// Token: 0x04001B86 RID: 7046
		Unknown = -1,
		// Token: 0x04001B87 RID: 7047
		Mono,
		// Token: 0x04001B88 RID: 7048
		TopBottom,
		// Token: 0x04001B89 RID: 7049
		LeftRight,
		// Token: 0x04001B8A RID: 7050
		Mesh
	}
}
