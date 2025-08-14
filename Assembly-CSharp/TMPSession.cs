using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KID.Model;
using UnityEngine;

// Token: 0x020008C1 RID: 2241
public class TMPSession
{
	// Token: 0x1700056D RID: 1389
	// (get) Token: 0x060037ED RID: 14317 RVA: 0x001209C9 File Offset: 0x0011EBC9
	public bool IsValidSession
	{
		get
		{
			return (this.IsDefault && this.Permissions != null && this.Permissions.Count > 0) || (!this.IsDefault && this.SessionId != Guid.Empty);
		}
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x00120A08 File Offset: 0x0011EC08
	public TMPSession(Session session, KIDDefaultSession defaultSession, int? age, SessionStatus status)
	{
		this.Permissions = new Dictionary<EKIDFeatures, Permission>();
		this.OptedInPermissions = new HashSet<EKIDFeatures>();
		this.Age = age.GetValueOrDefault();
		this.SessionStatus = status;
		if (session == null && defaultSession == null)
		{
			return;
		}
		if (session == null)
		{
			this.IsDefault = true;
			this.AgeStatus = defaultSession.AgeStatus;
			this.InitialiseDefaultPermissionSet(defaultSession);
			return;
		}
		this.SessionId = session.SessionId;
		this.Etag = session.Etag;
		this.AgeStatus = session.AgeStatus;
		this.KidStatus = session.Status;
		this.DateOfBirth = session.DateOfBirth;
		this.KUID = session.Kuid;
		this.Jurisdiction = session.Jurisdiction;
		this.ManagedBy = session.ManagedBy;
		this.Age = this.GetAgeFromDateOfBirth();
		for (int i = 0; i < session.Permissions.Count; i++)
		{
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(session.Permissions[i].Name);
			if (ekidfeatures != null && !this.Permissions.TryAdd(ekidfeatures.Value, session.Permissions[i]))
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but permission for [" + ekidfeatures.Value.ToStandardisedString() + "] already exists");
			}
		}
	}

	// Token: 0x060037EF RID: 14319 RVA: 0x00120B50 File Offset: 0x0011ED50
	public void SetOptInPermissions(string[] optedInPermissions)
	{
		if (optedInPermissions == null || optedInPermissions.Length == 0)
		{
			Debug.LogWarning("[KID::SESSION] OptedInPermissions is null or empty. Returning without setting.");
			return;
		}
		int num = 0;
		for (;;)
		{
			int num2 = num;
			int? num3 = (optedInPermissions != null) ? new int?(optedInPermissions.Length) : null;
			if (!(num2 < num3.GetValueOrDefault() & num3 != null))
			{
				break;
			}
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(optedInPermissions[num]);
			if (ekidfeatures != null)
			{
				this.OptInToPermission(ekidfeatures.Value, true);
			}
			num++;
		}
		Debug.Log(string.Format("[KID::SESSION::OptInRefactor] Constructor OptedInPermissions: {0}", this.GetOptedInPermissions()));
	}

	// Token: 0x060037F0 RID: 14320 RVA: 0x00120BD7 File Offset: 0x0011EDD7
	public bool TryGetPermission(EKIDFeatures feature, out Permission permission)
	{
		if (!this.Permissions.ContainsKey(feature))
		{
			Debug.LogError("[KID::SESSION] Tried retreiving permission for [" + feature.ToStandardisedString() + "], but does not exist");
			permission = null;
			return false;
		}
		permission = this.Permissions[feature];
		return true;
	}

	// Token: 0x060037F1 RID: 14321 RVA: 0x00120C15 File Offset: 0x0011EE15
	public List<Permission> GetAllPermissions()
	{
		return this.Permissions.Values.ToList<Permission>();
	}

	// Token: 0x060037F2 RID: 14322 RVA: 0x00120C28 File Offset: 0x0011EE28
	public bool HasPermissionForFeature(EKIDFeatures feature)
	{
		Permission permission;
		if (!this.TryGetPermission(feature, out permission))
		{
			Debug.LogError("[KID::SESSION] Tried checking for permission but couldn't find [" + feature.ToStandardisedString() + "]. Assuming disabled");
			return false;
		}
		return permission.Enabled;
	}

	// Token: 0x060037F3 RID: 14323 RVA: 0x00120C64 File Offset: 0x0011EE64
	public void OptInToPermission(EKIDFeatures feature, bool optIn)
	{
		Debug.Log(string.Format("[KID::SESSION::OptInRefactor] Opting in to permission for [{0}] with optIn: {1}", feature.ToStandardisedString(), optIn));
		if (optIn && !this.OptedInPermissions.Contains(feature))
		{
			this.OptedInPermissions.Add(feature);
			return;
		}
		if (!optIn && this.OptedInPermissions.Contains(feature))
		{
			this.OptedInPermissions.Remove(feature);
			return;
		}
	}

	// Token: 0x060037F4 RID: 14324 RVA: 0x00120CCA File Offset: 0x0011EECA
	public bool HasOptedInToPermission(EKIDFeatures feature)
	{
		return this.OptedInPermissions.Contains(feature);
	}

