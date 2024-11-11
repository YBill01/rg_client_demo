using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Effects_HQBehaviour : MonoBehaviour
{
    public static Effects_HQBehaviour Instance;
    [SerializeField] private MeshRenderer material1;
    [SerializeField] private MeshRenderer material2;
    [SerializeField] private Image image1;
    [SerializeField] private Image image2;

    public void SetNeedIcons(string image)
    {

        //var texture = new Texture2D(100,100);
        //var fileName = image;
        //var bytes = Image.ReadAllBytes();
        //texture.LoadImage(bytes);
        //texture.name = fileName;
        //material1.materials[0].SetTexture(texture.name,texture);
        //material2.materials[0].SetTexture(texture.name,texture);
    }
}
