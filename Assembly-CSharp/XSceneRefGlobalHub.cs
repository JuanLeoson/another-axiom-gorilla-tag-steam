using System;
using System.Collections.Generic;

// Token: 0x0200026F RID: 623
public static class XSceneRefGlobalHub
{
	// Token: 0x06000E65 RID: 3685 RVA: 0x00057C4C File Offset: 0x00055E4C
	public static void Register(int ID, XSceneRefTarget obj)
	{
		if (ID > 0)
		{
			int sceneIndex = (int)obj.GetSceneIndex();
			if (sceneIndex >= 0)
			{
				XSceneRefGlobalHub.registry[sceneIndex][ID] = obj;
			}
		}
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x00057C7C File Offset: 0x00055E7C
	public static void Unregister(int ID, XSceneRefTarget obj)
	{
		int sceneIndex = (int)obj.GetSceneIndex();
		if (ID > 0 && sceneIndex >= 0)
		{
			XSceneRefGlobalHub.registry[sceneIndex].Remove(ID);
		}
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x00057CAA File Offset: 0x00055EAA
	public static bool TryResolve(SceneIndex sceneIndex, int ID, out XSceneRefTarget result)
	{
		return XSceneRefGlobalHub.registry[(int)sceneIndex].TryGetValue(ID, out result);
	}

	// Token: 0x0400174A RID: 5962
	private static List<Dictionary<int, XSceneRefTarget>> registry = new List<Dictionary<int, XSceneRefTarget>>
	{
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		},
		new Dictionary<int, XSceneRefTarget>
		{
			{
				0,
				null
			}
		}
	};
}
