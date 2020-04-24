using GooglePlayGames;
using GooglePlayGames.BasicApi;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;

public class PlayfabManager : MonoBehaviour, IStoreListener
{
	public static PlayfabManager Instance { get; private set; }

	internal bool IsLoading;

	public string Id { get; private set; }
	public string UserName { get; private set; }
	public LeaderboardData MyBest;
	public Dictionary<string, LeaderboardData> Total = new Dictionary<string, LeaderboardData>();
	private IStoreController storeController;
	private Dictionary<string, string> UserData;

	public bool IsLogin => !string.IsNullOrEmpty(Id);

	public static event Action OnNewUser;
	public static event Action InventoryUpdated;
	public static event Action UserDataUpdated;
	public static event Action WorldBestUpdated;

	private void Awake()
	{
		if (Instance)
		{
			Destroy(gameObject);
			return;
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

#if UNITY_ANDROID
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			.AddOauthScope("profile")
			.RequestServerAuthCode(false)
			.Build();
		PlayGamesPlatform.InitializeInstance(config);
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate();
#endif
	}

	public void Login()
	{
#if UNITY_ANDROID
		IsLoading = true;
		Social.localUser.Authenticate((bool success) =>
				{
					if (success)
					{
						var authCode = PlayGamesPlatform.Instance.GetServerAuthCode();
						var request = new LoginWithGoogleAccountRequest
						{
							ServerAuthCode = authCode,
							CreateAccount = true
						};

						PlayFabClientAPI.LoginWithGoogleAccount(request, OnLoginSuccess, error => ToastManager.Instance.Show("로그인에 실패했습니다."));
					}
					else
					{
						IsLoading = false;
						ToastManager.Instance.Show("구글 로그인에 실패했습니다.");
					}
				});
#else
#endif
	}

	public void PCLogin(string id, Action onSuccess = null)
	{
		var request = new LoginWithCustomIDRequest { CustomId = id };
		PlayFabClientAPI.LoginWithCustomID(request, loginResult =>
		{
			PlayerPrefs.SetString("TesterId", id);
			OnLoginSuccess(loginResult);
			onSuccess?.Invoke();
		}, error => ToastManager.Instance.Show(error.ErrorMessage));
	}

	public void CreateUser(string name, Action onSuccess = null)
	{
		if (!CheckName(name))
		{
			ToastManager.Instance.Show("이름은 3글자 이상이어야 합니다.");
			return;
		}

		PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest { TitleDisplayName = name },
			(result) => ToastManager.Instance.Show("이미 존재하는 별명입니다."),
			(error1) =>
			{
				PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest { DisplayName = name },
					(result) =>
					{
						UserName = name;
						ToastManager.Instance.Show("계정 생성에 성공했습니다.");
						GetUserData();
						onSuccess?.Invoke();
					},
					error2 => ToastManager.Instance.Show("계정 생성에 실패했습니다."));
			});
	}

	private void OnLoginSuccess(LoginResult loginResult)
	{
		IsLoading = false;
		Id = loginResult.PlayFabId;

		if (loginResult.NewlyCreated)
		{
			OnNewUser?.Invoke();
		}
		else
		{
			IsLoading = true;
			PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest { PlayFabId = Id },
				(profileResult) =>
				{
					UserName = profileResult.PlayerProfile.DisplayName;

					if (string.IsNullOrEmpty(UserName)) OnNewUser?.Invoke();
					else GetUserData();
				},
				(error) => { });
		}
	}

	private void GetUserData()
	{
		InitializePurchasing();

		PlayFabClientAPI.GetUserData(new GetUserDataRequest { PlayFabId = Id },
			result => {
				UserData = result.Data.ToDictionary(v=> v.Key, v => v.Value.Value);
				UserDataUpdated?.Invoke();
			},
			error => ToastManager.Instance.Show("유저 정보를 불러오는데 실패했습니다."));

		GetMyRecord();
		GetTotal();
	}

	public void GetMyRecord()
	{
		IsLoading = true;
		PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest { StatisticName = "record", PlayFabId = Id, MaxResultsCount = 1 },
			(result) =>
			{
				IsLoading = false;
				PlayerLeaderboardEntry entry = result.Leaderboard.FirstOrDefault();

				if (entry == null) return;

				MyBest = new LeaderboardData(entry);
				UserDataUpdated?.Invoke();
			},
			error =>
			{
				IsLoading = false;
				ToastManager.Instance.Show("내 기록을 불러오는데 실패했습니다.");
			});
	}

	public void GetTotal()
	{
		IsLoading = true;
		PlayFabClientAPI.GetLeaderboardAroundPlayer(new GetLeaderboardAroundPlayerRequest { StatisticName = "record", PlayFabId = "DDCB130D4AD0F5A7", MaxResultsCount = 1 },
			(result) =>
			{
				PlayerLeaderboardEntry entry = result.Leaderboard.FirstOrDefault();

				if (entry == null) return;

				Total[name] = new LeaderboardData(entry);
				UserDataUpdated?.Invoke();
				IsLoading = false;
			},
			error =>
			{
				IsLoading = false;
				ToastManager.Instance.Show("내 기록을 불러오는데 실패했습니다.");
			});
	}

	public void MoreScores(int start, Action<List<LeaderboardData>> callback)
	{
		if (IsLoading) return;

		IsLoading = true;
		PlayFabClientAPI.GetLeaderboard(new GetLeaderboardRequest { StatisticName = "record", StartPosition = start, MaxResultsCount = 10 },
			result =>
			{
				callback(result.Leaderboard.Select(v => new LeaderboardData(v)).ToList());
				IsLoading = false;
			},
			error =>
			{
				ToastManager.Instance.Show("리더보드를 불러오는데 실패했습니다.");
				IsLoading = false;
			});
	}

	public void ReportScore(float score)
	{
		IsLoading = true;
		PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest { Statistics = new List<StatisticUpdate> { new StatisticUpdate { StatisticName = "record", Value = -(int) (score * 100) } } },
			result =>
			{
				IsLoading = false;
			},
			error =>
			{
				IsLoading = false;
				switch (error.Error)
				{
					case PlayFabErrorCode.AccountNotFound:
						ToastManager.Instance?.Show("계정 로그인이 제대로 되지 않았습니다. 재로그인해주세요");
						break;

					case PlayFabErrorCode.APINotEnabledForGameClientAccess:
						ToastManager.Instance?.Show("서버에서 접근을 허용하지 않았습니다. 관리자에게 문의해주세요");
						break;

					case PlayFabErrorCode.DuplicateStatisticName:
						ToastManager.Instance?.Show("이미 존재하는 데이터입니다. 관리자에게 문의해주세요");
						break;

					case PlayFabErrorCode.StatisticCountLimitExceeded:
						ToastManager.Instance?.Show("데이터 한도를 넘었습니다. 관리자에게 문의해주세요");
						break;

					case PlayFabErrorCode.StatisticNameConflict:
						ToastManager.Instance?.Show("데이터 이름이 충돌합니다. 관리자에게 문의해주세요");
						break;

					default:
						ToastManager.Instance?.Show("알 수 없는 오류가 발생했습니다. 관리자에게 문의해주세요");
						break;
				}
			});
	}

	public void SaveStage(int level)
	{
		string stringLevel = level.ToString();
		string chestString = string.Join(", ", GameManager.IsOpenChestList.ToArray());
		PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { Data = new Dictionary<string, string> { { "level", stringLevel }, { "chest", chestString } } },
			result =>
			{
				UserData["level"] = stringLevel;
				UserData["chest"] = chestString;
				UserDataUpdated?.Invoke();

			}, null);
	}

	private bool CheckName(string name) => !string.IsNullOrEmpty(name) && name.Length >= 3;

	private void InitializePurchasing()
	{
		if (storeController != null) return;

		PlayFabClientAPI.GetStoreItems(new GetStoreItemsRequest { StoreId = "InApp" }, result =>
		{
			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance(AppStore.GooglePlay));
			foreach (var item in result.Store) builder.AddProduct(item.ItemId, ProductType.NonConsumable);
			UnityPurchasing.Initialize(this, builder);
		}, error => Debug.LogError(error.GenerateErrorReport()));
	}

	public void InAppPurchase(string id)
	{
		if (!IsLogin)
		{
			ToastManager.Instance.Show("로그인이 필요합니다.");
			return;
		}

		if (storeController == null)
		{
			ToastManager.Instance.Show("인앱 결제가 실패했습니다. 인앱 결제를 하려면 앱을 재시작해주세요");
			return;
		}

		storeController.InitiatePurchase(id);
	}

	public bool NoAd
	{
		get
		{
			if (UserData == null || !UserData.ContainsKey("remove_ads")) return false;

			return bool.Parse(UserData["remove_ads"]);
		}
	}

	public int Level
	{
		get
		{
			if (UserData == null || !UserData.ContainsKey("level")) return 1;

			return int.Parse(UserData["level"]);
		}
	}

	public List<bool> ChestList
	{
		get
		{
			if (UserData == null || !UserData.ContainsKey("chest")) return new List<bool>(new bool[11]);

			return UserData["chest"].Split(',').Select(v => bool.Parse(v)).ToList();
		}
	}

	#region STORE
	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		storeController = controller;
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.LogError(error);
		ToastManager.Instance.Show("인앱 구매가 불가능합니다. 인앱 구매를 하시려면 재시작해주세요");
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		if (e.purchasedProduct == null) return PurchaseProcessingResult.Complete;

		if (string.IsNullOrEmpty(e.purchasedProduct.receipt)) return PurchaseProcessingResult.Complete;

		var googleReceipt = GooglePurchase.FromJson(e.purchasedProduct.receipt);

		PlayFabClientAPI.ValidateGooglePlayPurchase(new ValidateGooglePlayPurchaseRequest()
		{
			CurrencyCode = e.purchasedProduct.metadata.isoCurrencyCode,
			PurchasePrice = (uint)(e.purchasedProduct.metadata.localizedPrice * 100),
			ReceiptJson = googleReceipt.PayloadData.json,
			Signature = googleReceipt.PayloadData.signature
		}, result =>
		{
			switch (e.purchasedProduct.definition.storeSpecificId)
			{
				case "remove_ads":
					PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest { Data = new Dictionary<string, string> { { "remove_ads", true.ToString() } } },
						result2 => {
							ToastManager.Instance.Show("광고 제거를 구매했습니다.");
							UserData["remove_ads"] = true.ToString();
							UserDataUpdated?.Invoke();
						},
						error => ToastManager.Instance.Show("구매 도중 오류가 발생했습니다. 반드시 개발자에게 문의해주세요!"));
					break;
			}
		}, error => ToastManager.Instance.Show("비정상적인 구매요청입니다."));

		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		switch (p)
		{
			case PurchaseFailureReason.PurchasingUnavailable:
				ToastManager.Instance.Show("구매가 불가능합니다");
				break;

			case PurchaseFailureReason.ExistingPurchasePending:
				ToastManager.Instance.Show("이미 구매한 물품입니다");
				break;

			case PurchaseFailureReason.ProductUnavailable:
				ToastManager.Instance.Show("구매가 불가능한 아이템입니다");
				break;

			case PurchaseFailureReason.SignatureInvalid:
				ToastManager.Instance.Show("서명에 실패했습니다");
				break;

			case PurchaseFailureReason.UserCancelled:
				ToastManager.Instance.Show("구매를 취소했습니다");
				break;

			case PurchaseFailureReason.PaymentDeclined:
				ToastManager.Instance.Show("결제가 거절되었습니다");
				break;

			case PurchaseFailureReason.DuplicateTransaction:
				ToastManager.Instance.Show("중복된 요청입니다");
				break;

			default:
				ToastManager.Instance.Show("알 수 없는 오류가 발생했습니다");
				break;
		}
	}
	#endregion
}

#region SUBCLASS
public class JsonData
{
	// JSON Fields, ! Case-sensitive
	public string orderId;
	public string packageName;
	public string productId;
	public long purchaseTime;
	public int purchaseState;
	public string purchaseToken;
}

public class PayloadData
{
	public JsonData JsonData;

	// JSON Fields, ! Case-sensitive
	public string signature;

	public string json;

	public static PayloadData FromJson(string json)
	{
		var payload = JsonUtility.FromJson<PayloadData>(json);
		payload.JsonData = JsonUtility.FromJson<JsonData>(payload.json);
		return payload;
	}
}

public class GooglePurchase
{
	public PayloadData PayloadData;

	// JSON Fields, ! Case-sensitive
	public string Store;

	public string TransactionID;
	public string Payload;

	public static GooglePurchase FromJson(string json)
	{
		var purchase = JsonUtility.FromJson<GooglePurchase>(json);
		purchase.PayloadData = PayloadData.FromJson(purchase.Payload);
		return purchase;
	}
}
#endregion