using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020003B1 RID: 945
[RequireComponent(typeof(OVRSpatialAnchor))]
public class Anchor : MonoBehaviour
{
	// Token: 0x060015D6 RID: 5590 RVA: 0x000776E8 File Offset: 0x000758E8
	private void Awake()
	{
		this._anchorMenu.SetActive(false);
		this._renderers = base.GetComponentsInChildren<MeshRenderer>();
		this._canvas.worldCamera = Camera.main;
		this._selectedButton = this._buttonList[0];
		this._selectedButton.OnSelect(null);
		this._spatialAnchor = base.GetComponent<OVRSpatialAnchor>();
		this._icon = base.GetComponent<Transform>().FindChildRecursive("Sphere").gameObject;
	}

	// Token: 0x060015D7 RID: 5591 RVA: 0x00077764 File Offset: 0x00075964
	private static string ConvertUuidToString(Guid guid)
	{
		byte[] array = guid.ToByteArray();
		StringBuilder stringBuilder = new StringBuilder(array.Length * 2 + 4);
		for (int i = 0; i < array.Length; i++)
		{
			if (3 < i && i < 11 && i % 2 == 0)
			{
				stringBuilder.Append("-");
			}
			stringBuilder.AppendFormat("{0:x2}", array[i]);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060015D8 RID: 5592 RVA: 0x000777C8 File Offset: 0x000759C8
	private IEnumerator Start()
	{
		while (this._spatialAnchor && !this._spatialAnchor.Created)
		{
			yield return null;
		}
		if (this._spatialAnchor)
		{
			this._anchorName.text = Anchor.ConvertUuidToString(this._spatialAnchor.Uuid);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
		yield break;
	}

	// Token: 0x060015D9 RID: 5593 RVA: 0x000777D7 File Offset: 0x000759D7
	private void Update()
	{
		this.BillboardPanel(this._canvas.transform);
		this.BillboardPanel(this._pivot);
		this.HandleMenuNavigation();
		this.BillboardPanel(this._icon.transform);
	}

	// Token: 0x060015DA RID: 5594 RVA: 0x0007780D File Offset: 0x00075A0D
	public void OnSaveLocalButtonPressed()
	{
		if (!this._spatialAnchor)
		{
			return;
		}
		this._spatialAnchor.Save(delegate(OVRSpatialAnchor anchor, bool success)
		{
			if (!success)
			{
				return;
			}
			this.ShowSaveIcon = true;
			this.SaveUuidToPlayerPrefs(anchor.Uuid);
		});
	}

	// Token: 0x060015DB RID: 5595 RVA: 0x00077834 File Offset: 0x00075A34
	private void SaveUuidToPlayerPrefs(Guid uuid)
	{
		if (!PlayerPrefs.HasKey("numUuids"))
		{
			PlayerPrefs.SetInt("numUuids", 0);
		}
		int @int = PlayerPrefs.GetInt("numUuids");
		PlayerPrefs.SetString("uuid" + @int.ToString(), uuid.ToString());
		PlayerPrefs.SetInt("numUuids", @int + 1);
	}

	// Token: 0x060015DC RID: 5596 RVA: 0x0004061F File Offset: 0x0003E81F
	public void OnHideButtonPressed()
	{
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x00077895 File Offset: 0x00075A95
	public void OnEraseButtonPressed()
	{
		if (!this._spatialAnchor)
		{
			return;
		}
		this._spatialAnchor.Erase(delegate(OVRSpatialAnchor anchor, bool success)
		{
			if (success)
			{
				this._saveIcon.SetActive(false);
			}
		});
	}

	// Token: 0x1700026A RID: 618
	// (set) Token: 0x060015DE RID: 5598 RVA: 0x000778BC File Offset: 0x00075ABC
	public bool ShowSaveIcon
	{
		set
		{
			this._saveIcon.SetActive(value);
		}
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x000778CC File Offset: 0x00075ACC
	public void OnHoverStart()
	{
		if (this._isHovered)
		{
			return;
		}
		this._isHovered = true;
		MeshRenderer[] renderers = this._renderers;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material.SetColor("_EmissionColor", Color.yellow);
		}
		this._labelImage.color = this._labelHighlightColor;
	}

	// Token: 0x060015E0 RID: 5600 RVA: 0x00077928 File Offset: 0x00075B28
	public void OnHoverEnd()
	{
		if (!this._isHovered)
		{
			return;
		}
		this._isHovered = false;
		MeshRenderer[] renderers = this._renderers;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].material.SetColor("_EmissionColor", Color.clear);
		}
		if (this._isSelected)
		{
			this._labelImage.color = this._labelSelectedColor;
			return;
		}
		this._labelImage.color = this._labelBaseColor;
	}

	// Token: 0x060015E1 RID: 5601 RVA: 0x0007799C File Offset: 0x00075B9C
	public void OnSelect()
	{
		if (this._isSelected)
		{
			this._anchorMenu.SetActive(false);
			this._isSelected = false;
			this._selectedButton = null;
			if (this._isHovered)
			{
				this._labelImage.color = this._labelHighlightColor;
				return;
			}
			this._labelImage.color = this._labelBaseColor;
			return;
		}
		else
		{
			this._anchorMenu.SetActive(true);
			this._isSelected = true;
			this._menuIndex = -1;
			this.NavigateToIndexInMenu(true);
			if (this._isHovered)
			{
				this._labelImage.color = this._labelHighlightColor;
				return;
			}
			this._labelImage.color = this._labelSelectedColor;
			return;
		}
	}

	// Token: 0x060015E2 RID: 5602 RVA: 0x00077A44 File Offset: 0x00075C44
	private void BillboardPanel(Transform panel)
	{
		panel.LookAt(new Vector3(panel.position.x * 2f - Camera.main.transform.position.x, panel.position.y * 2f - Camera.main.transform.position.y, panel.position.z * 2f - Camera.main.transform.position.z), Vector3.up);
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x00077AD4 File Offset: 0x00075CD4
	private void HandleMenuNavigation()
	{
		if (!this._isSelected)
		{
			return;
		}
		if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickUp, OVRInput.Controller.Active))
		{
			this.NavigateToIndexInMenu(false);
		}
		if (OVRInput.GetDown(OVRInput.RawButton.RThumbstickDown, OVRInput.Controller.Active))
		{
			this.NavigateToIndexInMenu(true);
		}
		if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.Active))
		{
			this._selectedButton.OnSubmit(null);
		}
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x00077B38 File Offset: 0x00075D38
	private void NavigateToIndexInMenu(bool moveNext)
	{
		if (moveNext)
		{
			this._menuIndex++;
			if (this._menuIndex > this._buttonList.Count - 1)
			{
				this._menuIndex = 0;
			}
		}
		else
		{
			this._menuIndex--;
			if (this._menuIndex < 0)
			{
				this._menuIndex = this._buttonList.Count - 1;
			}
		}
		if (this._selectedButton)
		{
			this._selectedButton.OnDeselect(null);
		}
		this._selectedButton = this._buttonList[this._menuIndex];
		this._selectedButton.OnSelect(null);
	}

