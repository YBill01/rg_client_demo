using System;
using System.Collections;
using System.Collections.Generic;
using Legacy.Client;
using UnityEngine;

namespace Legacy.Client
{
    public class ArenaEventsPositionsBehaviour : MonoBehaviour
    {
        SortedDictionary<ushort, RectTransform> rewards = new SortedDictionary<ushort, RectTransform>();

        public void AddReward(ushort rating, RectTransform RectTransform)
        {
            rewards.Add(rating, RectTransform);
        }

        internal float GetPosition(ushort rating)
        {
			if (rewards.Count == 0)
			{
				return 0.0f;
			}

			KeyValuePair<ushort, RectTransform> previous = default;
            KeyValuePair<ushort, RectTransform> next = default;
            foreach (var pair in rewards)
            {
                next = pair;
                if (rating >= pair.Key)
                {
                    previous = pair;
                }
                else
                {
                    break;
                }
            }

            float percentage = GetPercentage(rating, previous.Key, next.Key);

            float xDelta = next.Value.anchoredPosition.x - previous.Value.anchoredPosition.x;
            return previous.Value.anchoredPosition.x + (xDelta * percentage);
        }

        private float GetPercentage(ushort rating, ushort start, ushort end)
        {
            float total = end - start;
            if (total == 0)
            {
                return 1;
            }

            float delta = rating - start;
            if (total < delta)
            {
                return 1;
            }

            return delta / total;
        }
    }
}