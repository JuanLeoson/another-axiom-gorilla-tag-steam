using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000374 RID: 884
public class PassthroughBrush : MonoBehaviour
{
	// Token: 0x060014E2 RID: 5346 RVA: 0x00072244 File Offset: 0x00070444
	private void OnDisable()
	{
		this.brushStatus = PassthroughBrush.BrushState.Idle;
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x00072250 File Offset: 0x00070450
	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(base.transform.position - Camera.main.transform.position);
		if (this.controllerHand != OVRInput.Controller.LTouch && this.controllerHand != OVRInput.Controller.RTouch)
		{
			return;
		}
		Vector3 position = base.transform.position;
		PassthroughBrush.BrushState brushState = this.brushStatus;
		if (brushState != PassthroughBrush.BrushState.Idle)
		{
			if (brushState != PassthroughBrush.BrushState.Inking)
			{
				return;
			}
			this.UpdateLine(position);
			if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, this.controllerHand))
			{
				this.brushStatus = PassthroughBrush.BrushState.Idle;
			}
		}
		else
		{
			if (OVRInput.GetUp(OVRInput.Button.One, this.controllerHand))
			{
				this.UndoInkLine();
			}
			if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, this.controllerHand))
			{
				this.StartLine(position);
				this.brushStatus = PassthroughBrush.BrushState.Inking;
				return;
			}
		}
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x00072310 File Offset: 0x00070510
	private void StartLine(Vector3 inkPos)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.lineSegmentPrefab, inkPos, Quaternion.identity);
		this.currentLineSegment = gameObject.GetComponent<LineRenderer>();
		this.currentLineSegment.positionCount = 1;
		this.currentLineSegment.SetPosition(0, inkPos);
		this.strokeWidth = this.currentLineSegment.startWidth;
		this.strokeLength = 0f;
		this.inkPositions.Clear();
		this.inkPositions.Add(inkPos);
		gameObject.transform.parent = this.lineContainer.transform;
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x000723A0 File Offset: 0x000705A0
	private void UpdateLine(Vector3 inkPos)
	{
		float magnitude = (inkPos - this.inkPositions[this.inkPositions.Count - 1]).magnitude;
		if (magnitude >= this.minInkDist)
		{
			this.inkPositions.Add(inkPos);
			this.currentLineSegment.positionCount = this.inkPositions.Count;
			this.currentLineSegment.SetPositions(this.inkPositions.ToArray());
			this.strokeLength += magnitude;
			this.currentLineSegment.material.SetFloat("_LineLength", this.strokeLength / this.strokeWidth);
		}
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x00072448 File Offset: 0x00070648
	public void ClearLines()
	{
		for (int i = 0; i < this.lineContainer.transform.childCount; i++)
		{
			Object.Destroy(this.lineContainer.transform.GetChild(i).gameObject);
		}
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x0007248C File Offset: 0x0007068C
	public void UndoInkLine()
	{
		if (this.lineContainer.transform.childCount >= 1)
		{
			Object.Destroy(this.lineContainer.transform.GetChild(this.lineContainer.transform.childCount - 1).gameObject);
		}
	}

	// Token: 0x04001C7A RID: 7290
	public OVRInput.Controller controllerHand;

	// Token: 0x04001C7B RID: 7291
	public GameObject lineSegmentPrefab;

	// Token: 0x04001C7C RID: 7292
	public GameObject lineContainer;

	// Token: 0x04001C7D RID: 7293
	public bool forceActive = true;

	// Token: 0x04001C7E RID: 7294
	private LineRenderer currentLineSegment;

	// Token: 0x04001C7F RID: 7295
	private List<Vector3> inkPositions = new List<Vector3>();

	// Token: 0x04001C80 RID: 7296
	private float minInkDist = 0.01f;

	// Token: 0x04001C81 RID: 7297
	private float strokeWidth = 0.1f;

	// Token: 0x04001C82 RID: 7298
	private float strokeLength;

	// Token: 0x04001C83 RID: 7299
	private PassthroughBrush.BrushState brushStatus;

	// Token: 0x02000375 RID: 885
	public enum BrushState
	{
		// Token: 0x04001C85 RID: 7301
		Idle,
		// Token: 0x04001C86 RID: 7302
		Inking
	}
}
