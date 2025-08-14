using System;
using UnityEngine;

// Token: 0x02000901 RID: 2305
public class KIDTelemetry
{
	// Token: 0x17000584 RID: 1412
	// (get) Token: 0x060038F8 RID: 14584 RVA: 0x000C8428 File Offset: 0x000C6628
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x17000585 RID: 1413
	// (get) Token: 0x060038F9 RID: 14585 RVA: 0x001278D2 File Offset: 0x00125AD2
	public static string Open_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Open";
		}
	}

	// Token: 0x17000586 RID: 1414
	// (get) Token: 0x060038FA RID: 14586 RVA: 0x001278D9 File Offset: 0x00125AD9
	public static string Updated_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Updated";
		}
	}

	// Token: 0x17000587 RID: 1415
	// (get) Token: 0x060038FB RID: 14587 RVA: 0x001278E0 File Offset: 0x00125AE0
	public static string Closed_MetricActionCustomTag
	{
		get
		{
			return "metric_action_Closed";
		}
	}

	// Token: 0x17000588 RID: 1416
	// (get) Token: 0x060038FC RID: 14588 RVA: 0x000C8439 File Offset: 0x000C6639
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x060038FD RID: 14589 RVA: 0x001278E7 File Offset: 0x00125AE7
	public static string GetPermissionManagedByBodyData(string permission)
	{
		return "permission_managedby_" + permission.Replace('-', '_');
	}

	// Token: 0x060038FE RID: 14590 RVA: 0x001278FD File Offset: 0x00125AFD
	public static string GetPermissionEnabledBodyData(string permission)
	{
		return "permission_eneabled_" + permission.Replace('-', '_');
	}

	// Token: 0x040045FF RID: 17919
	public const string SCREEN_SHOWN_EVENT_NAME = "kid_screen_shown";

	// Token: 0x04004600 RID: 17920
	public const string PHASE_TWO_IN_COHORT_EVENT_NAME = "kid_phase2_incohort";

	// Token: 0x04004601 RID: 17921
	public const string PHASE_THREE_OPTIONAL_EVENT_NAME = "kid_phase3_optional";

	// Token: 0x04004602 RID: 17922
	public const string AGE_GATE_EVENT_NAME = "kid_age_gate";

	// Token: 0x04004603 RID: 17923
	public const string AGE_GATE_CONFIRM_EVENT_NAME = "kid_age_gate_confirm";

	// Token: 0x04004604 RID: 17924
	public const string AGE_DISCREPENCY_EVENT_NAME = "kid_age_gate_discrepency";

	// Token: 0x04004605 RID: 17925
	public const string GAME_SETTINGS_EVENT_NAME = "kid_game_settings";

	// Token: 0x04004606 RID: 17926
	public const string EMAIL_CONFIRM_EVENT_NAME = "kid_email_confirm";

	// Token: 0x04004607 RID: 17927
	public const string AGE_APPEAL_EVENT_NAME = "kid_age_appeal";

	// Token: 0x04004608 RID: 17928
	public const string APPEAL_AGE_GATE_EVENT_NAME = "kid_age_appeal_age_gate";

	// Token: 0x04004609 RID: 17929
	public const string APPEAL_ENTER_EMAIL_EVENT_NAME = "kid_age_appeal_enter_email";

	// Token: 0x0400460A RID: 17930
	public const string APPEAL_CONFIRM_EMAIL_EVENT_NAME = "kid_age_appeal_confirm_email";

	// Token: 0x0400460B RID: 17931
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x0400460C RID: 17932
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x0400460D RID: 17933
	public const string WARNING_SCREEN_CUSTOM_TAG = "kid_warning_screen";

	// Token: 0x0400460E RID: 17934
	public const string PHASE_TWO = "kid_phase_2";

	// Token: 0x0400460F RID: 17935
	public const string PHASE_THREE = "kid_phase_3";

	// Token: 0x04004610 RID: 17936
	public const string PHASE_FOUR = "kid_phase_4";

	// Token: 0x04004611 RID: 17937
	public const string AGE_GATE_CUSTOM_TAG = "kid_age_gate";

	// Token: 0x04004612 RID: 17938
	public const string SETTINGS_CUSTOM_TAG = "kid_settings";

	// Token: 0x04004613 RID: 17939
	public const string SETUP_CUSTOM_TAG = "kid_setup";

	// Token: 0x04004614 RID: 17940
	public const string APPEAL_CUSTOM_TAG = "kid_age_appeal";

	// Token: 0x04004615 RID: 17941
	public const string SCREEN_TYPE_BODY_DATA = "screen";

	// Token: 0x04004616 RID: 17942
	public const string OPT_IN_CHOICE_BODY_DATA = "opt_in_choice";

	// Token: 0x04004617 RID: 17943
	public const string BUTTON_PRESSED_BODY_DATA = "button_pressed";

	// Token: 0x04004618 RID: 17944
	public const string MISMATCH_EXPECTED_BODY_DATA = "mismatch_expected";

	// Token: 0x04004619 RID: 17945
	public const string MISMATCH_ACTUAL_BODY_DATA = "mismatch_actual";

	// Token: 0x0400461A RID: 17946
	public const string AGE_DECLARED_BODY_DATA = "age_declared";

	// Token: 0x0400461B RID: 17947
	public const string LEARN_MORE_URL_PRESSED_BODY_DATA = "learn_more_url_pressed";

	// Token: 0x0400461C RID: 17948
	public const string SCREEN_SHOWN_REASON_BODY_DATA = "screen_shown_reason";

	// Token: 0x0400461D RID: 17949
	public const string SUBMITTED_AGE_BODY_DATA = "submitted_age";

	// Token: 0x0400461E RID: 17950
	public const string CORRECT_AGE_BODY_DATA = "correct_age";

	// Token: 0x0400461F RID: 17951
	public const string APPEAL_EMAIL_TYPE_BODY_DATA = "email_type";

	// Token: 0x04004620 RID: 17952
	public const string SHOWN_SETTINGS_SCREEN = "saw_game_settings";

	// Token: 0x04004621 RID: 17953
	public const string KID_STATUS_BODY_DATA = "kid_status";

	// Token: 0x04004622 RID: 17954
	private const string PERMISSION_MANAGED_BY_BODY_DATA = "permission_managedby_";

	// Token: 0x04004623 RID: 17955
	private const string PERMISSION_ENABLED_BODY_DATA = "permission_eneabled_";
}
