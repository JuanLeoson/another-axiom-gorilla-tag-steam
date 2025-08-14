using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

// Token: 0x02000639 RID: 1593
public class GREntityDebugCanvas : MonoBehaviour
{
	// Token: 0x0600274C RID: 10060 RVA: 0x000D3F4F File Offset: 0x000D214F
	private void Awake()
	{
		this.builder = new StringBuilder(50);
	}

	// Token: 0x0600274D RID: 10061 RVA: 0x000D3F5E File Offset: 0x000D215E
	private void Start()
	{
		if (this.text != null)
		{
			this.text.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600274E RID: 10062 RVA: 0x000D3F80 File Offset: 0x000D2180
	private bool UpdateActive()
	{
		bool entityDebugEnabled = GhostReactorManager.entityDebugEnabled;
		if (this.text != null)
		{
			this.text.gameObject.SetActive(entityDebugEnabled);
		}
		return entityDebugEnabled;
	}

	// Token: 0x0600274F RID: 10063 RVA: 0x000023F5 File Offset: 0x000005F5
	private void Update()
	{
	}

	// Token: 0x06002750 RID: 10064 RVA: 0x000D3FB4 File Offset: 0x000D21B4
	private void UpdateText()
	{
		if (this.text)
		{
			this.builder.Clear();
			List<IGameEntityDebugComponent> list = new List<IGameEntityDebugComponent>();
			base.GetComponents<IGameEntityDebugComponent>(list);
			foreach (IGameEntityDebugComponent gameEntityDebugComponent in list)
			{
				List<string> list2 = new List<string>();
				gameEntityDebugComponent.GetDebugTextLines(out list2);
				foreach (string value in list2)
				{
					this.builder.AppendLine(value);
				}
			}
			this.text.text = this.builder.ToString();
		}
	}

	// Token: 0x04003264 RID: 12900
	[SerializeField]
	public TMP_Text text;

	// Token: 0x04003265 RID: 12901
	private StringBuilder builder;
}
