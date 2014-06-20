using UnityEngine;
using System.Collections;

namespace Types
{
	public enum Difficulty
	{
		VeryEasy,
		Easy,
		Medium,
		Hard,
		NoSolutionFound,
		Impossible,
		Unknown
	}

	public enum DayOfTheWeek
	{
		Monday,
		Tuesday,
		Wednesday,
		Thursday,
		Friday
	}

	public enum SpecialLevelAttributes
	{
		FallToRed,
		OneClick,
		CantTouch,
		NoLines,
		None
	}

	public enum Mood
	{
		Negative,
		Neutral,
		Positive
	}

	public enum Friendship
	{
		Negative,
		Neutral,
		Positive
	}
}
