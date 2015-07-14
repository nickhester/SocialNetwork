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
    private Vector2 instructionDimensions = new Vector2(1516.0f, 2178.0f);
    private float instructionDimensionScale = 0.007f;

    private int currentSet = -1;
    private int currentIndexWithinSet = -1;

    private List<string> intendedObjects;
    private List<string> allowedObjects_NotificationStays;
    private List<string> allowedObjects_NotificationEnds;
    private bool isInExclusiveEvent = false;

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

        if (_setIndex == 0)      // first greeting at calendar =====================================================
        {
            if (_indexWithinSet == 0)
            {
                // introduction screen
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_introduction"),
                    "introduction",
                    new Vector2(0.0f, 0.0f),
                    instructionDimensions.x * instructionDimensionScale,
                    instructionDimensions.y * instructionDimensionScale,
                    true,
                    true);
            }
            else if (_indexWithinSet == 1)
            {
                // introduction screen 2
                m_finger.SendFinger(new Vector2(-1.8f, 2.5f));
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_introduction 2"),
                    "introduction 2",
                    new Vector2(0.0f, -4.0f),
                    instructionDimensions.x * instructionDimensionScale,
                    instructionDimensions.y * instructionDimensionScale,
                    false,
                    false);
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
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_appointments"),
                    "appointments",
                    new Vector2(0.0f, -1.5f),
                    instructionDimensions.x * instructionDimensionScale,
                    instructionDimensions.y * instructionDimensionScale,
                    false,
                    false);
                RequestExclusiveControl();
                AllowActions(new List<string> { "appointment 0" }, new List<string>(), new List<string>());
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
                print("properly ending clipboard");
            }
        }
        else if (_setIndex == 2)        // first session tutorial =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level"),
                    "first level",
                    new Vector2(0.0f, -4.6f),
                    instructionDimensions.x * instructionDimensionScale,
                    instructionDimensions.y * instructionDimensionScale,
                    true,
                    true);
            }
            else if (_indexWithinSet == 1)
            {
                m_finger.SendFinger(new Vector2(-0.5f, 3.5f));
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_1"),
                    "first level 2",
                    new Vector2(0.0f, -4.6f),
                    instructionDimensions.x * instructionDimensionScale,
                    instructionDimensions.y * instructionDimensionScale,
                    false,
                    false);
                RequestExclusiveControl();
                AllowActions(new List<string> { "person 2" }, new List<string>(), new List<string>());
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else
        {
            Debug.LogError("Attempting to view notification that doesn't exist");
        }
        print("ending active notification function");
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
}
