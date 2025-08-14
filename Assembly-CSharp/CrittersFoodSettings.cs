using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class CrittersFoodSettings : CrittersActorSettings
{
	// Token: 0x060001A2 RID: 418 RVA: 0x0000A520 File Offset: 0x00008720
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersFood crittersFood = (CrittersFood)this.parentActor;
		crittersFood.maxFood = this._maxFood;
		crittersFood.currentFood = this._currentFood;
		crittersFood.startingSize = this._startingSize;
		crittersFood.currentSize = this._currentSize;
		crittersFood.food = this._food;
		crittersFood.disableWhenEmpty = this._disableWhenEmpty;
		crittersFood.SpawnData(this._maxFood, this._currentFood, this._startingSize);
	}

	// Token: 0x040001EC RID: 492
	public float _maxFood;

	// Token: 0x040001ED RID: 493
	public float _currentFood;

	// Token: 0x040001EE RID: 494
	public float _startingSize;

	// Token: 0x040001EF RID: 495
	public float _currentSize;

	// Token: 0x040001F0 RID: 496
	public Transform _food;

	// Token: 0x040001F1 RID: 497
	public bool _disableWhenEmpty;
}
