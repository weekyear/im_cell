using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardWindow : MonoBehaviour
{
	private readonly string END_PLAYER_ID = "CA0E658C24842CA3";
	[SerializeField] GameObject RankItemPrefab;
	[SerializeField] GameObject Content;

	private List<LeaderboardData> leaderboards = new List<LeaderboardData>();
	private bool isLeaderboardEnd;

	public void Show()
	{
		if (!PlayfabManager.Instance.IsLogin)
		{
			ToastManager.Instance.Show("로그인이 필요합니다");
			return;
		}

		foreach (Transform child in Content.transform) Destroy(child.gameObject);
		leaderboards.Clear();

		PlayfabManager.Instance.MoreScores(0, newLeaderboards =>
		{
			UpdateMoreScores(newLeaderboards);
			gameObject.SetActive(true);
		});
	}

	private void UpdateMoreScores(List<LeaderboardData> newLeaderboards)
	{
		foreach (var data in newLeaderboards)
		{
			if (IsEndData(data.Id)) continue;

			leaderboards.Add(data);

			var rankItem = Instantiate(RankItemPrefab);
			rankItem.transform.SetParent(Content.transform, false);

			rankItem.transform.Find("Rank").gameObject.GetComponent<Text>().text = data.Rank.ToString();
			rankItem.transform.Find("Name").gameObject.GetComponent<Text>().text = data.DisplayName;
			rankItem.transform.Find("Score").gameObject.GetComponent<Text>().text = data.Score.ToString("0.00");
		}
	}

	public void OnScrollChanged(Vector2 scroll)
	{
		if (!IsShown) return;

		if (isLeaderboardEnd) return;

		if (scroll.y > -0.01f) return;

		if (PlayfabManager.Instance.IsLoading) return;

		PlayfabManager.Instance.MoreScores(leaderboards.Count, newLeaderboards =>
		{
			if (newLeaderboards.Count < 20) isLeaderboardEnd = true;

			UpdateMoreScores(newLeaderboards);
		});
	}

		public void Hide()
	{
		leaderboards.Clear();
		gameObject.SetActive(false);
	}

	public bool IsShown => gameObject.activeSelf;


	private bool IsEndData(string name) => name == END_PLAYER_ID;
}
