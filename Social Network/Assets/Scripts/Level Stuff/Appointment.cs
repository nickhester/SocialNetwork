using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class Appointment : MonoBehaviour {

	[SerializeField] private GameObject textObject;
	private Vector3 textObjectOffset = new Vector3(0.0f, -0.1f, -0.1f);
	private float textObjectScalar = 0.036f;
    [SerializeField] private GameObject myTextObject;
    [SerializeField] private TextMesh myTextComponent;
	private Vector3 specialOverlayOffset = new Vector3(0.25f, -0.4f, 0.0f);
    [SerializeField] private GameObject mySpecialOverlay_FallToRed;
    [SerializeField] private GameObject mySpecialOverlay_OneClick;
    [SerializeField] private GameObject mySpecialOverlay_CantTouch;
    [SerializeField] private GameObject mySpecialOverlay_NoLines;
	private Vector3 starOverlayOffset = new Vector3(-0.294f, -0.071f, -0.5f);
	private Vector3 starOverlayScale = new Vector3(0.053f, 0.321f, 1.0f);
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
		myTextObject = Instantiate(textObject, (transform.position + textObjectOffset), Quaternion.identity) as GameObject;
		myTextObject.transform.localScale = myTextObject.transform.localScale * textObjectScalar;
		myTextObject.transform.parent = gameObject.transform;
		myTextComponent = myTextObject.GetComponent<TextMesh>();
	}

	public void SetMySpecialOverlays()
	{
		Vector3 overlaySpecialPos = specialOverlayOffset;
		Vector3 overlaySpecialScale = Vector3.zero;

		if (myLevel.isFallToRed)
		{
			mySpecialOverlay_FallToRed = InstantiateAndPositionOverlay(mySpecialOverlay_FallToRed, overlaySpecialPos, overlaySpecialScale);
		}
		if (myLevel.isOneClick)
		{
			mySpecialOverlay_OneClick = InstantiateAndPositionOverlay(mySpecialOverlay_OneClick, overlaySpecialPos, overlaySpecialScale);
		}
		if (myLevel.isCantTouch)
		{
			mySpecialOverlay_CantTouch = InstantiateAndPositionOverlay(mySpecialOverlay_CantTouch, overlaySpecialPos, overlaySpecialScale);
		}
		if (myLevel.isNoLines)
		{
			mySpecialOverlay_NoLines = InstantiateAndPositionOverlay(mySpecialOverlay_NoLines, overlaySpecialPos, overlaySpecialScale);
		}
        
		UpdateStarCount(true);
	}

	public void UpdateStarCount(bool _isOverlayVisible)
	{
		if (starSlot != null)
		{
			Destroy(starSlot);
			_isOverlayVisible = starSlot.GetComponent<MeshRenderer>().enabled;
		}

        int thisAppointmentStarCount = SaveGame.GetRoundStarCount(GetMyDayIndex(), GetMyLevelIndex());
		if (thisAppointmentStarCount == 3)
		{
			starSlot = InstantiateAndPositionOverlay(overlay_3Star, starOverlayOffset, starOverlayScale);
		}
		else if (thisAppointmentStarCount == 2)
		{
			starSlot = InstantiateAndPositionOverlay(overlay_2Star, starOverlayOffset, starOverlayScale);
		}
		else if (thisAppointmentStarCount == 1)
		{
			starSlot = InstantiateAndPositionOverlay(overlay_1Star, starOverlayOffset, starOverlayScale);
		}
		else
		{
			return;
		}

		starSlot.GetComponent<MeshRenderer>().enabled = _isOverlayVisible;
	}

    GameObject InstantiateAndPositionOverlay(GameObject _overlay, Vector3 _overlayPos, Vector3 _overlayScale)
	{
        GameObject returnGO = Instantiate(_overlay, Vector3.zero, Quaternion.identity) as GameObject;

        returnGO.transform.SetParent(transform);
		if (_overlayScale != Vector3.zero)
		{
			returnGO.transform.localScale = _overlayScale;
		}
		returnGO.transform.localPosition = _overlayPos;

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
