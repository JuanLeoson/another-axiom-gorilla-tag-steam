using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Text;
using GorillaLocomotion;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200009E RID: 158
public class EyeScannerMono : MonoBehaviour, ISpawnable, IGorillaSliceableSimple
{
	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060003E4 RID: 996 RVA: 0x00017646 File Offset: 0x00015846
	// (set) Token: 0x060003E5 RID: 997 RVA: 0x00017650 File Offset: 0x00015850
	private Color32 KeyTextColor
	{
		get
		{
			return this.m_keyTextColor;
		}
		set
		{
			this.m_keyTextColor = value;
			this._keyRichTextColorTagString = string.Format(CultureInfo.InvariantCulture.NumberFormat, "<color=#{0:X2}{1:X2}{2:X2}>", value.r, value.g, value.b);
		}
	}

	// Token: 0x17000043 RID: 67
	// (get) Token: 0x060003E6 RID: 998 RVA: 0x0001769F File Offset: 0x0001589F
	private List<IEyeScannable> registeredScannables
	{
		get
		{
			return EyeScannerMono._registeredScannables;
		}
	}

	// Token: 0x060003E7 RID: 999 RVA: 0x000176A6 File Offset: 0x000158A6
	public static void Register(IEyeScannable scannable)
	{
		if (EyeScannerMono._registeredScannableIds.Add(scannable.scannableId))
		{
			EyeScannerMono._registeredScannables.Add(scannable);
		}
	}

	// Token: 0x060003E8 RID: 1000 RVA: 0x000176C5 File Offset: 0x000158C5
	public static void Unregister(IEyeScannable scannable)
	{
		if (EyeScannerMono._registeredScannableIds.Remove(scannable.scannableId))
		{
			EyeScannerMono._registeredScannables.Remove(scannable);
		}
	}

	// Token: 0x060003E9 RID: 1001 RVA: 0x000176E8 File Offset: 0x000158E8
	protected void Awake()
	{
		this._sb = ZString.CreateStringBuilder();
		this.KeyTextColor = this.KeyTextColor;
		math.sign(this.m_textTyper.transform.parent.localScale);
		this.m_textTyper.SetText(string.Empty);
		this.m_reticle.gameObject.SetActive(false);
		this.m_textTyper.gameObject.SetActive(false);
		this.m_overlayBg.SetActive(false);
		this._line = base.GetComponent<LineRenderer>();
		this._line.enabled = false;
	}

	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060003EA RID: 1002 RVA: 0x00017782 File Offset: 0x00015982
	// (set) Token: 0x060003EB RID: 1003 RVA: 0x0001778A File Offset: 0x0001598A
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060003EC RID: 1004 RVA: 0x00017793 File Offset: 0x00015993
	// (set) Token: 0x060003ED RID: 1005 RVA: 0x0001779B File Offset: 0x0001599B
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060003EE RID: 1006 RVA: 0x000177A4 File Offset: 0x000159A4
	// (set) Token: 0x060003EF RID: 1007 RVA: 0x000177AC File Offset: 0x000159AC
	public string DebugData { get; private set; }

	// Token: 0x060003F0 RID: 1008 RVA: 0x000177B8 File Offset: 0x000159B8
	public void OnSpawn(VRRig rig)
	{
		if (rig != null && !rig.isOfflineVRRig)
		{
			Object.Destroy(base.gameObject);
		}
		if (GTPlayer.hasInstance)
		{
			GTPlayer instance = GTPlayer.Instance;
			this._firstPersonCamera = instance.GetComponentInChildren<Camera>();
			this._has_firstPersonCamera = (this._firstPersonCamera != null);
		}
	}

	// Token: 0x060003F1 RID: 1009 RVA: 0x000023F5 File Offset: 0x000005F5
	public void OnDespawn()
	{
	}

