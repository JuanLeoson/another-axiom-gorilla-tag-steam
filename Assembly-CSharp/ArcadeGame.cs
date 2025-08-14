using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002A4 RID: 676
public abstract class ArcadeGame : MonoBehaviour
{
	// Token: 0x06000F94 RID: 3988 RVA: 0x0005BA37 File Offset: 0x00059C37
	protected virtual void Awake()
	{
		this.InitializeMemoryStreams();
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x0005BA3F File Offset: 0x00059C3F
	public void InitializeMemoryStreams()
	{
		if (!this.memoryStreamsInitialized)
		{
			this.netStateMemStream = new MemoryStream(this.netStateBuffer, true);
			this.netStateMemStreamAlt = new MemoryStream(this.netStateBufferAlt, true);
			this.memoryStreamsInitialized = true;
		}
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x0005BA74 File Offset: 0x00059C74
	public void SetMachine(ArcadeMachine machine)
	{
		this.machine = machine;
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0005BA7D File Offset: 0x00059C7D
	protected bool getButtonState(int player, ArcadeButtons button)
	{
		return this.playerInputs[player].HasFlag(button);
	}

	// Token: 0x06000F98 RID: 3992 RVA: 0x0005BA98 File Offset: 0x00059C98
	public void OnInputStateChange(int player, ArcadeButtons buttons)
	{
		for (int i = 1; i < 256; i += i)
		{
			ArcadeButtons arcadeButtons = (ArcadeButtons)i;
			bool flag = buttons.HasFlag(arcadeButtons);
			bool flag2 = this.playerInputs[player].HasFlag(arcadeButtons);
			if (flag != flag2)
			{
				if (flag)
				{
					this.ButtonDown(player, arcadeButtons);
				}
				else
				{
					this.ButtonUp(player, arcadeButtons);
				}
			}
		}
		this.playerInputs[player] = buttons;
	}

	// Token: 0x06000F99 RID: 3993
	public abstract byte[] GetNetworkState();

	// Token: 0x06000F9A RID: 3994
	public abstract void SetNetworkState(byte[] obj);

	// Token: 0x06000F9B RID: 3995 RVA: 0x0005BB04 File Offset: 0x00059D04
	protected static void WrapNetState(object ns, MemoryStream stream)
	{
		if (stream == null)
		{
			Debug.LogWarning("Null MemoryStream passed to WrapNetState");
			return;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		stream.SetLength(0L);
		stream.Position = 0L;
		binaryFormatter.Serialize(stream, ns);
	}

	// Token: 0x06000F9C RID: 3996 RVA: 0x0005BB30 File Offset: 0x00059D30
	protected static object UnwrapNetState(byte[] b)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(b);
		memoryStream.Position = 0L;
		object result = binaryFormatter.Deserialize(memoryStream);
		memoryStream.Close();
		return result;
	}

	// Token: 0x06000F9D RID: 3997 RVA: 0x0005BB68 File Offset: 0x00059D68
	protected void SwapNetStateBuffersAndStreams()
	{
		byte[] array = this.netStateBufferAlt;
		byte[] array2 = this.netStateBuffer;
		this.netStateBuffer = array;
		this.netStateBufferAlt = array2;
		MemoryStream memoryStream = this.netStateMemStreamAlt;
		MemoryStream memoryStream2 = this.netStateMemStream;
		this.netStateMemStream = memoryStream;
		this.netStateMemStreamAlt = memoryStream2;
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x0005BBAD File Offset: 0x00059DAD
	protected void PlaySound(int clipId, int prio = 3)
	{
		this.machine.PlaySound(clipId, prio);
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x0005BBBC File Offset: 0x00059DBC
	protected bool IsPlayerLocallyControlled(int player)
	{
		return this.machine.IsPlayerLocallyControlled(player);
	}

	// Token: 0x06000FA0 RID: 4000
	protected abstract void ButtonUp(int player, ArcadeButtons button);

	// Token: 0x06000FA1 RID: 4001
	protected abstract void ButtonDown(int player, ArcadeButtons button);

	// Token: 0x06000FA2 RID: 4002
	public abstract void OnTimeout();

	// Token: 0x06000FA3 RID: 4003 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void ReadPlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x000023F5 File Offset: 0x000005F5
	public virtual void WritePlayerDataPUN(int player, PhotonStream stream, PhotonMessageInfo info)
	{
	}

	// Token: 0x0400183B RID: 6203
	[SerializeField]
	public Vector2 Scale = new Vector2(1f, 1f);

	// Token: 0x0400183C RID: 6204
	private ArcadeButtons[] playerInputs = new ArcadeButtons[4];

	// Token: 0x0400183D RID: 6205
	public AudioClip[] audioClips;

	// Token: 0x0400183E RID: 6206
	private ArcadeMachine machine;

	// Token: 0x0400183F RID: 6207
	protected static int NetStateBufferSize = 512;

	// Token: 0x04001840 RID: 6208
	protected byte[] netStateBuffer = new byte[ArcadeGame.NetStateBufferSize];

	// Token: 0x04001841 RID: 6209
	protected byte[] netStateBufferAlt = new byte[ArcadeGame.NetStateBufferSize];

	// Token: 0x04001842 RID: 6210
	protected MemoryStream netStateMemStream;

	// Token: 0x04001843 RID: 6211
	protected MemoryStream netStateMemStreamAlt;

	// Token: 0x04001844 RID: 6212
	public bool memoryStreamsInitialized;
}
