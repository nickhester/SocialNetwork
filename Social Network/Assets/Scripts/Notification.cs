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
	[SerializeField] private GameObject buttonPrefab_UpgradeFull;
	[SerializeField] private GameObject buttonPrefab_UpgradeHalf;
	[SerializeField] private GameObject buttonPrefab_UpgradeTwice;
	[SerializeField] private GameObject buttonPrefab_Yes;
	[SerializeField] private GameObject buttonPrefab_No;
	[SerializeField] private GameObject buttonPrefab_KeepPlaying;
	private Vector3 buttonPosition_full = new Vector3(4.0f, 1.0f);
	private Vector3 buttonPosition_half = new Vector3(4.0f, 0.0f);
	private Vector3 buttonPosition_twice = new Vector3(4.0f, -1.0f);
	private Vector3 buttonPosition_yes = new Vector3(-2.2f, -2.8f);
	private Vector3 buttonPosition_no = new Vector3(2.2f, -2.8f);
	private Vector3 buttonPosition_keepPlaying = new Vector3(2.2f, -2.8f);


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

		if (upgradeToShow == 0)	// upgrade warning
		{
			GameObject go_yes = Instantiate(buttonPrefab_Yes, new Vector3(buttonPosition_yes.x, buttonPosition_yes.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_keepPlaying = Instantiate(buttonPrefab_KeepPlaying, new Vector3(buttonPosition_keepPlaying.x, buttonPosition_keepPlaying.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			go_yes.transform.SetParent(activeNotification.transform);
			go_keepPlaying.transform.SetParent(activeNotification.transform);
		}
		else if (upgradeToShow == 1) // upgrade final
		{
			GameObject go_yes = Instantiate(buttonPrefab_Yes, new Vector3(buttonPosition_yes.x, buttonPosition_yes.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_no = Instantiate(buttonPrefab_No, new Vector3(buttonPosition_no.x, buttonPosition_no.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			go_yes.transform.SetParent(activeNotification.transform);
			go_no.transform.SetParent(activeNotification.transform);
		}
		else if (upgradeToShow == 2) // upgrade choices
		{
			GameObject go_full = Instantiate(buttonPrefab_UpgradeFull, new Vector3(buttonPosition_full.x, buttonPosition_full.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_half = Instantiate(buttonPrefab_UpgradeHalf, new Vector3(buttonPosition_half.x, buttonPosition_half.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			GameObject go_twice = Instantiate(buttonPrefab_UpgradeTwice, new Vector3(buttonPosition_twice.x, buttonPosition_twice.y, activeNotification.transform.position.z - 0.1f), Quaternion.identity) as GameObject;
			go_full.transform.SetParent(activeNotification.transform);
			go_half.transform.SetParent(activeNotification.transform);
			go_twice.transform.SetParent(activeNotification.transform);
		}
    }

	public void DisplayNotification(Texture texture, Vector2 screenPos, bool isModal, bool isDarkened)
	{
		DisplayNotification(texture, screenPos, isModal, isDarkened, -1);
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
