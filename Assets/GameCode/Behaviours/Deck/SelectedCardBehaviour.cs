using Legacy.Client;
using Legacy.Database;
using Legacy.Server;
using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static Legacy.Client.InputSystem;

public class SelectedCardBehaviour : MonoBehaviour
{
    public RectTransform Card;

    public Transform DragObject;
    [SerializeField]
    private Vector2 startPosition = Vector2.zero;

    [SerializeField]
    private Vector2 startDragPosition = Vector2.zero;

    private Vector3 startScale = Vector3.zero;

    Vector3 AllyZoneCenter = new Vector3(-7, 0, 0);

    Vector3 RayDirection = Vector3.down;

    [SerializeField]
    private Touch Touch;

    private float maxDelta = 60f;

    public BattleCardBehaviour CardBehaviour;

	public static bool firstZoomed;
    void Start()
    {
        BasicAllyColor = StaticColliders.instance.AllyZone.GetComponentInChildren<MeshRenderer>().material.GetColor("_BaseColor");
		firstZoomed = false;
	}

    void Update()
    {
        if (startPosition != Vector2.zero)
        {
            //if (!Input.touchSupported)
            //{
                if (!Input.GetMouseButton(0))
                {
                    ResetPosition(0.5f);
                }
            //}
            //else
            //{
                //if (!isStillTouched())
                //{
                    //ResetPosition(0.5f);
                //}
            //}
        }
    }

    internal void UpdateSelected()
    {
        if (startPosition != Vector2.zero)
        {
            if (CardBehaviour.Selected)
            {
                //if (!Input.touchSupported)
                //{
                    if (Input.GetMouseButton(0))
                    {
                        Vector2 pos = Input.mousePosition;
                        if (Touch.position == pos) return;
                        Touch.position = Input.mousePosition;
                        Touch.deltaPosition = Touch.position - startDragPosition;
                        DragCard();
                    }
                    else
                    {
                        return;
                    }
                //}
                //else
                //{
                    //Debug.Log("Touch.phase: " + Touch.phase);
                    //if (isStillTouched())
                    //{
                        //DragCard();
                    //}                    
                //}                
            }
        }
    }

    internal bool CheckSecondTouch(int fingerId)
    {
        return Touch.fingerId != fingerId || !Input.touchSupported;
    }

    internal void SetTouch(Touch touch)
    {
        Touch = touch;
    }

    internal void ResetAddParams()
	{
		isPartizan = false;
	}

    internal void SetStartPosition()
    {
        DragObject.gameObject.SetActive(false);
		InitDragObject();
        startScale = Card.localScale;
        startPosition = Card.localPosition;
        startDragPosition = Input.mousePosition;
    }

    internal void UseCardBySecondClick(TouchResult hit)
    {
        SetDragObjectPosition(hit);
        DragObject.gameObject.SetActive(true);
        //var cublist = DragObject.GetComponentsInChildren<ConnectedUnitBehaviour>(true);
        //foreach (var c in cublist)
            //c.SetupConnectedUnit();
        UseCard();
    }
    internal void SetDragObjectPosition(TouchResult hit)
    {
		if (isPartizan || ClientWorld.Instance.isSandbox)
		{
			SetPartizanPosition(hit);
		}
		else
		{
			SetRegularPosition(hit);
		}
	}

	private void SetRegularPosition(TouchResult hit)
	{
		//if(!firstZoomed)
		//{
		//	firstZoomed = true;
		//	GestureMoveBehaviour.doZoomOnce = true;
		//}
		Vector3 Limit1 = Vector3.LerpUnclamped(AllyZoneCenter, hit.Position3d, 20 / Vector3.Distance(AllyZoneCenter, hit.Position3d));
		Limit1.y = 5;
		Vector3 Limit2 = AllyZoneCenter;
		Vector3 _position = Vector3.zero;

		bool Casted = false;
		GameObject Zone = null;
		while (Vector3.Distance(Limit1, Limit2) > 0.1f)
		{
			Vector3 Mid = Vector3.Lerp(Limit1, Limit2, 0.5f);
			Vector3 RayStart = Mid;
			RayStart.y = 5;

			RaycastHit[] ZonesTouchRay = Physics.RaycastAll(RayStart, RayDirection);
			bool Found = false;
			foreach (RaycastHit r in ZonesTouchRay)
			{
				if (r.transform.gameObject.tag != "AllySpawnZone") continue;
				Found = true;
				Zone = r.transform.gameObject;
				break;
			}
			if (Found)
			{
				Limit2 = Mid;
				Casted = true;
				_position = Mid;
				_position.y = 0;
			}
			else
			{
				Limit1 = Mid;
			}

			HighlightAllyQuarter(Zone);
			if (Casted)
			{
				_position = Vector3.Lerp(_position, AllyZoneCenter, 0.01f);
				if (Vector3.Distance(hit.Position3d, AllyZoneCenter) < Vector3.Distance(_position, AllyZoneCenter))
				{
					_position = hit.Position3d;
				}
				_position.y = 0;
			}
		}
		DragObject.transform.position = _position;
	}

