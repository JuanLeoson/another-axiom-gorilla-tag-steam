using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D29 RID: 3369
	public class SelectionCylinder : MonoBehaviour
	{
		// Token: 0x170007F9 RID: 2041
		// (get) Token: 0x06005360 RID: 21344 RVA: 0x0019C9E6 File Offset: 0x0019ABE6
		// (set) Token: 0x06005361 RID: 21345 RVA: 0x0019C9F0 File Offset: 0x0019ABF0
		public SelectionCylinder.SelectionState CurrSelectionState
		{
			get
			{
				return this._currSelectionState;
			}
			set
			{
				SelectionCylinder.SelectionState currSelectionState = this._currSelectionState;
				this._currSelectionState = value;
				if (currSelectionState != this._currSelectionState)
				{
					if (this._currSelectionState > SelectionCylinder.SelectionState.Off)
					{
						this._selectionMeshRenderer.enabled = true;
						this.AffectSelectionColor((this._currSelectionState == SelectionCylinder.SelectionState.Selected) ? this._defaultSelectionColors : this._highlightColors);
						return;
					}
					this._selectionMeshRenderer.enabled = false;
				}
			}
		}

		// Token: 0x06005362 RID: 21346 RVA: 0x0019CA54 File Offset: 0x0019AC54
		private void Awake()
		{
			this._selectionMaterials = this._selectionMeshRenderer.materials;
			int num = this._selectionMaterials.Length;
			this._defaultSelectionColors = new Color[num];
			this._highlightColors = new Color[num];
			for (int i = 0; i < num; i++)
			{
				this._defaultSelectionColors[i] = this._selectionMaterials[i].GetColor(SelectionCylinder._colorId);
				this._highlightColors[i] = new Color(1f, 1f, 1f, this._defaultSelectionColors[i].a);
			}
			this.CurrSelectionState = SelectionCylinder.SelectionState.Off;
		}

		// Token: 0x06005363 RID: 21347 RVA: 0x0019CAF8 File Offset: 0x0019ACF8
		private void OnDestroy()
		{
			if (this._selectionMaterials != null)
			{
				foreach (Material material in this._selectionMaterials)
				{
					if (material != null)
					{
						Object.Destroy(material);
					}
				}
			}
		}

		// Token: 0x06005364 RID: 21348 RVA: 0x0019CB38 File Offset: 0x0019AD38
		private void AffectSelectionColor(Color[] newColors)
		{
			int num = newColors.Length;
			for (int i = 0; i < num; i++)
			{
				this._selectionMaterials[i].SetColor(SelectionCylinder._colorId, newColors[i]);
			}
		}

		// Token: 0x04005CA3 RID: 23715
		[SerializeField]
		private MeshRenderer _selectionMeshRenderer;

		// Token: 0x04005CA4 RID: 23716
		private static int _colorId = Shader.PropertyToID("_Color");

		// Token: 0x04005CA5 RID: 23717
		private Material[] _selectionMaterials;

		// Token: 0x04005CA6 RID: 23718
		private Color[] _defaultSelectionColors;

		// Token: 0x04005CA7 RID: 23719
		private Color[] _highlightColors;

		// Token: 0x04005CA8 RID: 23720
		private SelectionCylinder.SelectionState _currSelectionState;

		// Token: 0x02000D2A RID: 3370
		public enum SelectionState
		{
			// Token: 0x04005CAA RID: 23722
			Off,
			// Token: 0x04005CAB RID: 23723
			Selected,
			// Token: 0x04005CAC RID: 23724
			Highlighted
		}
	}
}
