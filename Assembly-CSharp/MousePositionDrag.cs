using System;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class MousePositionDrag : MonoBehaviour
{
	// Token: 0x06000074 RID: 116 RVA: 0x000045EA File Offset: 0x000027EA
	private void Start()
	{
		this.m_currFrameHasFocus = false;
		this.m_prevFrameHasFocus = false;
	}

	// Token: 0x06000075 RID: 117 RVA: 0x000045FC File Offset: 0x000027FC
	private void Update()
	{
		this.m_currFrameHasFocus = Application.isFocused;
		bool prevFrameHasFocus = this.m_prevFrameHasFocus;
		this.m_prevFrameHasFocus = this.m_currFrameHasFocus;
		if (!prevFrameHasFocus && !this.m_currFrameHasFocus)
		{
			return;
		}
		Vector3 mousePosition = Input.mousePosition;
		Vector3 prevMousePosition = this.m_prevMousePosition;
		Vector3 a = mousePosition - prevMousePosition;
		this.m_prevMousePosition = mousePosition;
		if (!prevFrameHasFocus)
		{
			return;
		}
		if (Input.GetMouseButton(0))
		{
			base.transform.position += 0.02f * a;
		}
	}

	// Token: 0x04000087 RID: 135
	private bool m_currFrameHasFocus;

	// Token: 0x04000088 RID: 136
	private bool m_prevFrameHasFocus;

	// Token: 0x04000089 RID: 137
	private Vector3 m_prevMousePosition;
}
