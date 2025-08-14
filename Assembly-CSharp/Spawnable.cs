using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020003AD RID: 941
[Serializable]
public class Spawnable : ISerializationCallbackReceiver
{
	// Token: 0x060015CD RID: 5581 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnBeforeSerialize()
	{
	}

	// Token: 0x060015CE RID: 5582 RVA: 0x000772D4 File Offset: 0x000754D4
	public void OnAfterDeserialize()
	{
		if (this.ClassificationLabel != "")
		{
			this._editorClassificationIndex = Spawnable.<OnAfterDeserialize>g__IndexOf|4_0(this.ClassificationLabel, OVRSceneManager.Classification.List);
			if (this._editorClassificationIndex < 0)
			{
				Debug.LogError("[Spawnable] OnAfterDeserialize() " + this.ClassificationLabel + " not found. The Classification list in OVRSceneManager has likely changed");
				return;
			}
		}
		else
		{
			this._editorClassificationIndex = 0;
		}
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x00077348 File Offset: 0x00075548
	[CompilerGenerated]
	internal static int <OnAfterDeserialize>g__IndexOf|4_0(string label, IEnumerable<string> collection)
	{
		int num = 0;
		using (IEnumerator<string> enumerator = collection.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current == label)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	// Token: 0x04001D90 RID: 7568
	public SimpleResizable ResizablePrefab;

	// Token: 0x04001D91 RID: 7569
	public string ClassificationLabel = "";

	// Token: 0x04001D92 RID: 7570
	[SerializeField]
	private int _editorClassificationIndex;
}
