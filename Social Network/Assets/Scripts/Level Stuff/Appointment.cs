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
	private Vector3 specialOverlayOffset = new Vector3(0.294f, -0.075f, 0.0f);
	private float specialOverlayScalar = 0.321f;
    [SerializeField] private GameObject mySpecialOverlay_FallToRed;
    [SerializeField] private GameObject mySpecialOverlay_OneClick;
    [SerializeField] private GameObject mySpecialOverlay_CantTouch;
    [SerializeField] private GameObject mySpecialOverlay_NoLines;
	private Vector3 starOverlayOffset = new Vector3(-0.294f, -0.071f, -0.5f);
	private float starOverlayScale = 0.35f;
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
        return GameObject.FindObjectOfType<CalendarDay>().dayIndex_internal;
    }

    public int GetMyLevelIndex()
    {
        return levelIndex;
    }
}
