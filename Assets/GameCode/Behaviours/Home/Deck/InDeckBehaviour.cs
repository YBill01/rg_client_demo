using DG.Tweening;
using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InDeckBehaviour : MonoBehaviour
{
    public BinaryCard card;

    private Vector3 currentScale = Vector3.one;
    private Vector3 currenPosition = Vector3.zero;

    private Sequence scaleTween;
    private Sequence moveTween;
    private Ease typeEase = Ease.OutExpo;
    private float speedTab = .25f;
    private float speedChange = .5f;
    private CardViewBehaviour cardView;
    public void SetBinary(BinaryCard _card)
    {
        card = _card;
    }

    public void SetPoolPosition()
    {
        scaleTween.Kill(); ;
        moveTween.Kill();
        //isMove = false;
      //  isScale = false;
        transform.localPosition = Vector3.zero;
    }

    [SerializeField, Range(0.0f, 1.0f)]
    private float LerpPositionSpeed;

    [SerializeField, Range(0.0f, 1.0f)]
    private float LerpScaleSpeed;

    public void SetScale(float scale)
    {
        currentScale.x = scale;
        currentScale.y = scale;
        currentScale.z = scale;
        if (transform.localScale != currentScale /*&& !isScale*/)
        {
            if (cardView == null)
            {
                cardView = GetComponent<CardViewBehaviour>();
            }
             StartScale();
        }
    }
    public void SetPosition(Vector3 newPos)
    {
        currenPosition = newPos;
        if (transform.localPosition != currenPosition /*&& !isMove*/)
        {
            if (cardView == null)
            {
                cardView = GetComponent<CardViewBehaviour>();
            }

            StartMove();
        }
    }
  
  /*  void Update()
    {
        if(transform.localScale != currentScale && !isScale)
        {
            if (cardView == null)
            {
                cardView = GetComponent<CardViewBehaviour>();
            }
            cardView.StopJump();
            isScale = true;
            StartScale();
        }
            
        if(transform.localPosition != currenPosition && !isMove)
        {
            if (cardView == null)
            {
                cardView = GetComponent<CardViewBehaviour>();
            }
            cardView.StopJump();
            isMove = true;
            StartMove();
        }
       /*   transform.localScale = Vector3.Lerp(transform.localScale, currentScale, LerpScaleSpeed);
        if (this.transform.parent.GetComponent<DeckCardBehaviour>())
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, currenPosition, LerpPositionSpeed);
        
    }
*/
    public bool typeAnim = true;
    private void StartMove()
    {
        /* if(WindowManager.Instance.CurrentWindow is DecksWindowBehaviour)  // для настройки рантайм
         {
             DecksWindowBehaviour deks = WindowManager.Instance.CurrentWindow.GetComponent<DecksWindowBehaviour>();
             if (deks)
             {
                 typeEase = deks.typeEase;
                 speedTab = deks.speedTab;
                 speedChange = deks.speedChange;
             }
         }*/
        if (moveTween == null)
            moveTween = DOTween.Sequence();
        float time = speedTab;
        if (!typeAnim) time = speedChange;
        scaleTween.Append(
            transform.DOLocalMove(currenPosition, time)
            .SetEase(typeEase)
            .OnComplete(() => {}));
    }

    private void StartScale()
    {
        if (scaleTween == null)
            scaleTween = DOTween.Sequence();
        scaleTween.Append(
        transform.DOScale(currentScale, LerpScaleSpeed)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => {}));
    }
}
