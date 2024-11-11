using Legacy.Client;
using Legacy.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHeroesBehaviour : MonoBehaviour
{
    public static MenuHeroesBehaviour Instance;

    [SerializeField]
    private Transform ShadowTransform;

    [SerializeField]
    private Camera RenderCamera;

    private Dictionary<ushort, MenuHeroModelBehaviour> InstantiatedModels = new Dictionary<ushort, MenuHeroModelBehaviour>();

    [SerializeField]
    private Transform HeroContainer;

    private MenuHeroModelBehaviour activeHero = null;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ClientWorld.Instance.Profile.HeroSelectEvent.AddListener(ShowHero);
        StartCoroutine(StartHeroCoroutine(ClientWorld.Instance.Profile.SelectedHero));
    }

    IEnumerator StartHeroCoroutine(ushort heroID)
    {
        yield return new WaitForSeconds(2.0f);
        ShowHero(heroID);
    }

    internal void RotateHeroStart()
    {
        activeHero.SetRotating(true);
    }
    internal void RotateHeroEnd()
    {
        activeHero.SetRotating(false);
    }

    public Transform GetHeroContainer()
    {
        return HeroContainer;
    }
    public Camera Get3DRenderCamera()
    {
        return RenderCamera;
    }    

    

    MenuHeroModelBehaviour CreateModel(ushort index)
    {
        var model = Instantiate(GetPrefab(index), HeroContainer).GetComponent<MenuHeroModelBehaviour>();
        InstantiatedModels.Add(index, model);
        return model;
    }

    public void ShowHero(ushort index)
    {
        MenuHeroModelBehaviour model;
        if (InstantiatedModels.TryGetValue(index, out MenuHeroModelBehaviour HeroModel))
        {
            model = HeroModel;
        }
        else
        {
            model = CreateModel(index);
        }
        if (model == null || activeHero == model) return;
        if(activeHero != null)
        {
            activeHero.Enable(false);
            activeHero = null;
        }
        activeHero = model;
        model.Enable(true);
    }

    public void PlayHelloIdle(ushort index)
    {
        MenuHeroModelBehaviour model;
        if (InstantiatedModels.TryGetValue(index, out MenuHeroModelBehaviour HeroModel))
        {
            model = HeroModel;
            model.PlayHelloIdle();
        }
    }
    public void PlayIdle(ushort index, float duration = 1.0f)
    {
        MenuHeroModelBehaviour model;
        if (InstantiatedModels.TryGetValue(index, out MenuHeroModelBehaviour HeroModel))
        {
            model = HeroModel;
            model.PlayIdle(duration);
        }
    }

    GameObject GetPrefab(ushort index)
    {        
        return VisualContent.Instance.GetHeroVisualData(index).MenuModelPrefab;
    }

    

    public void EnableRender(bool toggle)
    {
        gameObject.SetActive(toggle);
        if (toggle)
        {
            var profile = ClientWorld.Instance.Profile;
            ShowHero(profile.SelectedHero);                 
        }
    }        

    public void RotateHero(float offset)
    {
        activeHero.Rotate(offset);
    }
}
