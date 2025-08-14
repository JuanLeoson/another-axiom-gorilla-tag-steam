using System;
using GorillaExtensions;
using Steamworks;
using UnityEngine;

// Token: 0x02000A06 RID: 2566
public class MothershipAuthenticator : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06003EAA RID: 16042 RVA: 0x0013F308 File Offset: 0x0013D508
	public void Awake()
	{
		if (MothershipAuthenticator.Instance == null)
		{
			MothershipAuthenticator.Instance = this;
		}
		else if (MothershipAuthenticator.Instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		if (!MothershipClientApiUnity.IsEnabled())
		{
			Debug.Log("Mothership is not enabled.");
			return;
		}
		if (MothershipAuthenticator.Instance.SteamAuthenticator == null)
		{
			MothershipAuthenticator.Instance.SteamAuthenticator = MothershipAuthenticator.Instance.gameObject.GetOrAddComponent<SteamAuthenticator>();
		}
		MothershipClientApiUnity.SetAuthRefreshedCallback(delegate(string id)
		{
			this.BeginLoginFlow();
		});
	}

	// Token: 0x06003EAB RID: 16043 RVA: 0x0013F39C File Offset: 0x0013D59C
	public void BeginLoginFlow()
	{
		Debug.Log("making login call");
		this.LogInWithSteam();
	}

	// Token: 0x06003EAC RID: 16044 RVA: 0x0013F3AE File Offset: 0x0013D5AE
	private void LogInWithInsecure()
	{
		MothershipClientApiUnity.LogInWithInsecure1(this.TestNickname, this.TestAccountId, delegate(LoginResponse LoginResponse)
		{
			Debug.Log("Logged in with Mothership Id " + LoginResponse.MothershipPlayerId);
			MothershipClientApiUnity.OpenNotificationsSocket();
			Action onLoginSuccess = this.OnLoginSuccess;
			if (onLoginSuccess == null)
			{
				return;
			}
			onLoginSuccess();
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.Log(string.Format("Failed to log in, error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
			{
				MothershipError.Message,
				MothershipError.TraceId,
				errorCode,
				MothershipError.MothershipErrorCode
			}));
			Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure != null)
			{
				onLoginAttemptFailure(1);
			}
			Action onLoginFailure = this.OnLoginFailure;
			if (onLoginFailure == null)
			{
				return;
			}
			onLoginFailure();
		});
	}

	// Token: 0x06003EAD RID: 16045 RVA: 0x0013F3DA File Offset: 0x0013D5DA
	private void LogInWithSteam()
	{
		MothershipClientApiUnity.StartLoginWithSteam(delegate(PlayerSteamBeginLoginResponse resp)
		{
			string nonce = resp.Nonce;
			SteamAuthTicket ticketHandle = HAuthTicket.Invalid;
			Action<LoginResponse> <>9__4;
			Action<MothershipError, int> <>9__5;
			ticketHandle = this.SteamAuthenticator.GetAuthTicketForWebApi(nonce, delegate(string ticket)
			{
				string nonce = nonce;
				Action<LoginResponse> successAction;
				if ((successAction = <>9__4) == null)
				{
					successAction = (<>9__4 = delegate(LoginResponse successResp)
					{
						ticketHandle.Dispose();
						Debug.Log("Logged in to Mothership with Steam");
						MothershipClientApiUnity.OpenNotificationsSocket();
						Action onLoginSuccess = this.OnLoginSuccess;
						if (onLoginSuccess == null)
						{
							return;
						}
						onLoginSuccess();
					});
				}
				Action<MothershipError, int> errorAction;
				if ((errorAction = <>9__5) == null)
				{
					errorAction = (<>9__5 = delegate(MothershipError MothershipError, int errorCode)
					{
						ticketHandle.Dispose();
						Debug.Log(string.Format("Couldn't log into Mothership with Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
						{
							MothershipError.Message,
							MothershipError.TraceId,
							errorCode,
							MothershipError.MothershipErrorCode
						}));
						Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
						if (onLoginAttemptFailure != null)
						{
							onLoginAttemptFailure(1);
						}
						Action onLoginFailure = this.OnLoginFailure;
						if (onLoginFailure == null)
						{
							return;
						}
						onLoginFailure();
					});
				}
				MothershipClientApiUnity.CompleteLoginWithSteam(nonce, ticket, successAction, errorAction);
			}, delegate(EResult error)
			{
				Debug.Log(string.Format("Couldn't get an auth ticket for logging into Mothership with Steam {0}", error));
				Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
				if (onLoginAttemptFailure != null)
				{
					onLoginAttemptFailure(1);
				}
				Action onLoginFailure = this.OnLoginFailure;
				if (onLoginFailure == null)
				{
					return;
				}
				onLoginFailure();
			});
		}, delegate(MothershipError MothershipError, int errorCode)
		{
			Debug.Log(string.Format("Couldn't start Mothership auth for Steam error {0} trace ID: {1} status: {2} Mothership error code: {3}", new object[]
			{
				MothershipError.Message,
				MothershipError.TraceId,
				errorCode,
				MothershipError.MothershipErrorCode
			}));
			Action<int> onLoginAttemptFailure = this.OnLoginAttemptFailure;
			if (onLoginAttemptFailure != null)
			{
				onLoginAttemptFailure(1);
			}
			Action onLoginFailure = this.OnLoginFailure;
			if (onLoginFailure == null)
			{
				return;
			}
			onLoginFailure();
		});
	}

	// Token: 0x06003EAE RID: 16046 RVA: 0x000172AD File Offset: 0x000154AD
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003EAF RID: 16047 RVA: 0x000172B6 File Offset: 0x000154B6
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06003EB0 RID: 16048 RVA: 0x0013F3FA File Offset: 0x0013D5FA
	public void SliceUpdate()
	{
		if (MothershipClientApiUnity.IsEnabled())
		{
			MothershipClientApiUnity.Tick(Time.deltaTime);
		}
	}

	// Token: 0x04004ABB RID: 19131
	public static volatile MothershipAuthenticator Instance;

	// Token: 0x04004ABC RID: 19132
	public MetaAuthenticator MetaAuthenticator;

	// Token: 0x04004ABD RID: 19133
	public SteamAuthenticator SteamAuthenticator;

	// Token: 0x04004ABE RID: 19134
	public string TestNickname;

	// Token: 0x04004ABF RID: 19135
	public string TestAccountId;

	// Token: 0x04004AC0 RID: 19136
	public bool UseConstantTestAccountId = true;

	// Token: 0x04004AC1 RID: 19137
	public int MaxMetaLoginAttempts = 5;

	// Token: 0x04004AC2 RID: 19138
	public Action OnLoginSuccess;

	// Token: 0x04004AC3 RID: 19139
	public Action OnLoginFailure;

	// Token: 0x04004AC4 RID: 19140
	public Action<int> OnLoginAttemptFailure;
}
