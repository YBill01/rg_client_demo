using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Legacy.Client;

public class OnBridgeCapturingEffectsManager : MonoBehaviour
{
    [SerializeField] private RectTransform _leftSkillRectTransform;
    [SerializeField] private RectTransform _rightSkillRectTransform;
    [Space]
    [SerializeField] private SpeedUpSkilArrowController arrow_1;
    [SerializeField] private SpeedUpSkilArrowController arrow_2;
    [Header("Particles Systems")]
    [SerializeField] private ParticleSystem VFX_FlashOnBridge_Top_blue; // on the ground
    [SerializeField] private ParticleSystem VFX_FlashOnBridge_Bottom_blue; // on the ground
    [Space]
    [SerializeField] private ParticleSystem VFX_FlashOnBridge_Top_red;// on the ground
    [SerializeField] private ParticleSystem VFX_FlashOnBridge_Bottom_red;// on the ground
    [Space]
    [SerializeField] private ParticleSystem VFX_FlyingStar_1;// in the air
    [SerializeField] private ParticleSystem VFX_FlyingStar_2;// in the air
    [SerializeField] private ParticleSystem VFX_FlyingStar_3;// in the air
    [SerializeField] private ParticleSystem VFX_FlyingStar_4;// in the air
    [Space]
    [SerializeField] private ParticleSystem VFX_FlashOnSkillButtonRegular_1;//on the button
    [SerializeField] private ParticleSystem VFX_FlashOnSkillButtonRegular_2;//on the button
    [Space]
    [SerializeField] private ParticleSystem VFX_FlashOnSkillButtonBoosted_1;//on the button
    [SerializeField] private ParticleSystem VFX_FlashOnSkillButtonBoosted_2;//on the button
    [Space]
    [SerializeField] private ParticleSystem VFX_FlashOnSkillButtonSpeedUpEffect_1;//on the button
    [SerializeField] private ParticleSystem VFX_FlashOnSkillButtonSpeedUpEffect_2;//on the button
    [Space]
    [SerializeField] private Canvas canvas;
    [Header("Flying Particles Prefs")]
    [SerializeField] private float flightoutFlightSpeed;
    [SerializeField] private float parabolaFlightSpeed;

    [SerializeField] private float startPosHightOnTheGroundShift;
    [SerializeField] private float delayBeforeParticlesStartToFly = 0.2f;

    [SerializeField] private float flightputHight_Top;
    [SerializeField] private float flightputHight_Bot;
    [SerializeField] private float parabolaHight;

    [SerializeField] private AnimationCurve flightoutSpeedCurve;
    [SerializeField] private AnimationCurve parabolaSpeedCurve;

    private Vector2 _uiCamTopBridgePos;
    private Vector2 _uiCamBottomBridgePos;
    private BattleSkillViewBehaviour _firstSkillButtonScript;
    private BattleSkillViewBehaviour _secondSkillButtonScript;

    private int _bridgesCapturedCount
    {
        get
        {
            var counter = 0;
            if (topBridge == BridgeOwnerTipe.Player)
                counter++;
            if (botBridge == BridgeOwnerTipe.Player)
                counter++;
            return counter;
        }
    }
    private BridgeOwnerTipe topBridge = BridgeOwnerTipe.None;
    private BridgeOwnerTipe botBridge = BridgeOwnerTipe.None;

    private enum BridgeOwnerTipe 
    { 
        None = 0,
        Player = 1,
        Enemy = 2
    }

