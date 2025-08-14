using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x02000FFB RID: 4091
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x06006637 RID: 26167 RVA: 0x0020838F File Offset: 0x0020658F
		public bool ShowRange
		{
			get
			{
				return this.Min != this.Max;
			}
		}

		// Token: 0x06006638 RID: 26168 RVA: 0x002083A4 File Offset: 0x002065A4
		public ConditionalFieldAttribute(string propertyToCheck = null, object compareValue = null, object compareValue2 = null, object compareValue3 = null, object compareValue4 = null, object compareValue5 = null, object compareValue6 = null)
		{
			this.PropertyToCheck = propertyToCheck;
			this.CompareValue = compareValue;
			this.CompareValue2 = compareValue2;
			this.CompareValue3 = compareValue3;
			this.CompareValue4 = compareValue4;
			this.CompareValue5 = compareValue5;
			this.CompareValue6 = compareValue6;
			this.Label = "";
			this.Tooltip = "";
			this.Min = 0f;
			this.Max = 0f;
		}

		// Token: 0x04007112 RID: 28946
		public string PropertyToCheck;

		// Token: 0x04007113 RID: 28947
		public object CompareValue;

		// Token: 0x04007114 RID: 28948
		public object CompareValue2;

		// Token: 0x04007115 RID: 28949
		public object CompareValue3;

		// Token: 0x04007116 RID: 28950
		public object CompareValue4;

		// Token: 0x04007117 RID: 28951
		public object CompareValue5;

		// Token: 0x04007118 RID: 28952
		public object CompareValue6;

		// Token: 0x04007119 RID: 28953
		public string Label;

		// Token: 0x0400711A RID: 28954
		public string Tooltip;

		// Token: 0x0400711B RID: 28955
		public float Min;

		// Token: 0x0400711C RID: 28956
		public float Max;
	}
}
