using DG.Tweening;
using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ZacZarBloodyRazorDragBehaviour : DragRadiusScale
{
    private Transform hero = null;
    public Transform Axe;
    Tween tween;
    public Transform Point;
    public Transform AxeTrail;
    private void Start()
    {
        var manager = ClientWorld.Instance.EntityManager;

        var _battle_query = manager.CreateEntityQuery(ComponentType.ReadOnly<BattleInstance>());
        var _heroes_query = manager.CreateEntityQuery(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<MinionData>(),
            ComponentType.Exclude<PauseState>());
        var _battle = _battle_query.GetSingleton<BattleInstance>();
        var _heroes = _heroes_query.ToEntityArray(Allocator.TempJob);
        var _transforms = _heroes_query.ToComponentArray<Transform>();
        for (int i = 0; i < _heroes.Length; ++i)
        {
            var _hero = manager.GetComponentData<MinionData>(_heroes[i]);
            if (_hero.layer == MinionLayerType.Hero)
            {
                if (_battle.players[_battle.players.player].side == _hero.side)
                {
                    if (_transforms[i].gameObject.GetComponent<ZacZarBehaviour>())
                        hero = _transforms[i];
                }
            }
        }
        //Axe.SetPositionAndRotation(hero.position, Axe.rotation);
        //tween = Axe.DOPath(new Vector3[] { new Vector3(-12, 0, 0), new Vector3(0, 0, -5.5f), new Vector3(12, 0, 0), new Vector3(0, 0, 5.5f), new Vector3(-12, 0, 0) }
        //    , 5f, PathType.CatmullRom, PathMode.Full3D, 10, Color.blue)
        //   .SetEase(Ease.Linear)
        //   .SetLoops(-1, LoopType.Restart);
        _heroes.Dispose();

    }
    void OnEnable()
    {
        AxeTrail.position = Vector3.zero;
    }

    public void Update()
    {
        AxeTrail.position = Vector3.zero;
    }

    //void OnDisable()
    //{
    //    if (hero != null)
    //    {
    //        var zaczar = hero.GetComponent<ZacZarBehaviour>();
    //        {
    //            zaczar.SetAxeTargetHeroPosition(hero.position);
    //        }
    //    }
    //}
}
