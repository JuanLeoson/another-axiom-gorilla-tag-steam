using System;
using UnityEngine;

// Token: 0x0200086A RID: 2154
[Serializable]
public struct FrameStamp
{
	// Token: 0x17000529 RID: 1321
	// (get) Token: 0x06003619 RID: 13849 RVA: 0x0011C0D1 File Offset: 0x0011A2D1
	public int framesElapsed
	{
		get
		{
			return Time.frameCount - this._lastFrame;
		}
	}

	// Token: 0x0600361A RID: 13850 RVA: 0x0011C0E0 File Offset: 0x0011A2E0
	public static FrameStamp Now()
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount
		};
	}

	// Token: 0x0600361B RID: 13851 RVA: 0x0011C102 File Offset: 0x0011A302
	public override string ToString()
	{
		return string.Format("{0} frames elapsed", this.framesElapsed);
	}

	// Token: 0x0600361C RID: 13852 RVA: 0x0011C119 File Offset: 0x0011A319
	public override int GetHashCode()
	{
		return StaticHash.Compute(this._lastFrame);
	}

	// Token: 0x0600361D RID: 13853 RVA: 0x0011C126 File Offset: 0x0011A326
	public static implicit operator int(FrameStamp fs)
	{
		return fs.framesElapsed;
	}

	// Token: 0x0600361E RID: 13854 RVA: 0x0011C130 File Offset: 0x0011A330
	public static implicit operator FrameStamp(int framesElapsed)
	{
		return new FrameStamp
		{
			_lastFrame = Time.frameCount - framesElapsed
		};
	}

	// Token: 0x040042F2 RID: 17138
	private int _lastFrame;
}
