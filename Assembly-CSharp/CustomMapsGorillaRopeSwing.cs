using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaLocomotion.Gameplay;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x02000808 RID: 2056
public class CustomMapsGorillaRopeSwing : GorillaRopeSwing
{
	// Token: 0x06003375 RID: 13173 RVA: 0x0010B83A File Offset: 0x00109A3A
	protected override void Awake()
	{
		base.CalculateId(true);
		base.StartCoroutine(this.WaitForRopeLength());
	}

	// Token: 0x06003376 RID: 13174 RVA: 0x000023F5 File Offset: 0x000005F5
	protected override void Start()
	{
	}

	// Token: 0x06003377 RID: 13175 RVA: 0x0010B850 File Offset: 0x00109A50
	protected override void OnEnable()
	{
		if (!this.isRopeLengthSet)
		{
			return;
		}
		base.OnEnable();
	}

	// Token: 0x06003378 RID: 13176 RVA: 0x0010B861 File Offset: 0x00109A61
	public void SetRopeLength(int length)
	{
		this.ropeLength = length;
		this.isRopeLengthSet = true;
	}

	// Token: 0x06003379 RID: 13177 RVA: 0x0010B871 File Offset: 0x00109A71
	private IEnumerator WaitForRopeLength()
	{
		while (!this.isRopeLengthSet)
		{
			yield return null;
		}
		this.RopeGeneration();
		base.Awake();
		base.OnEnable();
		base.Start();
		yield break;
	}

	// Token: 0x0600337A RID: 13178 RVA: 0x0010B880 File Offset: 0x00109A80
	private void RopeGeneration()
	{
		List<Transform> list = new List<Transform>();
		Vector3 vector = Vector3.zero;
		float y = this.prefabRopeBit.GetComponentInChildren<Renderer>().bounds.size.y;
		WaterVolume[] array = Object.FindObjectsOfType<WaterVolume>();
		List<Collider> list2 = new List<Collider>(array.Length);
		WaterVolume[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			foreach (Collider collider in array2[i].volumeColliders)
			{
				if (!(collider == null))
				{
					list2.Add(collider);
				}
			}
		}
		for (int j = 0; j < this.ropeLength + 1; j++)
		{
			bool flag = false;
			if (list2.Count > 0)
			{
				Collider collider2 = list2[0];
				if (collider2 != null)
				{
					Vector3 vector2 = base.transform.position + vector;
					Vector3 point = vector2 + new Vector3(0f, -y, 0f);
					flag = (collider2.bounds.Contains(vector2) || collider2.bounds.Contains(point));
				}
			}
			GameObject gameObject = Object.Instantiate<GameObject>(flag ? this.partiallyUnderwaterPrefab : this.prefabRopeBit, base.transform);
			gameObject.name = string.Format("RopeBone_{0:00}", j);
			gameObject.transform.localPosition = vector;
			gameObject.transform.localRotation = Quaternion.identity;
			vector += new Vector3(0f, -1f, 0f);
			GorillaRopeSegment component = gameObject.GetComponent<GorillaRopeSegment>();
			component.swing = this;
			component.boneIndex = j;
			list.Add(gameObject.transform);
		}
		list[0].GetComponent<BoxCollider>().center = new Vector3(0f, -0.65f, 0f);
		list[0].GetComponent<BoxCollider>().size = new Vector3(0.3f, 0.65f, 0.3f);
		list.Last<Transform>().gameObject.SetActive(false);
		this.nodes = list.ToArray();
	}

	// Token: 0x0400406A RID: 16490
	[SerializeField]
	private GameObject partiallyUnderwaterPrefab;

	// Token: 0x0400406B RID: 16491
	private bool isRopeLengthSet;
}
