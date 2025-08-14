using System;
using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000489 RID: 1161
public class GrabbingColorPicker : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001CD1 RID: 7377 RVA: 0x0009B37C File Offset: 0x0009957C
	private void Start()
	{
		if (!this.setPlayerColor)
		{
			return;
		}
		float @float = PlayerPrefs.GetFloat("redValue", 0f);
		float float2 = PlayerPrefs.GetFloat("greenValue", 0f);
		float float3 = PlayerPrefs.GetFloat("blueValue", 0f);
		this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, @float));
		this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, float2));
		this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, float3));
		this.R_PushSlider.SetProgress(@float);
		this.G_PushSlider.SetProgress(float2);
		this.B_PushSlider.SetProgress(float3);
		this.UpdateDisplay();
	}

	// Token: 0x06001CD2 RID: 7378 RVA: 0x0009B440 File Offset: 0x00099640
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (this.setPlayerColor && GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged += this.HandleLocalColorChanged;
		}
	}

	// Token: 0x06001CD3 RID: 7379 RVA: 0x0009B494 File Offset: 0x00099694
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (GorillaTagger.Instance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.OnColorChanged -= this.HandleLocalColorChanged;
		}
	}

	// Token: 0x06001CD4 RID: 7380 RVA: 0x0009B4E0 File Offset: 0x000996E0
	public void SliceUpdate()
	{
		float num = Vector3.Distance(base.transform.position, GTPlayer.Instance.transform.position);
		this.hasUpdated = false;
		if (num < 5f)
		{
			int segment = this.Segment1;
			int segment2 = this.Segment2;
			int segment3 = this.Segment3;
			this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.R_PushSlider.GetProgress()));
			this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.G_PushSlider.GetProgress()));
			this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, this.B_PushSlider.GetProgress()));
			if (segment != this.Segment1 || segment2 != this.Segment2 || segment3 != this.Segment3)
			{
				this.hasUpdated = true;
				if (this.setPlayerColor)
				{
					this.SetPlayerColor();
				}
				this.UpdateDisplay();
				this.UpdateColor.Invoke(new Vector3((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f));
				if (segment != this.Segment1)
				{
					this.R_SliderAudio.transform.position = this.R_PushSlider.transform.position;
					this.R_SliderAudio.GTPlay();
				}
				if (segment2 != this.Segment2)
				{
					this.G_SliderAudio.transform.position = this.G_PushSlider.transform.position;
					this.G_SliderAudio.GTPlay();
				}
				if (segment3 != this.Segment3)
				{
					this.B_SliderAudio.transform.position = this.B_PushSlider.transform.position;
					this.B_SliderAudio.GTPlay();
				}
			}
		}
	}

	// Token: 0x06001CD5 RID: 7381 RVA: 0x0009B6B0 File Offset: 0x000998B0
	private void SetPlayerColor()
	{
		PlayerPrefs.SetFloat("redValue", (float)this.Segment1 / 9f);
		PlayerPrefs.SetFloat("greenValue", (float)this.Segment2 / 9f);
		PlayerPrefs.SetFloat("blueValue", (float)this.Segment3 / 9f);
		GorillaTagger.Instance.UpdateColor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		GorillaComputer.instance.UpdateColor((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		PlayerPrefs.Save();
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaTagger.Instance.myVRRig.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.All, new object[]
			{
				(float)this.Segment1 / 9f,
				(float)this.Segment2 / 9f,
				(float)this.Segment3 / 9f
			});
		}
	}

	// Token: 0x06001CD6 RID: 7382 RVA: 0x0009B7D4 File Offset: 0x000999D4
	private void SetSliderColors(float r, float g, float b)
	{
		if (!this.hasUpdated)
		{
			this.Segment1 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, r));
			this.Segment2 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, g));
			this.Segment3 = Mathf.RoundToInt(Mathf.Lerp(0f, 9f, b));
			this.R_PushSlider.SetProgress(r);
			this.G_PushSlider.SetProgress(g);
			this.B_PushSlider.SetProgress(b);
			this.UpdateDisplay();
		}
	}

	// Token: 0x06001CD7 RID: 7383 RVA: 0x0009B864 File Offset: 0x00099A64
	private void HandleLocalColorChanged(Color newColor)
	{
		this.SetSliderColors(newColor.r, newColor.g, newColor.b);
	}

	// Token: 0x06001CD8 RID: 7384 RVA: 0x0009B880 File Offset: 0x00099A80
	private void UpdateDisplay()
	{
		this.textR.text = this.Segment1.ToString();
		this.textG.text = this.Segment2.ToString();
		this.textB.text = this.Segment3.ToString();
		Color color = new Color((float)this.Segment1 / 9f, (float)this.Segment2 / 9f, (float)this.Segment3 / 9f);
		Renderer[] componentsInChildren = this.ColorSwatch.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Material[] materials = componentsInChildren[i].materials;
			for (int j = 0; j < materials.Length; j++)
			{
				materials[j].color = color;
			}
		}
	}

	// Token: 0x06001CD9 RID: 7385 RVA: 0x0009B93C File Offset: 0x00099B3C
	public void ResetSliders(Vector3 v)
	{
		this.SetSliderColors(v.x, v.y, v.z);
	}

	// Token: 0x04002537 RID: 9527
	[SerializeField]
	private bool setPlayerColor = true;

	// Token: 0x04002538 RID: 9528
	[SerializeField]
	private PushableSlider R_PushSlider;

	// Token: 0x04002539 RID: 9529
	[SerializeField]
	private PushableSlider G_PushSlider;

	// Token: 0x0400253A RID: 9530
	[SerializeField]
	private PushableSlider B_PushSlider;

	// Token: 0x0400253B RID: 9531
	[SerializeField]
	private AudioSource R_SliderAudio;

	// Token: 0x0400253C RID: 9532
	[SerializeField]
	private AudioSource G_SliderAudio;

	// Token: 0x0400253D RID: 9533
	[SerializeField]
	private AudioSource B_SliderAudio;

	// Token: 0x0400253E RID: 9534
	[SerializeField]
	private TextMeshPro textR;

	// Token: 0x0400253F RID: 9535
	[SerializeField]
	private TextMeshPro textG;

	// Token: 0x04002540 RID: 9536
	[SerializeField]
	private TextMeshPro textB;

	// Token: 0x04002541 RID: 9537
	[SerializeField]
	private GameObject ColorSwatch;

	// Token: 0x04002542 RID: 9538
	[SerializeField]
	private UnityEvent<Vector3> UpdateColor;

	// Token: 0x04002543 RID: 9539
	private int Segment1;

	// Token: 0x04002544 RID: 9540
	private int Segment2;

	// Token: 0x04002545 RID: 9541
	private int Segment3;

	// Token: 0x04002546 RID: 9542
	private bool hasUpdated;
}
