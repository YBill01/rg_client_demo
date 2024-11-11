/*using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TutorialEntityClass
{
    private static TutorialEntityClass instance;
    public Entity TutorialEntity { get; set; }

    private TutorialEntityClass()
    { }

    public static TutorialEntityClass getInstance()
    {
        if (instance == null)
            instance = new TutorialEntityClass();
        return instance;
    }

    public void CreateTutorialEntity()
    {
        if (TutorialEntity != Entity.Null) return;
        MenuTutorialInstance tutorial = new MenuTutorialInstance { currentTrigger = 0, _timer_start = int.MaxValue };
        var tutorialEntity = ClientWorld.Instance.EntityManager.CreateEntity();
        ClientWorld.Instance.EntityManager.AddComponentData(tutorialEntity, tutorial);
        TutorialEntity = tutorialEntity;
    }

    public void SetDataToTutorialEntity(MenuTutorialInstance tutorialInstance)
    {
        ClientWorld.Instance.EntityManager.SetComponentData<MenuTutorialInstance>(TutorialEntity, tutorialInstance);
    }

    public void DestroyTutorialEntity()
    {
        ClientWorld.Instance.EntityManager.DestroyEntity(TutorialEntity);
        TutorialEntity = Entity.Null;
    }

}*/
