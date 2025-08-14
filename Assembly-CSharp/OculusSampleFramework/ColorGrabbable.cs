using System;
using UnityEngine;

namespace OculusSampleFramework
{
	// Token: 0x02000CFE RID: 3326
	public class ColorGrabbable : OVRGrabbable
	{
		// Token: 0x170007B4 RID: 1972
		// (get) Token: 0x0600524E RID: 21070 RVA: 0x00199096 File Offset: 0x00197296
		// (set) Token: 0x0600524F RID: 21071 RVA: 0x0019909E File Offset: 0x0019729E
		public bool Highlight
		{
			get
			{
				return this.m_highlight;
			}
			set
			{
				this.m_highlight = value;
				this.UpdateColor();
			}
		}

		// Token: 0x06005250 RID: 21072 RVA: 0x001990AD File Offset: 0x001972AD
		protected void UpdateColor()
		{
			if (base.isGrabbed)
			{
				this.SetColor(ColorGrabbable.COLOR_GRAB);
				return;
			}
			if (this.Highlight)
			{
				this.SetColor(ColorGrabbable.COLOR_HIGHLIGHT);
				return;
			}
			this.SetColor(this.m_color);
		}

		// Token: 0x06005251 RID: 21073 RVA: 0x001990E3 File Offset: 0x001972E3
		public override void GrabBegin(OVRGrabber hand, Collider grabPoint)
		{
			base.GrabBegin(hand, grabPoint);
			this.UpdateColor();
		}

		// Token: 0x06005252 RID: 21074 RVA: 0x001990F3 File Offset: 0x001972F3
		public override void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
		{
			base.GrabEnd(linearVelocity, angularVelocity);
			this.UpdateColor();
		}

		// Token: 0x06005253 RID: 21075 RVA: 0x00199104 File Offset: 0x00197304
		private void Awake()
		{
			if (this.m_grabPoints.Length == 0)
			{
				Collider component = base.GetComponent<Collider>();
				if (component == null)
				{
					throw new ArgumentException("Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
				}
				this.m_grabPoints = new Collider[]
				{
					component
				};
				this.m_meshRenderers = new MeshRenderer[1];
				this.m_meshRenderers[0] = base.GetComponent<MeshRenderer>();
			}
			else
			{
				this.m_meshRenderers = base.GetComponentsInChildren<MeshRenderer>();
			}
			this.m_color = new Color(Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f), Random.Range(0.1f, 0.95f), 1f);
			this.SetColor(this.m_color);
		}

		// Token: 0x06005254 RID: 21076 RVA: 0x001991B8 File Offset: 0x001973B8
		private void SetColor(Color color)
		{
			for (int i = 0; i < this.m_meshRenderers.Length; i++)
			{
				MeshRenderer meshRenderer = this.m_meshRenderers[i];
				for (int j = 0; j < meshRenderer.materials.Length; j++)
				{
					meshRenderer.materials[j].color = color;
				}
			}
		}

		// Token: 0x04005BBE RID: 23486
		public static readonly Color COLOR_GRAB = new Color(1f, 0.5f, 0f, 1f);

		// Token: 0x04005BBF RID: 23487
		public static readonly Color COLOR_HIGHLIGHT = new Color(1f, 0f, 1f, 1f);

		// Token: 0x04005BC0 RID: 23488
		private Color m_color = Color.black;

		// Token: 0x04005BC1 RID: 23489
		private MeshRenderer[] m_meshRenderers;

		// Token: 0x04005BC2 RID: 23490
		private bool m_highlight;
	}
}
