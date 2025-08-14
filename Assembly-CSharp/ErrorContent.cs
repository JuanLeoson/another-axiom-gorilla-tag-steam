using System;

// Token: 0x020008B7 RID: 2231
[Serializable]
public class ErrorContent
{
	// Token: 0x17000561 RID: 1377
	// (get) Token: 0x060037CA RID: 14282 RVA: 0x001208E0 File Offset: 0x0011EAE0
	// (set) Token: 0x060037CB RID: 14283 RVA: 0x001208E8 File Offset: 0x0011EAE8
	public string Message { get; set; }

	// Token: 0x17000562 RID: 1378
	// (get) Token: 0x060037CC RID: 14284 RVA: 0x001208F1 File Offset: 0x0011EAF1
	// (set) Token: 0x060037CD RID: 14285 RVA: 0x001208F9 File Offset: 0x0011EAF9
	public string Error { get; set; }

	// Token: 0x060037CE RID: 14286 RVA: 0x00120902 File Offset: 0x0011EB02
	public override string ToString()
	{
		return "Error: " + this.Error + ", Message: " + this.Message;
	}
}