    #region init
    void OnEnable()
    {
        Init();
    }
    private void Init()
    {
        StartCoroutine(LateInit());
    }
    private IEnumerator LateInit()
    {
        yield return null;
        CoordsInit();
        _firstSkillButtonScript  = _leftSkillRectTransform.GetComponent<BattleSkillViewBehaviour>();
        _secondSkillButtonScript = _rightSkillRectTransform.GetComponent<BattleSkillViewBehaviour>();
    }
    private void CoordsInit()
    {
        var _topBridgePosForFlightStart = VFX_FlashOnBridge_Top_blue.transform.position
            + new Vector3(0, startPosHightOnTheGroundShift, 0);
        var _bottomBridgePosForFlightStart = VFX_FlashOnBridge_Bottom_blue.transform.position
            + new Vector3(0, startPosHightOnTheGroundShift, 0);

        _uiCamTopBridgePos      = FromWorldToUiCameraScreenCoordsTransformer(_topBridgePosForFlightStart);
        _uiCamBottomBridgePos   = FromWorldToUiCameraScreenCoordsTransformer(_bottomBridgePosForFlightStart);
    }
    #endregion

    #region coreLogic
    public void StartSequenceOfEffectsPlayingOnBridgeCapturing(bool isTop, bool isEnemy)
    {
        
        BridgeCapturedCounter(isTop, isEnemy);
        if (isTop)
        {
            if (isEnemy)
            {
                PlayOnBridgeVFX(VFX_FlashOnBridge_Top_red);
                PlayVFXonSkillsButtons(true, SkillButton.Both);
            }
            else
            {
                PlayOnBridgeVFX(VFX_FlashOnBridge_Top_blue);
                StartFlightVFXtoSkillsPanel(_uiCamTopBridgePos, true);
            }
        }
        else
        {
            if (isEnemy)
            {
                PlayOnBridgeVFX(VFX_FlashOnBridge_Bottom_red);
                PlayVFXonSkillsButtons(true, SkillButton.Both);
            }
            else
            {
                PlayOnBridgeVFX(VFX_FlashOnBridge_Bottom_blue);
                StartFlightVFXtoSkillsPanel(_uiCamBottomBridgePos, false);
            }
        }
    }
    private void PlayOnBridgeVFX(ParticleSystem onBridgeVFX)
    {
        onBridgeVFX.Play();
    }
    private void StartFlightVFXtoSkillsPanel(Vector2 fromWhereToStartFlight, bool isToop)
    {
        var whereToFly = WhereToFlyLogic();

        if (isToop)
        {
            StartCoroutine(VFX_FlightCoroutine(fromWhereToStartFlight, whereToFly, VFX_FlyingStar_1, SkillButton.Both, true));
            //StartCoroutine(VFX_FlightCoroutine(fromWhereToStartFlight, _leftSkillRectTransform, VFX_FlyingStar_1, SkillButton.LeftButton));
            //StartCoroutine(VFX_FlightCoroutine(fromWhereToStartFlight, _rightSkillRectTransform, VFX_FlyingStar_2, SkillButton.RightButton));
        }
        else
        {
            StartCoroutine(VFX_FlightCoroutine(fromWhereToStartFlight, whereToFly, VFX_FlyingStar_2, SkillButton.Both, false));
            //StartCoroutine(VFX_FlightCoroutine(fromWhereToStartFlight, _leftSkillRectTransform, VFX_FlyingStar_3, SkillButton.LeftButton));
            //StartCoroutine(VFX_FlightCoroutine(fromWhereToStartFlight, _rightSkillRectTransform, VFX_FlyingStar_4, SkillButton.RightButton));
        }
    }
    private IEnumerator VFX_FlightCoroutine(Vector2 start, RectTransform skillRect, ParticleSystem particleSyst, SkillButton whichButton, bool isTop)
    {
        yield return new WaitForSeconds(delayBeforeParticlesStartToFly);

        var vfxTrans            = particleSyst.transform;
        vfxTrans.parent         = canvas.transform;
        vfxTrans.localPosition  = start;
        vfxTrans.parent         = skillRect.transform;
        var startLocalPos       = vfxTrans.localPosition;

        var highPos = startLocalPos;
        if (isTop)
            highPos = highPos.AddCoords(0, flightputHight_Top, 0);
        else
            highPos = highPos.AddCoords(0, flightputHight_Bot, 0);

        var distToFly_flightout = Vector2.Distance(startLocalPos, highPos);

        var midPosition         = Vector2.Lerp(highPos, Vector2.zero, Random.Range(0.35f, 0.75f));
        midPosition             += new Vector2(0, parabolaHight * Random.Range(0.75f, 1.25f));
        var distToFly_parabola  = Vector2.Distance(highPos, Vector2.zero);
                                
        var flightFlightoutTime = distToFly_flightout / flightoutFlightSpeed;
        var flightParabolaTime  = distToFly_parabola / parabolaFlightSpeed;
        
        particleSyst.Play();

        var overallTime = flightFlightoutTime + flightParabolaTime;

        StartCoroutine(ScaleUpRoutine(overallTime, vfxTrans));
        yield return StartCoroutine(PosLerpRout(startLocalPos, highPos, flightFlightoutTime, vfxTrans, particleSyst));
        yield return StartCoroutine(PosLerpRoutineParabola(highPos, midPosition, Vector3.zero, flightParabolaTime, vfxTrans, particleSyst));
        particleSyst.Stop();

        PlayVFXonSkillsButtons(false, whichButton);
    }
    private IEnumerator PosLerpRoutineParabola(Vector3 A, Vector3 B, Vector3 C, float lasting, Transform trans, ParticleSystem vfx)
    {
        var partsStopped = false;

        var timer = 0f;
        while (timer < lasting)
        {
            if (_bridgesCapturedCount == 0 && !partsStopped)
            {
                vfx.Stop();
                partsStopped = true;
            }

            var progress = timer / lasting;
            var curveProgress = parabolaSpeedCurve.Evaluate(progress);

            var start = Vector3.Lerp(A, B, curveProgress);
            var finish = Vector3.Lerp(B, C, curveProgress);
            trans.localPosition = Vector3.Lerp(start, finish, curveProgress);

            yield return null;
            timer += Time.deltaTime;
        }
        trans.localPosition = C;
    }
    private IEnumerator PosLerpRout(Vector3 A, Vector3 B, float lasting, Transform trans, ParticleSystem vfx)
    {
        var partsStopped = false;
        
        var timer = 0f;
        while (timer < lasting)
        {
            if (_bridgesCapturedCount == 0 && !partsStopped)
            {
                vfx.Stop();
                partsStopped = true;
            }

            var progress = timer / lasting;
            var curveProgress = flightoutSpeedCurve.Evaluate(progress);

            trans.localPosition = Vector3.Lerp(A, B, curveProgress);

            yield return null;
            timer += Time.deltaTime;
        }
        trans.localPosition = B;
    }
    private IEnumerator ScaleUpRoutine(float lasting, Transform trans)
    {
        var finScale = trans.localScale;
        var startScale = new Vector3(finScale.x / 4, finScale.y / 4, finScale.z / 4);

        var timer = 0f;
        while (timer < lasting)
        {
            var progress = timer / lasting;
            trans.localScale = Vector3.Lerp(startScale, finScale, progress);

            yield return null;
            timer += Time.deltaTime;
        }
        trans.localScale = finScale;
    }
    private void PlayVFXonSkillsButtons(bool byEnemy, SkillButton whichButton)
    {
        if (_bridgesCapturedCount > 0 && !byEnemy)
            PlaySpeedUpEffect(whichButton);

        if (_bridgesCapturedCount <= 0)
            StopVFXOnButtons(whichButton);
        if (_bridgesCapturedCount == 1)
            PlayRegularButtonsHighlight(whichButton);
        if (_bridgesCapturedCount == 2)
            PlayBoostedButtonsHighlight(whichButton);
    }
    #endregion

