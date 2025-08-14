using System;
using UnityEngine;

// Token: 0x02000B14 RID: 2836
[Serializable]
public class SceneObject : IEquatable<SceneObject>
{
	// Token: 0x06004463 RID: 17507 RVA: 0x00155EA1 File Offset: 0x001540A1
	public Type GetObjectType()
	{
		if (string.IsNullOrWhiteSpace(this.typeString))
		{
			return null;
		}
		if (this.typeString.Contains("ProxyType"))
		{
			return ProxyType.Parse(this.typeString);
		}
		return Type.GetType(this.typeString);
	}

	// Token: 0x06004464 RID: 17508 RVA: 0x00155EDB File Offset: 0x001540DB
	public SceneObject(int classID, ulong fileID)
	{
		this.classID = classID;
		this.fileID = fileID;
		this.typeString = UnityYaml.ClassIDToType[classID].AssemblyQualifiedName;
	}

	// Token: 0x06004465 RID: 17509 RVA: 0x00155F07 File Offset: 0x00154107
	public bool Equals(SceneObject other)
	{
		return this.fileID == other.fileID && this.classID == other.classID;
	}

	// Token: 0x06004466 RID: 17510 RVA: 0x00155F28 File Offset: 0x00154128
	public override bool Equals(object obj)
	{
		SceneObject sceneObject = obj as SceneObject;
		return sceneObject != null && this.Equals(sceneObject);
	}

	// Token: 0x06004467 RID: 17511 RVA: 0x00155F48 File Offset: 0x00154148
	public override int GetHashCode()
	{
		int i = this.classID;
		int i2 = StaticHash.Compute((long)this.fileID);
		return StaticHash.Compute(i, i2);
	}

	// Token: 0x06004468 RID: 17512 RVA: 0x00155F6D File Offset: 0x0015416D
	public static bool operator ==(SceneObject x, SceneObject y)
	{
		return x.Equals(y);
	}

	// Token: 0x06004469 RID: 17513 RVA: 0x00155F76 File Offset: 0x00154176
	public static bool operator !=(SceneObject x, SceneObject y)
	{
		return !x.Equals(y);
	}

	// Token: 0x04004E79 RID: 20089
	public int classID;

	// Token: 0x04004E7A RID: 20090
	public ulong fileID;

	// Token: 0x04004E7B RID: 20091
	[SerializeField]
	public string typeString;

	// Token: 0x04004E7C RID: 20092
	public string json;
}
