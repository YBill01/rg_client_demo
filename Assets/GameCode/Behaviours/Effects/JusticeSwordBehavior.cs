using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EZCameraShake.CameraShaker;

enum SwordStates
{
    Spawn,
    Fall,
    Grounded,
    Invisible
}
public class JusticeSwordBehavior : MonoBehaviour
{
    [Header("ShakeCamera Settings")]
    [SerializeField] ShakeSettings ShakeSettings;
    
    [Space]
    [SerializeField] private Transform galahardSword;
    [SerializeField] private float waitSwordTime;
    [SerializeField] private float waitBeforeGround;
    [Range(0, 2)] [SerializeField] private float swordSpeed;
    [Range(0, 2)] [SerializeField] private float intensity;
    private Vector3 startPosition;
    private SwordStates swordState;
    private SwordStates previousState;
    private bool canMove = false;
    private bool canFall = false;
    private float stateTime = 0;
    [Range(-20,10)] [SerializeField] private float bottomYPoint;

    private void OnEnable()
    {
        startPosition = galahardSword.localPosition;
        previousState = SwordStates.Invisible;
        SwitchState(SwordStates.Invisible);
        canFall = true;
    }

    private void Update()
    {
        if(swordState == SwordStates.Invisible && canFall && stateTime>= waitBeforeGround)
        {
            SwitchState(SwordStates.Spawn);
        }
        if (canMove && galahardSword.localPosition.y >= bottomYPoint)
        {
            SwitchState(SwordStates.Fall);
        }
        if (galahardSword.localPosition.y <= bottomYPoint)
        {
            if (previousState != SwordStates.Grounded)
            {
                SwitchState(SwordStates.Grounded);
                ShakeCamera();
            }
            SwitchState(SwordStates.Invisible, waitSwordTime);
        }

        StatesOrders();

        stateTime += Time.deltaTime;
    }

    private void SwitchState(SwordStates state, float waitTime = 0)
    {
        if (waitTime == 0 || stateTime >= waitTime && previousState != state)
        {
            previousState = swordState;
            swordState = state;
            stateTime = 0;
        }
    }

    private void StatesOrders()
    {
        switch (swordState)
        {
            case SwordStates.Spawn:
                galahardSword.gameObject.SetActive(true);
                canMove = true;
                break;
            case SwordStates.Fall:
                galahardSword.Translate(galahardSword.up * swordSpeed, Space.World);
                break;
            case SwordStates.Grounded:
                canMove = false;
                canFall = false;
                break;
            case SwordStates.Invisible:
                galahardSword.localPosition = startPosition;
                galahardSword.gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void ShakeCamera()
    {
        ShakeSettings.Shake();
    }
}
