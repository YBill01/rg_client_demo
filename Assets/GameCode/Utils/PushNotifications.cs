using EasyMobile;
using EasyMobile.MiniJSON;
using Legacy.Database;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class PushNotifications : MonoBehaviour
	{
        public static PushNotifications Instance;

        private ProfileInstance _profileInstance;
        private ProfileInstance Profile
        {
            get
            {
                if (_profileInstance == null)
                {
                    _profileInstance = ClientWorld.Instance.Profile;
                }
                return _profileInstance;
            }
        }

        [Header("Chests opening notification")]
        [SerializeField]
        private bool ChestOpeningEnabled = false;
        [Space(10)]
        [SerializeField]
        private string ChestOpeningTitle = "";
        [SerializeField]
        private string ChestOpeningSubtitle = "";
        [SerializeField]
        private string ChestOpeningMessage = "";
        [Space(10)]
        [SerializeField]
        private string ChestOpeningSmallIcon = "ic_stat_em_default";
        [SerializeField]
        private string ChestOpeningLargeIcon = "ic_large_em_default";

        [Header("Chests reminder notification")]
        [SerializeField]
        private bool ChestReminderEnabled = false;
        [Space(10)]
        [SerializeField]
        private int ChestReminderDurationMinutes = 0;
        [Space(10)]
        [SerializeField]
        private string ChestReminderTitle = "";
        [SerializeField]
        private string ChestReminderSubtitle = "";
        [SerializeField]
        private string ChestReminderMessage = "";
        [Space(10)]
        [SerializeField]
        private string ChestReminderSmallIcon = "ic_stat_em_default";
        [SerializeField]
        private string ChestReminderLargeIcon = "ic_large_em_default";
        
        [Header("Daily notification")]
        [SerializeField]
        private bool DailyEnabled = false;
        [Space(10)]
        [SerializeField]
        private string DailyTitle = "";
        [SerializeField]
        private string DailySubtitle = "";
        [SerializeField]
        private string DailyMessage = "";
        [Space(10)]
        [SerializeField]
        private string DailySmallIcon = "ic_stat_em_default";
        [SerializeField]
        private string DailyLargeIcon = "ic_large_em_default";
        
        [Header("Play me! notification")]
        [SerializeField]
        private bool PlaymeEnabled = false;
        [Space(10)]
        [SerializeField]
        private int PlaymeDurationHours = 0;
        [Space(10)]
        [SerializeField]
        private string PlaymeTitle = "";
        [SerializeField]
        private string PlaymeSubtitle = "";
        [SerializeField]
        private string PlaymeMessage = "";
        [Space(10)]
        [SerializeField]
        private string PlaymeSmallIcon = "ic_stat_em_default";
        [SerializeField]
        private string PlaymeLargeIcon = "ic_large_em_default";


        //[SerializeField]
        //private TMP_Text resultTextOnClickMessage;


        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        void Start()
        {
            Notifications.Init();

            InvokeRepeating(nameof(UpdatePendingNotificationList), 1, 1);
            //resultTextOnClickMessage.text = CheckInit().ToString();
        }

        public bool CheckInit()
        {
            bool isInit = Notifications.IsInitialized();

            /*if (!isInit)
            {
                NativeUI.Alert("Alert", "Please initialize first.");
            }*/

            return isInit;
        }

        // Subscribes to notification events.
        void OnEnable()
        {
            Notifications.LocalNotificationOpened += OnLocalNotificationOpened;
        }
        void OnDisable()
        {
            Notifications.LocalNotificationOpened -= OnLocalNotificationOpened;
        }

		void OnLocalNotificationOpened(EasyMobile.LocalNotification delivered)
		{
			NotificationContent content = delivered.content;

			/*StringBuilder sb = new StringBuilder();

			sb.Append("ID: " + delivered.id.ToString() + "\n")
				.Append("Title: " + content.title + "\n")
				.Append("Subtitle: " + content.subtitle + "\n")
				.Append("Body: " + content.body + "\n")
				.Append("Badge: " + content.badge.ToString() + "\n")
				.Append("UserInfo: " + Json.Serialize(content.userInfo) + "\n")
				.Append("CategoryID: " + content.categoryId + "\n")
				.Append("-------------------------\n");

			resultTextOnClickMessage.text = sb.ToString();*/
		}




        /*private void ScheduleLocalNotificationT(NotificationContent content)
        {
            DateTime triggerDate = DateTime.Now + new TimeSpan(0, 0, 5);
            Notifications.ScheduleLocalNotification(triggerDate, content);
        }

        private NotificationContent NotificationContent01()
        {
            // notification message
            NotificationContent content = new NotificationContent();
            content.title = "title 01";
            content.subtitle = "subtitle 01";
            content.body = "message 01";

			// optionally info
			content.userInfo = new Dictionary<string, object>
			{
				{ "string", "OK" },
				{ "type", 3 }
			};

			// categoryId
			content.categoryId = EM_NotificationsConstants.UserCategory_notification_category_chests_opening;

            // Increase badge number (iOS only)
            //content.badge = Notifications.GetAppIconBadgeNumber() + 1;

            return content;
        }


        // for tested...
        public void ButtonClick01()
        {
            ScheduleLocalNotificationT(NotificationContent01());
        }*/



        //////////////////////////////////////////////////////////////////////////
        #region GetNotificationContent
        private NotificationContent GetNotificationContent(NotificationData nData)
        {
            // notification message
            NotificationContent content = new NotificationContent();
            content.title = nData.title;
            content.subtitle = nData.subtitle;
            content.body = nData.body;

            // optionally info
            content.userInfo = nData.userInfo;

			// icons
			if (!string.IsNullOrEmpty(nData.smallIcon))
			{
                content.smallIcon = nData.smallIcon;
            }
            if (!string.IsNullOrEmpty(nData.largeIcon))
			{
                content.largeIcon = nData.largeIcon;
            }
            
            // categoryId
            content.categoryId = nData.categoryId;

            // Increase badge number (iOS only)
            content.badge = Notifications.GetAppIconBadgeNumber() + 1;

            return content;
        }
        #endregion

        #region ChestOpeningLocalNotification
        public void ChestOpeningLocalNotificationStart(int index)
        {
			if (!ChestOpeningEnabled)
			{
                return;
			}

            PlayerProfileLootBox box = Profile.loot.boxes[index - 1];

            NotificationPanding nPanding = new NotificationPanding();
            nPanding.GetPendingLocalNotifications("type", NotificationType.ChestOpening).
                GetPendingLocalNotifications("index", index);

            bool rePending = false;
            bool pending = true;

            foreach (var request in NotificationPanding.localPendingRequests)
            {
                if (request.content.userInfo.TryGetValue("date", out object dateValue))
                {
                    if ((DateTime)dateValue != box.timer)
                    {
                        Notifications.CancelPendingLocalNotification(request.id);
                        rePending = true;
                        pending = true;
                    }
					else if (!rePending)
					{
                        pending = false;
                    }
                }
            }

            if (box.index == 0 || (!pending && !rePending))
			{
                return;
			}

            NotificationData nData = new NotificationData
            {
                type = NotificationType.ChestOpening,

                title = Locales.Get(ChestOpeningTitle),
                subtitle = Locales.Get(ChestOpeningSubtitle),
                body = Locales.Get(ChestOpeningMessage),

                userInfo = new Dictionary<string, object>
                {
                    { "type", NotificationType.ChestOpening },
                    { "index", index },
                    { "date", box.timer }
                },

                date = box.timer,

                smallIcon = ChestOpeningSmallIcon,
                largeIcon = ChestOpeningLargeIcon,

                categoryId = EM_NotificationsConstants.UserCategory_notification_category_chests_opening
            };

            if (!IsValidNotification(nData, ValidationType.Time))
			{
                return;
			}

            Notifications.ScheduleLocalNotification(nData.date, GetNotificationContent(nData));

            ScheduleLocalNotificationLog(nData);
        }
        public void ChestOpeningLocalNotificationCancel(int index)
        {
            NotificationPanding nPanding = new NotificationPanding();
            nPanding.GetPendingLocalNotifications("type", NotificationType.ChestOpening).
                GetPendingLocalNotifications("index", index);

            foreach (var request in NotificationPanding.localPendingRequests)
            {
                Notifications.CancelPendingLocalNotification(request.id);
            }
        }
        #endregion

        #region ChestReminderLocalNotification
        public void ChestReminderLocalNotificationStart()
        {
            if (!ChestReminderEnabled)
            {
                return;
            }

            ChestReminderLocalNotificationCancel();

            NotificationData nData = new NotificationData
            {
                type = NotificationType.ChestReminder,

                title = Locales.Get(ChestReminderTitle),
                subtitle = Locales.Get(ChestReminderSubtitle),
                body = Locales.Get(ChestReminderMessage),

                userInfo = new Dictionary<string, object>
                {
                    { "type", NotificationType.ChestReminder },
                    { "date", DateTime.UtcNow.AddMinutes(ChestReminderDurationMinutes) }
                },

                date = DateTime.UtcNow.AddMinutes(ChestReminderDurationMinutes),

                smallIcon = ChestReminderSmallIcon,
                largeIcon = ChestReminderLargeIcon,

                categoryId = EM_NotificationsConstants.UserCategory_notification_category_chests_reminder
            };

            if (!IsValidNotification(nData, ValidationType.Time))
            {
                return;
            }

            Notifications.ScheduleLocalNotification(nData.date, GetNotificationContent(nData));

            ScheduleLocalNotificationLog(nData);
        }
        public void ChestReminderLocalNotificationCancel()
        {
            NotificationPanding nPanding = new NotificationPanding();
            nPanding.GetPendingLocalNotifications("type", NotificationType.ChestReminder);

            foreach (var request in NotificationPanding.localPendingRequests)
            {
                Notifications.CancelPendingLocalNotification(request.id);
            }
        }
        #endregion

        #region DailyLocalNotification
        public void DailyLocalNotificationStart()
        {
            if (!DailyEnabled)
            {
                return;
            }

            NotificationPanding nPanding = new NotificationPanding();
            nPanding.GetPendingLocalNotifications("type", NotificationType.Daily);

            bool rePending = false;
            bool pending = true;

            foreach (var request in NotificationPanding.localPendingRequests)
            {
                if (request.content.userInfo.TryGetValue("date", out object dateValue))
                {
                    if ((DateTime)dateValue != Profile.dailyDeals.nextGeneration)
                    {
                        Notifications.CancelPendingLocalNotification(request.id);
                        rePending = true;
                        pending = true;
                    }
                    else if (!rePending)
                    {
                        pending = false;
                    }
                }
            }

            if (!pending && !rePending)
            {
                return;
            }

            NotificationData nData = new NotificationData
            {
                type = NotificationType.Daily,

                title = Locales.Get(DailyTitle),
                subtitle = Locales.Get(DailySubtitle),
                body = Locales.Get(DailyMessage),

                userInfo = new Dictionary<string, object>
                {
                    { "type", NotificationType.Daily },
                    { "date", Profile.dailyDeals.nextGeneration }
                },

                date = Profile.dailyDeals.nextGeneration,

                smallIcon = DailySmallIcon,
                largeIcon = DailyLargeIcon,

                categoryId = EM_NotificationsConstants.UserCategory_notification_category_daily
            };

            if (!IsValidNotification(nData, ValidationType.Time))
            {
                return;
            }

            Notifications.ScheduleLocalNotification(nData.date, GetNotificationContent(nData));

            ScheduleLocalNotificationLog(nData);
        }
        public void DailyLocalNotificationCancel()
        {
            NotificationPanding nPanding = new NotificationPanding();
            nPanding.GetPendingLocalNotifications("type", NotificationType.Daily);

            foreach (var request in NotificationPanding.localPendingRequests)
            {
                Notifications.CancelPendingLocalNotification(request.id);
            }
        }
        #endregion

        #region PlaymeLocalNotification
        public void PlaymeLocalNotificationStart()
        {
            if (!PlaymeEnabled)
            {
                return;
            }

            PlaymeLocalNotificationCancel();

            NotificationData nData = new NotificationData
            {
                type = NotificationType.Playme,

                title = Locales.Get(PlaymeTitle),
                subtitle = Locales.Get(PlaymeSubtitle),
                body = Locales.Get(PlaymeMessage),

                userInfo = new Dictionary<string, object>
                {
                    { "type", NotificationType.Playme },
                    { "date", DateTime.UtcNow.AddHours(PlaymeDurationHours) }
                },

                date = DateTime.UtcNow.AddHours(PlaymeDurationHours),

                smallIcon = ChestReminderSmallIcon,
                largeIcon = ChestReminderLargeIcon,

                categoryId = EM_NotificationsConstants.UserCategory_notification_category_playme
            };

            if (!IsValidNotification(nData, ValidationType.Time))
            {
                return;
            }

            Notifications.ScheduleLocalNotification(nData.date, GetNotificationContent(nData));

            ScheduleLocalNotificationLog(nData);
        }
        public void PlaymeLocalNotificationCancel()
        {
            NotificationPanding nPanding = new NotificationPanding();
            nPanding.GetPendingLocalNotifications("type", NotificationType.Playme);

            foreach (var request in NotificationPanding.localPendingRequests)
            {
                Notifications.CancelPendingLocalNotification(request.id);
            }
        }
        #endregion



        // Разрешение отправки по кастомным критериям
        private bool IsValidNotification(NotificationData nData, ValidationType vType)
        {


            return true;
        }

        // Update list pending notifications
        void UpdatePendingNotificationList()
        {
            NotificationPanding.GetPendingLocalNotifications();
        }

        // логи
        private void ScheduleLocalNotificationLog(NotificationData nData)
        {
            Debug.Log($"<color=red>PusNotifications push: </color>" + " type " + nData.type + ", time end " + nData.date);
        }
    }


    public enum NotificationType
    {
        ChestOpening,
        ChestReminder,
        Daily,
        Playme
    }
    
    public enum ValidationType
    {
        Time,
        SomeType01,
        SomeType02
    }

    public struct NotificationData
    {
        public NotificationType type;

        public string title;
        public string subtitle;
        public string body;

        public Dictionary<string, object> userInfo;

        public DateTime date;

        public string smallIcon;
        public string largeIcon;

        public string categoryId;
    }


    public class NotificationPanding
    {
        static public List<NotificationRequest> localPendingRequests;
        
        public NotificationPanding()
		{
            //GetPendingLocalNotifications();
        }

        public void Revert()
        {
            //GetPendingLocalNotifications();
        }

        public NotificationPanding GetPendingLocalNotifications(string userInfoKey, object userInfoValue)
        {
            List<NotificationRequest> result = new List<NotificationRequest>();

            if (localPendingRequests != null)
            {
                foreach (var request in localPendingRequests)
                {
                    if (request.content.userInfo.TryGetValue(userInfoKey, out object value))
                    {
                        if (value == userInfoValue)
                        {
                            result.Add(request);
                        }
                    }
                }
            }

            localPendingRequests = result;

            return this;
        }

        static public void GetPendingLocalNotifications()
        {
            Notifications.GetPendingLocalNotifications(pendingRequests =>
            {
                localPendingRequests = new List<NotificationRequest>(pendingRequests);
            });
        }
    }
}