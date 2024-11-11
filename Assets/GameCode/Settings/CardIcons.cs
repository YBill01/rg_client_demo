using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameLegacy/CardIcons")]
public class CardIcons : SettingObject
{
	public static CardIcons Instance;
	public override void Init()
	{
		Instance = this;
		LoadTextures();
	}

	[SerializeField]
    private Sprite defaultTexture;

	internal void LoadTextures()
    {
		UnityEngine.Debug.Log("CardIcons >> LoadTextures");
        for (ushort i = 0; i < Icons.Count; i++)
        {
            if(!TexturesDictionary.ContainsKey(Icons[i].ID))
            TexturesDictionary.Add(Icons[i].ID, Icons[i].texture);
        }
    }

    [SerializeField]
    public List<ImageID> Icons = new List<ImageID>();

    private Dictionary<ushort, Sprite> TexturesDictionary = new Dictionary<ushort, Sprite>();

    [System.Serializable]
    public struct ImageID
    {
        public ushort ID;
        public Sprite texture;
    }

    internal Sprite GetTexture(ushort id)
    {
        if(TexturesDictionary.TryGetValue(id, out Sprite texture))
        {
            return texture;
        }
        else
        {
            return defaultTexture;
        }
    }
}