    #region tools 
    private Vector2 FromWorldToUiCameraScreenCoordsTransformer(Vector3 position)
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(position);
        Vector2 result;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>(), screenPoint, BattleInstanceInterface.instance.UICamera, out result);

        return result;
    }
    private void PlaySpeedUpEffect(SkillButton whichButton)
    {
        switch (whichButton)
        {
            case SkillButton.Both:
                VFX_FlashOnSkillButtonSpeedUpEffect_1.Play();
                VFX_FlashOnSkillButtonSpeedUpEffect_2.Play();
                break;
            case SkillButton.LeftButton:
                VFX_FlashOnSkillButtonSpeedUpEffect_1.Play();
                break;
            case SkillButton.RightButton:
                VFX_FlashOnSkillButtonSpeedUpEffect_2.Play();
                break;
        }
    }
    private void StopVFXOnButtons(SkillButton whichButton)
    {
        switch (whichButton)
        {
            case SkillButton.Both:
                VFX_FlashOnSkillButtonBoosted_1.Stop();
                VFX_FlashOnSkillButtonBoosted_2.Stop();
                VFX_FlashOnSkillButtonRegular_1.Stop();
                VFX_FlashOnSkillButtonRegular_2.Stop();
                arrow_1.HireArrow();
                arrow_2.HireArrow();
                break;
            case SkillButton.LeftButton:
                VFX_FlashOnSkillButtonBoosted_1.Stop();
                VFX_FlashOnSkillButtonRegular_1.Stop();
                break;
            case SkillButton.RightButton:
                VFX_FlashOnSkillButtonBoosted_2.Stop();
                VFX_FlashOnSkillButtonRegular_2.Stop();
                break;
        }
    }
    private void PlayRegularButtonsHighlight(SkillButton whichButton)
    {
        switch (whichButton)
        {
            case SkillButton.Both:
                VFX_FlashOnSkillButtonBoosted_1.Stop();
                VFX_FlashOnSkillButtonBoosted_2.Stop();
                VFX_FlashOnSkillButtonRegular_1.Play();
                VFX_FlashOnSkillButtonRegular_2.Play();
                arrow_1.ShowArrow(1);
                arrow_2.ShowArrow(1);
                break;
            case SkillButton.LeftButton:
                VFX_FlashOnSkillButtonBoosted_1.Stop();
                VFX_FlashOnSkillButtonRegular_1.Play();
                break;
            case SkillButton.RightButton:
                VFX_FlashOnSkillButtonBoosted_2.Stop();
                VFX_FlashOnSkillButtonRegular_2.Play();
                break;
        }
    }
    private void PlayBoostedButtonsHighlight(SkillButton whichButton)
    {
        switch (whichButton)
        {
            case SkillButton.Both:
                VFX_FlashOnSkillButtonRegular_1.Stop();
                VFX_FlashOnSkillButtonRegular_2.Stop();
                VFX_FlashOnSkillButtonBoosted_1.Play();
                VFX_FlashOnSkillButtonBoosted_2.Play();
                arrow_1.ShowArrow(2);
                arrow_2.ShowArrow(2);
                break;
            case SkillButton.LeftButton:
                VFX_FlashOnSkillButtonRegular_1.Stop();
                VFX_FlashOnSkillButtonBoosted_1.Play();
                break;
            case SkillButton.RightButton:
                VFX_FlashOnSkillButtonRegular_2.Stop();
                VFX_FlashOnSkillButtonBoosted_2.Play();
                break;
        }
    }
    private void BridgeCapturedCounter(bool isTop, bool isEnemy)
    {
        if (isTop && isEnemy)   topBridge = BridgeOwnerTipe.Enemy;
        if (isTop && !isEnemy)  topBridge = BridgeOwnerTipe.Player;
        if (!isTop && isEnemy)  botBridge = BridgeOwnerTipe.Enemy;
        if (!isTop && !isEnemy) botBridge = BridgeOwnerTipe.Player;
    }
    private RectTransform WhereToFlyLogic()
    {
        var isLeftSkillReady = _firstSkillButtonScript.IsReady;
        var isRightSkillReady = _secondSkillButtonScript.IsReady;

        if (!isLeftSkillReady && isRightSkillReady)
            return _leftSkillRectTransform;
        else if (isLeftSkillReady && !isRightSkillReady)
            return _rightSkillRectTransform;
        else
            return _leftSkillRectTransform;
    }
    private enum SkillButton
    {
        Both        = 0,
        LeftButton  = 1,
        RightButton = 2
    }
    #endregion
}
