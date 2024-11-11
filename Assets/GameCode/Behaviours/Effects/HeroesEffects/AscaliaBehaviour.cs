using Legacy.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AscaliaBehaviour : MonoBehaviour
{

    public float PiercingArrowSpeed = 100f;
    public GameObject PiercingArrow;
    public GameObject PiercingCharge;
    public GameObject RayContainer;
    //public Animator BowAnimator;
    public ParticleSystem AscaliaVolleys;

    private Transform Target;

    bool ray = false;
    bool piercing = false;

    private Transform RayPoint;

    void Update()
    {
        if (ray)
        {
            if (RayPoint)
            {
                var p = RayPoint.position;
                p.y = 2f;
                if (RayContainer)
                {
                    RayContainer.transform.LookAt(p);
                }
            }
        }
        if (RayContainer)
        {
            RayContainer.SetActive(ray);
        }
        if (piercing)
        {
            if (PiercingArrow)
            {
                PiercingArrow.transform.Translate(Vector3.forward * PiercingArrowSpeed * Time.deltaTime);
            }
        }
    }

    public void ChargeCloud()
    {
        GetComponent<RangeHitEffect>().Charge(true);
    }

    public void Fire(int ArrowNumber)
    {
        AscaliaVolleys.gameObject.transform.position = GetComponent<RangeHitEffect>().HitStartPosition.transform.position;
        //if (GetComponent<MinionPanel>().IsEnemy)
        //{
        //AscaliaVolleys.gameObject.transform.localScale = new Vector3(-1, 1, 1);
        //}
        AscaliaVolleys.Play();
        GetComponent<RangeHitEffect>().Charge(false);
        if (ArrowNumber != 4)
        {
            GetComponent<RangeHitEffect>().Charge(true);
        }
    }

    public void ChargeStart()
    {
        PiercingCharge.SetActive(true);
        PiercingCharge.GetComponent<MeshRenderer>().enabled = true;
        LegacyHelpers.TurnParticlesOn(PiercingCharge);
    }
    public void PiercingFinish()
    {
        PiercingCharge.GetComponent<MeshRenderer>().enabled = false;
        LegacyHelpers.TurnParticlesOff(PiercingCharge);
    }

    public void CloudSkillFinished()
    {
        PiercingCharge.GetComponent<MeshRenderer>().enabled = false;
        LegacyHelpers.TurnParticlesOff(PiercingCharge);
    }

    internal void Piercing(Transform target)
    {
        Target = target;
        ChargeBow(true);
        //PiercingCharge.SetActive(true);

    }

    public void PiercingStartRay()
    {
        ray = true;
    }
    public void PiercingFire()
    {
        PiercingArrow.transform.position = GetComponent<RangeHitEffect>().HitStartPosition.position;
        ChargeBow(false);
        RayContainer.SetActive(false);
        PiercingCharge.SetActive(false);
        PiercingCharge.GetComponent<MeshRenderer>().enabled = false;
        LegacyHelpers.TurnParticlesOff(PiercingCharge);
        PiercingArrow.SetActive(true);
        piercing = true;

        ray = false;
        StartCoroutine("PiercingArrowFire");
    }

    IEnumerator PiercingArrowFire()
    {
        yield return new WaitForSeconds(0.5f);
        piercing = false;
        PiercingArrow.SetActive(false);
    }

    internal void ChargeBow(bool Switch)
    {
        //BowAnimator.Play(Switch ? "BowChargeIn" : "Fire");
    }

    internal void RayTo(Transform point)
    {
        ray = true;
        RayPoint = point;
    }

    internal void RayStop()
    {
        ray = false;
    }
}
