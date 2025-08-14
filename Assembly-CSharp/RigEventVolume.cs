using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003FF RID: 1023
public class RigEventVolume : MonoBehaviour
{
	// Token: 0x060017F0 RID: 6128 RVA: 0x000801B0 File Offset: 0x0007E3B0
	private void OnTriggerEnter(Collider other)
	{
		VRRig rig;
		if (other.gameObject.TryGetComponent<VRRig>(out rig) && !this.gameObjects.Contains(other.gameObject))
		{
			this.gameObjects.Add(other.gameObject);
			this.countChanged(this.gameObjects.Count - 1, this.gameObjects.Count, rig);
		}
	}

	// Token: 0x060017F1 RID: 6129 RVA: 0x00080210 File Offset: 0x0007E410
	private void OnTriggerExit(Collider other)
	{
		VRRig rig;
		if (other.gameObject.TryGetComponent<VRRig>(out rig) && this.gameObjects.Contains(other.gameObject))
		{
			this.gameObjects.Remove(other.gameObject);
			this.countChanged(this.gameObjects.Count + 1, this.gameObjects.Count, rig);
		}
	}

	// Token: 0x060017F2 RID: 6130 RVA: 0x00080270 File Offset: 0x0007E470
	private void countChanged(int oldValue, int newValue, VRRig rig)
	{
		if (newValue > oldValue)
		{
			UnityEvent<VRRig> rigEnters = this.RigEnters;
			if (rigEnters != null)
			{
				rigEnters.Invoke(rig);
			}
			switch (newValue)
			{
			case 1:
			{
				UnityEvent upTo = this.UpTo1;
				if (upTo != null)
				{
					upTo.Invoke();
				}
				break;
			}
			case 2:
			{
				UnityEvent upTo2 = this.UpTo2;
				if (upTo2 != null)
				{
					upTo2.Invoke();
				}
				break;
			}
			case 3:
			{
				UnityEvent upTo3 = this.UpTo3;
				if (upTo3 != null)
				{
					upTo3.Invoke();
				}
				break;
			}
			case 4:
			{
				UnityEvent upTo4 = this.UpTo4;
				if (upTo4 != null)
				{
					upTo4.Invoke();
				}
				break;
			}
			case 5:
			{
				UnityEvent upTo5 = this.UpTo5;
				if (upTo5 != null)
				{
					upTo5.Invoke();
				}
				break;
			}
			case 6:
			{
				UnityEvent upTo6 = this.UpTo6;
				if (upTo6 != null)
				{
					upTo6.Invoke();
				}
				break;
			}
			case 7:
			{
				UnityEvent upTo7 = this.UpTo7;
				if (upTo7 != null)
				{
					upTo7.Invoke();
				}
				break;
			}
			case 8:
			{
				UnityEvent upTo8 = this.UpTo8;
				if (upTo8 != null)
				{
					upTo8.Invoke();
				}
				break;
			}
			case 9:
			{
				UnityEvent upTo9 = this.UpTo9;
				if (upTo9 != null)
				{
					upTo9.Invoke();
				}
				break;
			}
			case 10:
			{
				UnityEvent upTo10 = this.UpTo10;
				if (upTo10 != null)
				{
					upTo10.Invoke();
				}
				break;
			}
			}
		}
		if (newValue < oldValue)
		{
			UnityEvent<VRRig> rigExits = this.RigExits;
			if (rigExits != null)
			{
				rigExits.Invoke(rig);
			}
			switch (newValue)
			{
			case 0:
			{
				UnityEvent downTo = this.DownTo0;
				if (downTo == null)
				{
					return;
				}
				downTo.Invoke();
				return;
			}
			case 1:
			{
				UnityEvent downTo2 = this.DownTo1;
				if (downTo2 == null)
				{
					return;
				}
				downTo2.Invoke();
				return;
			}
			case 2:
			{
				UnityEvent downTo3 = this.DownTo2;
				if (downTo3 == null)
				{
					return;
				}
				downTo3.Invoke();
				return;
			}
			case 3:
			{
				UnityEvent downTo4 = this.DownTo3;
				if (downTo4 == null)
				{
					return;
				}
				downTo4.Invoke();
				return;
			}
			case 4:
			{
				UnityEvent downTo5 = this.DownTo4;
				if (downTo5 == null)
				{
					return;
				}
				downTo5.Invoke();
				return;
			}
			case 5:
			{
				UnityEvent downTo6 = this.DownTo5;
				if (downTo6 == null)
				{
					return;
				}
				downTo6.Invoke();
				return;
			}
			case 6:
			{
				UnityEvent downTo7 = this.DownTo6;
				if (downTo7 == null)
				{
					return;
				}
				downTo7.Invoke();
				return;
			}
			case 7:
			{
				UnityEvent downTo8 = this.DownTo7;
				if (downTo8 == null)
				{
					return;
				}
				downTo8.Invoke();
				return;
			}
			case 8:
			{
				UnityEvent downTo9 = this.DownTo8;
				if (downTo9 == null)
				{
					return;
				}
				downTo9.Invoke();
				return;
			}
			case 9:
			{
				UnityEvent downTo10 = this.DownTo9;
				if (downTo10 == null)
				{
					return;
				}
				downTo10.Invoke();
				break;
			}
			default:
				return;
			}
		}
	}