	// Token: 0x060003F2 RID: 1010 RVA: 0x00010F6F File Offset: 0x0000F16F
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060003F3 RID: 1011 RVA: 0x00010F78 File Offset: 0x0000F178
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x060003F4 RID: 1012 RVA: 0x0001780C File Offset: 0x00015A0C
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (GorillaTagger.Instance.offlineVRRig.zoneEntity.currentZone == GTZone.bayou)
		{
			if (this._oldClosestScannable != null)
			{
				this._OnScannableChanged(null, false);
				this._oldClosestScannable = null;
			}
			return;
		}
		IEyeScannable eyeScannable = null;
		Transform transform = base.transform;
		Vector3 position = transform.position;
		Vector3 forward = transform.forward;
		float num = this.m_LookPrecision;
		for (int i = 0; i < EyeScannerMono._registeredScannables.Count; i++)
		{
			IEyeScannable eyeScannable2 = EyeScannerMono._registeredScannables[i];
			Vector3 normalized = (eyeScannable2.Position - position).normalized;
			float num2 = Vector3.Distance(position, eyeScannable2.Position);
			float num3 = Vector3.Dot(forward, normalized);
			if (num2 >= this.m_scanDistanceMin && num2 <= this.m_scanDistanceMax && num3 > num)
			{
				RaycastHit raycastHit;
				if (!this.m_xrayVision && Physics.Raycast(position, normalized, out raycastHit, this.m_scanDistanceMax, this._layerMask.value))
				{
					IEyeScannable componentInParent = raycastHit.collider.GetComponentInParent<IEyeScannable>();
					if (componentInParent == null || componentInParent != eyeScannable2)
					{
						goto IL_EF;
					}
				}
				num = num3;
				eyeScannable = eyeScannable2;
			}
			IL_EF:;
		}
		if (eyeScannable != this._oldClosestScannable)
		{
			if (this._oldClosestScannable != null)
			{
				this._oldClosestScannable.OnDataChange -= this.Scannable_OnDataChange;
			}
			this._OnScannableChanged(eyeScannable, true);
			this._oldClosestScannable = eyeScannable;
			if (this._oldClosestScannable != null)
			{
				this._oldClosestScannable.OnDataChange += this.Scannable_OnDataChange;
			}
		}
	}

	// Token: 0x060003F5 RID: 1013 RVA: 0x00017975 File Offset: 0x00015B75
	private void Scannable_OnDataChange()
	{
		this._OnScannableChanged(this._oldClosestScannable, false);
	}

	// Token: 0x060003F6 RID: 1014 RVA: 0x00017984 File Offset: 0x00015B84
	private void LateUpdate()
	{
		if (this._oldClosestScannable != null)
		{
			this.m_reticle.position = this._oldClosestScannable.Position;
			float num = math.distance(base.transform.position, this.m_reticle.position);
			Mathf.Clamp(num * 0.33333f, 0f, 1f);
			float num2 = num * this.m_reticleScale;
			float d = num * this.m_textScale;
			float num3 = num * this.m_overlayScale;
			this.m_reticle.localScale = new Vector3(num2, num2, num2);
			this.m_overlay.localPosition = new Vector3(this.m_position.x * num, this.m_position.y * num, num);
			this.m_overlay.localScale = new Vector3(num3, num3, 1f);
			this._line.SetPosition(0, this.m_reticle.position);
			this._line.SetPosition(1, this.m_textTyper.transform.position + this.m_pointerOffset * d);
			this._line.widthMultiplier = num2;
		}
	}

	// Token: 0x060003F7 RID: 1015 RVA: 0x00017AB0 File Offset: 0x00015CB0
	private void _OnScannableChanged(IEyeScannable scannable, bool typeingShow)
	{
		this._sb.Clear();
		if (scannable == null)
		{
			this.m_textTyper.SetText(this._sb);
			this.m_textTyper.gameObject.SetActive(false);
			this.m_reticle.gameObject.SetActive(false);
			this.m_overlayBg.SetActive(false);
			this.m_reticle.parent = base.transform;
			this._line.enabled = false;
			return;
		}
		this.m_reticle.gameObject.SetActive(true);
		this.m_textTyper.gameObject.SetActive(true);
		this.m_overlayBg.SetActive(true);
		this.m_reticle.position = scannable.Position;
		this._line.enabled = true;
		this._sb.AppendLine(this.DebugData);
		this._entryIndexes[0] = 0;
		int i = 1;
		int num = 0;
		for (int j = 0; j < scannable.Entries.Count; j++)
		{
			KeyValueStringPair keyValueStringPair = scannable.Entries[j];
			if (!string.IsNullOrEmpty(keyValueStringPair.Key))
			{
				this._sb.Append(this._keyRichTextColorTagString);
				this._sb.Append(keyValueStringPair.Key);
				this._sb.Append("</color>: ");
				num += keyValueStringPair.Key.Length + 2;
			}
			if (!string.IsNullOrEmpty(keyValueStringPair.Value))
			{
				this._sb.Append(keyValueStringPair.Value);
				num += keyValueStringPair.Value.Length;
			}
			this._sb.AppendLine();
			num += Environment.NewLine.Length;
			if (i < this._entryIndexes.Length)
			{
				this._entryIndexes[i++] = num - 1;
			}
		}
		while (i < this._entryIndexes.Length)
		{
			this._entryIndexes[i] = -1;
			i++;
		}
		if (typeingShow)
		{
			this.m_textTyper.SetText(this._sb, this._entryIndexes, num);
			return;
		}
		this.m_textTyper.UpdateText(this._sb, num);
	}

	// Token: 0x04000461 RID: 1121
	[FormerlySerializedAs("_scanDistance")]
	[Tooltip("Any scannables with transforms beyond this distance will be automatically ignored.")]
	[SerializeField]
	private float m_scanDistanceMax = 10f;

	// Token: 0x04000462 RID: 1122
	[SerializeField]
	private float m_scanDistanceMin = 0.5f;

	// Token: 0x04000463 RID: 1123
	[FormerlySerializedAs("_textTyper")]
	[Tooltip("The component that handles setting text in the TextMeshPro and animates the text typing.")]
	[SerializeField]
	private TextTyperAnimatorMono m_textTyper;

	// Token: 0x04000464 RID: 1124
	[SerializeField]
	private Transform m_reticle;

	// Token: 0x04000465 RID: 1125
	[SerializeField]
	private Transform m_overlay;

	// Token: 0x04000466 RID: 1126
	[SerializeField]
	private GameObject m_overlayBg;

	// Token: 0x04000467 RID: 1127
	[SerializeField]
	private float m_reticleScale = 1f;

	// Token: 0x04000468 RID: 1128
	[SerializeField]
	private float m_textScale = 1f;

	// Token: 0x04000469 RID: 1129
	[SerializeField]
	private float m_overlayScale = 1f;

	// Token: 0x0400046A RID: 1130
	[SerializeField]
	private Vector3 m_pointerOffset;

	// Token: 0x0400046B RID: 1131
	[SerializeField]
	private Vector2 m_position;

	// Token: 0x0400046C RID: 1132
	[HideInInspector]
	[SerializeField]
	private Color32 m_keyTextColor = new Color32(byte.MaxValue, 34, 0, byte.MaxValue);

	// Token: 0x0400046D RID: 1133
	private string _keyRichTextColorTagString = "";

	// Token: 0x0400046E RID: 1134
	private static readonly List<IEyeScannable> _registeredScannables = new List<IEyeScannable>(128);

	// Token: 0x0400046F RID: 1135
	private static readonly HashSet<int> _registeredScannableIds = new HashSet<int>(128);

	// Token: 0x04000470 RID: 1136
	private IEyeScannable _oldClosestScannable;

	// Token: 0x04000471 RID: 1137
	private Utf16ValueStringBuilder _sb;

	// Token: 0x04000472 RID: 1138
	private readonly int[] _entryIndexes = new int[16];

	// Token: 0x04000473 RID: 1139
	[SerializeField]
	private LayerMask _layerMask;

	// Token: 0x04000474 RID: 1140
	private Camera _firstPersonCamera;

	// Token: 0x04000475 RID: 1141
	private bool _has_firstPersonCamera;

	// Token: 0x04000479 RID: 1145
	[SerializeField]
	private float m_LookPrecision = 0.65f;

	// Token: 0x0400047A RID: 1146
	[SerializeField]
	private bool m_xrayVision;

	// Token: 0x0400047B RID: 1147
	private LineRenderer _line;
}
