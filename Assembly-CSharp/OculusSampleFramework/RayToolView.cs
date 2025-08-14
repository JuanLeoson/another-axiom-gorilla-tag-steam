using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000D23 RID: 3363
	public class RayToolView : MonoBehaviour, InteractableToolView
	{
		// Token: 0x170007F2 RID: 2034
		// (get) Token: 0x06005334 RID: 21300 RVA: 0x0019C26F File Offset: 0x0019A46F
		// (set) Token: 0x06005335 RID: 21301 RVA: 0x0019C27C File Offset: 0x0019A47C
		public bool EnableState
		{
			get
			{
				return this._lineRenderer.enabled;
			}
			set
			{
				this._targetTransform.gameObject.SetActive(value);
				this._lineRenderer.enabled = value;
			}
		}

		// Token: 0x170007F3 RID: 2035
		// (get) Token: 0x06005336 RID: 21302 RVA: 0x0019C29B File Offset: 0x0019A49B
		// (set) Token: 0x06005337 RID: 21303 RVA: 0x0019C2A3 File Offset: 0x0019A4A3
		public bool ToolActivateState
		{
			get
			{
				return this._toolActivateState;
			}
			set
			{
				this._toolActivateState = value;
				this._lineRenderer.colorGradient = (this._toolActivateState ? this._highLightColorGradient : this._oldColorGradient);
			}
		}

		// Token: 0x06005338 RID: 21304 RVA: 0x0019C2D0 File Offset: 0x0019A4D0
		private void Awake()
		{
			this._lineRenderer.positionCount = 25;
			this._oldColorGradient = this._lineRenderer.colorGradient;
			this._highLightColorGradient = new Gradient();
			this._highLightColorGradient.SetKeys(new GradientColorKey[]
			{
				new GradientColorKey(new Color(0.9f, 0.9f, 0.9f), 0f),
				new GradientColorKey(new Color(0.9f, 0.9f, 0.9f), 1f)
			}, new GradientAlphaKey[]
			{
				new GradientAlphaKey(1f, 0f),
				new GradientAlphaKey(1f, 1f)
			});
		}

		// Token: 0x170007F4 RID: 2036
		// (get) Token: 0x06005339 RID: 21305 RVA: 0x0019C393 File Offset: 0x0019A593
		// (set) Token: 0x0600533A RID: 21306 RVA: 0x0019C39B File Offset: 0x0019A59B
		public InteractableTool InteractableTool { get; set; }

		// Token: 0x0600533B RID: 21307 RVA: 0x0019C3A4 File Offset: 0x0019A5A4
		public void SetFocusedInteractable(Interactable interactable)
		{
			if (interactable == null)
			{
				this._focusedTransform = null;
				return;
			}
			this._focusedTransform = interactable.transform;
		}

		// Token: 0x0600533C RID: 21308 RVA: 0x0019C3C4 File Offset: 0x0019A5C4
		private void Update()
		{
			Vector3 position = this.InteractableTool.ToolTransform.position;
			Vector3 forward = this.InteractableTool.ToolTransform.forward;
			Vector3 vector = (this._focusedTransform != null) ? this._focusedTransform.position : (position + forward * 3f);
			float magnitude = (vector - position).magnitude;
			Vector3 p = position;
			Vector3 p2 = position + forward * magnitude * 0.3333333f;
			Vector3 p3 = position + forward * magnitude * 0.6666667f;
			Vector3 p4 = vector;
			for (int i = 0; i < 25; i++)
			{
				this.linePositions[i] = RayToolView.GetPointOnBezierCurve(p, p2, p3, p4, (float)i / 25f);
			}
			this._lineRenderer.SetPositions(this.linePositions);
			this._targetTransform.position = vector;
		}

		// Token: 0x0600533D RID: 21309 RVA: 0x0019C4BC File Offset: 0x0019A6BC
		public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
		{
			t = Mathf.Clamp01(t);
			float num = 1f - t;
			float num2 = num * num;
			float num3 = t * t;
			return num * num2 * p0 + 3f * num2 * t * p1 + 3f * num * num3 * p2 + t * num3 * p3;
		}

		// Token: 0x04005C82 RID: 23682
		private const int NUM_RAY_LINE_POSITIONS = 25;

		// Token: 0x04005C83 RID: 23683
		private const float DEFAULT_RAY_CAST_DISTANCE = 3f;

		// Token: 0x04005C84 RID: 23684
		[SerializeField]
		private Transform _targetTransform;

		// Token: 0x04005C85 RID: 23685
		[SerializeField]
		private LineRenderer _lineRenderer;

		// Token: 0x04005C86 RID: 23686
		private bool _toolActivateState;

		// Token: 0x04005C87 RID: 23687
		private Transform _focusedTransform;

		// Token: 0x04005C88 RID: 23688
		private Vector3[] linePositions = new Vector3[25];

		// Token: 0x04005C89 RID: 23689
		private Gradient _oldColorGradient;

		// Token: 0x04005C8A RID: 23690
		private Gradient _highLightColorGradient;
	}
}