	// Token: 0x04001FAA RID: 8106
	private List<GameObject> gameObjects = new List<GameObject>();

	// Token: 0x04001FAB RID: 8107
	[SerializeField]
	private UnityEvent<VRRig> RigEnters;

	// Token: 0x04001FAC RID: 8108
	[SerializeField]
	private UnityEvent<VRRig> RigExits;

	// Token: 0x04001FAD RID: 8109
	[SerializeField]
	private UnityEvent UpTo1;

	// Token: 0x04001FAE RID: 8110
	[SerializeField]
	private UnityEvent DownTo0;

	// Token: 0x04001FAF RID: 8111
	[SerializeField]
	private UnityEvent UpTo2;

	// Token: 0x04001FB0 RID: 8112
	[SerializeField]
	private UnityEvent DownTo1;

	// Token: 0x04001FB1 RID: 8113
	[SerializeField]
	private UnityEvent UpTo3;

	// Token: 0x04001FB2 RID: 8114
	[SerializeField]
	private UnityEvent DownTo2;

	// Token: 0x04001FB3 RID: 8115
	[SerializeField]
	private UnityEvent UpTo4;

	// Token: 0x04001FB4 RID: 8116
	[SerializeField]
	private UnityEvent DownTo3;

	// Token: 0x04001FB5 RID: 8117
	[SerializeField]
	private UnityEvent UpTo5;

	// Token: 0x04001FB6 RID: 8118
	[SerializeField]
	private UnityEvent DownTo4;

	// Token: 0x04001FB7 RID: 8119
	[SerializeField]
	private UnityEvent UpTo6;

	// Token: 0x04001FB8 RID: 8120
	[SerializeField]
	private UnityEvent DownTo5;

	// Token: 0x04001FB9 RID: 8121
	[SerializeField]
	private UnityEvent UpTo7;

	// Token: 0x04001FBA RID: 8122
	[SerializeField]
	private UnityEvent DownTo6;

	// Token: 0x04001FBB RID: 8123
	[SerializeField]
	private UnityEvent UpTo8;

	// Token: 0x04001FBC RID: 8124
	[SerializeField]
	private UnityEvent DownTo7;

	// Token: 0x04001FBD RID: 8125
	[SerializeField]
	private UnityEvent UpTo9;

	// Token: 0x04001FBE RID: 8126
	[SerializeField]
	private UnityEvent DownTo8;

	// Token: 0x04001FBF RID: 8127
	[SerializeField]
	private UnityEvent UpTo10;

	// Token: 0x04001FC0 RID: 8128
	[SerializeField]
	private UnityEvent DownTo9;
}
