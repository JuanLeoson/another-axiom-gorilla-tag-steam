using System;

namespace LitJson
{
	// Token: 0x02000BB5 RID: 2997
	internal struct ArrayMetadata
	{
		// Token: 0x170006FE RID: 1790
		// (get) Token: 0x0600486F RID: 18543 RVA: 0x001610EC File Offset: 0x0015F2EC
		// (set) Token: 0x06004870 RID: 18544 RVA: 0x0016110D File Offset: 0x0015F30D
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

		// Token: 0x170006FF RID: 1791
		// (get) Token: 0x06004871 RID: 18545 RVA: 0x00161116 File Offset: 0x0015F316
		// (set) Token: 0x06004872 RID: 18546 RVA: 0x0016111E File Offset: 0x0015F31E
		public bool IsArray
		{
			get
			{
				return this.is_array;
			}
			set
			{
				this.is_array = value;
			}
		}

		// Token: 0x17000700 RID: 1792
		// (get) Token: 0x06004873 RID: 18547 RVA: 0x00161127 File Offset: 0x0015F327
		// (set) Token: 0x06004874 RID: 18548 RVA: 0x0016112F File Offset: 0x0015F32F
		public bool IsList
		{
			get
			{
				return this.is_list;
			}
			set
			{
				this.is_list = value;
			}
		}

		// Token: 0x040051CE RID: 20942
		private Type element_type;

		// Token: 0x040051CF RID: 20943
		private bool is_array;

		// Token: 0x040051D0 RID: 20944
		private bool is_list;
	}
}
