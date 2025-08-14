using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class ButterflySwarmManager : MonoBehaviour
{
	// Token: 0x1700008D RID: 141
	// (get) Token: 0x060006BE RID: 1726 RVA: 0x00026FA7 File Offset: 0x000251A7
	// (set) Token: 0x060006BF RID: 1727 RVA: 0x00026FAF File Offset: 0x000251AF
	public float PerchedFlapSpeed { get; private set; }

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x060006C0 RID: 1728 RVA: 0x00026FB8 File Offset: 0x000251B8
	// (set) Token: 0x060006C1 RID: 1729 RVA: 0x00026FC0 File Offset: 0x000251C0
	public float PerchedFlapPhase { get; private set; }

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00026FC9 File Offset: 0x000251C9
	// (set) Token: 0x060006C3 RID: 1731 RVA: 0x00026FD1 File Offset: 0x000251D1
	public float BeeSpeed { get; private set; }

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x060006C4 RID: 1732 RVA: 0x00026FDA File Offset: 0x000251DA
	// (set) Token: 0x060006C5 RID: 1733 RVA: 0x00026FE2 File Offset: 0x000251E2
	public float BeeMaxTravelTime { get; private set; }

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x060006C6 RID: 1734 RVA: 0x00026FEB File Offset: 0x000251EB
	// (set) Token: 0x060006C7 RID: 1735 RVA: 0x00026FF3 File Offset: 0x000251F3
	public float BeeAcceleration { get; private set; }

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x060006C8 RID: 1736 RVA: 0x00026FFC File Offset: 0x000251FC
	// (set) Token: 0x060006C9 RID: 1737 RVA: 0x00027004 File Offset: 0x00025204
	public float BeeJitterStrength { get; private set; }

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x060006CA RID: 1738 RVA: 0x0002700D File Offset: 0x0002520D
	// (set) Token: 0x060006CB RID: 1739 RVA: 0x00027015 File Offset: 0x00025215
	public float BeeJitterDamping { get; private set; }

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x060006CC RID: 1740 RVA: 0x0002701E File Offset: 0x0002521E
	// (set) Token: 0x060006CD RID: 1741 RVA: 0x00027026 File Offset: 0x00025226
	public float BeeMaxJitterRadius { get; private set; }

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x060006CE RID: 1742 RVA: 0x0002702F File Offset: 0x0002522F
	// (set) Token: 0x060006CF RID: 1743 RVA: 0x00027037 File Offset: 0x00025237
	public float BeeNearDestinationRadius { get; private set; }

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x060006D0 RID: 1744 RVA: 0x00027040 File Offset: 0x00025240
	// (set) Token: 0x060006D1 RID: 1745 RVA: 0x00027048 File Offset: 0x00025248
	public float DestRotationAlignmentSpeed { get; private set; }

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x060006D2 RID: 1746 RVA: 0x00027051 File Offset: 0x00025251
	// (set) Token: 0x060006D3 RID: 1747 RVA: 0x00027059 File Offset: 0x00025259
	public Vector3 TravellingLocalRotationEuler { get; private set; }

	// Token: 0x17000098 RID: 152
	// (get) Token: 0x060006D4 RID: 1748 RVA: 0x00027062 File Offset: 0x00025262
	// (set) Token: 0x060006D5 RID: 1749 RVA: 0x0002706A File Offset: 0x0002526A
	public Quaternion TravellingLocalRotation { get; private set; }

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x060006D6 RID: 1750 RVA: 0x00027073 File Offset: 0x00025273
	// (set) Token: 0x060006D7 RID: 1751 RVA: 0x0002707B File Offset: 0x0002527B
	public float AvoidPointRadius { get; private set; }

	// Token: 0x1700009A RID: 154
	// (get) Token: 0x060006D8 RID: 1752 RVA: 0x00027084 File Offset: 0x00025284
	// (set) Token: 0x060006D9 RID: 1753 RVA: 0x0002708C File Offset: 0x0002528C
	public float BeeMinFlowerDuration { get; private set; }

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x060006DA RID: 1754 RVA: 0x00027095 File Offset: 0x00025295
	// (set) Token: 0x060006DB RID: 1755 RVA: 0x0002709D File Offset: 0x0002529D
	public float BeeMaxFlowerDuration { get; private set; }

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x060006DC RID: 1756 RVA: 0x000270A6 File Offset: 0x000252A6
	// (set) Token: 0x060006DD RID: 1757 RVA: 0x000270AE File Offset: 0x000252AE
	public Color[] BeeColors { get; private set; }

	// Token: 0x060006DE RID: 1758 RVA: 0x000270B8 File Offset: 0x000252B8
	private void Awake()
	{
		this.TravellingLocalRotation = Quaternion.Euler(this.TravellingLocalRotationEuler);
		this.butterflies = new List<AnimatedButterfly>(this.numBees);
		for (int i = 0; i < this.numBees; i++)
		{
			AnimatedButterfly item = default(AnimatedButterfly);
			item.InitVisual(this.beePrefab, this);
			if (this.BeeColors.Length != 0)
			{
				item.SetColor(this.BeeColors[i % this.BeeColors.Length]);
			}
			this.butterflies.Add(item);
		}
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x00027140 File Offset: 0x00025340
	private void Start()
	{
		foreach (XSceneRef xsceneRef in this.perchSections)
		{
			GameObject gameObject;
			if (xsceneRef.TryResolve(out gameObject))
			{
				List<GameObject> list = new List<GameObject>();
				this.allPerchZones.Add(list);
				foreach (object obj in gameObject.transform)
				{
					Transform transform = (Transform)obj;
					list.Add(transform.gameObject);
				}
			}
		}
		this.OnSeedChange();
		RandomTimedSeedManager.instance.AddCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00027200 File Offset: 0x00025400
	private void OnDestroy()
	{
		RandomTimedSeedManager.instance.RemoveCallbackOnSeedChanged(new Action(this.OnSeedChange));
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x00027218 File Offset: 0x00025418
	private void Update()
	{
		for (int i = 0; i < this.butterflies.Count; i++)
		{
			AnimatedButterfly value = this.butterflies[i];
			value.UpdateVisual(RandomTimedSeedManager.instance.currentSyncTime, this);
			this.butterflies[i] = value;
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00027268 File Offset: 0x00025468
	private void OnSeedChange()
	{
		SRand srand = new SRand(RandomTimedSeedManager.instance.seed);
		List<List<GameObject>> list = new List<List<GameObject>>(this.allPerchZones.Count);
		for (int i = 0; i < this.allPerchZones.Count; i++)
		{
			List<GameObject> list2 = new List<GameObject>();
			list2.AddRange(this.allPerchZones[i]);
			list.Add(list2);
		}
		List<GameObject> list3 = new List<GameObject>(this.loopSizePerBee);
		List<float> list4 = new List<float>(this.loopSizePerBee);
		for (int j = 0; j < this.butterflies.Count; j++)
		{
			AnimatedButterfly value = this.butterflies[j];
			value.SetFlapSpeed(srand.NextFloat(this.minFlapSpeed, this.maxFlapSpeed));
			list3.Clear();
			list4.Clear();
			this.PickPoints(this.loopSizePerBee, list, ref srand, list3);
			for (int k = 0; k < list3.Count; k++)
			{
				list4.Add(srand.NextFloat(this.BeeMinFlowerDuration, this.BeeMaxFlowerDuration));
			}
			if (list3.Count == 0)
			{
				this.butterflies.Clear();
				return;
			}
			value.InitRoute(list3, list4, this);
			this.butterflies[j] = value;
		}
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x000273AC File Offset: 0x000255AC
	private void PickPoints(int n, List<List<GameObject>> pickBuffer, ref SRand rand, List<GameObject> resultBuffer)
	{
		int exclude = rand.NextInt(0, pickBuffer.Count);
		int num = -1;
		int num2 = n - 2;
		while (resultBuffer.Count < n)
		{
			int num3;
			if (resultBuffer.Count < num2)
			{
				num3 = rand.NextIntWithExclusion(0, pickBuffer.Count, num);
			}
			else
			{
				num3 = rand.NextIntWithExclusion2(0, pickBuffer.Count, num, exclude);
			}
			int num4 = 10;
			while (num3 == num || pickBuffer[num3].Count == 0)
			{
				num3 = (num3 + 1) % pickBuffer.Count;
				num4--;
				if (num4 <= 0)
				{
					return;
				}
			}
			num = num3;
			List<GameObject> list = pickBuffer[num];
			while (list.Count == 0)
			{
				num = (num + 1) % pickBuffer.Count;
				list = pickBuffer[num];
			}
			resultBuffer.Add(list[list.Count - 1]);
			list.RemoveAt(list.Count - 1);
		}
	}

	// Token: 0x04000831 RID: 2097
	[SerializeField]
	private XSceneRef[] perchSections;

	// Token: 0x04000832 RID: 2098
	[SerializeField]
	private int loopSizePerBee;

	// Token: 0x04000833 RID: 2099
	[SerializeField]
	private int numBees;

	// Token: 0x04000834 RID: 2100
	[SerializeField]
	private MeshRenderer beePrefab;

	// Token: 0x04000835 RID: 2101
	[SerializeField]
	private float maxFlapSpeed;

	// Token: 0x04000836 RID: 2102
	[SerializeField]
	private float minFlapSpeed;

	// Token: 0x04000847 RID: 2119
	private List<AnimatedButterfly> butterflies;

	// Token: 0x04000848 RID: 2120
	private List<List<GameObject>> allPerchZones = new List<List<GameObject>>();
}
