using System;
using System.Collections.Generic;
using System.Reflection;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Animations.Rigging;

// Token: 0x02000406 RID: 1030
public class CalibrationCube : MonoBehaviour
{
	// Token: 0x0600180C RID: 6156 RVA: 0x0008095F File Offset: 0x0007EB5F
	private void Awake()
	{
		this.calibratedLength = this.baseLength;
	}

	// Token: 0x0600180D RID: 6157 RVA: 0x00080970 File Offset: 0x0007EB70
	private void Start()
	{
		try
		{
			this.OnCollisionExit(null);
		}
		catch
		{
		}
	}

	// Token: 0x0600180E RID: 6158 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnTriggerEnter(Collider other)
	{
	}

	// Token: 0x0600180F RID: 6159 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnTriggerExit(Collider other)
	{
	}

	// Token: 0x06001810 RID: 6160 RVA: 0x0008099C File Offset: 0x0007EB9C
	public void RecalibrateSize(bool pressed)
	{
		this.lastCalibratedLength = this.calibratedLength;
		this.calibratedLength = (this.rightController.transform.position - this.leftController.transform.position).magnitude;
		this.calibratedLength = ((this.calibratedLength > this.maxLength) ? this.maxLength : ((this.calibratedLength < this.minLength) ? this.minLength : this.calibratedLength));
		float d = this.calibratedLength / this.lastCalibratedLength;
		Vector3 localScale = this.playerBody.transform.localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Clear();
		this.playerBody.transform.localScale = new Vector3(1f, 1f, 1f);
		this.playerBody.GetComponentInChildren<TransformReset>().ResetTransforms();
		this.playerBody.transform.localScale = d * localScale;
		this.playerBody.GetComponentInChildren<RigBuilder>().Build();
		this.playerBody.GetComponentInChildren<VRRig>().SetHeadBodyOffset();
		GorillaPlaySpace.Instance.bodyColliderOffset *= d;
		GorillaPlaySpace.Instance.bodyCollider.gameObject.transform.localScale *= d;
	}

	// Token: 0x06001811 RID: 6161 RVA: 0x000023F5 File Offset: 0x000005F5
	private void OnCollisionEnter(Collision collision)
	{
	}

	// Token: 0x06001812 RID: 6162 RVA: 0x00080AF8 File Offset: 0x0007ECF8
	private void OnCollisionExit(Collision collision)
	{
		try
		{
			bool flag = false;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				AssemblyName name = assemblies[i].GetName();
				if (!this.calibrationPresetsTest3[0].Contains(name.Name))
				{
					flag = true;
				}
			}
			if (!flag || Application.platform == RuntimePlatform.Android)
			{
				GorillaComputer.instance.includeUpdatedServerSynchTest = 0;
			}
		}
		catch
		{
		}
	}

	// Token: 0x04001FE9 RID: 8169
	public PrimaryButtonWatcher watcher;

	// Token: 0x04001FEA RID: 8170
	public GameObject rightController;

	// Token: 0x04001FEB RID: 8171
	public GameObject leftController;

	// Token: 0x04001FEC RID: 8172
	public GameObject playerBody;

	// Token: 0x04001FED RID: 8173
	private float calibratedLength;

	// Token: 0x04001FEE RID: 8174
	private float lastCalibratedLength;

	// Token: 0x04001FEF RID: 8175
	public float minLength = 1f;

	// Token: 0x04001FF0 RID: 8176
	public float maxLength = 2.5f;

	// Token: 0x04001FF1 RID: 8177
	public float baseLength = 1.61f;

	// Token: 0x04001FF2 RID: 8178
	public string[] calibrationPresets;

	// Token: 0x04001FF3 RID: 8179
	public string[] calibrationPresetsTest;

	// Token: 0x04001FF4 RID: 8180
	public string[] calibrationPresetsTest2;

	// Token: 0x04001FF5 RID: 8181
	public string[] calibrationPresetsTest3;

	// Token: 0x04001FF6 RID: 8182
	public string[] calibrationPresetsTest4;

	// Token: 0x04001FF7 RID: 8183
	public string outputstring;

	// Token: 0x04001FF8 RID: 8184
	private List<string> stringList = new List<string>();
}
