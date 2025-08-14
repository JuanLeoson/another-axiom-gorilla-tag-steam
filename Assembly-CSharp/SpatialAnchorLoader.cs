using System;
using UnityEngine;

// Token: 0x020003B6 RID: 950
public class SpatialAnchorLoader : MonoBehaviour
{
	// Token: 0x06001609 RID: 5641 RVA: 0x00078184 File Offset: 0x00076384
	public void LoadAnchorsByUuid()
	{
		if (!PlayerPrefs.HasKey("numUuids"))
		{
			PlayerPrefs.SetInt("numUuids", 0);
		}
		int @int = PlayerPrefs.GetInt("numUuids");
		SpatialAnchorLoader.Log(string.Format("Attempting to load {0} saved anchors.", @int));
		if (@int == 0)
		{
			return;
		}
		Guid[] array = new Guid[@int];
		for (int i = 0; i < @int; i++)
		{
			string @string = PlayerPrefs.GetString("uuid" + i.ToString());
			SpatialAnchorLoader.Log("QueryAnchorByUuid: " + @string);
			array[i] = new Guid(@string);
		}
		this.Load(new OVRSpatialAnchor.LoadOptions
		{
			Timeout = 0.0,
			StorageLocation = OVRSpace.StorageLocation.Local,
			Uuids = array
		});
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x00078243 File Offset: 0x00076443
	private void Awake()
	{
		this._onLoadAnchor = new Action<OVRSpatialAnchor.UnboundAnchor, bool>(this.OnLocalized);
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x00078257 File Offset: 0x00076457
	private void Load(OVRSpatialAnchor.LoadOptions options)
	{
		OVRSpatialAnchor.LoadUnboundAnchors(options, delegate(OVRSpatialAnchor.UnboundAnchor[] anchors)
		{
			if (anchors == null)
			{
				SpatialAnchorLoader.Log("Query failed.");
				return;
			}
			foreach (OVRSpatialAnchor.UnboundAnchor arg in anchors)
			{
				if (arg.Localized)
				{
					this._onLoadAnchor(arg, true);
				}
				else if (!arg.Localizing)
				{
					arg.Localize(this._onLoadAnchor, 0.0);
				}
			}
		});
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x0007826C File Offset: 0x0007646C
	private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
	{
		if (!success)
		{
			SpatialAnchorLoader.Log(string.Format("{0} Localization failed!", unboundAnchor));
			return;
		}
		Pose pose = unboundAnchor.Pose;
		OVRSpatialAnchor ovrspatialAnchor = Object.Instantiate<OVRSpatialAnchor>(this._anchorPrefab, pose.position, pose.rotation);
		unboundAnchor.BindTo(ovrspatialAnchor);
		Anchor anchor;
		if (ovrspatialAnchor.TryGetComponent<Anchor>(out anchor))
		{
			anchor.ShowSaveIcon = true;
		}
	}

	// Token: 0x0600160D RID: 5645 RVA: 0x000782CB File Offset: 0x000764CB
	private static void Log(string message)
	{
		Debug.Log("[SpatialAnchorsUnity]: " + message);
	}

	// Token: 0x04001DC6 RID: 7622
	[SerializeField]
	private OVRSpatialAnchor _anchorPrefab;

	// Token: 0x04001DC7 RID: 7623
	private Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor;
}
