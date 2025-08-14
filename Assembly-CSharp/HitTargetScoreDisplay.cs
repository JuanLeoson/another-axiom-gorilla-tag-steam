using System;
using System.Collections;
using GorillaTag;
using UnityEngine;

// Token: 0x020003D4 RID: 980
public class HitTargetScoreDisplay : MonoBehaviour
{
	// Token: 0x060016E9 RID: 5865 RVA: 0x0007C680 File Offset: 0x0007A880
	protected void Awake()
	{
		this.rotateTimeTotal = 180f / (float)this.rotateSpeed;
		this.matPropBlock = new MaterialPropertyBlock();
		this.networkedScore.AddCallback(new Action<int>(this.OnScoreChanged), true);
		this.ResetRotation();
		this.tensOld = 0;
		this.hundredsOld = 0;
		this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[0]);
		this.singlesRend.SetPropertyBlock(this.matPropBlock);
		this.tensRend.SetPropertyBlock(this.matPropBlock);
		this.hundredsRend.SetPropertyBlock(this.matPropBlock);
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x0007C726 File Offset: 0x0007A926
	private void OnDestroy()
	{
		this.networkedScore.RemoveCallback(new Action<int>(this.OnScoreChanged));
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x0007C740 File Offset: 0x0007A940
	private void ResetRotation()
	{
		Quaternion rotation = base.transform.rotation;
		this.singlesCard.rotation = rotation;
		this.tensCard.rotation = rotation;
		this.hundredsCard.rotation = rotation;
	}

	// Token: 0x060016EC RID: 5868 RVA: 0x0007C77D File Offset: 0x0007A97D
	private IEnumerator RotatingCo()
	{
		float timeElapsedSinceHit = 0f;
		int singlesPlace = this.currentScore % 10;
		int tensPlace = this.currentScore / 10 % 10;
		bool tensChange = this.tensOld != tensPlace;
		this.tensOld = tensPlace;
		int hundredsPlace = this.currentScore / 100 % 10;
		bool hundredsChange = this.hundredsOld != hundredsPlace;
		this.hundredsOld = hundredsPlace;
		bool digitsChange = true;
		while (timeElapsedSinceHit < this.rotateTimeTotal)
		{
			this.singlesCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
			Vector3 localEulerAngles = this.singlesCard.localEulerAngles;
			localEulerAngles.x = Mathf.Clamp(localEulerAngles.x, 0f, 180f);
			this.singlesCard.localEulerAngles = localEulerAngles;
			if (tensChange)
			{
				this.tensCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles2 = this.tensCard.localEulerAngles;
				localEulerAngles2.x = Mathf.Clamp(localEulerAngles2.x, 0f, 180f);
				this.tensCard.localEulerAngles = localEulerAngles2;
			}
			if (hundredsChange)
			{
				this.hundredsCard.Rotate((float)this.rotateSpeed * Time.deltaTime, 0f, 0f, Space.Self);
				Vector3 localEulerAngles3 = this.hundredsCard.localEulerAngles;
				localEulerAngles3.x = Mathf.Clamp(localEulerAngles3.x, 0f, 180f);
				this.hundredsCard.localEulerAngles = localEulerAngles3;
			}
			if (digitsChange && timeElapsedSinceHit >= this.rotateTimeTotal / 2f)
			{
				this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[singlesPlace]);
				this.singlesRend.SetPropertyBlock(this.matPropBlock);
				if (tensChange)
				{
					this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[tensPlace]);
					this.tensRend.SetPropertyBlock(this.matPropBlock);
				}
				if (hundredsChange)
				{
					this.matPropBlock.SetVector(ShaderProps._BaseMap_ST, this.numberSheet[hundredsPlace]);
					this.hundredsRend.SetPropertyBlock(this.matPropBlock);
				}
				digitsChange = false;
			}
			yield return null;
			timeElapsedSinceHit += Time.deltaTime;
		}
		this.ResetRotation();
		yield break;
		yield break;
	}

	// Token: 0x060016ED RID: 5869 RVA: 0x0007C78C File Offset: 0x0007A98C
	private void OnScoreChanged(int newScore)
	{
		if (newScore == this.currentScore)
		{
			return;
		}
		if (this.currentRotationCoroutine != null)
		{
			base.StopCoroutine(this.currentRotationCoroutine);
		}
		this.currentScore = newScore;
		this.currentRotationCoroutine = base.StartCoroutine(this.RotatingCo());
	}

	// Token: 0x04001EB7 RID: 7863
	[SerializeField]
	private WatchableIntSO networkedScore;

	// Token: 0x04001EB8 RID: 7864
	private int currentScore;

	// Token: 0x04001EB9 RID: 7865
	private int tensOld;

	// Token: 0x04001EBA RID: 7866
	private int hundredsOld;

	// Token: 0x04001EBB RID: 7867
	private float rotateTimeTotal;

	// Token: 0x04001EBC RID: 7868
	private MaterialPropertyBlock matPropBlock;

	// Token: 0x04001EBD RID: 7869
	private readonly Vector4[] numberSheet = new Vector4[]
	{
		new Vector4(1f, 1f, 0.8f, -0.5f),
		new Vector4(1f, 1f, 0f, 0f),
		new Vector4(1f, 1f, 0.2f, 0f),
		new Vector4(1f, 1f, 0.4f, 0f),
		new Vector4(1f, 1f, 0.6f, 0f),
		new Vector4(1f, 1f, 0.8f, 0f),
		new Vector4(1f, 1f, 0f, -0.5f),
		new Vector4(1f, 1f, 0.2f, -0.5f),
		new Vector4(1f, 1f, 0.4f, -0.5f),
		new Vector4(1f, 1f, 0.6f, -0.5f)
	};

	// Token: 0x04001EBE RID: 7870
	public int rotateSpeed = 180;

	// Token: 0x04001EBF RID: 7871
	public Transform singlesCard;

	// Token: 0x04001EC0 RID: 7872
	public Transform tensCard;

	// Token: 0x04001EC1 RID: 7873
	public Transform hundredsCard;

	// Token: 0x04001EC2 RID: 7874
	public Renderer singlesRend;

	// Token: 0x04001EC3 RID: 7875
	public Renderer tensRend;

	// Token: 0x04001EC4 RID: 7876
	public Renderer hundredsRend;

	// Token: 0x04001EC5 RID: 7877
	private Coroutine currentRotationCoroutine;
}
