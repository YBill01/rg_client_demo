using Legacy.Client;
using Legacy.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Events;

public static class MinionsSoundsManager
{
    //battle particles
    //battle particles
    //battle particles

    public static UnityEvent<List<MinionSoundManager>, bool> OnListChanged = new UnityEvent<List<MinionSoundManager>, bool>();
    public static List<MinionSoundManager> EnemyList = new List<MinionSoundManager>();
    public static List<MinionSoundManager> PlayerList = new List<MinionSoundManager>();
    public static List<MinionSoundManager> DeathList = new List<MinionSoundManager>();

    static MinionsSoundsManager()
    {
        OnListChanged.AddListener(onChanges);
        OnMenuListChanged.AddListener(onMenuChanges);
    }

    public static void AddSourceToList(ref MinionSoundManager source, bool isEnemy)
    {
        var list = isEnemy ? EnemyList : PlayerList;
        if (source != null)
        {
            if (!list.Contains(source))
                list.Add(source);
            else
                source.enabled = true;
        }
        OnListChanged.Invoke(list, isEnemy);
    }
    public static void RemoveSourceFromList(MinionSoundManager source, bool isEnemy)
    {
        var list = isEnemy ? EnemyList : PlayerList;
        if (list.Contains(source))
            source.enabled = false;
    }
    public static bool canPlay(bool isEnemy, string name)
    {
        var list = isEnemy ? EnemyList : PlayerList;
        var clipsCount = list.Where(
            x => 
            x.CurrentClip?.name == name 
            && x.MinionAudioSource != null
            && x.MinionAudioSource.isPlaying 
            && x.MinionAudioSource.enabled).ToList().Count();

        return clipsCount < 1;
    }
    public static void playDeath(MinionSoundManager manager, bool isEnemy, string name)
    {
        var list = isEnemy ? EnemyList : PlayerList;
        var clipsCount = list.Select(x => x)
                   .Where(x => x.CurrentClip?.name == name && x.MinionAudioSource != null && x.MinionAudioSource.enabled)
                   .Skip(1)
                   .ToList();

        clipsCount.ForEach(x => x.SetEmptyClip());
    }

    public static void onChanges(List<MinionSoundManager> list, bool isEnemy)
    {
        var temp = list.GroupBy(x => new { x.CurrentClip?.name, isEnemy })
              .Where(g => g.Count() > 1)
              .SelectMany(x => x)
              .Where(x => x.enabled)
              .Skip(1)
              .ToList();
        temp.ForEach(x => x.SetEmptyClip());
    }

    public static void ClearAll()
    {
        EnemyList.Clear();
        PlayerList.Clear();
    }


    //menu particles
    //menu particles
    //menu particles

    public static UnityEvent<List<AudioSource>> OnMenuListChanged = new UnityEvent<List<AudioSource>>();
    public static List<AudioSource> MenuList = new List<AudioSource>();
    public static void AddSourceToList(AudioSource source)
    {
        var list = MenuList;
        if (source != null)
            list.Add(source);
        OnMenuListChanged.Invoke(list);
    }

    public static void RemoveSourceFromList(AudioSource source)
    {
        var list = MenuList;
        if (list.Contains(source))
            list.Remove(source);
    }

    public static void onMenuChanges(List<AudioSource> list)
    {
        var temp = list.GroupBy(x => new { x.clip.name })
              .Where(g => g.Count() > 1)
              .SelectMany(x => x)
              .Skip(1)
              .ToList();
        temp.ForEach(x => x.clip = null);
    }
}
