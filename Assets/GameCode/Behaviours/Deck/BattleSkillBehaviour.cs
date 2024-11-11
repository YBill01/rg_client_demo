using Legacy.Client;
using Legacy.Database;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static Legacy.Client.InputSystem;

public class BattleSkillBehaviour : MonoBehaviour
{
    public byte _index = 0;

    [SerializeField]
    private TextMeshProUGUI TimerText;

    [SerializeField]
    private GameObject TimerObject;

    [SerializeField]
    private Image Fader;

    [SerializeField]
    private BattleSkillBehaviour battleSkill;

    [SerializeField]
    private SkillViewBehaviour skillView;

    private bool inited = false;

    public GameObject DragObject;
    public ParticleSystem DragSkillParticle;

    [SerializeField]
    private Touch Touch;

    public bool IsDraggable;
    public bool Selected;

    public Vector2 CustomSpawnPosition;

    [SerializeField]
    private Vector2 startPosition = Vector2.zero;

    [SerializeField]
    private Vector2 startDragPosition = Vector2.zero;
    private Vector3 startScale;

    private HeroRageSettings settings;

    [SerializeField]
    private GameObject effectSkillPrepareSpeed_0;
    [SerializeField]
    private GameObject effectSkillPrepareSpeed_1;
    [SerializeField]
    private GameObject effectSkillPrepareSpeed_2;
    [SerializeField]
    private GameObject effectSkillPrepared;

    public bool isBlockedByTutorial;

    void Start()
    {
        Card = transform as RectTransform;
        WorldFloorCollider = StaticColliders.instance.TouchCollider;
        DragObject.SetActive(true);
        settings = Settings.Instance.Get<HeroRageSettings>();
    }

    public int skillCooldown { get { return _index == 0 ? settings.skill1_cooldown : settings.skill2_cooldown; } }
    public bool IsReady { get { return settings.cooldown - timer > skillCooldown; } }

    public byte Index { get => _index; }
    public SkillViewBehaviour SkillView { get => skillView; }

    public int timer = 0;

    public void UpdateSkill(int time, byte level, byte myBridgesCount, float skillSpeed)
    {
        timer = time;
        if (_binary.cooldown._value(level) == 0) return;

        var param = settings.cooldown - skillCooldown;
        var delta = time - param;
        var skillCooldownParam = Index == 0 ? skillCooldown : (settings.skill2_cooldown - settings.skill1_cooldown);
        var value = Mathf.Clamp01((float)(delta) / skillCooldownParam);
        battleSkill.SetFaderAmount(value);
        if (startPosition != Vector2.zero)
        {
            if (!Input.GetMouseButton(0))
            {
                ResetPosition(0.5f);
            }
        }

        // Переменная показывает что мы либо первый скилл и он никого не ждет
        // либо мы второй скил и часть визуала запустится только когда первый скилд будет готов
        var firstSkillReady = Index == 0 || (settings.cooldown - settings.skill1_cooldown - time) > 0;
        SetupChargeEffects(delta < 0, firstSkillReady, myBridgesCount);

        TimerObject.SetActive(!IsReady && firstSkillReady);
        float timeToShow = skillCooldown - (settings.cooldown - timer);
        timeToShow /= skillSpeed;
        TimerText.text = ((byte)(timeToShow / 1000)).ToString();
    }

    private void SetupChargeEffects(bool isFull, bool firstSkillReady, byte myBridgesCount)
    {
        effectSkillPrepared.SetActive(isFull && !isBlockedByTutorial);
        effectSkillPrepareSpeed_0.SetActive(!isFull && firstSkillReady && myBridgesCount == 0);
        effectSkillPrepareSpeed_1.SetActive(!isFull && firstSkillReady && myBridgesCount == 1);
        effectSkillPrepareSpeed_2.SetActive(!isFull && firstSkillReady && myBridgesCount == 2);
    }

    private void SetFaderAmount(float v)
    {
        skillView.MakeGray(v > 0.001f || isBlockedByTutorial);
        Fader.fillAmount = v;
    }

    private BinarySkill _binary;
    public void InitSkillView(BinarySkill binary)
    {
        _binary = binary;

        IsDraggable = binary.drag_prefab.Length > 0;
        if (IsDraggable)
        {
            var loaded = Addressables.InstantiateAsync("Drag/" + binary.drag_prefab + ".prefab", DragObject.transform);
            loaded.Completed += (AsyncOperationHandle<GameObject> async) =>
            {
                var dragPrefab = async.Result;
                var scaler = dragPrefab.GetComponent<DragRadiusScale>();
                DragSkillParticle = dragPrefab.GetComponent<ParticleSystem>();
                HideDragSkillPrefab(true);

                if (scaler != null)
                {
                    for (byte i = 0; i < binary.effects.Count; i++)
                    {
                        if (Effects.Instance.Get(binary.effects[i], out BinaryEffect binaryEffect))
                        {
                            float scale = CheckEffects(binaryEffect);
                            if (scale > 0)
                            {
                                scaler.Scale(scale);
                                break;
                            }
                        }
                    }
                }
            };
        }
        battleSkill.Init(binary);
    }

