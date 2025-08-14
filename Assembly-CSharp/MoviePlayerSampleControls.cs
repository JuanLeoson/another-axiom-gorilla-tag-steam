using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000353 RID: 851
public class MoviePlayerSampleControls : MonoBehaviour
{
	// Token: 0x06001434 RID: 5172 RVA: 0x0006C870 File Offset: 0x0006AA70
	private void Start()
	{
		this.PlayPause.onButtonDown += this.OnPlayPauseClicked;
		this.FastForward.onButtonDown += this.OnFastForwardClicked;
		this.Rewind.onButtonDown += this.OnRewindClicked;
		this.ProgressBar.onValueChanged.AddListener(new UnityAction<float>(this.OnSeekBarMoved));
		this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
		this.FastForwardImage.buttonType = MediaPlayerImage.ButtonType.SkipForward;
		this.RewindImage.buttonType = MediaPlayerImage.ButtonType.SkipBack;
		this.SetVisible(false);
	}

	// Token: 0x06001435 RID: 5173 RVA: 0x0006C90C File Offset: 0x0006AB0C
	private void OnPlayPauseClicked()
	{
		switch (this._state)
		{
		case MoviePlayerSampleControls.PlaybackState.Playing:
			this.Player.Pause();
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Play;
			this.FastForwardImage.buttonType = MediaPlayerImage.ButtonType.SkipForward;
			this.RewindImage.buttonType = MediaPlayerImage.ButtonType.SkipBack;
			this._state = MoviePlayerSampleControls.PlaybackState.Paused;
			return;
		case MoviePlayerSampleControls.PlaybackState.Paused:
			this.Player.Play();
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			this.FastForwardImage.buttonType = MediaPlayerImage.ButtonType.FastForward;
			this.RewindImage.buttonType = MediaPlayerImage.ButtonType.Rewind;
			this._state = MoviePlayerSampleControls.PlaybackState.Playing;
			return;
		case MoviePlayerSampleControls.PlaybackState.Rewinding:
			this.Player.Play();
			this._state = MoviePlayerSampleControls.PlaybackState.Playing;
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			return;
		case MoviePlayerSampleControls.PlaybackState.FastForwarding:
			this.Player.SetPlaybackSpeed(1f);
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			this._state = MoviePlayerSampleControls.PlaybackState.Playing;
			return;
		default:
			return;
		}
	}

	// Token: 0x06001436 RID: 5174 RVA: 0x0006C9E8 File Offset: 0x0006ABE8
	private void OnFastForwardClicked()
	{
		switch (this._state)
		{
		case MoviePlayerSampleControls.PlaybackState.Playing:
			this.Player.SetPlaybackSpeed(2f);
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Play;
			this._state = MoviePlayerSampleControls.PlaybackState.FastForwarding;
			return;
		case MoviePlayerSampleControls.PlaybackState.Paused:
			this.Seek(this.Player.PlaybackPosition + 15000L);
			return;
		case MoviePlayerSampleControls.PlaybackState.Rewinding:
			this.Player.Play();
			this.Player.SetPlaybackSpeed(2f);
			this._state = MoviePlayerSampleControls.PlaybackState.FastForwarding;
			return;
		case MoviePlayerSampleControls.PlaybackState.FastForwarding:
			this.Player.SetPlaybackSpeed(1f);
			this._state = MoviePlayerSampleControls.PlaybackState.Playing;
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			return;
		default:
			return;
		}
	}

	// Token: 0x06001437 RID: 5175 RVA: 0x0006CA98 File Offset: 0x0006AC98
	private void OnRewindClicked()
	{
		switch (this._state)
		{
		case MoviePlayerSampleControls.PlaybackState.Playing:
		case MoviePlayerSampleControls.PlaybackState.FastForwarding:
			this.Player.SetPlaybackSpeed(1f);
			this.Player.Pause();
			this._rewindStartPosition = this.Player.PlaybackPosition;
			this._rewindStartTime = Time.time;
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Play;
			this._state = MoviePlayerSampleControls.PlaybackState.Rewinding;
			return;
		case MoviePlayerSampleControls.PlaybackState.Paused:
			this.Seek(this.Player.PlaybackPosition - 15000L);
			return;
		case MoviePlayerSampleControls.PlaybackState.Rewinding:
			this.Player.Play();
			this.PlayPauseImage.buttonType = MediaPlayerImage.ButtonType.Pause;
			this._state = MoviePlayerSampleControls.PlaybackState.Playing;
			return;
		default:
			return;
		}
	}

	// Token: 0x06001438 RID: 5176 RVA: 0x0006CB48 File Offset: 0x0006AD48
	private void OnSeekBarMoved(float value)
	{
		long num = (long)(value * (float)this.Player.Duration);
		if (Mathf.Abs((float)(num - this.Player.PlaybackPosition)) > 200f)
		{
			this.Seek(num);
		}
	}

