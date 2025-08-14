using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000446 RID: 1094
public class TestManipulatableSpinnerIcons : MonoBehaviour
{
	// Token: 0x06001AD0 RID: 6864 RVA: 0x0008F06A File Offset: 0x0008D26A
	private void Awake()
	{
		this.GenerateRollers();
	}

	// Token: 0x06001AD1 RID: 6865 RVA: 0x0008F072 File Offset: 0x0008D272
	private void LateUpdate()
	{
		this.currentRotation = this.spinner.angle * this.rotationScale;
		this.UpdateSelectedIndex();
		this.UpdateRollers();
	}

	// Token: 0x06001AD2 RID: 6866 RVA: 0x0008F098 File Offset: 0x0008D298
	private void GenerateRollers()
	{
		for (int i = 0; i < this.rollerElementCount; i++)
		{
			float x = this.rollerElementAngle * (float)i + this.rollerElementAngle * 0.5f;
			Object.Instantiate<GameObject>(this.rollerElementTemplate, base.transform).transform.localRotation = Quaternion.Euler(x, 0f, 0f);
			GameObject gameObject = Object.Instantiate<GameObject>(this.iconElementTemplate, this.iconCanvas.transform);
			gameObject.transform.localRotation = Quaternion.Euler(x, 0f, 0f);
			this.visibleIcons.Add(gameObject.GetComponentInChildren<Text>());
		}
		this.rollerElementTemplate.SetActive(false);
		this.iconElementTemplate.SetActive(false);
		this.UpdateRollers();
	}

	// Token: 0x06001AD3 RID: 6867 RVA: 0x0008F160 File Offset: 0x0008D360
	private void UpdateSelectedIndex()
	{
		float num = this.currentRotation / this.rollerElementAngle;
		if (this.rollerElementCount % 2 == 1)
		{
			num += 0.5f;
		}
		this.selectedIndex = Mathf.FloorToInt(num);
		this.selectedIndex %= this.scrollableCount;
		if (this.selectedIndex < 0)
		{
			this.selectedIndex = this.scrollableCount + this.selectedIndex;
		}
	}

	// Token: 0x06001AD4 RID: 6868 RVA: 0x0008F1CC File Offset: 0x0008D3CC
	private void UpdateRollers()
	{
		float num = this.currentRotation;
		if (Mathf.Abs(num) > this.rollerElementAngle / 2f)
		{
			if (num > 0f)
			{
				num += this.rollerElementAngle / 2f;
				num %= this.rollerElementAngle;
				num -= this.rollerElementAngle / 2f;
			}
			else
			{
				num -= this.rollerElementAngle / 2f;
				num %= this.rollerElementAngle;
				num += this.rollerElementAngle / 2f;
			}
		}
		num -= (float)this.rollerElementCount / 2f * this.rollerElementAngle;
		base.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		this.iconCanvas.transform.localRotation = Quaternion.Euler(num, 0f, 0f);
		int num2 = this.rollerElementCount / 2;
		for (int i = 0; i < this.visibleIcons.Count; i++)
		{
			int num3 = this.selectedIndex - i + num2;
			if (num3 < 0)
			{
				num3 += this.scrollableCount;
			}
			else
			{
				num3 %= this.scrollableCount;
			}
			this.visibleIcons[i].text = string.Format("{0}", num3 + 1);
		}
	}

	// Token: 0x0400230F RID: 8975
	public ManipulatableSpinner spinner;

	// Token: 0x04002310 RID: 8976
	public float rotationScale = 1f;

	// Token: 0x04002311 RID: 8977
	public int rollerElementCount = 5;

	// Token: 0x04002312 RID: 8978
	public GameObject rollerElementTemplate;

	// Token: 0x04002313 RID: 8979
	public GameObject iconCanvas;

	// Token: 0x04002314 RID: 8980
	public GameObject iconElementTemplate;

	// Token: 0x04002315 RID: 8981
	public float iconOffset = 1f;

	// Token: 0x04002316 RID: 8982
	public float rollerElementAngle = 15f;

	// Token: 0x04002317 RID: 8983
	private List<Text> visibleIcons = new List<Text>();

	// Token: 0x04002318 RID: 8984
	private float currentRotation;

	// Token: 0x04002319 RID: 8985
	public int scrollableCount = 50;

	// Token: 0x0400231A RID: 8986
	public int selectedIndex;
}
