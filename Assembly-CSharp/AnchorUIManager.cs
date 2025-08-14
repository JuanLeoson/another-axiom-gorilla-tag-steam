using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x020003B3 RID: 947
[RequireComponent(typeof(SpatialAnchorLoader))]
public class AnchorUIManager : MonoBehaviour
{
	// Token: 0x1700026D RID: 621
	// (get) Token: 0x060015EE RID: 5614 RVA: 0x00077CB0 File Offset: 0x00075EB0
	public Anchor AnchorPrefab
	{
		get
		{
			return this._anchorPrefab;
		}
	}

	// Token: 0x060015EF RID: 5615 RVA: 0x00077CB8 File Offset: 0x00075EB8
	private void Awake()
	{
		if (AnchorUIManager.Instance == null)
		{
			AnchorUIManager.Instance = this;
			return;
		}
		Object.Destroy(this);
	}

	// Token: 0x060015F0 RID: 5616 RVA: 0x00077CD4 File Offset: 0x00075ED4
	private void Start()
	{
		this._raycastOrigin = this._trackedDevice;
		this._mode = AnchorUIManager.AnchorMode.Select;
		this.StartSelectMode();
		this._menuIndex = 0;
		this._selectedButton = this._buttonList[0];
		this._selectedButton.OnSelect(null);
		this._lineRenderer.startWidth = 0.005f;
		this._lineRenderer.endWidth = 0.005f;
	}

