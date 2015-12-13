using UnityEngine;
using System.Collections;

public class GameServiceMock : GameService
{

	public void Initialize() { }

	public void ReportStarCount(int starCount) { }

	public void ReportWeekCompleted(int weekCompleted) { }

	public void ReportWeekPerfected(int weekPerfected) { }

	public void ReportGameCompleted() { }

	public void ReportGamePerfected() { }

	public void SendReport(string code) { }

	public bool GetIsPlayingOffline()
	{
		return true;
	}

	public void SetIsPlayingOffline(bool _isPlayingOffline) { }

	public void LeaveRating() { }
}
