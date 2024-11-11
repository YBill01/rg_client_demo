using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaticColliders : MonoBehaviour
{
    public static bool campaign;

    private static StaticColliders _instance;

    [SerializeField]
    private List<ArenaBehaviour> locations;

    public static StaticColliders instance {
        get {
            return _instance;
        }
    }

    void Awake()
    {
        _instance = this;

        GroundPlane = new Plane(Vector3.up, 0);

        leftHeroRageParticles = LeftHeroRage.GetComponentsInChildren<ParticleSystem>(true);
        rightHeroRageParticles = RightHeroRage.GetComponentsInChildren<ParticleSystem>(true);

        LoadingGroup.Instance.LoadingEnabled.AddListener(DisableEventSystem);
        LoadingGroup.Instance.LoadingDisabled.AddListener(EnableEventSystem);
    }

    public GameObject TouchCollider;//leftHand from the Hero
    public GameObject AllyZone;//leftHand from the Hero
    public GameObject EnemyZone;//leftHand from the Hero
    public Transform UiBorder;//leftHand from the Hero
    public MeshCollider PlayerRightCollider;//leftHand from the Hero
    public MeshCollider PlayerLeftCollider;//rightHand from the Hero
    public MeshRenderer AllZone;//All location highlight
    public GameObject Bridge1;//Up bridge
    public GameObject Bridge2;//Down Bridge
    public Plane GroundPlane;
    public Transform BattleFieldNearBorder;
    public BattleFlagsBehaviour AllyFlagsBehaviour;
    public BattleFlagsBehaviour EnemyFlagsBehaviour;

    [SerializeField]
    private GameObject LeftHeroRage;
    [SerializeField]
    private GameObject RightHeroRage;

    private ParticleSystem[] leftHeroRageParticles;
    private ParticleSystem[] rightHeroRageParticles;
    public ParticleSystem[] LeftHeroRageParticles => leftHeroRageParticles;
    public ParticleSystem[] RightHeroRageParticles => rightHeroRageParticles;
    [SerializeField] private EventSystem eventSystem;
    public void EnableEventSystem() => eventSystem.enabled = true;
    public void DisableEventSystem() => eventSystem.enabled = false;

    internal void SetBridges(GameObject b1, GameObject b2)
    {
        Bridge1 = b1;
        Bridge2 = b2;
    }
    internal BattleFlagsBehaviour GetFlags(bool isEnemy)
    {
        if (isEnemy) return EnemyFlagsBehaviour;
        else return AllyFlagsBehaviour;
    }
}
