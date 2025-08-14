using System;
using System.Collections.Generic;

// Token: 0x0200016C RID: 364
[Serializable]
public class FoundAllocatorsMapped
{
	// Token: 0x04000B5D RID: 2909
	public string path;

	// Token: 0x04000B5E RID: 2910
	public List<ViewsAndAllocator> allocators = new List<ViewsAndAllocator>();

	// Token: 0x04000B5F RID: 2911
	public List<FoundAllocatorsMapped> subGroups = new List<FoundAllocatorsMapped>();
}
