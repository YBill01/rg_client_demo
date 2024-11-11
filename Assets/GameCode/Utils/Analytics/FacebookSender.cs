using Facebook.Unity;
using Legacy.Database;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class FacebookSender : AnalyticsSender
    {

        public const string EVENT_ARENA_REACHED = "arena_reached";

        // Unity will call OnApplicationPause(false) when an app is resumed
        // from the background
        void OnApplicationPause(bool pauseStatus)
        {
            // Check the pauseStatus to see if we are in the foreground
            // or background
            if (!pauseStatus)
            {
                //app resume
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                }
                else
                {
                    //Handle FB.Init
                    FB.Init(() => {
                        FB.ActivateApp();
                    });
                }
            }
        }

        public override void Init()
        {
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() => {
                    FB.ActivateApp();
                });
            }
        }

        public override void SendEvent(CustomEventTypes eventType, Dictionary<string, object> eventData)
        {
            try
            {
                switch (eventType)
                {
                    case CustomEventTypes.TutorialStep:
                        OnTutorialEvent(eventData);
                        break;
                    case CustomEventTypes.RealPayment:
                        OnRealPayment(eventData);
                        break;
                    case CustomEventTypes.FirstPayment:
                        OnFirstPayment(eventData);
                        break;
                    case CustomEventTypes.ArenaReached:
                        OnArenaReached(eventData);
                        break;

                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Facebook Error [{eventType.ToString()}] - {ex.Message}");
            }
        }

        private void OnArenaReached(Dictionary<string, object> eventData)
        {
            FB.LogAppEvent(
                EVENT_ARENA_REACHED,
                parameters: eventData
            );
        }

        private void OnFirstPayment(Dictionary<string, object> eventData)
        {

        }

        private void OnRealPayment(Dictionary<string, object> eventData)
        {
            FB.LogPurchase(
              Convert.ToSingle(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, eventData)),
              Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, eventData)),
              eventData
            );
        }

        private void OnTutorialEvent(Dictionary<string, object> eventData)
        {
            long tutorial_step = Convert.ToInt64(GetEventParam(CustomEventAttr.TUTORIAL_STEP, eventData));

            if(tutorial_step == (long)AnalyticTutorialStep.Finish)
            {
                FB.LogAppEvent(AppEventName.CompletedTutorial);
            }            
        }
    }
}
