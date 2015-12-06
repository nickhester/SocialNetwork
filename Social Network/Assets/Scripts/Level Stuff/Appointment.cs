using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class Appointment : MonoBehaviour {

	[SerializeField] private GameObject textObject;
    [SerializeField] private GameObject myTextObject;
    [SerializeField] private TextMesh myTextComponent;
    [SerializeField] private GameObject mySpecialOverlay_FallToRed;
    [SerializeField] private GameObject mySpecialOverlay_OneClick;
    [SerializeField] private GameObject mySpecialOverlay_CantTouch;
    [SerializeField] private GameObject mySpecialOverlay_NoLines;
    [SerializeField] private GameObject overlay_1Star;
    [SerializeField] private GameObject overlay_2Star;
    [SerializeField] private GameObject overlay_3Star;
    [SerializeField] private GameObject starSlot;

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
			myTextComponent.text = value;
		}
	}

	public void Initialize()
	{
		// Create text on appointment block
		myTextObject = Instantiate(textObject, new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z - 0.1f), Quaternion.identity) as GameObject;
		myTextObject.transform.localScale = myTextObject.transform.localScale * 0.045f;
		myTextObject.transform.parent = gameObject.transform;
		myTextComponent = myTextObject.GetComponent<TextMesh>();
	}

	public void SetMySpecialOverlays()
	{
        Vector3 overlaySpecialPos = new Vector3(transform.position.x + 3.8f, transform.position.y, transform.position.z - 0.5f);
        Vector3 overlaySpecialScale = new Vector3(1, 1, 1);

		if (myLevel.isFallToRed)
		{ mySpecialOverlay_FallToRed = InstantiateAndPositionOverlay(mySpecialOverlay_FallToRed, overlaySpecialPos, overlaySpecialScale); }
		if (myLevel.isOneClick)
		{ mySpecialOverlay_OneClick = InstantiateAndPositionOverlay(mySpecialOverlay_OneClick, overlaySpecialPos, overlaySpecialScale); }
		if (myLevel.isCantTouch)
		{ mySpecialOverlay_CantTouch = InstantiateAndPositionOverlay(mySpecialOverlay_CantTouch, overlaySpecialPos, overlaySpecialScale); }
		if (myLevel.isNoLines)
		{ mySpecialOverlay_NoLines = InstantiateAndPositionOverlay(mySpecialOverlay_NoLines, overlaySpecialPos, overlaySpecialScale); }
        
		UpdateStarCount();
	}

	public void UpdateStarCount()
	{
		Vector3 overlayStarPos = new Vector3(transform.position.x + -2.9f, transform.position.y + 0.3f, transform.position.z - 0.1f);
		Vector3 overlayStarScale = new Vector3(2.0f, 2.0f, 1.0f);

		if (starSlot != null) { Destroy(starSlot); }

        int thisAppointmentStarCount = SaveGame.GetRoundStarCount(GetMyDayIndex(), GetMyLevelIndex());
		if (thisAppointmentStarCount == 3)
			starSlot = InstantiateAndPositionOverlay(overlay_3Star, overlayStarPos, overlayStarScale);
		else if (thisAppointmentStarCount == 2)
			starSlot = InstantiateAndPositionOverlay(overlay_2Star, overlayStarPos, overlayStarScale);
		else if (thisAppointmentStarCount == 1)
			starSlot = InstantiateAndPositionOverlay(overlay_1Star, overlayStarPos, overlayStarScale);
	}

    GameObject InstantiateAndPositionOverlay(GameObject _overlay, Vector3 _overlayPos, Vector3 _overlayScale)
	{

        GameObject returnGO = Instantiate(_overlay, _overlayPos, Quaternion.identity) as GameObject;

        returnGO.transform.localScale = _overlayScale;
		returnGO.transform.parent = transform;

		return returnGO;
	}

    public int GetMyDayIndex()
    {
        return GameObject.FindObjectOfType<CalendarDay>().dayIndex_internal;
    }

    public int GetMyLevelIndex()
    {
        return levelIndex;
    }
}
