using Legacy.Client;
using Legacy.Database;
using UnityEngine;

public class MinionPanel : MonoBehaviour
{
    [SerializeField] MinionInitBehaviour MinionInit;
    float minMinionDamage = 25f;
    public GameObject PanelPrefabHero;
    public GameObject PanelPrefabMinion;
    public bool PanelPrefab;

    public GameObject panel = null;
    private bool Spawned = false;
    public bool isNonTarget = false;

    public bool IsEnemy { get; private set; }
    public bool IsHero { get; private set; }

    public float maxHP;
    public MinionLayerType layer;
    public bool isInArgroRadius;

    private MinionHealthBar manager = null;
    internal void Spawn(byte level, float _maxHP = 100f, bool isHero = false, bool isEnemy = false, MinionLayerType _layer = MinionLayerType.Ground)
    {
        maxHP = _maxHP;
        layer = _layer;
        IsEnemy = isEnemy;
        IsHero = isHero;
        if (!isNonTarget)
        {
            GetComponent<DamageEffect>().SetHeroEnemy(isHero, isEnemy);

            panel = Instantiate(
                isHero ? PanelPrefabHero : PanelPrefabMinion,
                BattleInstanceInterface.instance.canvas.transform
            );
            SetPosition();
            panel.transform.SetAsFirstSibling();
            manager = (MinionHealthBar)panel.GetComponent(typeof(MinionHealthBar));
            manager.minionPanel = this;
            manager.SetLevel(level);
            manager.SetCollider(MinionInit.Binary.collider);
            manager.ActivateHealthBar(false);
            manager.Health = maxHP;
            panel.SetActive(isHero);
            if ((manager is HeroDamageBar))
                (manager as HeroDamageBar).soundManager = GetComponent<MinionSoundManager>();

            Spawned = true;
        }
    }

    public void SetMinionAgro(bool show)
    {
        if ((manager is MinionPanelManager))
        {
            isInArgroRadius = show;
            (manager as MinionPanelManager).SetExclamationPointIfIsInAgro(isInArgroRadius);

            SetAgroMaterial(show);
        }
    }

    private void SetAgroMaterial(bool isRed)
    {
        if (isRed)
        {
            MinionInit.MinionMatsBeh.SetDamageMaterials(true);
        }
        else
        {
            MinionInit.MinionMatsBeh.SetDefaultMaterials();
        }
    }

    internal void Delete()
    {
        Spawned = false;
        if (IsHero)
            (manager as HeroDamageBar).HPText.text = "";
        else
            DestroyImmediate(panel);
    }

    public void SetSliderValue(float value, MinionLayerType layer = MinionLayerType.Ground)
    {
        if (manager != null)
        {
            var difference = manager.Health - value;
            var shouldViewDamage = minMinionDamage <= difference;
            manager.Health = value;

            if (IsHero)
            {

                    (manager as HeroDamageBar).Health = value;
                    var starsCount = (int)(manager as HeroDamageBar).state;
                    StarsSingleton.Instance.SetStarsAndFlags(starsCount, !IsEnemy);
            }
            if (Spawned)
            {
                var val = Mathf.Clamp01(value / maxHP);
                manager.SetValue(val, shouldViewDamage, gameObject, layer);
            }
        }
    }

    public void HidePanel(bool hide)
    {
        if (!IsHero)
        {
            if (panel)
            {
                panel.SetActive(!hide);
            }
        }
    }
    public Rect limits = new Rect()
    {
        xMin = 50,
        xMax = 100,
        yMin = 100,
        yMax = 50
    };
    private void SetPosition()
    {
        if (PanelPosition != null)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(PanelPosition.position);
            screenPoint.x = Mathf.Clamp(screenPoint.x, limits.xMin, Camera.main.pixelWidth - limits.xMax);
            screenPoint.y = Mathf.Clamp(screenPoint.y, limits.yMin, Camera.main.pixelHeight - limits.yMax);
            Vector2 result;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(BattleInstanceInterface.instance.canvas.GetComponent<RectTransform>(), screenPoint, BattleInstanceInterface.instance.UICamera, out result);
            result.y += 15;
            panel.GetComponent<RectTransform>().localPosition = result;
        }
    }

    public Transform PanelPosition;
    void Update()
    {
        if (Spawned && panel != null)
        {
            SetPosition();
        }
    }
}
