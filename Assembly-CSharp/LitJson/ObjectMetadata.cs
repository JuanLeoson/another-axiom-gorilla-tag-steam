using System;
using System.Collections.Generic;

namespace LitJson
{
	// Token: 0x02000BB6 RID: 2998
	internal struct ObjectMetadata
	{
		// Token: 0x17000701 RID: 1793
		// (get) Token: 0x06004875 RID: 18549 RVA: 0x00161138 File Offset: 0x0015F338
		// (set) Token: 0x06004876 RID: 18550 RVA: 0x00161159 File Offset: 0x0015F359
		public Type ElementType
		{
			get
			{
				if (this.element_type == null)
				{
					return typeof(JsonData);
				}
				return this.element_type;
			}
			set
			{
				this.element_type = value;
			}
		}

		// Token: 0x17000702 RID: 1794
		// (get) Token: 0x06004877 RID: 18551 RVA: 0x00161162 File Offset: 0x0015F362
		// (set) Token: 0x06004878 RID: 18552 RVA: 0x0016116A File Offset: 0x0015F36A
		public bool IsDictionary
		{
			get
			{
				return this.is_dictionary;
			}
			set
			{
				this.is_dictionary = value;
			}
		}

		// Token: 0x17000703 RID: 1795
		// (get) Token: 0x06004879 RID: 18553 RVA: 0x00161173 File Offset: 0x0015F373
		// (set) Token: 0x0600487A RID: 18554 RVA: 0x0016117B File Offset: 0x0015F37B
		public IDictionary<string, PropertyMetadata> Properties
		{
			get
			{
				return this.properties;
			}
			set
			{
				this.properties = value;
			}
		}

		// Token: 0x040051D1 RID: 20945
		private Type element_type;

		// Token: 0x040051D2 RID: 20946
		private bool is_dictionary;

		// Token: 0x040051D3 RID: 20947
		private IDictionary<string, PropertyMetadata> properties;
	}
}