	// Token: 0x060037F5 RID: 14325 RVA: 0x00120CD8 File Offset: 0x0011EED8
	public string[] GetOptedInPermissions()
	{
		if (this.OptedInPermissions == null || this.OptedInPermissions.Count == 0)
		{
			Debug.LogWarning("[KID::SESSION] OptedInPermissions is null or empty. Returning empty array.");
			return Array.Empty<string>();
		}
		return (from f in this.OptedInPermissions
		select f.ToStandardisedString()).ToArray<string>();
	}

	// Token: 0x060037F6 RID: 14326 RVA: 0x00120D3C File Offset: 0x0011EF3C
	public void UpdatePermission(EKIDFeatures feature, Permission newData)
	{
		if (!this.Permissions.ContainsKey(feature))
		{
			Debug.Log("[KID::SESSION] Trying to update permission, but could not find [" + feature.ToStandardisedString() + "] in dictionary. Will add new one");
			this.Permissions.Add(feature, null);
		}
		this.Permissions[feature] = newData;
	}

	// Token: 0x060037F7 RID: 14327 RVA: 0x00120D8C File Offset: 0x0011EF8C
	private void InitialiseDefaultPermissionSet(KIDDefaultSession defaultSession)
	{
		for (int i = 0; i < defaultSession.Permissions.Count; i++)
		{
			EKIDFeatures? ekidfeatures = KIDFeaturesExtensions.FromString(defaultSession.Permissions[i].Name);
			if (ekidfeatures == null)
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but failed to cast from [" + defaultSession.Permissions[i].Name + "] to [EKIDFeatures] enum");
			}
			else if (!this.Permissions.TryAdd(ekidfeatures.Value, defaultSession.Permissions[i]))
			{
				Debug.LogError("[KID::SESSION] Tried creating new session, but permission for [" + ekidfeatures.Value.ToStandardisedString() + "] already exists");
			}
		}
	}

	// Token: 0x060037F8 RID: 14328 RVA: 0x00120E3C File Offset: 0x0011F03C
	private int GetAgeFromDateOfBirth()
	{
		DateTime today = DateTime.Today;
		int num = today.Year - this.DateOfBirth.Year;
		int num2 = today.Month - this.DateOfBirth.Month;
		if (num2 < 0)
		{
			num--;
		}
		else if (num2 == 0 && today.Day - this.DateOfBirth.Day < 0)
		{
			num--;
		}
		return num;
	}

	// Token: 0x060037F9 RID: 14329 RVA: 0x00120EA0 File Offset: 0x0011F0A0
	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("New TMPSession]:");
		stringBuilder.AppendLine(string.Format("    - Is Default    :   {0}", this.IsDefault));
		stringBuilder.AppendLine(string.Format("    - Is Valid      :   {0}", this.IsValidSession));
		stringBuilder.AppendLine(string.Format("    - SessionID     :   {0}", this.SessionId));
		stringBuilder.AppendLine(string.Format("    - Age           :   {0}", this.Age));
		stringBuilder.AppendLine(string.Format("    - AgeStatus     :   {0}", this.AgeStatus));
		stringBuilder.AppendLine(string.Format("    - SessionStatus :   {0}", this.KidStatus));
		stringBuilder.AppendLine("    - DoB           :   " + this.DateOfBirth.ToString());
		stringBuilder.AppendLine("    - KUID          :   " + this.KUID);
		stringBuilder.AppendLine("    - Jurisdiction  :   " + this.Jurisdiction);
		stringBuilder.AppendLine("    - PERMISSIONS   :");
		if (this.Permissions != null)
		{
			foreach (Permission permission in this.Permissions.Values)
			{
				stringBuilder.AppendLine(string.Format("        - {0} - Enabled: {1} - ManagedBy: {2}", permission.Name, permission.Enabled, permission.ManagedBy));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x040044A7 RID: 17575
	public readonly Guid SessionId;

	// Token: 0x040044A8 RID: 17576
	public readonly string Etag;

	// Token: 0x040044A9 RID: 17577
	public readonly AgeStatusType AgeStatus;

	// Token: 0x040044AA RID: 17578
	public readonly Session.StatusEnum KidStatus;

	// Token: 0x040044AB RID: 17579
	public readonly Session.ManagedByEnum ManagedBy;

	// Token: 0x040044AC RID: 17580
	public readonly DateTime DateOfBirth;

	// Token: 0x040044AD RID: 17581
	public readonly string Jurisdiction;

	// Token: 0x040044AE RID: 17582
	public readonly string KUID;

	// Token: 0x040044AF RID: 17583
	public readonly int Age;

	// Token: 0x040044B0 RID: 17584
	public readonly bool IsDefault;

	// Token: 0x040044B1 RID: 17585
	public readonly SessionStatus SessionStatus;

	// Token: 0x040044B2 RID: 17586
	private Dictionary<EKIDFeatures, Permission> Permissions;

	// Token: 0x040044B3 RID: 17587
	private HashSet<EKIDFeatures> OptedInPermissions;
}
