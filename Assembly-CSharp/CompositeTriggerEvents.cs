using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000AAA RID: 2730
public class CompositeTriggerEvents : MonoBehaviour
{
	// Token: 0x1700064B RID: 1611
	// (get) Token: 0x0600420B RID: 16907 RVA: 0x0014C9DA File Offset: 0x0014ABDA
	private Dictionary<Collider, int> CollderMasks
	{
		get
		{
			return this.overlapMask;
		}
	}

	// Token: 0x14000078 RID: 120
	// (add) Token: 0x0600420C RID: 16908 RVA: 0x0014C9E4 File Offset: 0x0014ABE4
	// (remove) Token: 0x0600420D RID: 16909 RVA: 0x0014CA1C File Offset: 0x0014AC1C
	public event CompositeTriggerEvents.TriggerEvent CompositeTriggerEnter;

	// Token: 0x14000079 RID: 121
	// (add) Token: 0x0600420E RID: 16910 RVA: 0x0014CA54 File Offset: 0x0014AC54
	// (remove) Token: 0x0600420F RID: 16911 RVA: 0x0014CA8C File Offset: 0x0014AC8C
	public event CompositeTriggerEvents.TriggerEvent CompositeTriggerExit;

	// Token: 0x06004210 RID: 16912 RVA: 0x0014CAC4 File Offset: 0x0014ACC4
	private void Awake()
	{
		if (this.individualTriggerColliders.Count > 32)
		{
			Debug.LogError("The max number of triggers was exceeded in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
		}
		for (int i = 0; i < this.individualTriggerColliders.Count; i++)
		{
			TriggerEventNotifier triggerEventNotifier = this.individualTriggerColliders[i].gameObject.AddComponent<TriggerEventNotifier>();
			triggerEventNotifier.maskIndex = i;
			triggerEventNotifier.TriggerEnterEvent += this.TriggerEnterReceiver;
			triggerEventNotifier.TriggerExitEvent += this.TriggerExitReceiver;
			this.triggerEventNotifiers.Add(triggerEventNotifier);
		}
	}

	// Token: 0x06004211 RID: 16913 RVA: 0x0014CB64 File Offset: 0x0014AD64
	public void AddCollider(Collider colliderToAdd)
	{
		if (this.individualTriggerColliders.Count >= 32)
		{
			Debug.LogError("The max number of triggers are already present in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
			return;
		}
		this.individualTriggerColliders.Add(colliderToAdd);
		TriggerEventNotifier triggerEventNotifier = colliderToAdd.gameObject.AddComponent<TriggerEventNotifier>();
		triggerEventNotifier.maskIndex = this.GetNextMaskIndex();
		triggerEventNotifier.TriggerEnterEvent += this.TriggerEnterReceiver;
		triggerEventNotifier.TriggerExitEvent += this.TriggerExitReceiver;
		this.triggerEventNotifiers.Add(triggerEventNotifier);
		this.triggerEventNotifiers.Sort((TriggerEventNotifier a, TriggerEventNotifier b) => a.maskIndex.CompareTo(b.maskIndex));
	}

	// Token: 0x06004212 RID: 16914 RVA: 0x0014CC20 File Offset: 0x0014AE20
	public void RemoveCollider(Collider colliderToRemove)
	{
		TriggerEventNotifier component = colliderToRemove.gameObject.GetComponent<TriggerEventNotifier>();
		if (component.IsNotNull())
		{
			foreach (KeyValuePair<Collider, int> keyValuePair in new Dictionary<Collider, int>(this.overlapMask))
			{
				this.TriggerExitReceiver(component, keyValuePair.Key);
			}
			component.maskIndex = -1;
			component.TriggerEnterEvent -= this.TriggerEnterReceiver;
			component.TriggerExitEvent -= this.TriggerExitReceiver;
			this.triggerEventNotifiers.Remove(component);
		}
		this.individualTriggerColliders.Remove(colliderToRemove);
	}

	// Token: 0x06004213 RID: 16915 RVA: 0x0014CCD8 File Offset: 0x0014AED8
	public void ResetColliders(bool sendExitEvent = true)
	{
		this.individualTriggerColliders.Clear();
		for (int i = this.triggerEventNotifiers.Count - 1; i >= 0; i--)
		{
			if (this.triggerEventNotifiers[i].IsNull())
			{
				this.triggerEventNotifiers.RemoveAt(i);
			}
			else
			{
				this.triggerEventNotifiers[i].maskIndex = -1;
				this.triggerEventNotifiers[i].TriggerEnterEvent -= this.TriggerEnterReceiver;
				this.triggerEventNotifiers[i].TriggerExitEvent -= this.TriggerExitReceiver;
				this.triggerEventNotifiers.RemoveAt(i);
			}
		}
		if (sendExitEvent)
		{
			foreach (KeyValuePair<Collider, int> keyValuePair in this.overlapMask)
			{
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit != null)
				{
					compositeTriggerExit(keyValuePair.Key);
				}
			}
		}
		this.overlapMask.Clear();
	}

	// Token: 0x06004214 RID: 16916 RVA: 0x0014CDEC File Offset: 0x0014AFEC
	public int GetNumColliders()
	{
		return this.individualTriggerColliders.Count;
	}

	// Token: 0x06004215 RID: 16917 RVA: 0x0014CDFC File Offset: 0x0014AFFC
	public int GetNextMaskIndex()
	{
		if (this.individualTriggerColliders.Count >= 32)
		{
			Debug.LogError("The max number of triggers are already present in this composite trigger event sender on GameObject: " + base.gameObject.name + ".");
			return -1;
		}
		int num = 0;
		int num2 = 0;
		while (num2 < this.triggerEventNotifiers.Count && this.triggerEventNotifiers[num2].maskIndex == num)
		{
			num++;
			num2++;
		}
		return num;
	}

	// Token: 0x06004216 RID: 16918 RVA: 0x0014CE6C File Offset: 0x0014B06C
	private void OnDestroy()
	{
		for (int i = 0; i < this.triggerEventNotifiers.Count; i++)
		{
			if (this.triggerEventNotifiers[i] != null)
			{
				this.triggerEventNotifiers[i].TriggerEnterEvent -= this.TriggerEnterReceiver;
				this.triggerEventNotifiers[i].TriggerExitEvent -= this.TriggerExitReceiver;
			}
		}
	}

	// Token: 0x06004217 RID: 16919 RVA: 0x0014CEE0 File Offset: 0x0014B0E0
	public void TriggerEnterReceiver(TriggerEventNotifier notifier, Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			num = this.SetMaskIndexTrue(num, notifier.maskIndex);
			this.overlapMask[other] = num;
			return;
		}
		int value = this.SetMaskIndexTrue(0, notifier.maskIndex);
		this.overlapMask.Add(other, value);
		CompositeTriggerEvents.TriggerEvent compositeTriggerEnter = this.CompositeTriggerEnter;
		if (compositeTriggerEnter == null)
		{
			return;
		}
		compositeTriggerEnter(other);
	}