	private void SetPartizanPosition(TouchResult hit)
	{
		Vector3 zoneCenter = AllyZoneCenter;
		if (hit.Position3d.x > 0)
		{
			zoneCenter.x = -zoneCenter.x;
		}
		Vector3 Limit1 = Vector3.LerpUnclamped(zoneCenter, hit.Position3d, 20 / Vector3.Distance(zoneCenter, hit.Position3d));
		Limit1.y = 5;
		Vector3 Limit2 = zoneCenter;
		Vector3 _position = Vector3.zero;

		bool Casted = false;
		GameObject Zone = null;
		while (Vector3.Distance(Limit1, Limit2) > 0.1f)
		{
			Vector3 Mid = Vector3.Lerp(Limit1, Limit2, 0.5f);
			Vector3 RayStart = Mid;
			RayStart.y = 5;

			RaycastHit[] ZonesTouchRay = Physics.RaycastAll(RayStart, RayDirection);
			bool Found = false;
			foreach (RaycastHit r in ZonesTouchRay)
			{
				if (r.transform.gameObject != StaticColliders.instance.AllZone.gameObject) continue;
				Found = true;
				Zone = r.transform.gameObject;
				break;
			}
			if (Found)
			{
				Limit2 = Mid;
				Casted = true;
				_position = Mid;
				_position.y = 0;
			}
			else
			{
				Limit1 = Mid;
			}

			HighlightAllyQuarter(StaticColliders.instance.AllZone.gameObject);
			if (Casted)
			{
				_position = Vector3.Lerp(_position, zoneCenter, 0.01f);
				if (Vector3.Distance(hit.Position3d, zoneCenter) < Vector3.Distance(_position, zoneCenter))
				{
					_position = hit.Position3d;
				}
				_position.y = 0;
			}
		}
		DragObject.transform.position = _position;
	}

	private GameObject HighLighted;
    private void HighlightAllyQuarter(GameObject Quarter)
    {
        if (HighLighted == Quarter) return;
        UnlightAllyZone();
        if (Quarter == null) return;
        HighLighted = Quarter;
        SetStateTo(Quarter, true);
	}

    private void UnlightAllyZone()
    {
        if (HighLighted == null) return;
        SetStateTo(HighLighted, false);
        HighLighted = null;
	}

    private void SetStateTo(GameObject Zone, bool active)
    {
        MeshRenderer mRend = Zone.GetComponent<MeshRenderer>();
        Material zoneMaterial = mRend.material;
        float Alpha = active ? 0 : 0.5f;
        zoneMaterial.SetColor("_BaseColor", Color.Lerp(BasicAllyColor, Color.black, Alpha));
        mRend.material = zoneMaterial;
    }

    private Color BasicAllyColor;

    private void InitDragObject()
    {
        DragObject.gameObject.SetActive(false);
		foreach (Transform child in DragObject)
        {
            child.gameObject.SetActive(false);
        }
        SetupSquad(CardBehaviour.DBCardData.entities);
    }

    private ushort[] spawnList;
    private GameObject[] spawnObjectsList;
    private void SetupSquad(List<ushort> DBEntities)
    {
        uint count = (uint)DBEntities.Count;
        spawnList = new ushort[count];
        spawnObjectsList = new GameObject[count];

        if (Entities.Instance.Get(DBEntities[0], out BinaryEntity entity))
        {
            for (int i = 0; i < count; i++)
            {
                ushort eDBID = DBEntities[i];
                spawnList[i] = eDBID;
                var j = i;
                CreateEntityPrefab(eDBID, count > 1, (obj) => {
                    spawnObjectsList[j] = obj;
                    obj.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
                    float2 inSquadPos = (SquadPosUtils.GetSquareSquadUnitPosition((byte)count, (byte)i) * entity.collider * 2) * (-1);

                    obj.transform.localPosition = new Vector3((float)inSquadPos.x, 0, (float)inSquadPos.y);
                    obj.SetActive(true);
                });                
            }
        }
    }

