using System;
using CustomMapSupport;
using GorillaExtensions;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using UnityEngine;

// Token: 0x0200080A RID: 2058
public class CustomMapsGorillaZipline : GorillaZipline
{
	// Token: 0x06003385 RID: 13189 RVA: 0x0010BB60 File Offset: 0x00109D60
	public bool GenerateZipline(CustomMapSupport.BezierSpline splineRef)
	{
		this.spline = base.GetComponent<global::BezierSpline>();
		if (this.spline.IsNull())
		{
			return false;
		}
		this.spline.BuildSplineFromPoints(splineRef.GetControlPoints(), this.ConvertControlPointModes(splineRef.GetControlPointModes()), splineRef.Loop);
		if (this.segmentsRoot == null)
		{
			return false;
		}
		this.ziplineDistance = 0f;
		float num = 0f;
		int num2 = 0;
		Transform transform = null;
		while (num < 1f)
		{
			float num3 = this.segmentDistance;
			if (num2 == 0)
			{
				num3 /= 2f;
			}
			base.FindTFromDistance(ref num, num3, 5000);
			if (num < 1f || this.spline.Loop)
			{
				Vector3 point = this.spline.GetPoint(num);
				GameObject gameObject = Object.Instantiate<GameObject>(this.segmentPrefab);
				gameObject.transform.SetParent(this.segmentsRoot);
				gameObject.transform.position = point;
				gameObject.transform.LookAt(point + this.spline.GetDirection(num));
				gameObject.transform.position -= gameObject.transform.forward * 0.5f;
				if (num2 > 0)
				{
					transform.LookAt(gameObject.transform);
				}
				gameObject.GetComponent<GorillaClimbableRef>().climb = this.slideHelper;
				this.ziplineDistance += this.segmentDistance;
				transform = gameObject.transform;
			}
			num2++;
		}
		return true;
	}

	// Token: 0x06003386 RID: 13190 RVA: 0x0010BCE4 File Offset: 0x00109EE4
	private global::BezierControlPointMode[] ConvertControlPointModes(CustomMapSupport.BezierControlPointMode[] refModes)
	{
		global::BezierControlPointMode[] array = new global::BezierControlPointMode[refModes.Length];
		for (int i = 0; i < refModes.Length; i++)
		{
			switch (refModes[i])
			{
			case CustomMapSupport.BezierControlPointMode.Free:
				array[i] = global::BezierControlPointMode.Free;
				break;
			case CustomMapSupport.BezierControlPointMode.Aligned:
				array[i] = global::BezierControlPointMode.Aligned;
				break;
			case CustomMapSupport.BezierControlPointMode.Mirrored:
				array[i] = global::BezierControlPointMode.Mirrored;
				break;
			}
		}
		return array;
	}

	// Token: 0x06003387 RID: 13191 RVA: 0x0010BD31 File Offset: 0x00109F31
	protected override void Start()
	{
		GorillaClimbable slideHelper = this.slideHelper;
		slideHelper.onBeforeClimb = (Action<GorillaHandClimber, GorillaClimbableRef>)Delegate.Combine(slideHelper.onBeforeClimb, new Action<GorillaHandClimber, GorillaClimbableRef>(base.OnBeforeClimb));
	}
}
