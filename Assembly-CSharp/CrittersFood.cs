using System;
using System.Collections.Generic;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class CrittersFood : CrittersActor
{
	// Token: 0x06000196 RID: 406 RVA: 0x0000A18D File Offset: 0x0000838D
	public override void Initialize()
	{
		base.Initialize();
		this.currentFood = this.maxFood;
	}

	// Token: 0x06000197 RID: 407 RVA: 0x0000A1A4 File Offset: 0x000083A4
	public void SpawnData(float _maxFood, float _currentFood, float _startingSize)
	{
		this.maxFood = _maxFood;
		this.currentFood = _currentFood;
		this.startingSize = _startingSize;
		this.currentSize = this.currentFood / this.maxFood * this.startingSize;
		this.food.localScale = new Vector3(this.currentSize, this.currentSize, this.currentSize);
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0000A204 File Offset: 0x00008404
	public override bool ProcessLocal()
	{
		bool flag = base.ProcessLocal();
		if (!this.isEnabled)
		{
			return flag;
		}
		this.wasEnabled = base.gameObject.activeSelf;
		this.ProcessFood();
		bool flag2 = Mathf.FloorToInt(this.currentFood) != this.lastFood;
		this.lastFood = Mathf.FloorToInt(this.currentFood);
		if (this.currentFood == 0f && this.disableWhenEmpty)
		{
			this.isEnabled = false;
		}
		if (base.gameObject.activeSelf != this.isEnabled)
		{
			base.gameObject.SetActive(this.isEnabled);
		}
		this.updatedSinceLastFrame = (flag || flag2 || this.wasEnabled != this.isEnabled);
		return this.updatedSinceLastFrame;
	}

	// Token: 0x06000199 RID: 409 RVA: 0x0000A2C6 File Offset: 0x000084C6
	public override void ProcessRemote()
	{
		base.ProcessRemote();
		if (!this.isEnabled)
		{
			return;
		}
		this.ProcessFood();
	}

	// Token: 0x0600019A RID: 410 RVA: 0x0000A2E0 File Offset: 0x000084E0
	public void ProcessFood()
	{
		if (this.currentSize != this.currentFood / this.maxFood * this.startingSize)
		{
			this.currentSize = this.currentFood / this.maxFood * this.startingSize;
			this.food.localScale = new Vector3(this.currentSize, this.currentSize, this.currentSize);
			if (this.storeCollider != null)
			{
				this.storeCollider.radius = this.currentSize / 2f;
			}
		}
	}

	// Token: 0x0600019B RID: 411 RVA: 0x0000A36A File Offset: 0x0000856A
	public void Feed(float amountEaten)
	{
		this.currentFood = Mathf.Max(0f, this.currentFood - amountEaten);
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000A384 File Offset: 0x00008584
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		int num;
		float value;
		float value2;
		if (!(base.UpdateSpecificActor(stream) & CrittersManager.ValidateDataType<int>(stream.ReceiveNext(), out num) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out value) & CrittersManager.ValidateDataType<float>(stream.ReceiveNext(), out value2)))
		{
			return false;
		}
		this.currentFood = (float)num;
		this.maxFood = value.GetFinite();
		this.startingSize = value2.GetFinite();
		return true;
	}

	// Token: 0x0600019D RID: 413 RVA: 0x0000A3E8 File Offset: 0x000085E8
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(Mathf.FloorToInt(this.currentFood));
		stream.SendNext(this.maxFood);
		stream.SendNext(this.startingSize);
	}

	// Token: 0x0600019E RID: 414 RVA: 0x0000A434 File Offset: 0x00008634
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(Mathf.FloorToInt(this.currentFood));
		objList.Add(this.maxFood);
		objList.Add(this.startingSize);
		return this.TotalActorDataLength();
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000A48A File Offset: 0x0000868A
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 3;
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x0000A494 File Offset: 0x00008694
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		int num;
		if (!CrittersManager.ValidateDataType<int>(data[startingIndex], out num))
		{
			return this.TotalActorDataLength();
		}
		float value;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 1], out value))
		{
			return this.TotalActorDataLength();
		}
		float value2;
		if (!CrittersManager.ValidateDataType<float>(data[startingIndex + 2], out value2))
		{
			return this.TotalActorDataLength();
		}
		this.currentFood = (float)num;
		this.maxFood = value.GetFinite();
		this.startingSize = value2.GetFinite();
		return this.TotalActorDataLength();
	}

	// Token: 0x040001E5 RID: 485
	public float maxFood;

	// Token: 0x040001E6 RID: 486
	public float currentFood;

	// Token: 0x040001E7 RID: 487
	private int lastFood;

	// Token: 0x040001E8 RID: 488
	public float startingSize;

	// Token: 0x040001E9 RID: 489
	public float currentSize;

	// Token: 0x040001EA RID: 490
	public Transform food;

	// Token: 0x040001EB RID: 491
	public bool disableWhenEmpty = true;
}
