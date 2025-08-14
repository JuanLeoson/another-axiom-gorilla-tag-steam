using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000098 RID: 152
public class DevWatch : MonoBehaviour
{
	// Token: 0x060003BD RID: 957 RVA: 0x00016EC0 File Offset: 0x000150C0
	private void Awake()
	{
		this.SearchButton.SearchEvent.AddListener(new UnityAction(this.SearchItems));
		this.TakeOwnershipButton.onClick.AddListener(new UnityAction(this.TakeOwneshipOfItem));
		this.DestroyObjectButton.onClick.AddListener(new UnityAction(this.TryDestroyItem));
	}

	// Token: 0x060003BE RID: 958 RVA: 0x00016F24 File Offset: 0x00015124
	public void SearchItems()
	{
		this.FoundNetworkObjects.Clear();
		RaycastHit[] array = Physics.SphereCastAll(new Ray(this.RayCastStartPos.position, this.RayCastDirection.position - this.RayCastStartPos.position), 0.3f, 100f);
		if (array.Length != 0)
		{
			foreach (RaycastHit raycastHit in array)
			{
				NetworkObject item;
				if (raycastHit.collider.gameObject.TryGetComponent<NetworkObject>(out item))
				{
					this.FoundNetworkObjects.Add(item);
				}
			}
		}
	}

	// Token: 0x060003BF RID: 959 RVA: 0x00016FB8 File Offset: 0x000151B8
	public void Cleanup()
	{
		this.FoundNetworkObjects.Clear();
		if (this.Items.Count > 0)
		{
			for (int i = this.Items.Count - 1; i >= 0; i--)
			{
				Object.Destroy(this.Items[i]);
			}
		}
		this.Items.Clear();
		this.Panel1.SetActive(true);
		this.Panel2.SetActive(false);
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x0001702A File Offset: 0x0001522A
	public void ItemSelected(DevWatchSelectableItem item)
	{
		this.Panel1.SetActive(false);
		this.Panel2.SetActive(true);
		this.SelectedItem = item;
		this.SelectedItemName.text = item.ItemName.text;
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x000023F5 File Offset: 0x000005F5
	public void TryDestroyItem()
	{
	}

	// Token: 0x060003C2 RID: 962 RVA: 0x000023F5 File Offset: 0x000005F5
	public void TakeOwneshipOfItem()
	{
	}

	// Token: 0x0400043F RID: 1087
	public DevWatchButton SearchButton;

	// Token: 0x04000440 RID: 1088
	public GameObject Panel1;

	// Token: 0x04000441 RID: 1089
	public GameObject Panel2;

	// Token: 0x04000442 RID: 1090
	public DevWatchSelectableItem SelectableItemPrefab;

	// Token: 0x04000443 RID: 1091
	public List<DevWatchSelectableItem> Items;

	// Token: 0x04000444 RID: 1092
	public Transform RayCastStartPos;

	// Token: 0x04000445 RID: 1093
	public Transform RayCastDirection;

	// Token: 0x04000446 RID: 1094
	public Transform ItemsFoundContainer;

	// Token: 0x04000447 RID: 1095
	public Button TakeOwnershipButton;

	// Token: 0x04000448 RID: 1096
	public Button DestroyObjectButton;

	// Token: 0x04000449 RID: 1097
	public List<NetworkObject> FoundNetworkObjects = new List<NetworkObject>();

	// Token: 0x0400044A RID: 1098
	public TextMeshProUGUI SelectedItemName;

	// Token: 0x0400044B RID: 1099
	public DevWatchSelectableItem SelectedItem;
}
