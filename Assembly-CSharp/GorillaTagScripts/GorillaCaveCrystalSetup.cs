using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000C35 RID: 3125
	[CreateAssetMenu(fileName = "GorillaCaveCrystalSetup", menuName = "ScriptableObjects/GorillaCaveCrystalSetup", order = 0)]
	public class GorillaCaveCrystalSetup : ScriptableObject
	{
		// Token: 0x1700074B RID: 1867
		// (get) Token: 0x06004D28 RID: 19752 RVA: 0x0017FB93 File Offset: 0x0017DD93
		public static GorillaCaveCrystalSetup Instance
		{
			get
			{
				return GorillaCaveCrystalSetup.gInstance;
			}
		}

		// Token: 0x06004D29 RID: 19753 RVA: 0x0017FB9A File Offset: 0x0017DD9A
		private void OnEnable()
		{
			if (GorillaCaveCrystalSetup.gInstance == null)
			{
				GorillaCaveCrystalSetup.gInstance = this;
			}
		}

		// Token: 0x06004D2A RID: 19754 RVA: 0x0017FBB0 File Offset: 0x0017DDB0
		public GorillaCaveCrystalSetup.CrystalDef[] GetCrystalDefs()
		{
			return (from f in typeof(GorillaCaveCrystalSetup).GetRuntimeFields()
			where f != null && f.FieldType == typeof(GorillaCaveCrystalSetup.CrystalDef)
			select (GorillaCaveCrystalSetup.CrystalDef)f.GetValue(this)).ToArray<GorillaCaveCrystalSetup.CrystalDef>();
		}

		// Token: 0x04005624 RID: 22052
		public Material SharedBase;

		// Token: 0x04005625 RID: 22053
		public Texture2D CrystalAlbedo;

		// Token: 0x04005626 RID: 22054
		public Texture2D CrystalDarkAlbedo;

		// Token: 0x04005627 RID: 22055
		public GorillaCaveCrystalSetup.CrystalDef Red;

		// Token: 0x04005628 RID: 22056
		public GorillaCaveCrystalSetup.CrystalDef Orange;

		// Token: 0x04005629 RID: 22057
		public GorillaCaveCrystalSetup.CrystalDef Yellow;

		// Token: 0x0400562A RID: 22058
		public GorillaCaveCrystalSetup.CrystalDef Green;

		// Token: 0x0400562B RID: 22059
		public GorillaCaveCrystalSetup.CrystalDef Teal;

		// Token: 0x0400562C RID: 22060
		public GorillaCaveCrystalSetup.CrystalDef DarkBlue;

		// Token: 0x0400562D RID: 22061
		public GorillaCaveCrystalSetup.CrystalDef Pink;

		// Token: 0x0400562E RID: 22062
		public GorillaCaveCrystalSetup.CrystalDef Dark;

		// Token: 0x0400562F RID: 22063
		public GorillaCaveCrystalSetup.CrystalDef DarkLight;

		// Token: 0x04005630 RID: 22064
		public GorillaCaveCrystalSetup.CrystalDef DarkLightUnderWater;

		// Token: 0x04005631 RID: 22065
		[SerializeField]
		[TextArea(4, 10)]
		private string _notes;

		// Token: 0x04005632 RID: 22066
		[Space]
		[SerializeField]
		private GameObject _target;

		// Token: 0x04005633 RID: 22067
		private static GorillaCaveCrystalSetup gInstance;

		// Token: 0x04005634 RID: 22068
		private static GorillaCaveCrystalSetup.CrystalDef[] gCrystalDefs;

		// Token: 0x02000C36 RID: 3126
		[Serializable]
		public class CrystalDef
		{
			// Token: 0x04005635 RID: 22069
			public Material keyMaterial;

			// Token: 0x04005636 RID: 22070
			public CrystalVisualsPreset visualPreset;

			// Token: 0x04005637 RID: 22071
			[Space]
			public int low;

			// Token: 0x04005638 RID: 22072
			public int mid;

			// Token: 0x04005639 RID: 22073
			public int high;
		}
	}
}
