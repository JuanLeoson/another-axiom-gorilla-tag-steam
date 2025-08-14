using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DynamicSceneManagerHelper
{
	// Token: 0x02000CF8 RID: 3320
	internal class SnapshotComparer
	{
		// Token: 0x170007B2 RID: 1970
		// (get) Token: 0x0600523D RID: 21053 RVA: 0x001987EB File Offset: 0x001969EB
		public SceneSnapshot BaseSnapshot { get; }

		// Token: 0x170007B3 RID: 1971
		// (get) Token: 0x0600523E RID: 21054 RVA: 0x001987F3 File Offset: 0x001969F3
		public SceneSnapshot NewSnapshot { get; }

		// Token: 0x0600523F RID: 21055 RVA: 0x001987FB File Offset: 0x001969FB
		public SnapshotComparer(SceneSnapshot baseSnapshot, SceneSnapshot newSnapshot)
		{
			this.BaseSnapshot = baseSnapshot;
			this.NewSnapshot = newSnapshot;
		}

		// Token: 0x06005240 RID: 21056 RVA: 0x00198814 File Offset: 0x00196A14
		public List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> Compare()
		{
			List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> list = new List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>>();
			foreach (OVRAnchor ovranchor in this.BaseSnapshot.Anchors.Keys)
			{
				if (!this.NewSnapshot.Contains(ovranchor))
				{
					list.Add(new ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>(ovranchor, SnapshotComparer.ChangeType.Missing));
				}
			}
			foreach (OVRAnchor ovranchor2 in this.NewSnapshot.Anchors.Keys)
			{
				if (!this.BaseSnapshot.Contains(ovranchor2))
				{
					list.Add(new ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>(ovranchor2, SnapshotComparer.ChangeType.New));
				}
			}
			this.CheckRoomChanges(list);
			this.CheckBoundsChanges(list);
			return list;
		}

		// Token: 0x06005241 RID: 21057 RVA: 0x001988FC File Offset: 0x00196AFC
		private void CheckRoomChanges(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes)
		{
			for (int i = 0; i < changes.Count; i++)
			{
				ValueTuple<OVRAnchor, SnapshotComparer.ChangeType> valueTuple = changes[i];
				OVRAnchor item = valueTuple.Item1;
				SnapshotComparer.ChangeType item2 = valueTuple.Item2;
				OVRRoomLayout ovrroomLayout;
				if (item.TryGetComponent<OVRRoomLayout>(out ovrroomLayout) && ovrroomLayout.IsEnabled && item2 != SnapshotComparer.ChangeType.ChangedId)
				{
					bool flag = this.NewSnapshot.Contains(item);
					bool flag2 = this.BaseSnapshot.Contains(item);
					if (flag || flag2)
					{
						List<OVRAnchor> list = flag ? this.NewSnapshot.Anchors[item].Children : this.BaseSnapshot.Anchors[item].Children;
						SceneSnapshot sceneSnapshot = (item2 == SnapshotComparer.ChangeType.New) ? this.BaseSnapshot : this.NewSnapshot;
						foreach (OVRAnchor anchor in list)
						{
							if (sceneSnapshot.Contains(anchor))
							{
								changes[i] = new ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>(item, SnapshotComparer.ChangeType.ChangedId);
							}
						}
					}
				}
			}
		}

		// Token: 0x06005242 RID: 21058 RVA: 0x00198A18 File Offset: 0x00196C18
		private void CheckBoundsChanges(List<ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>> changes)
		{
			using (Dictionary<OVRAnchor, SceneSnapshot.Data>.KeyCollection.Enumerator enumerator = this.BaseSnapshot.Anchors.Keys.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					OVRAnchor baseAnchor = enumerator.Current;
					OVRAnchor key = this.NewSnapshot.Anchors.Keys.FirstOrDefault((OVRAnchor newAnchor) => newAnchor.Uuid == baseAnchor.Uuid);
					if (key.Uuid == baseAnchor.Uuid)
					{
						SceneSnapshot.Data data = this.BaseSnapshot.Anchors[baseAnchor];
						SceneSnapshot.Data data2 = this.NewSnapshot.Anchors[key];
						bool flag = this.Has2DBounds(data, data2) && this.Are2DBoundsDifferent(data, data2);
						bool flag2 = this.Has3DBounds(data, data2) && this.Are3DBoundsDifferent(data, data2);
						if (flag || flag2)
						{
							changes.Add(new ValueTuple<OVRAnchor, SnapshotComparer.ChangeType>(baseAnchor, SnapshotComparer.ChangeType.ChangedBounds));
						}
					}
				}
			}
		}

		// Token: 0x06005243 RID: 21059 RVA: 0x00198B2C File Offset: 0x00196D2C
		private bool Has2DBounds(SceneSnapshot.Data data1, SceneSnapshot.Data data2)
		{
			return data1.Rect != null && data2.Rect != null;
		}

		// Token: 0x06005244 RID: 21060 RVA: 0x00198B48 File Offset: 0x00196D48
		private bool Are2DBoundsDifferent(SceneSnapshot.Data data1, SceneSnapshot.Data data2)
		{
			Vector2? vector = (data1.Rect != null) ? new Vector2?(data1.Rect.GetValueOrDefault().min) : null;
			if (!(vector != ((data2.Rect != null) ? new Vector2?(data2.Rect.GetValueOrDefault().min) : null)))
			{
				Vector2? vector2 = (data1.Rect != null) ? new Vector2?(data1.Rect.GetValueOrDefault().max) : null;
				return vector2 != ((data2.Rect != null) ? new Vector2?(data2.Rect.GetValueOrDefault().max) : null);
			}
			return true;
		}

		// Token: 0x06005245 RID: 21061 RVA: 0x00198C72 File Offset: 0x00196E72
		private bool Has3DBounds(SceneSnapshot.Data data1, SceneSnapshot.Data data2)
		{
			return data1.Bounds != null && data2.Bounds != null;
		}

		// Token: 0x06005246 RID: 21062 RVA: 0x00198C90 File Offset: 0x00196E90
		private bool Are3DBoundsDifferent(SceneSnapshot.Data data1, SceneSnapshot.Data data2)
		{
			Vector3? vector = (data1.Bounds != null) ? new Vector3?(data1.Bounds.GetValueOrDefault().min) : null;
			if (!(vector != ((data2.Bounds != null) ? new Vector3?(data2.Bounds.GetValueOrDefault().min) : null)))
			{
				Vector3? vector2 = (data1.Bounds != null) ? new Vector3?(data1.Bounds.GetValueOrDefault().max) : null;
				return vector2 != ((data2.Bounds != null) ? new Vector3?(data2.Bounds.GetValueOrDefault().max) : null);
			}
			return true;
		}

		// Token: 0x02000CF9 RID: 3321
		public enum ChangeType
		{
			// Token: 0x04005BB3 RID: 23475
			New,
			// Token: 0x04005BB4 RID: 23476
			Missing,
			// Token: 0x04005BB5 RID: 23477
			ChangedId,
			// Token: 0x04005BB6 RID: 23478
			ChangedBounds
		}
	}
}
