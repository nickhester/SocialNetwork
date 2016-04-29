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

	public void DisplayNotification(GameObject canvas, bool isModal, bool isDarkened, int specialButtonOption)
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
		for (int i = 0; i < buttons.Count; i++)
		{
			Button button = buttons[i];
			button.onClick.AddListener(() => InputManager.Instance.SendMouseClick(button.gameObject));
		}

		currentNotificationOriginalScale = activeNotification.transform.localScale * scaleFactor;
		currentNotificationIsModal = isModal;
		currentNotificationName = name;
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
