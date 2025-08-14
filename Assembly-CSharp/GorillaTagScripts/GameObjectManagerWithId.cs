using System;
using System.Collections.Generic;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C32 RID: 3122
	public class GameObjectManagerWithId : MonoBehaviour
	{
		// Token: 0x06004D14 RID: 19732 RVA: 0x0017F7F0 File Offset: 0x0017D9F0
		private void Awake()
		{
			Transform[] componentsInChildren = this.objectsContainer.GetComponentsInChildren<Transform>(false);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				GameObjectManagerWithId.gameObjectData gameObjectData = new GameObjectManagerWithId.gameObjectData();
				gameObjectData.transform = componentsInChildren[i];
				gameObjectData.id = this.zone.ToString() + i.ToString();
				this.objectData.Add(gameObjectData);
			}
		}

		// Token: 0x06004D15 RID: 19733 RVA: 0x0017F856 File Offset: 0x0017DA56
		private void OnDestroy()
		{
			this.objectData.Clear();
		}

		// Token: 0x06004D16 RID: 19734 RVA: 0x0017F864 File Offset: 0x0017DA64
		public void ReceiveEvent(string id, Transform _transform)
		{
			foreach (GameObjectManagerWithId.gameObjectData gameObjectData in this.objectData)
			{
				if (gameObjectData.id == id)
				{
					gameObjectData.isMatched = true;
					gameObjectData.followTransform = _transform;
				}
			}
		}

		// Token: 0x06004D17 RID: 19735 RVA: 0x0017F8CC File Offset: 0x0017DACC
		private void Update()
		{
			foreach (GameObjectManagerWithId.gameObjectData gameObjectData in this.objectData)
			{
				if (gameObjectData.isMatched)
				{
					gameObjectData.transform.transform.position = gameObjectData.followTransform.position;
					gameObjectData.transform.transform.rotation = gameObjectData.followTransform.rotation;
				}
			}
		}

		// Token: 0x0400560D RID: 22029
		public GameObject objectsContainer;

		// Token: 0x0400560E RID: 22030
		public GTZone zone;

		// Token: 0x0400560F RID: 22031
		private readonly List<GameObjectManagerWithId.gameObjectData> objectData = new List<GameObjectManagerWithId.gameObjectData>();

		// Token: 0x02000C33 RID: 3123
		private class gameObjectData
		{
			// Token: 0x04005610 RID: 22032
			public Transform transform;

			// Token: 0x04005611 RID: 22033
			public Transform followTransform;

			// Token: 0x04005612 RID: 22034
			public string id;

			// Token: 0x04005613 RID: 22035
			public bool isMatched;
		}
	}
}
