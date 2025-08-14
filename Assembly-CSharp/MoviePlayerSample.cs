using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

// Token: 0x0200034E RID: 846
public class MoviePlayerSample : MonoBehaviour
{
	// Token: 0x17000240 RID: 576
	// (get) Token: 0x06001419 RID: 5145 RVA: 0x0006BFCA File Offset: 0x0006A1CA
	// (set) Token: 0x0600141A RID: 5146 RVA: 0x0006BFD2 File Offset: 0x0006A1D2
	public bool IsPlaying { get; private set; }

	// Token: 0x17000241 RID: 577
	// (get) Token: 0x0600141B RID: 5147 RVA: 0x0006BFDB File Offset: 0x0006A1DB
	// (set) Token: 0x0600141C RID: 5148 RVA: 0x0006BFE3 File Offset: 0x0006A1E3
	public long Duration { get; private set; }

	// Token: 0x17000242 RID: 578
	// (get) Token: 0x0600141D RID: 5149 RVA: 0x0006BFEC File Offset: 0x0006A1EC
	// (set) Token: 0x0600141E RID: 5150 RVA: 0x0006BFF4 File Offset: 0x0006A1F4
	public long PlaybackPosition { get; private set; }

	// Token: 0x0600141F RID: 5151 RVA: 0x0006C000 File Offset: 0x0006A200
	private void Awake()
	{
		Debug.Log("MovieSample Awake");
		this.mediaRenderer = base.GetComponent<Renderer>();
		this.videoPlayer = base.GetComponent<VideoPlayer>();
		if (this.videoPlayer == null)
		{
			this.videoPlayer = base.gameObject.AddComponent<VideoPlayer>();
		}
		this.videoPlayer.isLooping = this.LoopVideo;
		this.overlay = base.GetComponent<OVROverlay>();
		if (this.overlay == null)
		{
			this.overlay = base.gameObject.AddComponent<OVROverlay>();
		}
		this.overlay.enabled = false;
		this.overlay.isExternalSurface = NativeVideoPlayer.IsAvailable;
		this.overlay.enabled = (this.overlay.currentOverlayShape != OVROverlay.OverlayShape.Equirect || Application.platform == RuntimePlatform.Android);
	}

	// Token: 0x06001420 RID: 5152 RVA: 0x0006C0CB File Offset: 0x0006A2CB
	private bool IsLocalVideo(string movieName)
	{
		return !movieName.Contains("://");
	}

	// Token: 0x06001421 RID: 5153 RVA: 0x0006C0DC File Offset: 0x0006A2DC
	private void UpdateShapeAndStereo()
	{
		if (this.AutoDetectStereoLayout && this.overlay.isExternalSurface)
		{
			int videoWidth = NativeVideoPlayer.VideoWidth;
			int videoHeight = NativeVideoPlayer.VideoHeight;
			switch (NativeVideoPlayer.VideoStereoMode)
			{
			case NativeVideoPlayer.StereoMode.Unknown:
				if (videoWidth > videoHeight)
				{
					this.Stereo = MoviePlayerSample.VideoStereo.LeftRight;
				}
				else
				{
					this.Stereo = MoviePlayerSample.VideoStereo.TopBottom;
				}
				break;
			case NativeVideoPlayer.StereoMode.Mono:
				this.Stereo = MoviePlayerSample.VideoStereo.Mono;
				break;
			case NativeVideoPlayer.StereoMode.TopBottom:
				this.Stereo = MoviePlayerSample.VideoStereo.TopBottom;
				break;
			case NativeVideoPlayer.StereoMode.LeftRight:
				this.Stereo = MoviePlayerSample.VideoStereo.LeftRight;
				break;
			}
		}
		if (this.Shape != this._LastShape || this.Stereo != this._LastStereo || this.DisplayMono != this._LastDisplayMono)
		{
			Rect rect = new Rect(0f, 0f, 1f, 1f);
			switch (this.Shape)
			{
			case MoviePlayerSample.VideoShape._360:
				this.overlay.currentOverlayShape = OVROverlay.OverlayShape.Equirect;
				goto IL_118;
			case MoviePlayerSample.VideoShape._180:
				this.overlay.currentOverlayShape = OVROverlay.OverlayShape.Equirect;
				rect = new Rect(0.25f, 0f, 0.5f, 1f);
				goto IL_118;
			}
			this.overlay.currentOverlayShape = OVROverlay.OverlayShape.Quad;
			IL_118:
			this.overlay.overrideTextureRectMatrix = true;
			this.overlay.invertTextureRects = false;
			Rect rect2 = new Rect(0f, 0f, 1f, 1f);
			Rect rect3 = new Rect(0f, 0f, 1f, 1f);
			switch (this.Stereo)
			{
			case MoviePlayerSample.VideoStereo.TopBottom:
				rect2 = new Rect(0f, 0.5f, 1f, 0.5f);
				rect3 = new Rect(0f, 0f, 1f, 0.5f);
				break;
			case MoviePlayerSample.VideoStereo.LeftRight:
				rect2 = new Rect(0f, 0f, 0.5f, 1f);
				rect3 = new Rect(0.5f, 0f, 0.5f, 1f);
				break;
			case MoviePlayerSample.VideoStereo.BottomTop:
				rect2 = new Rect(0f, 0f, 1f, 0.5f);
				rect3 = new Rect(0f, 0.5f, 1f, 0.5f);
				break;
			}
			this.overlay.SetSrcDestRects(rect2, this.DisplayMono ? rect2 : rect3, rect, rect);
			this._LastDisplayMono = this.DisplayMono;
			this._LastStereo = this.Stereo;
			this._LastShape = this.Shape;
		}
	}

