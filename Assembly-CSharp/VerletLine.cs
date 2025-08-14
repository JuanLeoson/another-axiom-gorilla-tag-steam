using System;
using System.Collections;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020007FC RID: 2044
[DisallowMultipleComponent]
public class VerletLine : MonoBehaviour
{
	// Token: 0x06003320 RID: 13088 RVA: 0x0010A2CC File Offset: 0x001084CC
	private void Awake()
	{
		this._nodes = new VerletLine.LineNode[this.segmentNumber];
		this._positions = new Vector3[this.segmentNumber];
		for (int i = 0; i < this.segmentNumber; i++)
		{
			float t = (float)i / (float)(this.segmentNumber - 1);
			Vector3 vector = Vector3.Lerp(this.lineStart.position, this.lineEnd.position, t);
			this._nodes[i] = new VerletLine.LineNode
			{
				position = vector,
				lastPosition = vector,
				acceleration = this.gravity
			};
		}
		this.line.positionCount = this._nodes.Length;
		this.endRigidbody = this.lineEnd.GetComponent<Rigidbody>();
		if (this.endRigidbody)
		{
			this.endRigidbody.maxLinearVelocity = this.endMaxSpeed;
			this.endRigidbodyParent = this.endRigidbody.transform.parent;
			this.rigidBodyStartingLocalPosition = this.endRigidbody.transform.localPosition;
			this.endRigidbody.transform.parent = null;
			this.endRigidbody.gameObject.SetActive(false);
		}
		this.totalLineLength = this.segmentLength * (float)this.segmentNumber;
	}

	// Token: 0x06003321 RID: 13089 RVA: 0x0010A40C File Offset: 0x0010860C
	private void OnEnable()
	{
		if (this.endRigidbody)
		{
			this.endRigidbody.gameObject.SetActive(true);
			this.endRigidbody.transform.localPosition = this.endRigidbodyParent.TransformPoint(this.rigidBodyStartingLocalPosition);
		}
	}

