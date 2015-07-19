using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private GameObject notificationPrefab;
    [SerializeField] private GameObject fingerPrefab;

    private Finger m_finger;
    private Notification m_notification;

    private InputManager inputManager;

    private int currentSet = -1;
    private int currentIndexWithinSet = -1;

    private List<string> intendedObjects;
    private List<string> allowedObjects_NotificationStays;
    private List<string> allowedObjects_NotificationEnds;
    private bool isInExclusiveEvent = false;

    // saved positions
    private Vector2 screenPos_greenButton = new Vector2(-3.3f, -5.7f);
    private Vector2 screenPos_redButton = new Vector2(1.3f, -5.7f);
    private Vector2 screenPos_patients_3_2 = new Vector2(-0.5f, 3.5f);
    private Vector2 screenPos_patients_3_1 = new Vector2(1.9f, -0.8f);
    private Vector2 screenPos_patients_3_0 = new Vector2(-3.3f, -0.8f);

    void Awake()
    {
        InputManager.Instance.OnClick += OnClick;
        InputManager.Instance.OnClick_Exclusive += OnClick_Exclusive;

        m_finger = GetComponent<Finger>();
        m_notification = GetComponent<Notification>();
    }

    private void OnClick(GameObject go)
    {
        if (go.tag == "notificationModal")
        {
            // remove notification or go to next
            m_notification.RemoveNotification();
            if (currentSet != -1)
            {
                ActivateNotification(currentSet, currentIndexWithinSet);
            }
        }
        else if (isInExclusiveEvent)
        {
            // if it's an exclusive event, then don't do anything b/c the exclusive onclick will handle it
        }
        else
        {
            // if you're clicking anywhere, ignore
        }
    }

    private void OnClick_Exclusive(GameObject go)
    {
        foreach (string objectName in intendedObjects)
        {
            if (go.name == objectName)
            {
                ActivateNotification(currentSet, currentIndexWithinSet);
                inputManager.ManuallySendEvent(GameObject.Find(objectName));
                return;
            }
        }
        foreach (string objectName in allowedObjects_NotificationStays)
        {
            if (go.name == objectName)
            {
                inputManager.ManuallySendEvent(GameObject.Find(objectName));
                return;
            }
        }
        foreach (string objectName in allowedObjects_NotificationEnds)
        {
            if (go.name == objectName)
            {
                inputManager.ManuallySendEvent(GameObject.Find(objectName));
                EndNotification();
                return;
            }
        }
    }

    void Start()
    {
        //DisplayNotificationSet(0);
    }

    void OnLevelWasLoaded(int level)
    {
        inputManager = Camera.main.GetComponent<InputManager>();
    }

    public void DisplayNotification(int _setIndex)
    {
        // trigger correct notifications

        // check to see if it's been seen
        if (SaveGame.GetSeenInstruction(_setIndex))
        {
            return;
        }

        if (currentSet != -1 && currentIndexWithinSet != -1)
        {
            Debug.LogWarning("Starting new notification, but old one was not terminated properly");
            EndNotification(true);
        }

        currentSet = _setIndex;
        ActivateNotification(currentSet, currentIndexWithinSet);
    }

    void ActivateNotification(int _setIndex, int _indexWithinSet)
    {
        currentIndexWithinSet++;
        _indexWithinSet++;

        m_notification.RemoveNotification();   // remove last notification if still there

        if (_setIndex == 0)      // first greeting at calendar =====================================================
        {
            if (_indexWithinSet == 0)
            {
                // introduction screen
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_introduction"), "introduction", new Vector2(0.0f, 0.0f), true, true);
            }
            else if (_indexWithinSet == 1)
            {
                // introduction screen 2
                m_finger.SendFinger(new Vector2(-1.8f, 2.5f));
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_introduction 2"), "introduction 2", new Vector2(0.0f, -4.0f), false, false);
                RequestExclusiveControl();
                AllowActions(new List<string> { "1 Monday" }, new List<string>(), new List<string>());
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification(true);
            }
        }
        else if (_setIndex == 1)      // first time at clipboard =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_finger.SendFinger(new Vector2(-4.5f, 2.5f));
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_appointments"), "appointments", new Vector2(0.0f, -1.5f), false, false);
                RequestExclusiveControl();
                AllowActions(new List<string> { "appointment 0" }, new List<string>(), new List<string>());
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 2)        // first session tutorial =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_0"), "first level 0", new Vector2(0.0f, -2.1f), true, true);
            }
            else if (_indexWithinSet == 1)
            {
                m_finger.SendFinger(screenPos_patients_3_2);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_1"), "first level 1", new Vector2(0.0f, -2.1f), false, false);
                RequestExclusiveControl();
                AllowActions(new List<string> { "person 2" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 2)
            {
                m_finger.SendFinger(screenPos_greenButton);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_2"), "first level 2", new Vector2(0.0f, -2.1f), false, false);
                AllowActions(new List<string> { "Button_green" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 3)
            {
                m_finger.SendFingerAway(false);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_3"), "first level 3", new Vector2(0.0f, -2.1f), true, true);
                EndExclusiveControl();
            }
            else if (_indexWithinSet == 4)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_4"), "first level 4", new Vector2(0.0f, -2.1f), true, true);
            }
            else if (_indexWithinSet == 5)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_5"), "first level 5", new Vector2(0.0f, -2.1f), true, true);
            }
            else if (_indexWithinSet == 6)
            {
                m_finger.SendFinger(screenPos_patients_3_1);
                RequestExclusiveControl();
                AllowActions(new List<string> { "person 1" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 7)
            {
                m_finger.SendFinger(screenPos_greenButton);
                AllowActions(new List<string> { "Button_green" }, new List<string>(), new List<string>());
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 3)      // first time completing session =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_levelSuccess"), "levelSuccess", new Vector2(0.0f, 0.0f), true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 4)      // first time returning to clipboard after a session ===================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_returnToClipboard"), "returnToClipboard", new Vector2(0.0f, 0.0f), true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 5)        // first session tutorial =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level2_0"), "level 2 0", new Vector2(0.0f, -2.1f), true, true);
            }
            else if (_indexWithinSet == 1)
            {
                m_finger.SendFinger(screenPos_patients_3_2);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level2_1"), "level 2 1", new Vector2(0.0f, -2.1f), false, false);
                RequestExclusiveControl();
                AllowActions(new List<string> { "person 2" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 2)
            {
                m_finger.SendFingerAway(false);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level2_2"), "level 2 2", new Vector2(0.0f, -4.5f), true, false);
                EndExclusiveControl();
            }
            else if (_indexWithinSet == 3)
            {
                m_finger.SendFinger(screenPos_greenButton);
                RequestExclusiveControl();
                AllowActions(new List<string> { "Button_green" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 4)
            {
                m_finger.SendFingerAway(false);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level2_3"), "level 2 3", new Vector2(0.0f, -2.1f), true, true);
                EndExclusiveControl();
            }
            else if (_indexWithinSet == 5)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level2_4"), "level 2 4", new Vector2(0.0f, -2.1f), true, true);
            }
            else if (_indexWithinSet == 6)
            {
                m_finger.SendFinger(screenPos_patients_3_0);
                RequestExclusiveControl();
                AllowActions(new List<string> { "person 0" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 7)
            {
                m_finger.SendFinger(screenPos_greenButton);
                AllowActions(new List<string> { "Button_green" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 8)
            {
                m_finger.SendFinger(screenPos_patients_3_1);
                RequestExclusiveControl();
                AllowActions(new List<string> { "person 1" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 9)
            {
                m_finger.SendFinger(screenPos_greenButton);
                AllowActions(new List<string> { "Button_green" }, new List<string>(), new List<string>());
            }
            else
            {
                EndExclusiveControl();
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 100)        // Show Me =====================================================
        {
            // take exclusive control
            
            // use the "_indexWithinSet" to make an action for the player
            // trigger each one with a timer

            StartCoroutine(DisplayShowMeSeries());

            // return exclusive control

            // send callback message to clipboard
            
            // ? maybe use a for loop with increasing "invoke" calls to call all actions at once?
        }
        else
        {
            Debug.LogError("Attempting to view notification that doesn't exist");
        }
    }

    IEnumerator DisplayShowMeSeries()
    {
        inputManager.IgnoreUserInput();

        ValidLevels currentLevel = GameObject.FindGameObjectWithTag("clipboard").GetComponent<Clipboard>().GetNextLevelUp().myLevel;
        ActionTrail currentLevelPath = currentLevel.path;

        List<Person> allPeople = GameObject.FindGameObjectWithTag("networkManager").GetComponent<NetworkManager>().GetAllPeople();

        float currentDelay = 0.9f;

        for (int i = 0; i < currentLevelPath.trail.Count; i++)
        {
            Vector3 personPos = allPeople[currentLevelPath.trail[i].Key].gameObject.transform.position;
            Vector2 personPosOffset = (Vector2)personPos - m_finger.GetFingerTipOffset();
            m_finger.SendFinger(personPosOffset);

            yield return new WaitForSeconds(currentDelay);

            ClickAtFinger();

            yield return new WaitForSeconds(currentDelay);

            if (currentLevelPath.trail[i].Value)
            {
                m_finger.SendFinger(screenPos_greenButton);
            }
            else
            {
                m_finger.SendFinger(screenPos_redButton);
            }
            

            yield return new WaitForSeconds(currentDelay);

            ClickAtFinger();

            yield return new WaitForSeconds(currentDelay);
        }

        m_finger.SendFingerAway(false);
        inputManager.ResumeUserInput();
        GameObject.FindGameObjectWithTag("clipboard").GetComponent<Clipboard>().Callback_CompletedShowMe();
    }

    void EndNotification(bool isImmediate)
    {
        m_finger.SendFingerAway(isImmediate);
        m_notification.RemoveNotification();
        currentSet = -1;
        currentIndexWithinSet = -1;
        inputManager.EndExclusiveControl();
        intendedObjects = null;
        allowedObjects_NotificationEnds = null;
        allowedObjects_NotificationStays = null;
        isInExclusiveEvent = false;
    }

    void EndNotification()
    {
        EndNotification(false);
    }

    void RequestExclusiveControl()
    {
        inputManager.RequestExclusiveControl(gameObject);
        isInExclusiveEvent = true;
    }

    void EndExclusiveControl()
    {
        inputManager.EndExclusiveControl();
        isInExclusiveEvent = false;
    }

    void AllowActions(List<string> _intendedObjects, List<string> _allowedObjectsThatWillRemoveNotification, List<string> _allowedObjectsThatWontRemoveNotification)
    {
        intendedObjects = _intendedObjects;
        allowedObjects_NotificationEnds = _allowedObjectsThatWillRemoveNotification;
        allowedObjects_NotificationStays = _allowedObjectsThatWontRemoveNotification;

        // always allow
        allowedObjects_NotificationStays.Add("audioToggle_music");
        allowedObjects_NotificationStays.Add("audioToggle_sfx");
        allowedObjects_NotificationEnds.Add("MainMenu");
        allowedObjects_NotificationEnds.Add("BackButton");
    }

    void ClickAtFinger()
    {
        inputManager.SendMouseClick(m_finger.FingerClick());
    }

    void FingerCompletedClick()
    {
        // send callback
    }
}