	// Token: 0x04001D9B RID: 7579
	public const string NumUuidsPlayerPref = "numUuids";

	// Token: 0x04001D9C RID: 7580
	[SerializeField]
	[FormerlySerializedAs("canvas_")]
	private Canvas _canvas;

	// Token: 0x04001D9D RID: 7581
	[SerializeField]
	[FormerlySerializedAs("pivot_")]
	private Transform _pivot;

	// Token: 0x04001D9E RID: 7582
	[SerializeField]
	[FormerlySerializedAs("anchorMenu_")]
	private GameObject _anchorMenu;

	// Token: 0x04001D9F RID: 7583
	private bool _isSelected;

	// Token: 0x04001DA0 RID: 7584
	private bool _isHovered;

	// Token: 0x04001DA1 RID: 7585
	[SerializeField]
	[FormerlySerializedAs("anchorName_")]
	private TextMeshProUGUI _anchorName;

	// Token: 0x04001DA2 RID: 7586
	[SerializeField]
	[FormerlySerializedAs("saveIcon_")]
	private GameObject _saveIcon;

	// Token: 0x04001DA3 RID: 7587
	[SerializeField]
	[FormerlySerializedAs("labelImage_")]
	private Image _labelImage;

	// Token: 0x04001DA4 RID: 7588
	[SerializeField]
	[FormerlySerializedAs("labelBaseColor_")]
	private Color _labelBaseColor;

	// Token: 0x04001DA5 RID: 7589
	[SerializeField]
	[FormerlySerializedAs("labelHighlightColor_")]
	private Color _labelHighlightColor;

	// Token: 0x04001DA6 RID: 7590
	[SerializeField]
	[FormerlySerializedAs("labelSelectedColor_")]
	private Color _labelSelectedColor;

	// Token: 0x04001DA7 RID: 7591
	[SerializeField]
	[FormerlySerializedAs("uiManager_")]
	private AnchorUIManager _uiManager;

	// Token: 0x04001DA8 RID: 7592
	[SerializeField]
	[FormerlySerializedAs("renderers_")]
	private MeshRenderer[] _renderers;

	// Token: 0x04001DA9 RID: 7593
	private int _menuIndex;

	// Token: 0x04001DAA RID: 7594
	[SerializeField]
	[FormerlySerializedAs("buttonList_")]
	private List<Button> _buttonList;

	// Token: 0x04001DAB RID: 7595
	private Button _selectedButton;

	// Token: 0x04001DAC RID: 7596
	private OVRSpatialAnchor _spatialAnchor;

	// Token: 0x04001DAD RID: 7597
	private GameObject _icon;
}
