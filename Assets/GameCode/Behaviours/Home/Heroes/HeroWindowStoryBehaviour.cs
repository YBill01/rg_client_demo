using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Legacy.Client
{
    public class HeroWindowStoryBehaviour : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI StoryText;

        internal void SetStory(string story, string second_name)
        {
            StoryText.text = second_name + " - " + story;
        }
    }
}