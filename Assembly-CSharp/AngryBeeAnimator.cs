using System;
using UnityEngine;

// Token: 0x02000101 RID: 257
public class AngryBeeAnimator : MonoBehaviour
{
	// Token: 0x06000662 RID: 1634 RVA: 0x00024B40 File Offset: 0x00022D40
	private void Awake()
	{
		this.bees = new GameObject[this.numBees];
		this.beeOrbits = new GameObject[this.numBees];
		this.beeOrbitalRadii = new float[this.numBees];
		this.beeOrbitalAxes = new Vector3[this.numBees];
		for (int i = 0; i < this.numBees; i++)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = base.transform;
			Vector2 vector = Random.insideUnitCircle * this.orbitMaxCenterDisplacement;
			gameObject.transform.localPosition = new Vector3(vector.x, Random.Range(-this.orbitMaxHeightDisplacement, this.orbitMaxHeightDisplacement), vector.y);
			gameObject.transform.localRotation = Quaternion.Euler(Random.Range(-this.orbitMaxTilt, this.orbitMaxTilt), (float)Random.Range(0, 360), 0f);
			this.beeOrbitalAxes[i] = gameObject.transform.up;
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.beePrefab, gameObject.transform);
			float num = Random.Range(this.orbitMinRadius, this.orbitMaxRadius);
			this.beeOrbitalRadii[i] = num;
			gameObject2.transform.localPosition = Vector3.forward * num;
			gameObject2.transform.localRotation = Quaternion.Euler(-90f, 90f, 0f);
			gameObject2.transform.localScale = Vector3.one * this.beeScale;
			this.bees[i] = gameObject2;
			this.beeOrbits[i] = gameObject;
		}
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00024CDC File Offset: 0x00022EDC
	private void Update()
	{
		float angle = this.orbitSpeed * Time.deltaTime;
		for (int i = 0; i < this.numBees; i++)
		{
			this.beeOrbits[i].transform.Rotate(this.beeOrbitalAxes[i], angle);
		}
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00024D28 File Offset: 0x00022F28
	public void SetEmergeFraction(float fraction)
	{
		for (int i = 0; i < this.numBees; i++)
		{
			this.bees[i].transform.localPosition = Vector3.forward * fraction * this.beeOrbitalRadii[i];
		}
	}

	// Token: 0x040007AF RID: 1967
	[SerializeField]
	private GameObject beePrefab;

	// Token: 0x040007B0 RID: 1968
	[SerializeField]
	private int numBees;

	// Token: 0x040007B1 RID: 1969
	[SerializeField]
	private float orbitMinRadius;

	// Token: 0x040007B2 RID: 1970
	[SerializeField]
	private float orbitMaxRadius;

	// Token: 0x040007B3 RID: 1971
	[SerializeField]
	private float orbitMaxHeightDisplacement;

	// Token: 0x040007B4 RID: 1972
	[SerializeField]
	private float orbitMaxCenterDisplacement;

	// Token: 0x040007B5 RID: 1973
	[SerializeField]
	private float orbitMaxTilt;

	// Token: 0x040007B6 RID: 1974
	[SerializeField]
	private float orbitSpeed;

	// Token: 0x040007B7 RID: 1975
	[SerializeField]
	private float beeScale;

	// Token: 0x040007B8 RID: 1976
	private GameObject[] beeOrbits;

	// Token: 0x040007B9 RID: 1977
	private GameObject[] bees;

	// Token: 0x040007BA RID: 1978
	private Vector3[] beeOrbitalAxes;

	// Token: 0x040007BB RID: 1979
	private float[] beeOrbitalRadii;
}
