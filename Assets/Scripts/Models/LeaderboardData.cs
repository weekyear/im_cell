using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayFab.ClientModels;

public class LeaderboardData
{
	public string Id;
	public string DisplayName;
	public int Rank;
	public float Score;

	public LeaderboardData(PlayerLeaderboardEntry entry)
	{
		Id = entry.PlayFabId;
		DisplayName = entry.DisplayName;
		Rank = entry.Position + 1;
		Score = -(float)entry.StatValue / 100f;
	}
}