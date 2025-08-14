using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UnityChan
{
	// Token: 0x02000FBA RID: 4026
	public class IdleChanger : MonoBehaviour
	{
		// Token: 0x060064AE RID: 25774 RVA: 0x001FE731 File Offset: 0x001FC931
		private void Start()
		{
			this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
			this.previousState = this.currentState;
			base.StartCoroutine("RandomChange");
			this.kb = Keyboard.current;
		}

		// Token: 0x060064AF RID: 25775 RVA: 0x001FE768 File Offset: 0x001FC968
		private void Update()
		{
			if (this.kb.upArrowKey.wasPressedThisFrame || this.kb.spaceKey.wasPressedThisFrame)
			{
				this.UnityChanA.SetBool("Next", true);
				this.UnityChanB.SetBool("Next", true);
			}
			if (this.kb.downArrowKey.wasPressedThisFrame)
			{
				this.UnityChanA.SetBool("Back", true);
				this.UnityChanB.SetBool("Back", true);
			}
			if (this.UnityChanA.GetBool("Next"))
			{
				this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
				if (this.previousState.fullPathHash != this.currentState.fullPathHash)
				{
					this.UnityChanA.SetBool("Next", false);
					this.UnityChanB.SetBool("Next", false);
					this.previousState = this.currentState;
				}
			}
			if (this.UnityChanA.GetBool("Back"))
			{
				this.currentState = this.UnityChanA.GetCurrentAnimatorStateInfo(0);
				if (this.previousState.fullPathHash != this.currentState.fullPathHash)
				{
					this.UnityChanA.SetBool("Back", false);
					this.UnityChanB.SetBool("Back", false);
					this.previousState = this.currentState;
				}
			}
		}

		// Token: 0x060064B0 RID: 25776 RVA: 0x001FE8C4 File Offset: 0x001FCAC4
		private void OnGUI()
		{
			if (this.isGUI)
			{
				GUI.Box(new Rect((float)(Screen.width - 110), 10f, 100f, 90f), "Change Motion");
				if (GUI.Button(new Rect((float)(Screen.width - 100), 40f, 80f, 20f), "Next"))
				{
					this.UnityChanA.SetBool("Next", true);
					this.UnityChanB.SetBool("Next", true);
				}
				if (GUI.Button(new Rect((float)(Screen.width - 100), 70f, 80f, 20f), "Back"))
				{
					this.UnityChanA.SetBool("Back", true);
					this.UnityChanB.SetBool("Back", true);
				}
			}
		}

		// Token: 0x060064B1 RID: 25777 RVA: 0x001FE999 File Offset: 0x001FCB99
		private IEnumerator RandomChange()
		{
			for (;;)
			{
				if (this._random)
				{
					float num = Random.Range(0f, 1f);
					if (num < this._threshold)
					{
						this.UnityChanA.SetBool("Back", true);
						this.UnityChanB.SetBool("Back", true);
					}
					else if (num >= this._threshold)
					{
						this.UnityChanA.SetBool("Next", true);
						this.UnityChanB.SetBool("Next", true);
					}
				}
				yield return new WaitForSeconds(this._interval);
			}
			yield break;
		}

		// Token: 0x04006F48 RID: 28488
		private AnimatorStateInfo currentState;

		// Token: 0x04006F49 RID: 28489
		private AnimatorStateInfo previousState;

		// Token: 0x04006F4A RID: 28490
		public bool _random;

		// Token: 0x04006F4B RID: 28491
		public float _threshold = 0.5f;

		// Token: 0x04006F4C RID: 28492
		public float _interval = 10f;

		// Token: 0x04006F4D RID: 28493
		public bool isGUI = true;

		// Token: 0x04006F4E RID: 28494
		public Animator UnityChanA;

		// Token: 0x04006F4F RID: 28495
		public Animator UnityChanB;

		// Token: 0x04006F50 RID: 28496
		private Keyboard kb;
	}
}
