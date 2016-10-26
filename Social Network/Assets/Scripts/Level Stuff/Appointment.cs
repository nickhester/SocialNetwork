using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;
using UnityEngine.UI;

public class Appointment : MonoBehaviour
{
	[SerializeField] private Text textDisplay;
	private Vector3 specialOverlayOffset = new Vector3(0.294f, -0.075f, 0.0f);
	private float specialOverlayScalar = 0.321f;
    [SerializeField] private GameObject mySpecialOverlay_FallToRed;
    [SerializeField] private GameObject mySpecialOverlay_OneClick;
    [SerializeField] private GameObject mySpecialOverlay_CantTouch;
    [SerializeField] private GameObject mySpecialOverlay_NoLines;
	private Vector3 starOverlayOffset = new Vector3(-0.294f, -0.071f, -0.5f);
	private float starOverlayScale = 0.35f;
    [SerializeField] private GameObject overlay_0Star;
    [SerializeField] private GameObject overlay_1Star;
	[SerializeField] private GameObject overlay_2Star;
    [SerializeField] private GameObject overlay_3Star;
    private GameObject starSlot;

	public ValidLevels myLevel;
	public int levelIndex;

	private string myDisplayText;
	public string myDisplayText_prop
	{
		get
		{
			return this.myDisplayText;
		}
		set
		{
			this.myDisplayText = value;
			textDisplay.text = value;
		}
	}

	public void SetMySpecialOverlays()
	{
		if (myLevel.isFallToRed)
		{
			mySpecialOverlay_FallToRed = InstantiateAndPositionOverlay(mySpecialOverlay_FallToRed, specialOverlayOffset, specialOverlayScalar);
		}
		if (myLevel.isOneClick)
		{
			mySpecialOverlay_OneClick = InstantiateAndPositionOverlay(mySpecialOverlay_OneClick, specialOverlayOffset, specialOverlayScalar);
		}
		if (myLevel.isCantTouch)
		{
			mySpecialOverlay_CantTouch = InstantiateAndPositionOverlay(mySpecialOverlay_CantTouch, specialOverlayOffset, specialOverlayScalar);
		}
		if (myLevel.isNoLines)
		{
			mySpecialOverlay_NoLines = InstantiateAndPositionOverlay(mySpecialOverlay_NoLines, specialOverlayOffset, specialOverlayScalar);
		}
        
		UpdateStarCount(true);
	}

	public void UpdateStarCount(bool _isOverlayVisible)
	{
		if (starSlot != null)
		{
			Destroy(starSlot);
		}

        int thisAppointmentStarCount = SaveGame.GetRoundStarCount(GetMyDayIndex(), GetMyLevelIndex());

		GameObject overlayStar = null;
		switch (thisAppointmentStarCount)
		{
			case 0:
			{
				overlayStar = overlay_0Star;
				break;
			}
			case 1:
			{
				overlayStar = overlay_1Star;
				break;
			}
			case 2:
			{
				overlayStar = overlay_2Star;
				break;
			}
			case 3:
			{
				overlayStar = overlay_3Star;
				break;
			}
			default:
			{
				break;
			}
		}
		starSlot = InstantiateAndPositionOverlay(overlayStar, starOverlayOffset, starOverlayScale);
		return;
	}

    GameObject InstantiateAndPositionOverlay(GameObject _overlay, Vector3 _overlayPos, float _overlayScale)
	{
        GameObject returnGO = Instantiate(_overlay, Vector3.zero, Quaternion.identity) as GameObject;

        returnGO.transform.SetParent(transform);
		returnGO.transform.localScale = returnGO.transform.localScale * _overlayScale;
		returnGO.transform.localPosition = _overlayPos;

		return returnGO;
	}

    public int GetMyDayIndex()
    {
		return GameObject.FindObjectOfType<LevelSelector>().dayToGenerate.dayIndex_internal;
    }

    public int GetMyLevelIndex()
    {
        return levelIndex;
    }
}
