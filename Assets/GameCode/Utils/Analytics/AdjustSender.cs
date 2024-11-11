using com.adjust.sdk;
using Legacy.Database;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class AdjustSender : AnalyticsSender
    {
        [SerializeField] Adjust adjustScript;
        void Awake()
        {
#if PRODUCTION
            adjustScript.environment = AdjustEnvironment.Production;
#endif
        }

        public override void Init()
        {
            Debug.Log($"AdjustSender. AdjustEnvironment is: {adjustScript.environment}");
        }

        const string LOGIN_TOKEN                    = "yrnw2t";
        const string COMPLETE_REGISTRATION_TOKEN    = "wtxkvm";
        const string FIRST_LOAD_COMPLETE_TOKEN      = "xdwqmv";
        const string TUTORIAL_START_TOKEN           = "9fw10r";
        const string TUTORIAL_FINISH_TOKEN          = "j79v45";
        const string FIRST_PAYMENT_TOKEN            = "tghc15";
        const string PAYMENT_TOKEN                  = "tnvjak";
        const string REACH_ARENA_1_TOKEN            = "lww3x1";
        const string REACH_ARENA_2_TOKEN            = "n8yo4y";
        const string REACH_ARENA_3_TOKEN            = "w5ip5n";

        public override void SendEvent(CustomEventTypes eventType, Dictionary<string, object> eventData)
        {
            try
            {
                GameDebug.Log("event: AdjustSender \n");
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
                    case CustomEventTypes.ArenaOpen:
                        OnArenaReached(eventData);
                        break;
                    case CustomEventTypes.Login:
                        OnLogin(eventData);
                        break;
                    case CustomEventTypes.CompleteRegistration:
                        OnCompleteRegistration(eventData);
                        break;
                    case CustomEventTypes.FirstLoadComplete:
                        OnFirstLoadComplete(eventData);
                        break;

                    default:
                        break;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Adjust Error [{eventType.ToString()}] - {ex.Message}");
            }
        }

        private void OnArenaReached(Dictionary<string, object> eventData)
        {
            AdjustEvent adjustEvent = null;
            byte number = Convert.ToByte(GetEventParam(CustomEventAttr.ARENA_NUMBER, eventData));
            switch (number)
            {
                case 1:
                    GameDebug.Log($"event: OnArenaReached param ARENA_NUMBER is {number}");
                    adjustEvent = new AdjustEvent(REACH_ARENA_1_TOKEN);
                    break;
                case 2:
                    GameDebug.Log($"event: OnArenaReached param ARENA_NUMBER is {number}");
                    adjustEvent = new AdjustEvent(REACH_ARENA_2_TOKEN);
                    break;
                case 3:
                    GameDebug.Log($"event: OnArenaReached param ARENA_NUMBER is {number}");
                    adjustEvent = new AdjustEvent(REACH_ARENA_3_TOKEN);
                    break;
                default:
                    break;                     
            }
            if(adjustEvent != null)
            {
                Adjust.trackEvent(adjustEvent);
            }
        }

        private void OnFirstPayment(Dictionary<string, object> eventData)
        {
            double revenue = double.Parse(Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, eventData)));
            string iso = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, eventData));
            string transactionID = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_TRANSACTION_ID, eventData));
            GameDebug.Log($"Adjust Sender Revenue: {revenue}");
            GameDebug.Log($"Adjust Sender ISO: {iso}");
            GameDebug.Log($"Adjust Sender TransactionID: {transactionID}");
            AdjustEvent adjustEvent = new AdjustEvent(FIRST_PAYMENT_TOKEN);
            /*
            adjustEvent.setRevenue(
                revenue,
                iso
            );*/
            adjustEvent.setTransactionId(transactionID);
            Adjust.trackEvent(adjustEvent);
        }

        private void OnRealPayment(Dictionary<string, object> eventData)
        {
            double revenue = double.Parse(Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_PRICE, eventData)));
            string iso = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_IN_APP_CURRENCY_ISO_CODE, eventData));
            string transactionID = Convert.ToString(GetEventParam(CustomEventAttr.REAL_PAYMENT_TRANSACTION_ID, eventData));
            GameDebug.Log($"Adjust Sender Revenue: {revenue}");
            GameDebug.Log($"Adjust Sender ISO: {iso}");
            GameDebug.Log($"Adjust Sender TransactionID: {transactionID}");
            AdjustEvent adjustEvent = new AdjustEvent(PAYMENT_TOKEN);
            adjustEvent.setRevenue(
                revenue,
                iso
            );
            adjustEvent.setTransactionId(transactionID);
            Adjust.trackEvent(adjustEvent);
        }

        private void OnTutorialEvent(Dictionary<string, object> eventData)
        {
            long tutorial_step = Convert.ToInt64(GetEventParam(CustomEventAttr.TUTORIAL_STEP, eventData));

            if (tutorial_step == (long)AnalyticTutorialStep.Start)
            {
                AdjustEvent adjustEvent = new AdjustEvent(TUTORIAL_START_TOKEN);
                Adjust.trackEvent(adjustEvent);
            }
            else if (tutorial_step == (long)AnalyticTutorialStep.Finish)
            {
                AdjustEvent adjustEvent = new AdjustEvent(TUTORIAL_FINISH_TOKEN);
                Adjust.trackEvent(adjustEvent);
            }            
        }

        private void OnLogin(Dictionary<string, object> eventData)
        {
            AdjustEvent adjustEvent = new AdjustEvent(LOGIN_TOKEN);
            Adjust.trackEvent(adjustEvent);
        }

        private void OnCompleteRegistration(Dictionary<string, object> eventData)
        {
            AdjustEvent adjustEvent = new AdjustEvent(COMPLETE_REGISTRATION_TOKEN);
            Adjust.trackEvent(adjustEvent);
        }

        private void OnFirstLoadComplete(Dictionary<string, object> eventData)
        {
            AdjustEvent adjustEvent = new AdjustEvent(FIRST_LOAD_COMPLETE_TOKEN);
            Adjust.trackEvent(adjustEvent);
        }
    }
}