	// Token: 0x06001422 RID: 5154 RVA: 0x0006C358 File Offset: 0x0006A558
	private IEnumerator Start()
	{
		if (this.mediaRenderer.material == null)
		{
			Debug.LogError("No material for movie surface");
			yield break;
		}
		yield return new WaitForSeconds(1f);
		if (!string.IsNullOrEmpty(this.MovieName))
		{
			if (this.IsLocalVideo(this.MovieName))
			{
				this.Play(Application.streamingAssetsPath + "/" + this.MovieName, null);
			}
			else
			{
				this.Play(this.MovieName, this.DrmLicenseUrl);
			}
		}
		yield break;
	}

	// Token: 0x06001423 RID: 5155 RVA: 0x0006C368 File Offset: 0x0006A568
	public void Play(string moviePath, string drmLicencesUrl)
	{
		if (moviePath != string.Empty)
		{
			Debug.Log("Playing Video: " + moviePath);
			if (this.overlay.isExternalSurface)
			{
				OVROverlay.ExternalSurfaceObjectCreated externalSurfaceObjectCreated = delegate()
				{
					Debug.Log("Playing ExoPlayer with SurfaceObject");
					NativeVideoPlayer.PlayVideo(moviePath, drmLicencesUrl, this.overlay.externalSurfaceObject);
					NativeVideoPlayer.SetLooping(this.LoopVideo);
				};
				if (this.overlay.externalSurfaceObject == IntPtr.Zero)
				{
					this.overlay.externalSurfaceObjectCreated = externalSurfaceObjectCreated;
				}
				else
				{
					externalSurfaceObjectCreated();
				}
			}
			else
			{
				Debug.Log("Playing Unity VideoPlayer");
				this.videoPlayer.url = moviePath;
				this.videoPlayer.Prepare();
				this.videoPlayer.Play();
			}
			Debug.Log("MovieSample Start");
			this.IsPlaying = true;
			return;
		}
		Debug.LogError("No media file name provided");
	}

