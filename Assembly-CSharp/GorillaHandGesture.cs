using System;
using UnityEngine;

// Token: 0x0200019E RID: 414
[CreateAssetMenu(fileName = "New Hand Gesture", menuName = "Gorilla/Hand Gesture")]
public class GorillaHandGesture : ScriptableObject
{
	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000A58 RID: 2648 RVA: 0x000387CC File Offset: 0x000369CC
	// (set) Token: 0x06000A59 RID: 2649 RVA: 0x000387DB File Offset: 0x000369DB
	public GestureHandNode hand
	{
		get
		{
			return (GestureHandNode)this.nodes[0];
		}
		set
		{
			this.nodes[0] = value;
		}
	}

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000A5A RID: 2650 RVA: 0x000387E6 File Offset: 0x000369E6
	// (set) Token: 0x06000A5B RID: 2651 RVA: 0x000387F0 File Offset: 0x000369F0
	public GestureNode palm
	{
		get
		{
			return this.nodes[1];
		}
		set
		{
			this.nodes[1] = value;
		}
	}

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x06000A5C RID: 2652 RVA: 0x000387FB File Offset: 0x000369FB
	// (set) Token: 0x06000A5D RID: 2653 RVA: 0x00038805 File Offset: 0x00036A05
	public GestureNode wrist
	{
		get
		{
			return this.nodes[2];
		}
		set
		{
			this.nodes[2] = value;
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x06000A5E RID: 2654 RVA: 0x00038810 File Offset: 0x00036A10
	// (set) Token: 0x06000A5F RID: 2655 RVA: 0x0003881A File Offset: 0x00036A1A
	public GestureNode digits
	{
		get
		{
			return this.nodes[3];
		}
		set
		{
			this.nodes[3] = value;
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000A60 RID: 2656 RVA: 0x00038825 File Offset: 0x00036A25
	// (set) Token: 0x06000A61 RID: 2657 RVA: 0x00038834 File Offset: 0x00036A34
	public GestureDigitNode thumb
	{
		get
		{
			return (GestureDigitNode)this.nodes[4];
		}
		set
		{
			this.nodes[4] = value;
		}
	}

	// Token: 0x170000FC RID: 252
	// (get) Token: 0x06000A62 RID: 2658 RVA: 0x0003883F File Offset: 0x00036A3F
	// (set) Token: 0x06000A63 RID: 2659 RVA: 0x0003884E File Offset: 0x00036A4E
	public GestureDigitNode index
	{
		get
		{
			return (GestureDigitNode)this.nodes[5];
		}
		set
		{
			this.nodes[5] = value;
		}
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x06000A64 RID: 2660 RVA: 0x00038859 File Offset: 0x00036A59
	// (set) Token: 0x06000A65 RID: 2661 RVA: 0x00038868 File Offset: 0x00036A68
	public GestureDigitNode middle
	{
		get
		{
			return (GestureDigitNode)this.nodes[6];
		}
		set
		{
			this.nodes[6] = value;
		}
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x00038873 File Offset: 0x00036A73
	private static GestureNode[] InitNodes()
	{
		return new GestureNode[]
		{
			new GestureHandNode(),
			new GestureNode(),
			new GestureNode(),
			new GestureNode(),
			new GestureDigitNode(),
			new GestureDigitNode(),
			new GestureDigitNode()
		};
	}

	// Token: 0x04000CB7 RID: 3255
	public bool track = true;

	// Token: 0x04000CB8 RID: 3256
	public GestureNode[] nodes = GorillaHandGesture.InitNodes();
}
