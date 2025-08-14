using System;

namespace TagEffects
{
	// Token: 0x02000E01 RID: 3585
	[Serializable]
	public class TagEffectsCombo : IEquatable<TagEffectsCombo>
	{
		// Token: 0x060058CA RID: 22730 RVA: 0x001B9924 File Offset: 0x001B7B24
		bool IEquatable<TagEffectsCombo>.Equals(TagEffectsCombo other)
		{
			return (other.inputA == this.inputA && other.inputB == this.inputB) || (other.inputA == this.inputB && other.inputB == this.inputA);
		}

		// Token: 0x060058CB RID: 22731 RVA: 0x001B997F File Offset: 0x001B7B7F
		public override bool Equals(object obj)
		{
			return this.Equals((TagEffectsCombo)obj);
		}

		// Token: 0x060058CC RID: 22732 RVA: 0x001B998D File Offset: 0x001B7B8D
		public override int GetHashCode()
		{
			return this.inputA.GetHashCode() * this.inputB.GetHashCode();
		}

		// Token: 0x04006299 RID: 25241
		public TagEffectPack inputA;

		// Token: 0x0400629A RID: 25242
		public TagEffectPack inputB;
	}
}
