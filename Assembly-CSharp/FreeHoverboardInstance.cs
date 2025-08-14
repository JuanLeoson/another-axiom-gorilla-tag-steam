using System;
using UnityEngine;

// Token: 0x02000752 RID: 1874
public class FreeHoverboardInstance : MonoBehaviour
{
	// Token: 0x1700044D RID: 1101
	// (get) Token: 0x06002EF4 RID: 12020 RVA: 0x000F8BA4 File Offset: 0x000F6DA4
	// (set) Token: 0x06002EF5 RID: 12021 RVA: 0x000F8BAC File Offset: 0x000F6DAC
	public Rigidbody Rigidbody { get; private set; }

	// Token: 0x1700044E RID: 1102
	// (get) Token: 0x06002EF6 RID: 12022 RVA: 0x000F8BB5 File Offset: 0x000F6DB5
	// (set) Token: 0x06002EF7 RID: 12023 RVA: 0x000F8BBD File Offset: 0x000F6DBD
	public Color boardColor { get; private set; }

	// Token: 0x06002EF8 RID: 12024 RVA: 0x000F8BC8 File Offset: 0x000F6DC8
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		Material[] sharedMaterials = this.boardMesh.sharedMaterials;
		this.colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = this.colorMaterial;
		this.boardMesh.sharedMaterials = sharedMaterials;
	}

	// Token: 0x06002EF9 RID: 12025 RVA: 0x000F8C10 File Offset: 0x000F6E10
	public void SetColor(Color col)
	{
		this.colorMaterial.color = col;
		this.boardColor = col;
	}

	// Token: 0x06002EFA RID: 12026 RVA: 0x000F8C28 File Offset: 0x000F6E28
	private void Update()
	{
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(base.transform.TransformPoint(this.sphereCastCenter), base.transform.TransformVector(Vector3.down)), this.sphereCastRadius, out raycastHit, 1f, this.hoverRaycastMask.value))
		{
			this.hasHoverPoint = true;
			this.hoverPoint = raycastHit.point;
			this.hoverNormal = raycastHit.normal;
			return;
		}
		this.hasHoverPoint = false;
	}

	// Token: 0x06002EFB RID: 12027 RVA: 0x000F8CA4 File Offset: 0x000F6EA4
	private void FixedUpdate()
	{
		if (this.hasHoverPoint)
		{
			float num = Vector3.Dot(base.transform.TransformPoint(this.sphereCastCenter) - this.hoverPoint, this.hoverNormal);
			if (num < this.hoverHeight)
			{
				base.transform.position += this.hoverNormal * (this.hoverHeight - num);
				this.Rigidbody.velocity = Vector3.ProjectOnPlane(this.Rigidbody.velocity, this.hoverNormal);
				Vector3 point = Quaternion.Inverse(base.transform.rotation) * this.Rigidbody.angularVelocity;
				point.x *= this.avelocityDragWhileHovering;
				point.z *= this.avelocityDragWhileHovering;
				this.Rigidbody.angularVelocity = base.transform.rotation * point;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, this.hoverNormal), this.hoverNormal), this.hoverRotationLerp);
			}
		}
	}

	// Token: 0x04003AF8 RID: 15096
	public int ownerActorNumber;

	// Token: 0x04003AF9 RID: 15097
	public int boardIndex;

	// Token: 0x04003AFA RID: 15098
	[SerializeField]
	private Vector3 sphereCastCenter;

	// Token: 0x04003AFB RID: 15099
	[SerializeField]
	private float sphereCastRadius;

	// Token: 0x04003AFC RID: 15100
	[SerializeField]
	private LayerMask hoverRaycastMask;

	// Token: 0x04003AFD RID: 15101
	[SerializeField]
	private float hoverHeight;

	// Token: 0x04003AFE RID: 15102
	[SerializeField]
	private float hoverRotationLerp;

	// Token: 0x04003AFF RID: 15103
	[SerializeField]
	private float avelocityDragWhileHovering;

	// Token: 0x04003B00 RID: 15104
	[SerializeField]
	private MeshRenderer boardMesh;

	// Token: 0x04003B02 RID: 15106
	private Material colorMaterial;

	// Token: 0x04003B03 RID: 15107
	private bool hasHoverPoint;

	// Token: 0x04003B04 RID: 15108
	private Vector3 hoverPoint;

	// Token: 0x04003B05 RID: 15109
	private Vector3 hoverNormal;
}