	// Token: 0x06004218 RID: 16920 RVA: 0x0014CF48 File Offset: 0x0014B148
	public void TriggerExitReceiver(TriggerEventNotifier notifier, Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			num = this.SetMaskIndexFalse(num, notifier.maskIndex);
			if (num == 0)
			{
				this.overlapMask.Remove(other);
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit == null)
				{
					return;
				}
				compositeTriggerExit(other);
				return;
			}
			else
			{
				this.overlapMask[other] = num;
			}
		}
	}

	// Token: 0x06004219 RID: 16921 RVA: 0x0014CFA4 File Offset: 0x0014B1A4
	public void ResetColliderMask(Collider other)
	{
		int num;
		if (this.overlapMask.TryGetValue(other, out num))
		{
			if (num != 0)
			{
				CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
				if (compositeTriggerExit != null)
				{
					compositeTriggerExit(other);
				}
			}
			this.overlapMask.Remove(other);
		}
	}

	// Token: 0x0600421A RID: 16922 RVA: 0x0014CFE3 File Offset: 0x0014B1E3
	public void CompositeTriggerEnterReceiver(Collider other)
	{
		CompositeTriggerEvents.TriggerEvent compositeTriggerEnter = this.CompositeTriggerEnter;
		if (compositeTriggerEnter == null)
		{
			return;
		}
		compositeTriggerEnter(other);
	}

	// Token: 0x0600421B RID: 16923 RVA: 0x0014CFF6 File Offset: 0x0014B1F6
	public void CompositeTriggerExitReceiver(Collider other)
	{
		CompositeTriggerEvents.TriggerEvent compositeTriggerExit = this.CompositeTriggerExit;
		if (compositeTriggerExit == null)
		{
			return;
		}
		compositeTriggerExit(other);
	}

	// Token: 0x0600421C RID: 16924 RVA: 0x0014D009 File Offset: 0x0014B209
	private bool TestMaskIndex(int mask, int index)
	{
		return (mask & 1 << index) != 0;
	}

	// Token: 0x0600421D RID: 16925 RVA: 0x0014D016 File Offset: 0x0014B216
	private int SetMaskIndexTrue(int mask, int index)
	{
		return mask | 1 << index;
	}

	// Token: 0x0600421E RID: 16926 RVA: 0x0014D020 File Offset: 0x0014B220
	private int SetMaskIndexFalse(int mask, int index)
	{
		return mask & ~(1 << index);
	}

	// Token: 0x0600421F RID: 16927 RVA: 0x0014D02C File Offset: 0x0014B22C
	private string MaskToString(int mask)
	{
		string text = "";
		for (int i = 31; i >= 0; i--)
		{
			text += (this.TestMaskIndex(mask, i) ? "1" : "0");
		}
		return text;
	}

	// Token: 0x04004D61 RID: 19809
	[SerializeField]
	private List<Collider> individualTriggerColliders = new List<Collider>();

	// Token: 0x04004D62 RID: 19810
	private List<TriggerEventNotifier> triggerEventNotifiers = new List<TriggerEventNotifier>();

	// Token: 0x04004D63 RID: 19811
	private Dictionary<Collider, int> overlapMask = new Dictionary<Collider, int>();

	// Token: 0x02000AAB RID: 2731
	// (Invoke) Token: 0x06004222 RID: 16930
	public delegate void TriggerEvent(Collider collider);
}