	// Token: 0x06001424 RID: 5156 RVA: 0x0006C44E File Offset: 0x0006A64E
	public void Play()
	{
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.Play();
		}
		else
		{
			this.videoPlayer.Play();
		}
		this.IsPlaying = true;
	}

	// Token: 0x06001425 RID: 5157 RVA: 0x0006C476 File Offset: 0x0006A676
	public void Pause()
	{
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.Pause();
		}
		else
		{
			this.videoPlayer.Pause();
		}
		this.IsPlaying = false;
	}

	// Token: 0x06001426 RID: 5158 RVA: 0x0006C4A0 File Offset: 0x0006A6A0
	public void SeekTo(long position)
	{
		long num = Math.Max(0L, Math.Min(this.Duration, position));
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.PlaybackPosition = num;
			return;
		}
		this.videoPlayer.time = (double)num / 1000.0;
	}

	// Token: 0x06001427 RID: 5159 RVA: 0x0006C4EC File Offset: 0x0006A6EC
	private void Update()
	{
		this.UpdateShapeAndStereo();
		if (!this.overlay.isExternalSurface)
		{
			Texture texture = (this.videoPlayer.texture != null) ? this.videoPlayer.texture : Texture2D.blackTexture;
			if (this.overlay.enabled)
			{
				if (this.overlay.textures[0] != texture)
				{
					this.overlay.enabled = false;
					this.overlay.textures[0] = texture;
					this.overlay.enabled = true;
				}
			}
			else
			{
				this.mediaRenderer.material.mainTexture = texture;
				this.mediaRenderer.material.SetVector("_SrcRectLeft", this.overlay.srcRectLeft.ToVector());
				this.mediaRenderer.material.SetVector("_SrcRectRight", this.overlay.srcRectRight.ToVector());
			}
			this.IsPlaying = this.videoPlayer.isPlaying;
			this.PlaybackPosition = (long)(this.videoPlayer.time * 1000.0);
			this.Duration = (long)(this.videoPlayer.length * 1000.0);
			return;
		}
		NativeVideoPlayer.SetListenerRotation(Camera.main.transform.rotation);
		this.IsPlaying = NativeVideoPlayer.IsPlaying;
		this.PlaybackPosition = NativeVideoPlayer.PlaybackPosition;
		this.Duration = NativeVideoPlayer.Duration;
		if (this.IsPlaying && (int)OVRManager.display.displayFrequency != 60)
		{
			OVRManager.display.displayFrequency = 60f;
			return;
		}
		if (!this.IsPlaying && (int)OVRManager.display.displayFrequency != 72)
		{
			OVRManager.display.displayFrequency = 72f;
		}
	}

	// Token: 0x06001428 RID: 5160 RVA: 0x0006C6A9 File Offset: 0x0006A8A9
	public void SetPlaybackSpeed(float speed)
	{
		speed = Mathf.Max(0f, speed);
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.SetPlaybackSpeed(speed);
			return;
		}
		this.videoPlayer.playbackSpeed = speed;
	}

	// Token: 0x06001429 RID: 5161 RVA: 0x0006C6D8 File Offset: 0x0006A8D8
	public void Stop()
	{
		if (this.overlay.isExternalSurface)
		{
			NativeVideoPlayer.Stop();
		}
		else
		{
			this.videoPlayer.Stop();
		}
		this.IsPlaying = false;
	}

	// Token: 0x0600142A RID: 5162 RVA: 0x0006C700 File Offset: 0x0006A900
	private void OnApplicationPause(bool appWasPaused)
	{
		Debug.Log("OnApplicationPause: " + appWasPaused.ToString());
		if (appWasPaused)
		{
			this.videoPausedBeforeAppPause = !this.IsPlaying;
		}
		if (!this.videoPausedBeforeAppPause)
		{
			if (appWasPaused)
			{
				this.Pause();
				return;
			}
			this.Play();
		}
	}

	// Token: 0x04001B95 RID: 7061
	private bool videoPausedBeforeAppPause;

	// Token: 0x04001B96 RID: 7062
	private VideoPlayer videoPlayer;

	// Token: 0x04001B97 RID: 7063
	private OVROverlay overlay;

	// Token: 0x04001B98 RID: 7064
	private Renderer mediaRenderer;

	// Token: 0x04001B9C RID: 7068
	private RenderTexture copyTexture;

	// Token: 0x04001B9D RID: 7069
	private Material externalTex2DMaterial;

	// Token: 0x04001B9E RID: 7070
	public string MovieName;

	// Token: 0x04001B9F RID: 7071
	public string DrmLicenseUrl;

	// Token: 0x04001BA0 RID: 7072
	public bool LoopVideo;

	// Token: 0x04001BA1 RID: 7073
	public MoviePlayerSample.VideoShape Shape;

	// Token: 0x04001BA2 RID: 7074
	public MoviePlayerSample.VideoStereo Stereo;

	// Token: 0x04001BA3 RID: 7075
	public bool AutoDetectStereoLayout;

	// Token: 0x04001BA4 RID: 7076
	public bool DisplayMono;

	// Token: 0x04001BA5 RID: 7077
	private MoviePlayerSample.VideoShape _LastShape = (MoviePlayerSample.VideoShape)(-1);

	// Token: 0x04001BA6 RID: 7078
	private MoviePlayerSample.VideoStereo _LastStereo = (MoviePlayerSample.VideoStereo)(-1);

	// Token: 0x04001BA7 RID: 7079
	private bool _LastDisplayMono;

	// Token: 0x0200034F RID: 847
	public enum VideoShape
	{
		// Token: 0x04001BA9 RID: 7081
		_360,
		// Token: 0x04001BAA RID: 7082
		_180,
		// Token: 0x04001BAB RID: 7083
		Quad
	}

	// Token: 0x02000350 RID: 848
	public enum VideoStereo
	{
		// Token: 0x04001BAD RID: 7085
		Mono,
		// Token: 0x04001BAE RID: 7086
		TopBottom,
		// Token: 0x04001BAF RID: 7087
		LeftRight,
		// Token: 0x04001BB0 RID: 7088
		BottomTop
	}
}
