﻿using UnityEngine;
using System.Collections;


public enum MessageState
{
    OK,
    YES,
    NO,
    RATED,
    REMIND,
    DECLINED,
    CLOSED
}


public class PopupView : MonoBehaviour
{

    #region UNITY_DEFAULT_CALLBACKS

    void OnEnable()
    {
        // Register all Delegate event listener
        //AndroidRateUsPopUp.onRateUSPopupComplete += OnRateUSPopupComplete;
        AndroidDialog.onDialogPopupComplete += OnDialogPopupComplete;
        AndroidMessage.onMessagePopupComplete += OnMessagePopupComplete;
    }

    void OnDisable()
    {
        // Deregister all Delegate event listener
        //AndroidRateUsPopUp.onRateUSPopupComplete -= OnRateUSPopupComplete;
        AndroidDialog.onDialogPopupComplete -= OnDialogPopupComplete;
        AndroidMessage.onMessagePopupComplete -= OnMessagePopupComplete;
    }

    #endregion

    #region DELEGATE_EVENT_LISTENER

    // Raise when click on any button of rate popup
    void OnRateUSPopupComplete(MessageState state)
    {
        switch (state)
        {
            case MessageState.RATED:
                Debug.Log("Rate Button pressed");
                break;
            case MessageState.REMIND:
                Debug.Log("Remind Button pressed");
                break;
            case MessageState.DECLINED:
                Debug.Log("Declined Button pressed");
                break;
        }
    }

    // Raise when click on any button of Dialog popup
    void OnDialogPopupComplete(MessageState state)
    {
        switch (state)
        {
            case MessageState.YES:
                Debug.Log("Yes button pressed");
                break;
            case MessageState.NO:
                Debug.Log("No button pressed");
                break;
        }
    }

    // Raise when click on ok button of message popup
    void OnMessagePopupComplete(MessageState state)
    {
        Debug.Log("Ok button Clicked");
    }

    #endregion

    #region BUTTON_EVENT_LISTENER

    // Dialog Button click event
    public void OnDialogPopUp()
    {
        NativeDialog dialog = new NativeDialog("Oops =(", "Connection server lost");
        //dialog.SetUrlString("");
        dialog.init();
    }

    // Rate Button click event
    public void OnRatePopUp()
    {
        //NativeRateUS ratePopUp = new NativeRateUS("Like this game?", "Please rate to support future updates!");
        //ratePopUp.SetAppLink(gameLink);
        //ratePopUp.InitRateUS();	
    }

    // Message Button click event
    public void OnMessagePopUp()
    {
        NativeMessage msg = new NativeMessage("Opps..", "Connection server lost");
    }

    #endregion
}
