using Legacy.Client;
using Legacy.Database;
using System;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class HeroDeckItem : MonoBehaviour
{
    private ProfileInstance profileInstance;
    private ushort heroID;
    [SerializeField]
    private Image image;
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private TextMeshProUGUI level_text;
    [SerializeField]
    private TextMeshProUGUI level_progress_text;
    [SerializeField]
    private Slider level_slider;
    private CardSet cardSet;
    void Start()
    {
        profileInstance = ClientWorld.Instance.GetExistingSystem<HomeSystems>().UserProfile;
        heroID = profileInstance.DecksCollection.ActiveSet.HeroID;
        UpdateView();

        cardSet = profileInstance.DecksCollection.ActiveSet;
        cardSet.ChangeHeroEvent.AddListener(OnChangeHero);
        profileInstance.DecksCollection.DeckChangeEvent.AddListener(OnDeckChange);
    }

    private void OnChangeHero()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        this.heroID = profileInstance.DecksCollection.ActiveSet.HeroID;
        profileInstance.heroes.GetByIndex(this.heroID, out PlayerProfileHero heroData);

        level_text.text = heroData.level.ToString();
        var hArray = new ushort[Heroes.Instance.List.Count];
        Heroes.Instance.List.Keys.CopyTo(hArray, 0);
        var hIndex = Array.IndexOf(hArray, heroID);
        image.sprite = sprites[hIndex];
    }

    public void NextHero()
    {
        var hArray = new ushort[Heroes.Instance.List.Count];
        Heroes.Instance.List.Keys.CopyTo(hArray, 0);
        var hIndex = Array.IndexOf(hArray, heroID);
        int nextHeroIndex = (hIndex + 1) % hArray.Length;

        this.heroID = hArray[nextHeroIndex];
        profileInstance.DecksCollection.ActiveSet.SetHero(this.heroID);
    }

    public void PreviousHero()
    {
        var hArray = new ushort[Heroes.Instance.List.Count];
        Heroes.Instance.List.Keys.CopyTo(hArray, 0);
        var hIndex = Array.IndexOf(hArray, heroID);

        int nextHeroIndex = (hIndex - 1 + hArray.Length) % hArray.Length;

        this.heroID = hArray[nextHeroIndex];
        profileInstance.DecksCollection.ActiveSet.SetHero(this.heroID);
    }

    private void OnDeckChange()
    {
        if(cardSet!= null)
        {
            cardSet.ChangeHeroEvent.RemoveListener(OnChangeHero);
        }
        UpdateView();
        cardSet = profileInstance.DecksCollection.ActiveSet;
        cardSet.ChangeHeroEvent.AddListener(OnChangeHero);
    }

    private void OnDestroy()
    {
        cardSet.ChangeHeroEvent.RemoveListener(OnChangeHero);
        profileInstance.DecksCollection.DeckChangeEvent.RemoveListener(OnDeckChange);
        cardSet = null;
    }
}