    private void Init(BinarySkill binary)
    {
        skillView.Init(binary);
    }

    private float CheckEffects(BinaryEffect binaryEffect)
    {
        for (byte j = 0; j < binaryEffect.components.Count; j++)
        {
            if (Components.Instance.GetLink(binaryEffect.components[j], out string name))
            {
                if (name == typeof(Legacy.Database.EffectRadius).Name)
                {
                    var effects = Components.Instance.Get<Legacy.Database.EffectRadius>();
                    if (effects.TryGetValue(binaryEffect.index, out Legacy.Database.EffectRadius radiusData))
                    {
                        return radiusData.radius;
                    }
                }
            }
        }
        return 0f;
    }

    public void SetSelected(bool Value)
    {
        if (Value == Selected) return;
        Selected = Value;

        if (Value)
        {
            SetStartPosition();
        }
        StaticColliders.instance.AllZone.enabled = Value;
    }

    public static bool isAnySelected;
    internal void Hold(TouchResult[] hits)
    {
        DragSkillParticle.gameObject.SetActive(!IsUnderUI(hits));
        if (DragSkillParticle.gameObject.activeSelf)
        {
            isAnySelected = true;
        }
        SetDragPrefabPosition(hits);
        if (startPosition != Vector2.zero)
        {
            if (Selected)
            {
                if (Input.GetMouseButton(0))
                {
                    Vector2 pos = Input.mousePosition;
                    if (Touch.position == pos) return;
                    Touch.position = Input.mousePosition;
                    Touch.deltaPosition = Touch.position - startDragPosition;
                    DragSkill();
                }
                else
                {
                    return;
                }
            }
        }
    }

    internal bool IsUnderUI(TouchResult[] _hits)
    {
        foreach (TouchResult hit in _hits)
        {
            if (hit.Target.GetComponent<RectTransform>() != null) return true;
        }
        return false;
    }

    internal void SetDragPrefabPosition(TouchResult[] _hits)
    {
        foreach (TouchResult hit in _hits)
        {
            if (hit.Target != WorldFloorCollider) continue;
            var hp = hit.Position3d;

            SetDragPrefabPosition(hp);
        }
    }

    private void SetDragPrefabPosition(Vector3 pos)
    {
        pos.y = 0.2f;
        DragObject.transform.position = pos;
    }

    internal void Use()
    {
        HideDragSkillPrefab();

        UseSkill(DragObject.transform.position);
        isAnySelected = false;
    }

    public void UseSkill(Vector3 Position = default)
    {
        var pos = new Unity.Mathematics.float2(Position.x, Position.z);

        if (CustomSpawnPosition != Vector2.zero)
        {
            pos = CustomSpawnPosition;
            SetDragPrefabPosition(new Vector3(pos.x, 0, pos.y));
            CustomSpawnPosition = Vector2.zero;
        }

        ClientWorld.Instance.ActionPlaySkill(
            _index,
            pos
        );

        TutorialMessageBehaviour.MakeTapEvent();
    }

    public RectTransform Card;

    public GameObject WorldFloorCollider;

    private float maxDelta = 60f;

    internal void SetStartPosition()
    {
        startPosition = Card.localPosition;
        startDragPosition = Input.mousePosition;
        startScale = Card.localScale;
    }

    private void DragSkill()
    {
        var scaleByY = Mathf.Clamp01((maxDelta - Touch.deltaPosition.y) / maxDelta);
        if (scaleByY == 0)
        {
            ShowDragSkillPrefab();

            Card.localScale = Vector3.zero;
        }
        else
        {
            HideDragSkillPrefab();

            Card.localScale = startScale * scaleByY;
            Card.localPosition = startPosition + Touch.deltaPosition;
        }
    }

    internal void ResetPosition(float lerp = 1f)
    {
        Card.GetComponent<Image>().raycastTarget = true;
        Card.localPosition = Vector3.Lerp(Card.localPosition, startPosition, lerp);
        Card.localScale = Vector3.Lerp(Card.localScale, Vector3.one, lerp);
    }

    private void ShowDragSkillPrefab()
    {
        if (!DragSkillParticle.gameObject.activeSelf)
            DragSkillParticle.gameObject.SetActive(true);

        if (DragSkillParticle.particleCount == 0)
        {
            DragSkillParticle.Play();
        }

        var main = DragSkillParticle.main;
        main.loop = true;
        HideAllParticlesAndMeshes(true);
    }

    private void HideDragSkillPrefab(bool forceHide = false)
    {
        var main = DragSkillParticle.main;
        main.loop = false;
        HideAllParticlesAndMeshes(false);
        if (forceHide)
            DragSkillParticle.gameObject.SetActive(false);
    }

    private void HideAllParticlesAndMeshes(bool flag)
    {
        var pss = DragSkillParticle.GetComponentsInChildren<ParticleSystem>().ToList();
        var mrs = DragSkillParticle.GetComponentsInChildren<MeshRenderer>().ToList();
        foreach (var ps in pss)
        {
            var main = ps.main;
            main.loop = flag;
        }
        foreach (var mr in mrs)
        {
            mr.enabled = flag;
        }
    }
}

