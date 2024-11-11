using Legacy.Client;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class TutorialPointerBehaviour : MonoBehaviour
{
    //public GameObject dragFrame;
    public BoxCollider plane;
    public Vector3 startPosition;
    public Transform sceneUnit;
    public float moveSpeed = 20f;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SkeletonAnimation spineAnimator;

    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>(), 
            startPosition, 
            BattleInstanceInterface.instance.UICamera, 
            out Vector2 statrps);

        rectTransform.anchoredPosition = statrps;
    }

    private bool diyng;
    void Update()
    {
        if (!move) return;
        if (!sceneUnit) return;
        
        UpdatePosition();

        
    }

    private void UpdatePosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>(), 
            startPosition, 
            BattleInstanceInterface.instance.UICamera, 
            out Vector2 statrps);

        var screenPoint = Camera.main.WorldToScreenPoint(sceneUnit.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>(),
            screenPoint,
            BattleInstanceInterface.instance.UICamera,
            out Vector2 finPos);

        var delta = finPos - statrps;
        var totalTime = delta.magnitude / moveSpeed;

        var np = Vector3.Lerp(statrps, finPos, progress);
        np.z = 0;
        rectTransform.anchoredPosition = np;
        progress += Time.deltaTime / totalTime;

        if (progress >= 1)
        {
            animator.SetTrigger("StopAnimation");
            move = false;
        }
    }

    private bool move;
    private void TapFinished()
    {
        move = true;
        animator.ResetTrigger("MoveAnimation");
        animator.SetTrigger("MoveAnimation");
    }

    private void StartLive()
    {
        progress = 0;
        move = false;
        UpdatePosition();
    }

    private float progress;
    private void OnEnable()
    {
        animator.ResetTrigger("StartAnimation");
        animator.ResetTrigger("MoveAnimation");
        animator.ResetTrigger("StopAnimation");
        animator.SetTrigger("StartAnimation");
        
        rectTransform = GetComponent<RectTransform>();
        progress = 0;
        Vector2 statrps;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>(), 
            startPosition, 
            BattleInstanceInterface.instance.UICamera, 
            out statrps);

        rectTransform.anchoredPosition = statrps;
    }

    private void OnDisable()
    {
        animator.ResetTrigger("StartAnimation");
        animator.ResetTrigger("MoveAnimation");
        animator.ResetTrigger("StopAnimation");
        progress = 0;
        
    }

    private void HideDone()
    {
        animator.SetTrigger("StartAnimation");
    }

    public void DestroyItself()
    {
        animator.SetTrigger("Close");
    }

    public void FinnallyDie()
    {
        DestroyImmediate(gameObject);
    }

    private void PlayHandTap()
    {
        spineAnimator.AnimationName = "click-start";
    }

    private void PlayHandTapOff()
    {
        spineAnimator.AnimationName = "click-completion";
    }
}
