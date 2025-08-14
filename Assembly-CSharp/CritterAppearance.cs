using System;
using GorillaExtensions;
using Photon.Pun;

// Token: 0x02000074 RID: 116
public struct CritterAppearance
{
	// Token: 0x060002DA RID: 730 RVA: 0x000121CF File Offset: 0x000103CF
	public CritterAppearance(string hatName, float size = 1f)
	{
		this.hatName = hatName;
		this.size = size;
	}

	// Token: 0x060002DB RID: 731 RVA: 0x000121E0 File Offset: 0x000103E0
	public object[] WriteToRPCData()
	{
		object[] array = new object[]
		{
			this.hatName,
			this.size
		};
		if (this.hatName == null)
		{
			array[0] = string.Empty;
		}
		if (this.size != 0f)
		{
			array[1] = this.size;
		}
		return array;
	}

	// Token: 0x060002DC RID: 732 RVA: 0x00012237 File Offset: 0x00010437
	public static int DataLength()
	{
		return 2;
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0001223C File Offset: 0x0001043C
	public static bool ValidateData(object[] data)
	{
		float num;
		return data != null && data.Length == CritterAppearance.DataLength() && CrittersManager.ValidateDataType<float>(data[1], out num) && num >= 0f && !float.IsNaN(num) && !float.IsInfinity(num);
	}

	// Token: 0x060002DE RID: 734 RVA: 0x00012284 File Offset: 0x00010484
	public static CritterAppearance ReadFromRPCData(object[] data)
	{
		string text;
		if (!CrittersManager.ValidateDataType<string>(data[0], out text))
		{
			return new CritterAppearance(string.Empty, 1f);
		}
		float value;
		if (!CrittersManager.ValidateDataType<float>(data[1], out value))
		{
			return new CritterAppearance(string.Empty, 1f);
		}
		return new CritterAppearance((string)data[0], value.GetFinite());
	}

	// Token: 0x060002DF RID: 735 RVA: 0x000122DC File Offset: 0x000104DC
	public static CritterAppearance ReadFromPhotonStream(PhotonStream data)
	{
		string text = (string)data.ReceiveNext();
		float num = (float)data.ReceiveNext();
		return new CritterAppearance(text, num);
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x00012306 File Offset: 0x00010506
	public override string ToString()
	{
		return string.Format("Size: {0} Hat: {1}", this.size, this.hatName);
	}

	// Token: 0x04000383 RID: 899
	public float size;

	// Token: 0x04000384 RID: 900
	public string hatName;
}
