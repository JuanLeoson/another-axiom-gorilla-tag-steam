using System;
using UnityEngine;

// Token: 0x020005E8 RID: 1512
public class GhostReactorTelemetry : MonoBehaviour
{
	// Token: 0x17000397 RID: 919
	// (get) Token: 0x06002532 RID: 9522 RVA: 0x000C8428 File Offset: 0x000C6628
	public static string GameVersionCustomTag
	{
		get
		{
			return "game_version_" + Application.version;
		}
	}

	// Token: 0x17000398 RID: 920
	// (get) Token: 0x06002533 RID: 9523 RVA: 0x000C8439 File Offset: 0x000C6639
	public static string GameEnvironment
	{
		get
		{
			return "game_environment_live";
		}
	}

	// Token: 0x04002EF1 RID: 12017
	public const string SHIFT_START_EVENT_NAME = "ghost_game_start";

	// Token: 0x04002EF2 RID: 12018
	public const string SHIFT_END_EVENT_NAME = "ghost_game_end";

	// Token: 0x04002EF3 RID: 12019
	private const string GAME_VERSION_CUSTOM_TAG_PREFIX = "game_version_";

	// Token: 0x04002EF4 RID: 12020
	private const string METRIC_ACTION_CUSTOM_TAG_PREFIX = "metric_action_";

	// Token: 0x04002EF5 RID: 12021
	public const string GHOST_GAME_ID_BODY_DATA = "ghost_game_id";

	// Token: 0x04002EF6 RID: 12022
	public const string EVENT_TIMESTAMP_BODY_DATA = "event_timestamp";

	// Token: 0x04002EF7 RID: 12023
	public const string INITIAL_CORES_BALANCE_BODY_DATA = "initial_cores_balance";

	// Token: 0x04002EF8 RID: 12024
	public const string FINAL_CORES_BALANCE_BODY_DATA = "final_cores_balance";

	// Token: 0x04002EF9 RID: 12025
	public const string CORES_SPENT_WAITING_IN_BREAKROOM_BODY_DATA = "cores_spent_waiting_in_breakroom";

	// Token: 0x04002EFA RID: 12026
	public const string CORES_COLLECTED_FROM_GHOSTS_BODY_DATA = "cores_collected_from_ghosts";

	// Token: 0x04002EFB RID: 12027
	public const string CORES_COLLECTED_FROM_GATHERING_BODY_DATA = "cores_collected_from_gathering";

	// Token: 0x04002EFC RID: 12028
	public const string CORES_SPENT_ON_ITEMS_BODY_DATA = "cores_spent_on_items";

	// Token: 0x04002EFD RID: 12029
	public const string CORES_SPENT_ON_GATES_BODY_DATA = "cores_spent_on_gates";

	// Token: 0x04002EFE RID: 12030
	public const string CORES_SPENT_ON_LEVELS_BODY_DATA = "cores_spent_on_levels";

	// Token: 0x04002EFF RID: 12031
	public const string CORES_GIVEN_TO_OTHERS_BODY_DATA = "cores_given_to_others";

	// Token: 0x04002F00 RID: 12032
	public const string CORES_RECEIVED_FROM_OTHERS_BODY_DATA = "cores_received_from_others";

	// Token: 0x04002F01 RID: 12033
	public const string SHIFT_CUT_DATA = "shift_cut_data";

	// Token: 0x04002F02 RID: 12034
	public const string GATES_UNLOCKED_BODY_DATA = "gates_unlocked";

	// Token: 0x04002F03 RID: 12035
	public const string DIED_BODY_DATA = "died";

	// Token: 0x04002F04 RID: 12036
	public const string CAUGHT_IN_ANAMOLE_BODY_DATA = "caught_in_anamole";

	// Token: 0x04002F05 RID: 12037
	public const string ITEMS_PURCHASED_BODY_DATA = "items_purchased";

	// Token: 0x04002F06 RID: 12038
	public const string LEVELS_UNLOCKED_BODY_DATA = "levels_unlocked";

	// Token: 0x04002F07 RID: 12039
	public const string NUMBER_OF_PLAYERS_BODY_DATA = "number_of_players";

	// Token: 0x04002F08 RID: 12040
	public const string START_AT_BEGINNING_BODY_DATA = "start_at_beginning";

	// Token: 0x04002F09 RID: 12041
	public const string SECONDS_INTO_SHIFT_AT_JOIN_BODY_DATA = "seconds_into_shift_at_join";

	// Token: 0x04002F0A RID: 12042
	public const string REASON_BODY_DATA = "reason";

	// Token: 0x04002F0B RID: 12043
	public const string PLAY_DURATION_BODY_DATA = "play_duration";

	// Token: 0x04002F0C RID: 12044
	public const string STARTED_LATE_BODY_DATA = "started_late";

	// Token: 0x04002F0D RID: 12045
	public const string TIME_STARTED_BODY_DATA = "time_started";

	// Token: 0x04002F0E RID: 12046
	public const string CORES_COLLECTED_BODY_DATA = "cores_collected";

	// Token: 0x04002F0F RID: 12047
	public const string MAX_NUMBER_IN_GAME_BODY_DATA = "max_number_in_game";

	// Token: 0x04002F10 RID: 12048
	public const string END_NUMBER_IN_GAME_BODY_DATA = "end_number_in_game";

	// Token: 0x04002F11 RID: 12049
	public const string ITEMS_PICKED_UP_BODY_DATA = "items_picked_up";
}
