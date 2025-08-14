using System;
using UnityEngine;
using UnityEngine.Events;

namespace OculusSampleFramework
{
	// Token: 0x02000D3B RID: 3387
	public class WindmillController : MonoBehaviour
	{
		// Token: 0x060053DB RID: 21467 RVA: 0x0019E77B File Offset: 0x0019C97B
		private void Awake()
		{
			this._bladesRotation = base.GetComponentInChildren<WindmillBladesController>();
			this._bladesRotation.SetMoveState(true, this._maxSpeed);
		}

		// Token: 0x060053DC RID: 21468 RVA: 0x0019E79B File Offset: 0x0019C99B
		private void OnEnable()
		{
			this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.AddListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
		}

		// Token: 0x060053DD RID: 21469 RVA: 0x0019E7BE File Offset: 0x0019C9BE
		private void OnDisable()
		{
			if (this._startStopButton != null)
			{
				this._startStopButton.GetComponent<Interactable>().InteractableStateChanged.RemoveListener(new UnityAction<InteractableStateArgs>(this.StartStopStateChanged));
			}
		}

		// Token: 0x060053DE RID: 21470 RVA: 0x0019E7F0 File Offset: 0x0019C9F0
		private void StartStopStateChanged(InteractableStateArgs obj)
		{
			if (obj.NewInteractableState == InteractableState.ActionState)
			{
				if (this._bladesRotation.IsMoving)
				{
					this._bladesRotation.SetMoveState(false, 0f);
				}
				else
				{
					this._bladesRotation.SetMoveState(true, this._maxSpeed);
				}
			}
			this._toolInteractingWithMe = ((obj.NewInteractableState > InteractableState.Default) ? obj.Tool : null);
		}

		// Token: 0x060053DF RID: 21471 RVA: 0x0019E854 File Offset: 0x0019CA54
		private void Update()
		{
			if (this._toolInteractingWithMe == null)
			{
				this._selectionCylinder.CurrSelectionState = SelectionCylinder.SelectionState.Off;
				return;
			}
			this._selectionCylinder.CurrSelectionState = ((this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDown || this._toolInteractingWithMe.ToolInputState == ToolInputState.PrimaryInputDownStay) ? SelectionCylinder.SelectionState.Highlighted : SelectionCylinder.SelectionState.Selected);
		}

		// Token: 0x04005D42 RID: 23874
		[SerializeField]
		private GameObject _startStopButton;

		// Token: 0x04005D43 RID: 23875
		[SerializeField]
		private float _maxSpeed = 10f;

		// Token: 0x04005D44 RID: 23876
		[SerializeField]
		private SelectionCylinder _selectionCylinder;

		// Token: 0x04005D45 RID: 23877
		private WindmillBladesController _bladesRotation;

		// Token: 0x04005D46 RID: 23878
		private InteractableTool _toolInteractingWithMe;
	}
}
