using System;
using GorillaExtensions;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace com.AnotherAxiom.Paddleball
{
	// Token: 0x02000DF3 RID: 3571
	public class Paddleball : ArcadeGame
	{
		// Token: 0x06005876 RID: 22646 RVA: 0x001B7B0E File Offset: 0x001B5D0E
		protected override void Awake()
		{
			base.Awake();
			this.yPosToByteFactor = 255f / (2f * this.tableSizeBall.y);
			this.byteToYPosFactor = 1f / this.yPosToByteFactor;
		}

		// Token: 0x06005877 RID: 22647 RVA: 0x001B7B48 File Offset: 0x001B5D48
		private void Start()
		{
			this.whiteWinScreen.SetActive(false);
			this.blackWinScreen.SetActive(false);
			this.titleScreen.SetActive(true);
			this.ball.gameObject.SetActive(false);
			this.currentScreenMode = Paddleball.ScreenMode.Title;
			this.paddleIdle = new float[this.p.Length];
			for (int i = 0; i < this.p.Length; i++)
			{
				this.p[i].gameObject.SetActive(false);
				this.paddleIdle[i] = 30f;
			}
			this.gameBallSpeed = this.initialBallSpeed;
			this.scoreR = (this.scoreL = 0);
			this.scoreFormat = this.scoreDisplay.text;
			this.UpdateScore();
		}

		// Token: 0x06005878 RID: 22648 RVA: 0x001B7C0C File Offset: 0x001B5E0C
		private void Update()
		{
			if (this.currentScreenMode == Paddleball.ScreenMode.Gameplay)
			{
				this.ball.Translate(this.ballTrajectory.normalized * Time.deltaTime * this.gameBallSpeed);
				if (this.ball.localPosition.y > this.tableSizeBall.y)
				{
					this.ball.localPosition = new Vector3(this.ball.localPosition.x, this.tableSizeBall.y, this.ball.localPosition.z);
					this.ballTrajectory.y = -this.ballTrajectory.y;
					base.PlaySound(0, 3);
				}
				if (this.ball.localPosition.y < -this.tableSizeBall.y)
				{
					this.ball.localPosition = new Vector3(this.ball.localPosition.x, -this.tableSizeBall.y, this.ball.localPosition.z);
					this.ballTrajectory.y = -this.ballTrajectory.y;
					base.PlaySound(0, 3);
				}
				if (this.ball.localPosition.x > this.tableSizeBall.x)
				{
					this.ball.localPosition = new Vector3(this.tableSizeBall.x, this.ball.localPosition.y, this.ball.localPosition.z);
					this.ballTrajectory.x = -this.ballTrajectory.x;
					this.gameBallSpeed = this.initialBallSpeed;
					this.scoreL++;
					this.UpdateScore();
					base.PlaySound(2, 3);
					if (this.scoreL >= 10)
					{
						this.ChangeScreen(Paddleball.ScreenMode.WhiteWin);
					}
				}
				if (this.ball.localPosition.x < -this.tableSizeBall.x)
				{
					this.ball.localPosition = new Vector3(-this.tableSizeBall.x, this.ball.localPosition.y, this.ball.localPosition.z);
					this.ballTrajectory.x = -this.ballTrajectory.x;
					this.gameBallSpeed = this.initialBallSpeed;
					this.scoreR++;
					this.UpdateScore();
					base.PlaySound(2, 3);
					if (this.scoreR >= 10)
					{
						this.ChangeScreen(Paddleball.ScreenMode.BlackWin);
					}
				}
			}
			if (this.returnToTitleAfterTimestamp != 0f && Time.time > this.returnToTitleAfterTimestamp)
			{
				this.ChangeScreen(Paddleball.ScreenMode.Title);
			}
			for (int i = 0; i < this.p.Length; i++)
			{
				if (base.IsPlayerLocallyControlled(i))
				{
					float num = this.requestedPos[i];
					if (base.getButtonState(i, ArcadeButtons.UP))
					{
						this.requestedPos[i] += Time.deltaTime * this.paddleSpeed;
					}
					else if (base.getButtonState(i, ArcadeButtons.DOWN))
					{
						this.requestedPos[i] -= Time.deltaTime * this.paddleSpeed;
					}
					this.requestedPos[i] = Mathf.Clamp(this.requestedPos[i], -this.tableSizePaddle.y, this.tableSizePaddle.y);
				}
				float value;
				if (!NetworkSystem.Instance.InRoom || NetworkSystem.Instance.IsMasterClient)
				{
					value = Mathf.MoveTowards(this.p[i].transform.localPosition.y, this.requestedPos[i], Time.deltaTime * this.paddleSpeed);
				}
				else
				{
					value = Mathf.MoveTowards(this.p[i].transform.localPosition.y, this.officialPos[i], Time.deltaTime * this.paddleSpeed);
				}
				this.p[i].transform.localPosition = this.p[i].transform.localPosition.WithY(Mathf.Clamp(value, -this.tableSizePaddle.y, this.tableSizePaddle.y));
				if (base.getButtonState(i, ArcadeButtons.GRAB))
				{
					this.paddleIdle[i] = 0f;
					Paddleball.ScreenMode screenMode = this.currentScreenMode;
					if (screenMode != Paddleball.ScreenMode.Title)
					{
						if (screenMode == Paddleball.ScreenMode.Gameplay)
						{
							this.returnToTitleAfterTimestamp = Time.time + 30f;
						}
					}
					else
					{
						this.ChangeScreen(Paddleball.ScreenMode.Gameplay);
					}
				}
				else
				{
					this.paddleIdle[i] += Time.deltaTime;
				}
				bool flag = this.paddleIdle[i] < 30f;
				if (this.p[i].gameObject.activeSelf != flag)
				{
					if (flag)
					{
						base.PlaySound(4, 3);
						Vector3 localPosition = this.p[i].transform.localPosition;
						localPosition.y = 0f;
						this.requestedPos[i] = localPosition.y;
						this.p[i].transform.localPosition = localPosition;
					}
					this.p[i].gameObject.SetActive(this.paddleIdle[i] < 30f);
				}
				if (this.p[i].gameObject.activeInHierarchy && Mathf.Abs(this.ball.localPosition.x - this.p[i].transform.localPosition.x) < 0.1f && Mathf.Abs(this.ball.localPosition.y - this.p[i].transform.localPosition.y) < 0.5f)
				{
					this.ballTrajectory.y = (this.ball.localPosition.y - this.p[i].transform.localPosition.y) * 1.25f;
					float x = this.ballTrajectory.x;
					if (this.p[i].Right)
					{
						this.ballTrajectory.x = Mathf.Abs(this.ballTrajectory.y) - 1f;
					}
					else
					{
						this.ballTrajectory.x = 1f - Mathf.Abs(this.ballTrajectory.y);
					}
					if (x > 0f != this.ballTrajectory.x > 0f)
					{
						base.PlaySound(1, 3);
					}
					this.ballTrajectory.Normalize();
					this.gameBallSpeed += this.ballSpeedBoost;
				}
			}
		}

		// Token: 0x06005879 RID: 22649 RVA: 0x001B8274 File Offset: 0x001B6474
		private void UpdateScore()
		{
			if (this.scoreFormat == null)
			{
				return;
			}
			this.scoreL = Mathf.Clamp(this.scoreL, 0, 10);
			this.scoreR = Mathf.Clamp(this.scoreR, 0, 10);
			this.scoreDisplay.text = string.Format(this.scoreFormat, this.scoreL, this.scoreR);
		}

		// Token: 0x0600587A RID: 22650 RVA: 0x001B82DE File Offset: 0x001B64DE
		private float ByteToYPos(byte Y)
		{
			return (float)Y / this.yPosToByteFactor - this.tableSizeBall.y;
		}

		// Token: 0x0600587B RID: 22651 RVA: 0x001B82F5 File Offset: 0x001B64F5
		private byte YPosToByte(float Y)
		{
			return (byte)Mathf.RoundToInt((Y + this.tableSizeBall.y) * this.yPosToByteFactor);
		}

		// Token: 0x0600587C RID: 22652 RVA: 0x001B8314 File Offset: 0x001B6514
		public override byte[] GetNetworkState()
		{
			this.netStateCur.P0LocY = this.YPosToByte(this.p[0].transform.localPosition.y);
			this.netStateCur.P1LocY = this.YPosToByte(this.p[1].transform.localPosition.y);
			this.netStateCur.P2LocY = this.YPosToByte(this.p[2].transform.localPosition.y);
			this.netStateCur.P3LocY = this.YPosToByte(this.p[3].transform.localPosition.y);
			this.netStateCur.BallLocX = this.ball.localPosition.x;
			this.netStateCur.BallLocY = this.YPosToByte(this.ball.localPosition.y);
			this.netStateCur.BallTrajectoryX = (byte)((this.ballTrajectory.x + 1f) * 127.5f);
			this.netStateCur.BallTrajectoryY = (byte)((this.ballTrajectory.y + 1f) * 127.5f);
			this.netStateCur.BallSpeed = this.gameBallSpeed;
			this.netStateCur.ScoreLeft = this.scoreL;
			this.netStateCur.ScoreRight = this.scoreR;
			this.netStateCur.ScreenMode = (int)this.currentScreenMode;
			if (!this.netStateCur.Equals(this.netStateLast))
			{
				this.netStateLast = this.netStateCur;
				base.SwapNetStateBuffersAndStreams();
				ArcadeGame.WrapNetState(this.netStateLast, this.netStateMemStream);
			}
			return this.netStateBuffer;
		}

		// Token: 0x0600587D RID: 22653 RVA: 0x001B84C8 File Offset: 0x001B66C8
		public override void SetNetworkState(byte[] b)
		{
			Paddleball.PaddleballNetState paddleballNetState = (Paddleball.PaddleballNetState)ArcadeGame.UnwrapNetState(b);
			this.officialPos[0] = this.ByteToYPos(paddleballNetState.P0LocY);
			this.officialPos[1] = this.ByteToYPos(paddleballNetState.P1LocY);
			this.officialPos[2] = this.ByteToYPos(paddleballNetState.P2LocY);
			this.officialPos[3] = this.ByteToYPos(paddleballNetState.P3LocY);
			Vector2 vector = new Vector2(paddleballNetState.BallLocX, this.ByteToYPos(paddleballNetState.BallLocY));
			Vector2 normalized = new Vector2((float)paddleballNetState.BallTrajectoryX * 0.007843138f - 1f, (float)paddleballNetState.BallTrajectoryY * 0.007843138f - 1f).normalized;
			Vector2 a = vector - normalized * Vector2.Dot(vector, normalized);
			Vector2 vector2 = this.ball.localPosition.xy();
			Vector2 b2 = vector2 - this.ballTrajectory * Vector2.Dot(vector2, this.ballTrajectory);
			if ((a - b2).IsLongerThan(0.1f))
			{
				this.ball.localPosition = vector;
				this.ballTrajectory = normalized.xy();
			}
			this.gameBallSpeed = paddleballNetState.BallSpeed;
			this.ChangeScreen((Paddleball.ScreenMode)paddleballNetState.ScreenMode);
			if (this.scoreL != paddleballNetState.ScoreLeft || this.scoreR != paddleballNetState.ScoreRight)
			{
				this.scoreL = paddleballNetState.ScoreLeft;
				this.scoreR = paddleballNetState.ScoreRight;
				this.UpdateScore();
			}
		}

		// Token: 0x0600587E RID: 22654 RVA: 0x000023F5 File Offset: 0x000005F5
		protected override void ButtonUp(int player, ArcadeButtons button)
		{
		}

		// Token: 0x0600587F RID: 22655 RVA: 0x000023F5 File Offset: 0x000005F5
		protected override void ButtonDown(int player, ArcadeButtons button)
		{
		}

		// Token: 0x06005880 RID: 22656 RVA: 0x001B8644 File Offset: 0x001B6844
		private void ChangeScreen(Paddleball.ScreenMode mode)
		{
			if (this.currentScreenMode == mode)
			{
				return;
			}
			switch (this.currentScreenMode)
			{
			case Paddleball.ScreenMode.Title:
				this.titleScreen.SetActive(false);
				break;
			case Paddleball.ScreenMode.Gameplay:
				this.ball.gameObject.SetActive(false);
				break;
			case Paddleball.ScreenMode.WhiteWin:
				this.whiteWinScreen.SetActive(false);
				break;
			case Paddleball.ScreenMode.BlackWin:
				this.blackWinScreen.SetActive(false);
				break;
			}
			this.currentScreenMode = mode;
			switch (mode)
			{
			case Paddleball.ScreenMode.Title:
				this.gameBallSpeed = this.initialBallSpeed;
				this.scoreL = 0;
				this.scoreR = 0;
				this.UpdateScore();
				this.returnToTitleAfterTimestamp = 0f;
				this.titleScreen.SetActive(true);
				return;
			case Paddleball.ScreenMode.Gameplay:
				this.ball.gameObject.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + 30f;
				return;
			case Paddleball.ScreenMode.WhiteWin:
				this.whiteWinScreen.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + this.winScreenDuration;
				base.PlaySound(3, 3);
				return;
			case Paddleball.ScreenMode.BlackWin:
				this.blackWinScreen.SetActive(true);
				this.returnToTitleAfterTimestamp = Time.time + this.winScreenDuration;
				base.PlaySound(3, 3);
				return;
			default:
				return;
			}
		}

		// Token: 0x06005881 RID: 22657 RVA: 0x001B877B File Offset: 0x001B697B
		public override void OnTimeout()
		{
			this.ChangeScreen(Paddleball.ScreenMode.Title);
		}

		// Token: 0x06005882 RID: 22658 RVA: 0x001B8784 File Offset: 0x001B6984
		public override void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
		{
			this.requestedPos[player] = this.ByteToYPos((byte)stream.ReceiveNext());
		}

		// Token: 0x06005883 RID: 22659 RVA: 0x001B879F File Offset: 0x001B699F
		public override void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
		{
			stream.SendNext(this.YPosToByte(this.requestedPos[player]));
		}

		// Token: 0x04006230 RID: 25136
		[SerializeField]
		private PaddleballPaddle[] p;

		// Token: 0x04006231 RID: 25137
		private float[] requestedPos = new float[4];

		// Token: 0x04006232 RID: 25138
		private float[] officialPos = new float[4];

		// Token: 0x04006233 RID: 25139
		[SerializeField]
		private Transform ball;

		// Token: 0x04006234 RID: 25140
		[SerializeField]
		private Vector2 ballTrajectory;

		// Token: 0x04006235 RID: 25141
		[SerializeField]
		private float paddleSpeed = 1f;

		// Token: 0x04006236 RID: 25142
		[SerializeField]
		private float initialBallSpeed = 1f;

		// Token: 0x04006237 RID: 25143
		[SerializeField]
		private float ballSpeedBoost = 0.02f;

		// Token: 0x04006238 RID: 25144
		private float gameBallSpeed = 1f;

		// Token: 0x04006239 RID: 25145
		[SerializeField]
		private Vector2 tableSizeBall;

		// Token: 0x0400623A RID: 25146
		[SerializeField]
		private Vector2 tableSizePaddle;

		// Token: 0x0400623B RID: 25147
		[SerializeField]
		private GameObject blackWinScreen;

		// Token: 0x0400623C RID: 25148
		[SerializeField]
		private GameObject whiteWinScreen;

		// Token: 0x0400623D RID: 25149
		[SerializeField]
		private GameObject titleScreen;

		// Token: 0x0400623E RID: 25150
		[SerializeField]
		private float winScreenDuration;

		// Token: 0x0400623F RID: 25151
		private float returnToTitleAfterTimestamp;

		// Token: 0x04006240 RID: 25152
		private int scoreL;

		// Token: 0x04006241 RID: 25153
		private int scoreR;

		// Token: 0x04006242 RID: 25154
		private string scoreFormat;

		// Token: 0x04006243 RID: 25155
		[SerializeField]
		private TMP_Text scoreDisplay;

		// Token: 0x04006244 RID: 25156
		private float[] paddleIdle;

		// Token: 0x04006245 RID: 25157
		private Paddleball.ScreenMode currentScreenMode;

		// Token: 0x04006246 RID: 25158
		private const int AUDIO_WALLBOUNCE = 0;

		// Token: 0x04006247 RID: 25159
		private const int AUDIO_PADDLEBOUNCE = 1;

		// Token: 0x04006248 RID: 25160
		private const int AUDIO_SCORE = 2;

		// Token: 0x04006249 RID: 25161
		private const int AUDIO_WIN = 3;

		// Token: 0x0400624A RID: 25162
		private const int AUDIO_PLAYERJOIN = 4;

		// Token: 0x0400624B RID: 25163
		private const int VAR_REQUESTEDPOS = 0;

		// Token: 0x0400624C RID: 25164
		private const int MAXSCORE = 10;

		// Token: 0x0400624D RID: 25165
		private float yPosToByteFactor;

		// Token: 0x0400624E RID: 25166
		private float byteToYPosFactor;

		// Token: 0x0400624F RID: 25167
		private const float directionToByteFactor = 127.5f;

		// Token: 0x04006250 RID: 25168
		private const float byteToDirectionFactor = 0.007843138f;

		// Token: 0x04006251 RID: 25169
		private Paddleball.PaddleballNetState netStateLast;

		// Token: 0x04006252 RID: 25170
		private Paddleball.PaddleballNetState netStateCur;

		// Token: 0x02000DF4 RID: 3572
		private enum ScreenMode
		{
			// Token: 0x04006254 RID: 25172
			Title,
			// Token: 0x04006255 RID: 25173
			Gameplay,
			// Token: 0x04006256 RID: 25174
			WhiteWin,
			// Token: 0x04006257 RID: 25175
			BlackWin
		}

		// Token: 0x02000DF5 RID: 3573
		[Serializable]
		private struct PaddleballNetState : IEquatable<Paddleball.PaddleballNetState>
		{
			// Token: 0x06005885 RID: 22661 RVA: 0x001B8814 File Offset: 0x001B6A14
			public bool Equals(Paddleball.PaddleballNetState other)
			{
				return this.P0LocY == other.P0LocY && this.P1LocY == other.P1LocY && this.P2LocY == other.P2LocY && this.P3LocY == other.P3LocY && this.BallLocX.Approx(other.BallLocX, 1E-06f) && this.BallLocY == other.BallLocY && this.BallTrajectoryX == other.BallTrajectoryX && this.BallTrajectoryY == other.BallTrajectoryY && this.BallSpeed.Approx(other.BallSpeed, 1E-06f) && this.ScoreLeft == other.ScoreLeft && this.ScoreRight == other.ScoreRight && this.ScreenMode == other.ScreenMode;
			}

			// Token: 0x04006258 RID: 25176
			public byte P0LocY;

			// Token: 0x04006259 RID: 25177
			public byte P1LocY;

			// Token: 0x0400625A RID: 25178
			public byte P2LocY;

			// Token: 0x0400625B RID: 25179
			public byte P3LocY;

			// Token: 0x0400625C RID: 25180
			public float BallLocX;

			// Token: 0x0400625D RID: 25181
			public byte BallLocY;

			// Token: 0x0400625E RID: 25182
			public byte BallTrajectoryX;

			// Token: 0x0400625F RID: 25183
			public byte BallTrajectoryY;

			// Token: 0x04006260 RID: 25184
			public float BallSpeed;

			// Token: 0x04006261 RID: 25185
			public int ScoreLeft;

			// Token: 0x04006262 RID: 25186
			public int ScoreRight;

			// Token: 0x04006263 RID: 25187
			public int ScreenMode;
		}
	}
}
