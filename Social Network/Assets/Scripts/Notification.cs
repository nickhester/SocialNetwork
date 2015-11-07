using UnityEngine;
using System.Collections;

public class Notification : MonoBehaviour
{
    [SerializeField] private GameObject quadPrefab;
    [SerializeField] private GameObject modalBackgroundPrefab;
    private GameObject modalBackgroundObject;
    private GameObject activeNotification;
    public Texture tempTexture;
    private bool currentNotificationIsModal;
    private string currentNotificationName;
    private Vector3 currentNotificationOriginalScale;
    private float currentPulseCounter = 0.0f;
    [SerializeField] private float pulseSpeed;
    [SerializeField] private float pulseAmount;
	private float scaleFactor = 0.9f;

	// buttons
	[SerializeField] private GameObject buttonPrefab_UpgradeUnlock;
	[SerializeField] private GameObject buttonPrefab_Yes;
	[SerializeField] private GameObject buttonPrefab_No;
	[SerializeField] private GameObject buttonPrefab_KeepPlaying;
	[SerializeField] private GameObject buttonPrefab_Cancel;
	[SerializeField] private GameObject buttonPrefab_RateIt;
	private Vector3 buttonPosition_unlock = new Vector3(0.0f, -0.5f);
	private Vector3 buttonPosition_cancel = new Vector3(0.0f, -4.0f);
	private Vector3 buttonPosition_yes = new Vector3(-2.4f, -3.2f);
	private Vector3 buttonPosition_no = new Vector3(2.4f, -3.2f);
	private Vector3 buttonPosition_keepPlaying = new Vector3(2.4f, -3.2f);


    void Awake()
    {
        InputManager.Instance.OnClick += OnClick;
    }

    void OnClick(GameObject go)
    {
        if (activeNotification != null && go == activeNotification && currentNotificationIsModal)
        {
            // what to do when clicking the (modal) notification
        }
    }

    void Update()
    {
        if (IsNotificationActive())
        {
            currentPulseCounter += Time.deltaTime;
            float pulseScaler = (Mathf.Sin(currentPulseCounter * pulseSpeed) * pulseAmount) + 1.0f;
            activeNotification.transform.localScale = new Vector3(currentNotificationOriginalScale.x * pulseScaler, currentNotificationOriginalScale.y * pulseScaler, currentNotificationOriginalScale.z);
        }
    }

