using System;
using UnityEngine;

// Token: 0x02000020 RID: 32
public class MouseRotationDrag : MonoBehaviour
{
	// Token: 0x06000077 RID: 119 RVA: 0x0000467C File Offset: 0x0000287C
	private void Start()
	{
		this.m_currFrameHasFocus = false;
		this.m_prevFrameHasFocus = false;
	}

	// Token: 0x06000078 RID: 120 RVA: 0x0000468C File Offset: 0x0000288C
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
		Vector3 vector = mousePosition - prevMousePosition;
		this.m_prevMousePosition = mousePosition;
		if (!prevFrameHasFocus)
		{
			this.m_euler = base.transform.rotation.eulerAngles;
			return;
		}
		if (Input.GetMouseButton(0))
		{
			this.m_euler.x = this.m_euler.x + vector.y;
			this.m_euler.y = this.m_euler.y + vector.x;
			base.transform.rotation = Quaternion.Euler(this.m_euler);
		}
	}

	// Token: 0x0400008A RID: 138
	private bool m_currFrameHasFocus;

	// Token: 0x0400008B RID: 139
	private bool m_prevFrameHasFocus;

	// Token: 0x0400008C RID: 140
	private Vector3 m_prevMousePosition;

	// Token: 0x0400008D RID: 141
	private Vector3 m_euler;
}