	// Token: 0x060015F1 RID: 5617 RVA: 0x00077D40 File Offset: 0x00075F40
	private void Update()
	{
		if (this._drawRaycast)
		{
			this.ControllerRaycast();
		}
		if (this._selectedAnchor == null)
		{
			this._selectedButton.OnSelect(null);
			this._isFocused = true;
		}
		this.HandleMenuNavigation();
		if (OVRInput.GetDown(OVRInput.RawButton.A, OVRInput.Controller.Active))
		{
			AnchorUIManager.PrimaryPressDelegate primaryPressDelegate = this._primaryPressDelegate;
			if (primaryPressDelegate == null)
			{
				return;
			}
			primaryPressDelegate();
		}
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x00077D9F File Offset: 0x00075F9F
	public void OnCreateModeButtonPressed()
	{
		this.ToggleCreateMode();
		this._createModeButton.SetActive(!this._createModeButton.activeSelf);
		this._selectModeButton.SetActive(!this._selectModeButton.activeSelf);
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x00077DD9 File Offset: 0x00075FD9
	public void OnLoadAnchorsButtonPressed()
	{
		base.GetComponent<SpatialAnchorLoader>().LoadAnchorsByUuid();
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x00077DE6 File Offset: 0x00075FE6
	private void ToggleCreateMode()
	{
		if (this._mode == AnchorUIManager.AnchorMode.Select)
		{
			this._mode = AnchorUIManager.AnchorMode.Create;
			this.EndSelectMode();
			this.StartPlacementMode();
			return;
		}
		this._mode = AnchorUIManager.AnchorMode.Select;
		this.EndPlacementMode();
		this.StartSelectMode();
	}

	// Token: 0x060015F5 RID: 5621 RVA: 0x00077E18 File Offset: 0x00076018
	private void StartPlacementMode()
	{
		this.ShowAnchorPreview();
		this._primaryPressDelegate = new AnchorUIManager.PrimaryPressDelegate(this.PlaceAnchor);
	}

	// Token: 0x060015F6 RID: 5622 RVA: 0x00077E32 File Offset: 0x00076032
	private void EndPlacementMode()
	{
		this.HideAnchorPreview();
		this._primaryPressDelegate = null;
	}

	// Token: 0x060015F7 RID: 5623 RVA: 0x00077E41 File Offset: 0x00076041
	private void StartSelectMode()
	{
		this.ShowRaycastLine();
		this._primaryPressDelegate = new AnchorUIManager.PrimaryPressDelegate(this.SelectAnchor);
	}

	// Token: 0x060015F8 RID: 5624 RVA: 0x00077E5B File Offset: 0x0007605B
	private void EndSelectMode()
	{
		this.HideRaycastLine();
		this._primaryPressDelegate = null;
	}

	// Token: 0x060015F9 RID: 5625 RVA: 0x00077E6C File Offset: 0x0007606C
	private void HandleMenuNavigation()
	{
		if (!this._isFocused)
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

	// Token: 0x060015FA RID: 5626 RVA: 0x00077ED0 File Offset: 0x000760D0
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
		this._selectedButton.OnDeselect(null);
		this._selectedButton = this._buttonList[this._menuIndex];
		this._selectedButton.OnSelect(null);
	}

	// Token: 0x060015FB RID: 5627 RVA: 0x00077F65 File Offset: 0x00076165
	private void ShowAnchorPreview()
	{
		this._placementPreview.SetActive(true);
	}

	// Token: 0x060015FC RID: 5628 RVA: 0x00077F73 File Offset: 0x00076173
	private void HideAnchorPreview()
	{
		this._placementPreview.SetActive(false);
	}

	// Token: 0x060015FD RID: 5629 RVA: 0x00077F81 File Offset: 0x00076181
	private void PlaceAnchor()
	{
		Object.Instantiate<Anchor>(this._anchorPrefab, this._anchorPlacementTransform.position, this._anchorPlacementTransform.rotation);
	}

	// Token: 0x060015FE RID: 5630 RVA: 0x00077FA5 File Offset: 0x000761A5
	private void ShowRaycastLine()
	{
		this._drawRaycast = true;
		this._lineRenderer.gameObject.SetActive(true);
	}

	// Token: 0x060015FF RID: 5631 RVA: 0x00077FBF File Offset: 0x000761BF
	private void HideRaycastLine()
	{
		this._drawRaycast = false;
		this._lineRenderer.gameObject.SetActive(false);
	}

	// Token: 0x06001600 RID: 5632 RVA: 0x00077FDC File Offset: 0x000761DC
	private void ControllerRaycast()
	{
		Ray ray = new Ray(this._raycastOrigin.position, this._raycastOrigin.TransformDirection(Vector3.forward));
		this._lineRenderer.SetPosition(0, this._raycastOrigin.position);
		this._lineRenderer.SetPosition(1, this._raycastOrigin.position + this._raycastOrigin.TransformDirection(Vector3.forward) * 10f);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit, float.PositiveInfinity))
		{
			Anchor component = raycastHit.collider.GetComponent<Anchor>();
			if (component != null)
			{
				this._lineRenderer.SetPosition(1, raycastHit.point);
				this.HoverAnchor(component);
				return;
			}
		}
		this.UnhoverAnchor();
	}

	// Token: 0x06001601 RID: 5633 RVA: 0x0007809B File Offset: 0x0007629B
	private void HoverAnchor(Anchor anchor)
	{
		this._hoveredAnchor = anchor;
		this._hoveredAnchor.OnHoverStart();
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x000780AF File Offset: 0x000762AF
	private void UnhoverAnchor()
	{
		if (this._hoveredAnchor == null)
		{
			return;
		}
		this._hoveredAnchor.OnHoverEnd();
		this._hoveredAnchor = null;
	}

	// Token: 0x06001603 RID: 5635 RVA: 0x000780D4 File Offset: 0x000762D4
	private void SelectAnchor()
	{
		if (this._hoveredAnchor != null)
		{
			if (this._selectedAnchor != null)
			{
				this._selectedAnchor.OnSelect();
				this._selectedAnchor = null;
			}
			this._selectedAnchor = this._hoveredAnchor;
			this._selectedAnchor.OnSelect();
			this._selectedButton.OnDeselect(null);
			this._isFocused = false;
			return;
		}
		if (this._selectedAnchor != null)
		{
			this._selectedAnchor.OnSelect();
			this._selectedAnchor = null;
			this._selectedButton.OnSelect(null);
			this._isFocused = true;
		}
	}

	// Token: 0x04001DB1 RID: 7601
	public static AnchorUIManager Instance;

	// Token: 0x04001DB2 RID: 7602
	[SerializeField]
	[FormerlySerializedAs("createModeButton_")]
	private GameObject _createModeButton;

	// Token: 0x04001DB3 RID: 7603
	[SerializeField]
	[FormerlySerializedAs("selectModeButton_")]
	private GameObject _selectModeButton;

	// Token: 0x04001DB4 RID: 7604
	[SerializeField]
	[FormerlySerializedAs("trackedDevice_")]
	private Transform _trackedDevice;

	// Token: 0x04001DB5 RID: 7605
	private Transform _raycastOrigin;

	// Token: 0x04001DB6 RID: 7606
	private bool _drawRaycast;

	// Token: 0x04001DB7 RID: 7607
	[SerializeField]
	[FormerlySerializedAs("lineRenderer_")]
	private LineRenderer _lineRenderer;

	// Token: 0x04001DB8 RID: 7608
	private Anchor _hoveredAnchor;

	// Token: 0x04001DB9 RID: 7609
	private Anchor _selectedAnchor;

	// Token: 0x04001DBA RID: 7610
	private AnchorUIManager.AnchorMode _mode = AnchorUIManager.AnchorMode.Select;

	// Token: 0x04001DBB RID: 7611
	[SerializeField]
	[FormerlySerializedAs("buttonList_")]
	private List<Button> _buttonList;

	// Token: 0x04001DBC RID: 7612
	private int _menuIndex;

	// Token: 0x04001DBD RID: 7613
	private Button _selectedButton;

	// Token: 0x04001DBE RID: 7614
	[SerializeField]
	private Anchor _anchorPrefab;

	// Token: 0x04001DBF RID: 7615
	[SerializeField]
	[FormerlySerializedAs("placementPreview_")]
	private GameObject _placementPreview;

	// Token: 0x04001DC0 RID: 7616
	[SerializeField]
	[FormerlySerializedAs("anchorPlacementTransform_")]
	private Transform _anchorPlacementTransform;

	// Token: 0x04001DC1 RID: 7617
	private AnchorUIManager.PrimaryPressDelegate _primaryPressDelegate;

	// Token: 0x04001DC2 RID: 7618
	private bool _isFocused = true;

	// Token: 0x020003B4 RID: 948
	public enum AnchorMode
	{
		// Token: 0x04001DC4 RID: 7620
		Create,
		// Token: 0x04001DC5 RID: 7621
		Select
	}

	// Token: 0x020003B5 RID: 949
	// (Invoke) Token: 0x06001606 RID: 5638
	private delegate void PrimaryPressDelegate();
}