	// Token: 0x06003322 RID: 13090 RVA: 0x0010A458 File Offset: 0x00108658
	private void OnDisable()
	{
		if (this.endRigidbody)
		{
			this.endRigidbody.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003323 RID: 13091 RVA: 0x0010A478 File Offset: 0x00108678
	public void SetLength(float total, float delay = 0f)
	{
		this.segmentTargetLength = total / (float)this.segmentNumber;
		if (this.segmentTargetLength < this.segmentMinLength)
		{
			this.segmentTargetLength = this.segmentMinLength;
		}
		if (this.segmentTargetLength > this.segmentMaxLength)
		{
			this.segmentTargetLength = this.segmentMaxLength;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06003324 RID: 13092 RVA: 0x0010A4E0 File Offset: 0x001086E0
	public void AddSegmentLength(float amount, float delay = 0f)
	{
		this.segmentTargetLength = this.segmentLength + amount;
		if (this.segmentTargetLength <= 0f)
		{
			return;
		}
		if (this.segmentTargetLength > this.segmentMaxLength)
		{
			this.segmentTargetLength = this.segmentMaxLength;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06003325 RID: 13093 RVA: 0x0010A53C File Offset: 0x0010873C
	public void RemoveSegmentLength(float amount, float delay = 0f)
	{
		this.segmentTargetLength = this.segmentLength - amount;
		if (this.segmentTargetLength <= this.segmentMinLength)
		{
			this.segmentTargetLength = (this.segmentLength = this.segmentMinLength);
			return;
		}
		if (delay >= 0.01f)
		{
			base.StartCoroutine(this.ResizeAfterDelay(delay));
		}
	}

	// Token: 0x06003326 RID: 13094 RVA: 0x0010A591 File Offset: 0x00108791
	private IEnumerator ResizeAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		yield break;
	}

	// Token: 0x06003327 RID: 13095 RVA: 0x0010A5A0 File Offset: 0x001087A0
	private void Update()
	{
		if (this.segmentLength.Approx(this.segmentTargetLength, 0.1f))
		{
			this.segmentLength = this.segmentTargetLength;
			return;
		}
		this.segmentLength = Mathf.Lerp(this.segmentLength, this.segmentTargetLength, this.resizeSpeed * this.resizeScale * Time.deltaTime);
		if (this.scaleLineWidth)
		{
			this.line.widthMultiplier = base.transform.lossyScale.x;
		}
	}

	// Token: 0x06003328 RID: 13096 RVA: 0x0010A620 File Offset: 0x00108820
	public void ForceTotalLength(float totalLength)
	{
		float num = totalLength / (float)((this.segmentNumber < 1) ? 1 : this.segmentNumber);
		this.segmentLength = (this.segmentTargetLength = num);
		this.totalLineLength = this.segmentLength * (float)this.segmentNumber;
	}

	// Token: 0x06003329 RID: 13097 RVA: 0x0010A668 File Offset: 0x00108868
	private void FixedUpdate()
	{
		for (int i = 0; i < this._nodes.Length; i++)
		{
			VerletLine.Simulate(ref this._nodes[i], Time.fixedDeltaTime);
		}
		for (int j = 0; j < this.simIterations; j++)
		{
			for (int k = 0; k < this._nodes.Length - 1; k++)
			{
				VerletLine.LimitDistance(ref this._nodes[k], ref this._nodes[k + 1], this.segmentLength);
			}
		}
		this._nodes[0].position = this.lineStart.position;
		if (this.endRigidbody)
		{
			if (this.onlyPullAtEdges)
			{
				if ((this.endRigidbody.transform.position - this.lineStart.position).IsLongerThan(this.totalLineLength))
				{
					Vector3 a = this.lineStart.position + (this.endRigidbody.transform.position - this.lineStart.position).normalized * this.totalLineLength;
					this.endRigidbody.velocity += (a - this.endRigidbody.transform.position) / Time.fixedDeltaTime;
					if (this.endRigidbody.velocity.IsLongerThan(this.endMaxSpeed))
					{
						this.endRigidbody.velocity = this.endRigidbody.velocity.normalized * this.endMaxSpeed;
					}
				}
			}
			else
			{
				VerletLine.LineNode[] nodes = this._nodes;
				Vector3 force = (nodes[nodes.Length - 1].position - this.lineEnd.position) * (this.tension * this.tensionScale);
				Quaternion rotation = this.endRigidbody.rotation;
				VerletLine.LineNode[] nodes2 = this._nodes;
				Vector3 position = nodes2[nodes2.Length - 1].position;
				VerletLine.LineNode[] nodes3 = this._nodes;
				Quaternion.LookRotation(position - nodes3[nodes3.Length - 2].position);
				if (!this.endRigidbody.isKinematic)
				{
					this.endRigidbody.AddForceAtPosition(force, this.endRigidbody.transform.TransformPoint(this.endLineAnchorLocalPosition));
				}
			}
		}
		VerletLine.LineNode[] nodes4 = this._nodes;
		nodes4[nodes4.Length - 1].position = this.lineEnd.position;
		for (int l = 0; l < this._nodes.Length; l++)
		{
			this._positions[l] = this._nodes[l].position;
		}
		this.line.SetPositions(this._positions);
	}

	// Token: 0x0600332A RID: 13098 RVA: 0x0010A928 File Offset: 0x00108B28
	private static void Simulate(ref VerletLine.LineNode p, float dt)
	{
		Vector3 position = p.position;
		p.position += p.position - p.lastPosition + p.acceleration * (dt * dt);
		p.lastPosition = position;
	}

	// Token: 0x0600332B RID: 13099 RVA: 0x0010A980 File Offset: 0x00108B80
	private static void LimitDistance(ref VerletLine.LineNode p1, ref VerletLine.LineNode p2, float restLength)
	{
		Vector3 a = p2.position - p1.position;
		float num = a.magnitude + 1E-05f;
		float num2 = (num - restLength) / num;
		p1.position += a * (num2 * 0.5f);
		p2.position -= a * (num2 * 0.5f);
	}

	// Token: 0x04004024 RID: 16420
	public Transform lineStart;

	// Token: 0x04004025 RID: 16421
	public Transform lineEnd;

	// Token: 0x04004026 RID: 16422
	[Space]
	public LineRenderer line;

	// Token: 0x04004027 RID: 16423
	public Rigidbody endRigidbody;

	// Token: 0x04004028 RID: 16424
	public Transform endRigidbodyParent;

	// Token: 0x04004029 RID: 16425
	public Vector3 endLineAnchorLocalPosition;

	// Token: 0x0400402A RID: 16426
	private Vector3 rigidBodyStartingLocalPosition;

	// Token: 0x0400402B RID: 16427
	[Space]
	public int segmentNumber = 10;

	// Token: 0x0400402C RID: 16428
	public float segmentLength = 0.03f;

	// Token: 0x0400402D RID: 16429
	public float segmentTargetLength = 0.03f;

	// Token: 0x0400402E RID: 16430
	public float segmentMaxLength = 0.03f;

	// Token: 0x0400402F RID: 16431
	public float segmentMinLength = 0.03f;

	// Token: 0x04004030 RID: 16432
	[Space]
	public Vector3 gravity = new Vector3(0f, -9.81f, 0f);

	// Token: 0x04004031 RID: 16433
	public int simIterations = 6;

	// Token: 0x04004032 RID: 16434
	public float tension = 10f;

	// Token: 0x04004033 RID: 16435
	public float tensionScale = 1f;

	// Token: 0x04004034 RID: 16436
	public float endMaxSpeed = 48f;

	// Token: 0x04004035 RID: 16437
	[FormerlySerializedAs("lerpSpeed")]
	[Space]
	public float resizeSpeed = 1f;

	// Token: 0x04004036 RID: 16438
	public float resizeScale = 1f;

	// Token: 0x04004037 RID: 16439
	[NonSerialized]
	private VerletLine.LineNode[] _nodes = new VerletLine.LineNode[0];

	// Token: 0x04004038 RID: 16440
	[NonSerialized]
	private Vector3[] _positions = new Vector3[0];

	// Token: 0x04004039 RID: 16441
	private float totalLineLength;

	// Token: 0x0400403A RID: 16442
	[SerializeField]
	private bool onlyPullAtEdges;

	// Token: 0x0400403B RID: 16443
	[SerializeField]
	private bool scaleLineWidth = true;

	// Token: 0x020007FD RID: 2045
	[Serializable]
	public struct LineNode
	{
		// Token: 0x0400403C RID: 16444
		public Vector3 position;

		// Token: 0x0400403D RID: 16445
		public Vector3 lastPosition;

		// Token: 0x0400403E RID: 16446
		public Vector3 acceleration;
	}
}