	// Token: 0x06001439 RID: 5177 RVA: 0x0006CB86 File Offset: 0x0006AD86
	private void Seek(long pos)
	{
		this._didSeek = true;
		this._seekPreviousPosition = this.Player.PlaybackPosition;
		this.Player.SeekTo(pos);
	}

	// Token: 0x0600143A RID: 5178 RVA: 0x0006CBAC File Offset: 0x0006ADAC
	private void Update()
	{
		if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.Active) || OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.Active) || OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger, OVRInput.Controller.Active))
		{
			this._lastButtonTime = Time.time;
			if (!this._isVisible)
			{
				this.SetVisible(true);
			}
		}
		if (OVRInput.GetActiveController() == OVRInput.Controller.LTouch)
		{
			this.InputModule.rayTransform = this.LeftHand.transform;
			this.GazePointer.rayTransform = this.LeftHand.transform;
		}
		else
		{
			this.InputModule.rayTransform = this.RightHand.transform;
			this.GazePointer.rayTransform = this.RightHand.transform;
		}
		if (OVRInput.Get(OVRInput.Button.Back, OVRInput.Controller.Active) && this._isVisible)
		{
			this.SetVisible(false);
		}
		if (this._state == MoviePlayerSampleControls.PlaybackState.Rewinding)
		{
			this.ProgressBar.value = Mathf.Clamp01(((float)this._rewindStartPosition - 1000f * (Time.time - this._rewindStartTime)) / (float)this.Player.Duration);
		}
		if (this._isVisible && this._state == MoviePlayerSampleControls.PlaybackState.Playing && Time.time - this._lastButtonTime > this.TimeoutTime)
		{
			this.SetVisible(false);
		}
		if (this._isVisible && (!this._didSeek || Mathf.Abs((float)(this._seekPreviousPosition - this.Player.PlaybackPosition)) > 50f))
		{
			this._didSeek = false;
			if (this.Player.Duration > 0L)
			{
				this.ProgressBar.value = (float)((double)this.Player.PlaybackPosition / (double)this.Player.Duration);
				return;
			}
			this.ProgressBar.value = 0f;
		}
	}

	// Token: 0x0600143B RID: 5179 RVA: 0x0006CD6C File Offset: 0x0006AF6C
	private void SetVisible(bool visible)
	{
		this.Canvas.enabled = visible;
		this._isVisible = visible;
		this.Player.DisplayMono = visible;
		this.LeftHand.SetActive(visible);
		this.RightHand.SetActive(visible);
		Debug.Log("Controls Visible: " + visible.ToString());
	}

	// Token: 0x04001BB7 RID: 7095
	public MoviePlayerSample Player;

	// Token: 0x04001BB8 RID: 7096
	public OVRInputModule InputModule;

	// Token: 0x04001BB9 RID: 7097
	public OVRGazePointer GazePointer;

	// Token: 0x04001BBA RID: 7098
	public GameObject LeftHand;

	// Token: 0x04001BBB RID: 7099
	public GameObject RightHand;

	// Token: 0x04001BBC RID: 7100
	public Canvas Canvas;

	// Token: 0x04001BBD RID: 7101
	public ButtonDownListener PlayPause;

	// Token: 0x04001BBE RID: 7102
	public MediaPlayerImage PlayPauseImage;

	// Token: 0x04001BBF RID: 7103
	public Slider ProgressBar;

	// Token: 0x04001BC0 RID: 7104
	public ButtonDownListener FastForward;

	// Token: 0x04001BC1 RID: 7105
	public MediaPlayerImage FastForwardImage;

	// Token: 0x04001BC2 RID: 7106
	public ButtonDownListener Rewind;

	// Token: 0x04001BC3 RID: 7107
	public MediaPlayerImage RewindImage;

	// Token: 0x04001BC4 RID: 7108
	public float TimeoutTime = 10f;

	// Token: 0x04001BC5 RID: 7109
	private bool _isVisible;

	// Token: 0x04001BC6 RID: 7110
	private float _lastButtonTime;

	// Token: 0x04001BC7 RID: 7111
	private bool _didSeek;

	// Token: 0x04001BC8 RID: 7112
	private long _seekPreviousPosition;

	// Token: 0x04001BC9 RID: 7113
	private long _rewindStartPosition;

	// Token: 0x04001BCA RID: 7114
	private float _rewindStartTime;

	// Token: 0x04001BCB RID: 7115
	private MoviePlayerSampleControls.PlaybackState _state;

	// Token: 0x02000354 RID: 852
	private enum PlaybackState
	{
		// Token: 0x04001BCD RID: 7117
		Playing,
		// Token: 0x04001BCE RID: 7118
		Paused,
		// Token: 0x04001BCF RID: 7119
		Rewinding,
		// Token: 0x04001BD0 RID: 7120
		FastForwarding
	}
}