	public void DisplayNotification(GameObject canvas, bool isModal, bool isDarkened, int upgradeToShow)
	{
		if (canvas == null)
		{
			Debug.LogError("Canvas for notification not found");
			return;
		}

		if (isModal)
		{
			if (modalBackgroundObject == null)
			{
				modalBackgroundObject = Instantiate(modalBackgroundPrefab, new Vector3(0.0f, 0.0f, -2.3f), Quaternion.identity) as GameObject;
			}

			if (isDarkened)
			{
				modalBackgroundObject.GetComponent<Renderer>().enabled = true;
			}
			else
			{
				modalBackgroundObject.GetComponent<Renderer>().enabled = false;
			}
		}

		activeNotification = Instantiate(canvas, new Vector3(0.0f, 0.0f, -2.4f), Quaternion.identity) as GameObject;

		currentNotificationOriginalScale = activeNotification.transform.localScale * scaleFactor;
		currentNotificationIsModal = isModal;
		currentNotificationName = name;
		string cloneString = "(Clone)";

		if (upgradeToShow == 0)	// upgrade warning
		{
			GameObject go_rateIt = Instantiate(buttonPrefab_RateIt, new Vector3(buttonPosition_yes.x, buttonPosition_yes.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_no = Instantiate(buttonPrefab_No, new Vector3(buttonPosition_no.x, buttonPosition_no.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			go_rateIt.transform.SetParent(activeNotification.transform);
			go_no.transform.SetParent(activeNotification.transform);
			go_rateIt.name = RemoveStringFromEnd(go_rateIt.name, cloneString);
			go_no.name = RemoveStringFromEnd(go_no.name, cloneString);
		}
		else if (upgradeToShow == 1) // upgrade final
		{
			GameObject go_yes = Instantiate(buttonPrefab_Yes, new Vector3(buttonPosition_yes.x, buttonPosition_yes.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_no = Instantiate(buttonPrefab_No, new Vector3(buttonPosition_no.x, buttonPosition_no.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			go_yes.transform.SetParent(activeNotification.transform);
			go_no.transform.SetParent(activeNotification.transform);
			go_yes.name = RemoveStringFromEnd(go_yes.name, cloneString);
			go_no.name = RemoveStringFromEnd(go_no.name, cloneString);
		}
		else if (upgradeToShow == 2) // upgrade choices
		{
			GameObject go_unlock = Instantiate(buttonPrefab_UpgradeUnlock, new Vector3(buttonPosition_unlock.x, buttonPosition_unlock.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_cancel = Instantiate(buttonPrefab_Cancel, new Vector3(buttonPosition_cancel.x, buttonPosition_cancel.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			go_unlock.transform.SetParent(activeNotification.transform);
			go_cancel.transform.SetParent(activeNotification.transform);
			go_unlock.name = RemoveStringFromEnd(go_unlock.name, cloneString);
			go_cancel.name = RemoveStringFromEnd(go_cancel.name, cloneString);
		}
	}

	public void DisplayNotification(GameObject canvas, bool isModal, bool isDarkened)
	{
		DisplayNotification(canvas, isModal, isDarkened, -1);
	}

	public void DisplayNotification(Texture texture, Vector2 screenPos, bool isModal, bool isDarkened, int upgradeToShow)
    {
        if (texture == null)
        {
            Debug.LogError("Texture for notification not found");
            return;
        }

        if (isModal)
        {
            if (modalBackgroundObject == null)
            {
                modalBackgroundObject = Instantiate(modalBackgroundPrefab, new Vector3(0.0f, 0.0f, -2.3f), Quaternion.identity) as GameObject;
            }
            
            if (isDarkened)
            {
                modalBackgroundObject.GetComponent<Renderer>().enabled = true;
            }
            else
            {
                modalBackgroundObject.GetComponent<Renderer>().enabled = false;
            }
        }

        activeNotification = Instantiate(quadPrefab, new Vector3(screenPos.x, screenPos.y, -2.4f), Quaternion.identity) as GameObject;
        activeNotification.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);

		currentNotificationOriginalScale = activeNotification.transform.localScale * scaleFactor;
        currentNotificationIsModal = isModal;
        currentNotificationName = name;
		string cloneString = "(Clone)";

		if (upgradeToShow == 0)	// upgrade warning
		{
			GameObject go_yes = Instantiate(buttonPrefab_Yes, new Vector3(buttonPosition_yes.x, buttonPosition_yes.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_keepPlaying = Instantiate(buttonPrefab_KeepPlaying, new Vector3(buttonPosition_keepPlaying.x, buttonPosition_keepPlaying.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			go_yes.transform.SetParent(activeNotification.transform);
			go_keepPlaying.transform.SetParent(activeNotification.transform);
			go_yes.name = RemoveStringFromEnd(go_yes.name, cloneString);
			go_keepPlaying.name = RemoveStringFromEnd(go_keepPlaying.name, cloneString);
		}
		else if (upgradeToShow == 1) // upgrade final
		{
			GameObject go_yes = Instantiate(buttonPrefab_Yes, new Vector3(buttonPosition_yes.x, buttonPosition_yes.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_no = Instantiate(buttonPrefab_No, new Vector3(buttonPosition_no.x, buttonPosition_no.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			go_yes.transform.SetParent(activeNotification.transform);
			go_no.transform.SetParent(activeNotification.transform);
			go_yes.name = RemoveStringFromEnd(go_yes.name, cloneString);
			go_no.name = RemoveStringFromEnd(go_no.name, cloneString);
		}
		else if (upgradeToShow == 2) // upgrade choices
		{
			GameObject go_unlock = Instantiate(buttonPrefab_UpgradeUnlock, new Vector3(buttonPosition_unlock.x, buttonPosition_unlock.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_cancel = Instantiate(buttonPrefab_Cancel, new Vector3(buttonPosition_cancel.x, buttonPosition_cancel.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			go_unlock.transform.SetParent(activeNotification.transform);
			go_cancel.transform.SetParent(activeNotification.transform);
			go_unlock.name = RemoveStringFromEnd(go_unlock.name, cloneString);
			go_cancel.name = RemoveStringFromEnd(go_cancel.name, cloneString);
		}
    }

	public void DisplayNotification(Texture texture, Vector2 screenPos, bool isModal, bool isDarkened)
	{
		DisplayNotification(texture, screenPos, isModal, isDarkened, -1);
	}

	string RemoveStringFromEnd(string targetString, string stringToRemove)
	{
		if (targetString.EndsWith(stringToRemove))
		{
			return targetString.Substring(0, targetString.Length - stringToRemove.Length);
		}
		Debug.LogWarning("Trying to remove end of string, but it doesn't match");
		return "ERROR!";
	}

    public void RemoveNotification()
    {
        if (activeNotification != null)
        {
            Destroy(activeNotification);
        }
        if (modalBackgroundObject != null)
        {
            Destroy(modalBackgroundObject);
            modalBackgroundObject = null;
        }
        currentPulseCounter = 0.0f;
    }

    public bool IsNotificationActive()
    {
        return (activeNotification != null);
    }

    public string GetNotificationName()
    {
        if (IsNotificationActive())
        {
            return currentNotificationName;
        }
        return "";
    }
}