    public void UseCard()
    {
		var _active_world = World.DefaultGameObjectInjectionWorld;
		var _visualization = _active_world.GetOrCreateSystem<MinionGameObjectInitializationSystem>();
		for (int i = 0; i < spawnList.Length; i++)
		{
            var mib = spawnObjectsList[i].GetComponent<MinionInitBehaviour>();
            mib.SetupWaitPrefab();            
            _visualization.Spawned(spawnList[i], spawnObjectsList[i]);
			ObjectPooler.instance.MinionBack(spawnObjectsList[i]);

            //Когда первый юнит будет активирован
            /*if (i == 0)
            {
                var manaCost = CardBehaviour.DBCardData.manaCost;
                mib.OnMakeStarted = () => 
                {
                    //Наш UI забудет, что тратил на него ману
                    ManaUpdateSystem.ManaToUse -= manaCost; 
                    mib.OnMakeStarted = null; 
                };
            }*/

        }

        ClientWorld.Instance.ActionPlay(
            PlayerGameMessage.ActionCard,
			(byte)CardBehaviour.hand_index,
			new float2(DragObject.transform.position.x, DragObject.transform.position.z)
		);
		DragObject.gameObject.SetActive(false);
		ManaUpdateSystem.setImmediatelly = true;
	}

    private GameObject SetupWaitUnit(GameObject go)
    {
        go.transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
        StartEffect SE = go.GetComponent<StartEffect>();
        if (SE != null)
        {
            SE.enabled = true;
            SE.SetWait(true);
        }
        DamageEffect DE = go.GetComponent<DamageEffect>();
        if (DE != null)
            DE.enabled = true;
        MinionPanel MP = go.GetComponent<MinionPanel>();
        if (MP != null)
            MP.enabled = true;
        return go;
    }
	public static bool isPartizan = false;
    private void CreateEntityPrefab(ushort DBEntity, bool isSquad, Action<GameObject> callback)
    {
		if (Entities.Instance.Get(DBEntity, out BinaryEntity entity))
        {
			foreach (var c in entity.components)
			{
				if (c == 86)
				{
					isPartizan = true;
					break;
				}
			}
			ObjectPooler.instance.GetMinion(entity, (obj) => {
                obj.transform.SetParent(DragObject.transform);

                //            var collection = Components.Instance.Get<MinionPartizan>();
                //if (collection.TryGetValue(DBEntity, out MinionPartizan pt))
                //{
                //	isPartizan = true;
                //}
                /*if (collection.TryGetValue(DBEntity, out MinionOffence dbMinionOffence))
                {
                    obj.GetComponent<UnderUnitCircle>().Scale(isSquad ? 0 : entity.collider);
                    if (AppInitSettings.Instance.EnableColliders)
                    {
                        obj.GetComponent<UnderUnitCircle>().ScaleDebugCollider(entity.collider + dbMinionOffence.radius);
                    }
                }*/
                //var cub = obj.GetComponent<ConnectedUnitBehaviour>();
                //if(cub != null)
                //{
                    //cub.SetupConnectedUnit();
                //}
                var animator = obj.GetComponent<Animator>();
                if(animator != null)
                {
                    animator.SetBool("Landing", false);
                    animator.SetBool("Stand", false);
                    animator.SetBool("Walk", false);
                    animator.SetBool("Death", false);
                    animator.SetBool("Skill1", false);
                    animator.SetBool("Skill2", false);
                    animator.Play("Stand");
                    animator.enabled = true;
                }
                obj.GetComponent<StartEffect>().Flying = entity.type == MinionLayerType.Fly;
                callback(obj);
            });
        }        
    }

    private void DragCard()
    {
        var scaleByY = Mathf.Clamp01((maxDelta - Touch.deltaPosition.y) / maxDelta);
        if (scaleByY == 0)
        {
            DragObject.gameObject.SetActive(true);
            //var cublist = DragObject.GetComponentsInChildren<ConnectedUnitBehaviour>(true);
            //foreach(var c in cublist)
                //c.SetupConnectedUnit();
            Card.localScale = Vector3.zero;
			//ManaUpdateSystem.ManaToUse = CardBehaviour.DBCardData.manaCost;
		}
        else
        {
            DragObject.gameObject.SetActive(false);
            var cublist = DragObject.GetComponentsInChildren<ConnectedUnitBehaviour>(true);
            foreach (var c in cublist)
                c.UnsetConnectedUnit();
			Card.localScale = startScale * scaleByY;
            Card.localPosition = startPosition + Touch.deltaPosition;
        }

    }

    internal void ResetPosition(float lerp = 1f)
    {
        Card.localPosition = Vector3.Lerp(Card.localPosition, startPosition, lerp);
        Card.localScale = Vector3.Lerp(Card.localScale, startScale, lerp);
        if (!Input.touchSupported)
        {
            Touch.phase = TouchPhase.Ended;
        }
    }

    internal bool isStillTouched()
    {
        return Input.touchSupported && (
            Touch.phase == TouchPhase.Moved || 
            Touch.phase == TouchPhase.Began || 
            Touch.phase == TouchPhase.Stationary
            );
    }
}
