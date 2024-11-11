using Legacy.Database;
using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using Firebase.Crashlytics;

namespace Legacy.Client
{
    public class FirebaseSender : AnalyticsSender
    {

        ProfileInstance profile;

        private string GameUID => profile.index.ToString();

        private Firebase.FirebaseApp firebase = null;

        public override void Init()
        {
            profile = ClientWorld.Instance.Profile;
            Crashlytics.SetUserId(GameUID);
            FirebaseAnalytics.SetUserId(GameUID);
            FirebaseAnalytics.SetUserProperty(CustomEventAttr.UID, GameUID);
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    firebase = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }        

        public override void SendEvent(CustomEventTypes eventType, Dictionary<string, object> eventData)
        {
            //if (firebase != null)
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
                    Debug.LogError($"Firebase Error [{eventType.ToString()}] - {ex.Message}");
                }
            }
        }

        private void OnArenaReached(Dictionary<string, object> eventData)
        {
        }

        private void OnFirstPayment(Dictionary<string, object> eventData)
        {
            var transactionID = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_TRANSACTION_ID, eventData));
            var price = Convert.ToSingle(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, eventData));
            var payment_name = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_NAME, eventData));
            var currency = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, eventData));

            Parameter[] eparams = new Parameter[] {
                new Parameter(CustomEventAttr.UID, GameUID),
                new Parameter(CustomEventAttr.REAL_PAYMENT_TRANSACTION_ID, transactionID),
                new Parameter(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, price),
                new Parameter(CustomEventAttr.REAL_PAYMENT_IN_APP_NAME, payment_name),
                new Parameter(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, currency)
            };

            FirebaseAnalytics.LogEvent($"first_payment", eparams);
        }

        private void OnRealPayment(Dictionary<string, object> eventData)
        {
            var transactionID = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_TRANSACTION_ID, eventData));
            var price = Convert.ToSingle(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, eventData));
            var payment_name = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_NAME, eventData));
            var currency = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, eventData));
            Parameter[] eparams = new Parameter[] {
                new Parameter(CustomEventAttr.UID, GameUID),
                new Parameter(CustomEventAttr.REAL_PAYMENT_TRANSACTION_ID, transactionID),
                new Parameter(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, price),
                new Parameter(CustomEventAttr.REAL_PAYMENT_IN_APP_NAME, payment_name),
                new Parameter(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, currency)
            };

            FirebaseAnalytics.LogEvent($"real_payment", eparams);

        }

        private void OnTutorialEvent(Dictionary<string, object> eventData)
        {
            int tutorial_step = Convert.ToInt32(GetEventParam(CustomEventAttr.TUTORIAL_STEP, eventData));

            if (tutorial_step == (int)AnalyticTutorialStep.Start)
            {
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialBegin);
            }
            else if (tutorial_step == (int)AnalyticTutorialStep.Finish)
            {
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventTutorialComplete);
            }

            Parameter[] eparams = new Parameter[] {
                new Parameter(CustomEventAttr.UID, GameUID),
                new Parameter(CustomEventAttr.TUTORIAL_STEP, tutorial_step.ToString())
            };

            FirebaseAnalytics.LogEvent($"tutorial_step_{(AnalyticTutorialStep)tutorial_step}", eparams);
            //FirebaseAnalytics.LogEvent($"tutorial_step", eparams);           

        }

    }
}
