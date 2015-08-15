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
    private float showMeInterval = 0.7f;

    // saved positions
    private Vector2 screenPos_greenButton = new Vector2(-3.3f, -5.7f);
    private Vector2 screenPos_redButton = new Vector2(1.3f, -5.7f);
    private Vector2 screenPos_patients_3_2 = new Vector2(-0.5f, 3.5f);
    private Vector2 screenPos_patients_3_1 = new Vector2(1.9f, -0.8f);
    private Vector2 screenPos_patients_3_0 = new Vector2(-3.3f, -0.8f);
    private Vector2 screenPos_patients_4_1 = new Vector2(-3.0f, 2.9f);
	private Vector2 screenPos_patients_4_2 = new Vector2(1.7f, -1.2f);
	private Vector2 screenPos_patients_4_3 = new Vector2(1.7f, 2.9f);
	private Vector2 screenPos_patients_4_0 = new Vector2(-3.0f, -1.2f);
	private Vector2 screenPos_patients_5_0 = new Vector2(-2.4f, -1.4f);
	private Vector2 screenPos_patients_5_1 = new Vector2(-3.6f, 1.8f);
	private Vector2 screenPos_patients_5_2 = new Vector2(1.1f, -1.4f);
	private Vector2 screenPos_patients_5_3 = new Vector2(-0.7f, 4.3f);
	private Vector2 screenPos_patients_5_4 = new Vector2(2.1f, 1.8f);
    private Vector2 screenPos_showMeButton = new Vector2(-4.3f, 6.0f);
    private Vector2 screenPos_smallStripBelowPeople = new Vector2(0.0f, -2.1f);
    private Vector2 screenPos_largeStripBelowPeople = new Vector2(0.0f, -4.5f);

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
        inputManager = Camera.main.GetComponent<InputManager>();
    }

    void OnLevelWasLoaded(int level)
    {
        inputManager = Camera.main.GetComponent<InputManager>();
    }

    public void DisplayNotification(int _setIndex, bool _forceShow)
    {
        // trigger correct notifications

        // check to see if it's been seen
        if (SaveGame.GetSeenInstruction(_setIndex) && !_forceShow)
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
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_introduction"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 1)
            {
                // introduction screen 2
                m_finger.SendFinger(new Vector2(-1.8f, 2.5f));
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_introduction 2"), new Vector2(0.0f, -4.0f), false, false);
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
                m_finger.SendFinger(new Vector2(-3.5f, 2.3f));
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_appointments"), new Vector2(0.0f, -1.5f), false, false);
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
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_0"), screenPos_smallStripBelowPeople, true, true);
            }
            else if (_indexWithinSet == 1)
            {
                m_finger.SendFinger(screenPos_patients_3_2);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_1"), screenPos_smallStripBelowPeople, false, false);
                RequestExclusiveControl();
                AllowActions(new List<string> { "person 2" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 2)
            {
                m_finger.SendFinger(screenPos_greenButton);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_2"), screenPos_smallStripBelowPeople, false, false);
                AllowActions(new List<string> { "Button_green" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 3)
            {
                m_finger.SendFingerAway(false);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_3"), screenPos_smallStripBelowPeople, true, true);
                EndExclusiveControl();
            }
            else if (_indexWithinSet == 4)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_4"), screenPos_smallStripBelowPeople, true, true);
            }
            else if (_indexWithinSet == 5)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level_5"), screenPos_smallStripBelowPeople, true, true);
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
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_levelSuccess"), Vector2.zero, true, true);
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
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_returnToClipboard"), Vector2.zero, true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 5)        // second session tutorial =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level2_0"), screenPos_smallStripBelowPeople, true, true);
            }
            else if (_indexWithinSet == 1)
            {
                m_finger.SendFinger(screenPos_patients_3_2);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level2_1"), screenPos_smallStripBelowPeople, false, false);
                RequestExclusiveControl();
                AllowActions(new List<string> { "person 2" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 2)
            {
                m_finger.SendFingerAway(false);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level2_2"), screenPos_largeStripBelowPeople, true, false);
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
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level2_3"), screenPos_smallStripBelowPeople, true, true);
                EndExclusiveControl();
            }
            else if (_indexWithinSet == 5)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level2_4"), screenPos_smallStripBelowPeople, true, true);
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
        else if (_setIndex == 6)      // show me introduction ===================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_showMe"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 1)
            {
                m_finger.SendFinger(screenPos_showMeButton);
                RequestExclusiveControl();
                AllowActions(new List<string> { "ShowMe" }, new List<string>(), new List<string>());
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 7)        // third session tutorial =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level3_0"), screenPos_smallStripBelowPeople, true, true);
            }
            else if (_indexWithinSet == 1)
            {
                m_finger.SendFinger(screenPos_patients_4_1);
                RequestExclusiveControl();
                AllowActions(new List<string> { "person 1" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 2)
            {
                m_finger.SendFingerAway(false);
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_level3_1"), screenPos_largeStripBelowPeople, true, true);
                EndExclusiveControl();
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 8)        // tip on lines of red =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_1"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 1)
            {
                m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_2"), screenPos_smallStripBelowPeople, false, false);
				m_finger.SendFinger(screenPos_patients_4_1);
				RequestExclusiveControl();
				AllowActions(new List<string> { "person 1" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 2)
            {
                m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_3"), screenPos_smallStripBelowPeople, false, false);
				m_finger.SendFinger(screenPos_patients_4_2);
				AllowActions(new List<string> { "person 2" }, new List<string>(), new List<string>());
            }
			else if (_indexWithinSet == 3)
			{
				m_finger.SendFinger(screenPos_redButton);
				AllowActions(new List<string> { "Button_red" }, new List<string>(), new List<string>());
			}
            else if (_indexWithinSet == 4)
            {
                m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_4"), screenPos_smallStripBelowPeople, false, false);
				m_finger.SendFinger(screenPos_patients_4_3);
				AllowActions(new List<string> { "person 3" }, new List<string>(), new List<string>());

            }
            else if (_indexWithinSet == 5)
            {
                m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_5"), screenPos_smallStripBelowPeople, false, false);
				m_finger.SendFinger(screenPos_redButton);
				AllowActions(new List<string> { "Button_red" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 6)
            {
                m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_6"), screenPos_smallStripBelowPeople, false, false);
				m_finger.SendFinger(screenPos_patients_4_0);
				AllowActions(new List<string> { "person 0" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 7)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_7"), Vector2.zero, true, true);
				m_finger.SendFingerAway(false);
				EndExclusiveControl();
            }
            else if (_indexWithinSet == 8)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_8"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 9)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_9"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 10)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_10"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 11)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_11"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 12)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_12"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 13)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip2_13"), Vector2.zero, true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 9)        // first friday encouragement =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_fridayEncouragement_0"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 1)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_fridayEncouragement_1"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 2)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_fridayEncouragement_2"), Vector2.zero, true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 10)        // tip on last move =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip3_1"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 1)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip3_2"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 2)
            {
                m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_tip3_3"), screenPos_smallStripBelowPeople, false, false);
				m_finger.SendFinger(screenPos_patients_5_3);
				RequestExclusiveControl();
				AllowActions(new List<string> { "person 3" }, new List<string>(), new List<string>());
            }
			else if (_indexWithinSet == 3)
			{
				m_finger.SendFinger(screenPos_patients_5_4);
				AllowActions(new List<string> { "person 4" }, new List<string>(), new List<string>());
			}
			else if (_indexWithinSet == 4)
			{
				m_finger.SendFinger(screenPos_patients_5_2);
				AllowActions(new List<string> { "person 2" }, new List<string>(), new List<string>());
			}
			else if (_indexWithinSet == 5)
			{
				m_finger.SendFinger(screenPos_patients_5_0);
				AllowActions(new List<string> { "person 0" }, new List<string>(), new List<string>());
			}
            else if (_indexWithinSet == 6)
            {
                m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_tip3_4"), screenPos_smallStripBelowPeople, false, false);
				m_finger.SendFinger(screenPos_patients_5_1);
				AllowActions(new List<string> { "person 1" }, new List<string>(), new List<string>());
            }
            else if (_indexWithinSet == 7)
            {
                m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_tip3_5"), screenPos_smallStripBelowPeople, true, false);
				m_finger.SendFingerAway(false);
				EndExclusiveControl();
			}
			else if (_indexWithinSet == 8)
			{
				m_finger.SendFinger(screenPos_patients_5_0);
				RequestExclusiveControl();
				AllowActions(new List<string> { "person 0" }, new List<string>(), new List<string>());
			}
			else if (_indexWithinSet == 9)
			{
				m_finger.SendFinger(screenPos_greenButton);
				RequestExclusiveControl();
				AllowActions(new List<string> { "Button_green" }, new List<string>(), new List<string>());
			}
			else if (_indexWithinSet == 10)
			{
				m_finger.SendFinger(screenPos_patients_5_2);
				RequestExclusiveControl();
				AllowActions(new List<string> { "person 2" }, new List<string>(), new List<string>());
			}
			else if (_indexWithinSet == 11)
			{
				m_finger.SendFinger(screenPos_redButton);
				RequestExclusiveControl();
				AllowActions(new List<string> { "Button_red" }, new List<string>(), new List<string>());
			}
            else if (_indexWithinSet == 12)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_tip3_6"), Vector2.zero, true, true);
				EndExclusiveControl();
            }
            else if (_indexWithinSet == 13)
            {
				m_finger.SendFinger(screenPos_patients_5_1);
				RequestExclusiveControl();
				AllowActions(new List<string> { "person 1" }, new List<string>(), new List<string>());
			}
			else if (_indexWithinSet == 14)
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
        else if (_setIndex == 11)        // introducing special - cant touch =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_special_cantTouch0"), Vector2.zero, true, true);
            }
			else if (_indexWithinSet == 1)
			{
				m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_special_cantTouch1"), Vector2.zero, true, true);
			}
			else if (_indexWithinSet == 2)
			{
				m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_special_cantTouch2"), Vector2.zero, true, true);
			}
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 12)        // introducing special - fall to red =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_special_fallToRed"), Vector2.zero, true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 13)        // introducing special - one click =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_special_oneClick"), Vector2.zero, true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 14)        // introducing special - no lines =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_special_noLines"), Vector2.zero, true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 15)        // congratulations: all sessions completed =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_allSessionsCompleted"), Vector2.zero, true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 16)        // congratulations: all stars earned =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_allStarsEarned"), Vector2.zero, true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 17)        // credits =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_credits"), new Vector2(0.0f, -19.2f), true, true);
            }
            else
            {
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
        else if (_setIndex == 18)        // restart level =====================================================
        {
            if (_indexWithinSet == 0)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_restartLevel_0"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 1)
            {
                m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_restartLevel_1"), Vector2.zero, true, true);
            }
            else if (_indexWithinSet == 2)
            {
                m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_restartLevel_2"), Vector2.zero, true, false);
				m_finger.SendFinger(GameObject.Find("restartLevel"));
            }
            else
            {
				m_finger.SendFingerAway(false);
                SaveGame.SetSeenInstruction(_setIndex, true);
                EndNotification();
            }
        }
		else if (_setIndex == 19)			// First time completing all levels in day, back at clipboard
		{
			if (_indexWithinSet == 0)
			{
				m_notification.DisplayNotification(
                    Resources.Load<Texture>("textures/instructions/Instruction Paper_completeFirstDay"), Vector2.zero, true, true);
			}
			else if (_indexWithinSet == 1)
			{
				m_finger.SendFinger(GameObject.Find("BackButton"));
				RequestExclusiveControl();
				AllowActions(new List<string> { "BackButton" }, new List<string>(), new List<string>());
			}
			else
			{
				SaveGame.SetSeenInstruction(_setIndex, true);
				EndNotification();
			}
		}
		else if (_setIndex == 20)        // Upgrade Warning =====================================================
		{
			if (_indexWithinSet == 0)
			{
				m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_UpgradeWarning"), Vector2.zero, true, true, 0);
			}
			else
			{
				SaveGame.SetSeenInstruction(_setIndex, true);
				EndNotification();
			}
		}
		else if (_setIndex == 21)        // Upgrade Final =====================================================
		{
			if (_indexWithinSet == 0)
			{
				m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_UpgradeFinal"), Vector2.zero, true, true, 1);
			}
			else
			{
				SaveGame.SetSeenInstruction(_setIndex, true);
				EndNotification();
			}
		}
		else if (_setIndex == 22)        // Upgrade Choice =====================================================
		{
			if (_indexWithinSet == 0)
			{
				m_notification.DisplayNotification(
					Resources.Load<Texture>("textures/instructions/Instruction Paper_UpgradeChoice"), Vector2.zero, true, true, 2);
			}
			else
			{
				SaveGame.SetSeenInstruction(_setIndex, true);
				EndNotification();
			}
		}
        else if (_setIndex == 100)			// Show Me =====================================================
        {
            StartCoroutine(DisplayShowMeSeries());
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

        for (int i = 0; i < currentLevelPath.trail.Count; i++)
        {
            Vector3 personPos = allPeople[currentLevelPath.trail[i].Key].gameObject.transform.position;
            Vector2 personPosOffset = (Vector2)personPos - m_finger.GetFingerTipOffset();
            m_finger.SendFinger(personPosOffset);

            yield return new WaitForSeconds(showMeInterval);

            ClickAtFinger();

            yield return new WaitForSeconds(showMeInterval);

            if (currentLevelPath.trail[i].Value)
            {
                m_finger.SendFinger(screenPos_greenButton);
            }
            else
            {
                m_finger.SendFinger(screenPos_redButton);
            }
            

            yield return new WaitForSeconds(showMeInterval);

            ClickAtFinger();

            yield return new WaitForSeconds(showMeInterval);
        }

        m_finger.SendFingerAway(false);
        inputManager.ResumeUserInput();
        GameObject.FindGameObjectWithTag("clipboard").GetComponent<Clipboard>().Callback_CompletedShowMe();
        EndNotification();
    }

    void EndNotification(bool isImmediate)
    {
        m_finger.SendFingerAway(isImmediate);
        m_notification.RemoveNotification();
        currentSet = -1;
        currentIndexWithinSet = -1;
        intendedObjects = null;
        allowedObjects_NotificationEnds = null;
        allowedObjects_NotificationStays = null;
        EndExclusiveControl();
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
