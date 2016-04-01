using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Notification : MonoBehaviour
{
    [SerializeField] private GameObject quadPrefab;
    [SerializeField] private GameObject modalBackgroundPrefab;
    private GameObject modalBackgroundObject;
    private GameObject activeNotification;
    public Texture tempTexture;
	[HideInInspector] public bool currentNotificationIsModal;
    private string currentNotificationName;
    private Vector3 currentNotificationOriginalScale;
    private float currentPulseCounter = 0.0f;
    [SerializeField] private float pulseSpeed;
    [SerializeField] private float pulseAmount;
	private float scaleFactor = 0.9f;
	private Vector3 notificationScaleUpdate = Vector3.zero;
	string cloneString = "(Clone)";

	// buttons
	[SerializeField] private GameObject buttonPrefab_UpgradeUnlock;
	[SerializeField] private GameObject buttonPrefab_Yes;
	[SerializeField] private GameObject buttonPrefab_No;
	[SerializeField] private GameObject buttonPrefab_KeepPlaying;
	[SerializeField] private GameObject buttonPrefab_Cancel;
	[SerializeField] private GameObject buttonPrefab_RateIt;
	[SerializeField] private GameObject buttonPrefab_WatchIt;
	private Vector3 buttonPosition_unlock = new Vector3(0.0f, -0.5f);
	private Vector3 buttonPosition_cancel = new Vector3(0.0f, -4.0f);
	private Vector3 buttonPosition_yes = new Vector3(-2.4f, -3.2f);
	private Vector3 buttonPosition_no = new Vector3(2.4f, -3.2f);
	private Vector3 buttonPosition_keepPlaying = new Vector3(2.4f, -3.2f);
	private Vector3 buttonPosition_watchItYes = new Vector3(-2.4f, 1.0f);
	private Vector3 buttonPosition_watchItCancel = new Vector3(2.4f, 1.0f);
	private Vector3 buttonPosition_watchItUnlock = new Vector3(0.0f, -3.8f);

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
			notificationScaleUpdate.x = currentNotificationOriginalScale.x * pulseScaler;
			notificationScaleUpdate.y = currentNotificationOriginalScale.y * pulseScaler;
			notificationScaleUpdate.z = currentNotificationOriginalScale.z;
            activeNotification.transform.localScale = notificationScaleUpdate;
        }
    }

	private void SetUpButtons(GameObject go)
	{
		go.transform.SetParent(activeNotification.transform);
		go.name = RemoveStringFromEnd(go.name, cloneString);
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

		// register UI buttons to call to input manager
		List<Button> buttons = new List<Button>();
		buttons.AddRange(activeNotification.GetComponentsInChildren<Button>());
		foreach (Button button in buttons)
		{
			button.onClick.AddListener(() => InputManager.Instance.SendMouseClick(button.gameObject));
		}

		currentNotificationOriginalScale = activeNotification.transform.localScale * scaleFactor;
		currentNotificationIsModal = isModal;
		currentNotificationName = name;

		if (upgradeToShow == 0)	// upgrade warning
		{
			GameObject go_rateIt = Instantiate(buttonPrefab_RateIt, new Vector3(buttonPosition_yes.x, buttonPosition_yes.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_no = Instantiate(buttonPrefab_No, new Vector3(buttonPosition_no.x, buttonPosition_no.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			SetUpButtons(go_rateIt);
			SetUpButtons(go_no);
		}
		else if (upgradeToShow == 1) // upgrade final
		{
			GameObject go_yes = Instantiate(buttonPrefab_Yes, new Vector3(buttonPosition_yes.x, buttonPosition_yes.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_no = Instantiate(buttonPrefab_No, new Vector3(buttonPosition_no.x, buttonPosition_no.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			SetUpButtons(go_yes);
			SetUpButtons(go_no);
		}
		else if (upgradeToShow == 2) // upgrade choices
		{
			GameObject go_unlock = Instantiate(buttonPrefab_UpgradeUnlock, new Vector3(buttonPosition_unlock.x, buttonPosition_unlock.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_cancel = Instantiate(buttonPrefab_Cancel, new Vector3(buttonPosition_cancel.x, buttonPosition_cancel.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			SetUpButtons(go_unlock);
			SetUpButtons(go_cancel);
		}
		else if (upgradeToShow == 3) // show me ad warning
		{
			GameObject go_unlock = Instantiate(buttonPrefab_UpgradeUnlock, new Vector3(buttonPosition_watchItUnlock.x, buttonPosition_watchItUnlock.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_cancel = Instantiate(buttonPrefab_Cancel, new Vector3(buttonPosition_watchItCancel.x, buttonPosition_watchItCancel.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_watchIt = Instantiate(buttonPrefab_WatchIt, new Vector3(buttonPosition_watchItYes.x, buttonPosition_watchItYes.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			SetUpButtons(go_unlock);
			SetUpButtons(go_cancel);
			SetUpButtons(go_watchIt);
		}
	}

	public void DisplayNotification(GameObject canvas, bool isModal, bool isDarkened)
	{
		DisplayNotification(canvas, isModal, isDarkened, -1);
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
