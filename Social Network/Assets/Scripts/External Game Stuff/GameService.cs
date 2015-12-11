using UnityEngine;
using System.Collections;

public interface GameService
{
	void Initialize();

	void ReportStarCount(int starCount);

	void ReportWeekCompleted(int weekCompleted);

	void ReportWeekPerfected(int weekPerfected);

	void ReportGameCompleted();

	void ReportGamePerfected();

	void SendReport(string code);

	bool GetIsPlayingOffline();

	void SetIsPlayingOffline(bool _isPlayingOffline);
}
