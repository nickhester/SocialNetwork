using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Types;

public class Appointment : MonoBehaviour {

	public GameObject textObject;
	public GameObject myTextObject;
	public TextMesh myTextComponent;
	public Vector3 myLerpTarget;
	public bool isLerping = false;
	public GameObject mySpecialOverlay_FallToRed;
	public GameObject mySpecialOverlay_OneClick;
	public GameObject mySpecialOverlay_CantTouch;
	public GameObject mySpecialOverlay_NoLines;
    public GameObject overlay_1Star;
    public GameObject overlay_2Star;
    public GameObject overlay_3Star;
	public GameObject starSlot;

	public validLevels myLevel;
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
	
	void Start ()
	{
			myLerpTarget = transform.position;
	}

	void Update () { 
		if (isLerping) { transform.position = Vector3.Lerp(transform.position, myLerpTarget, 0.1f); }
		// end lerp early
		if (Vector3.Distance(transform.position, myLerpTarget) < 0.1f) { transform.position = myLerpTarget; }
	}

	public void Initialize()
	{
		// Create text on appointment block
		myTextObject = Instantiate(textObject, transform.position, Quaternion.identity) as GameObject;
        myTextObject.transform.localScale = myTextObject.transform.localScale * 0.6f;
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
		Vector3 overlayStarPos = new Vector3(transform.position.x + -2.9f, transform.position.y + 0.3f, transform.position.z);
		Vector3 overlayStarScale = new Vector3(2.0f, 2.0f, 1.0f);

		if (starSlot != null) { Destroy(starSlot); }

		int thisAppointmentStarCount = SaveGame.GetRoundStarCount(GameObject.FindObjectOfType<CalendarDay>().dayIndex_internal, levelIndex);
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
}
