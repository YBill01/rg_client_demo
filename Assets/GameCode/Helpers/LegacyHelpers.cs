using Legacy.Database;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.Entities;
using UnityEngine;

namespace Legacy.Client
{
    public struct KillableTag : IComponentData { };
    public enum PlayerPrefsVariable
    {
        SETTINGS_MUSIC,
        SETTINGS_SFX
    }
    public class LegacyHelpers
    {
        public static void CopyTransform(Transform Target, Transform Source)
        {
            Target.localPosition = Source.localPosition;
            Target.localScale = Source.localScale;
            Target.localRotation = Source.localRotation;
        }

        public static void CopyTransform(RectTransform Target, RectTransform Source, bool ignore_position)
        {
            Target.offsetMin = Source.offsetMin;
            Target.offsetMax = Source.offsetMax;
            Target.anchorMin = Source.anchorMin;
            Target.anchorMax = Source.anchorMax;
            Target.pivot = Source.pivot;
            if (ignore_position)
            {
                Target.localPosition = Source.localPosition;
            }
            Target.localScale = Source.localScale;
            Target.localRotation = Source.localRotation;
            Target.ForceUpdateRectTransforms();
        }

        public static void CopyTransform(GameObject Target, GameObject Source)
        {
            Target.GetComponent<RectTransform>().offsetMin = Source.GetComponent<RectTransform>().offsetMin;
            Target.GetComponent<RectTransform>().offsetMax = Source.GetComponent<RectTransform>().offsetMax;
            Target.GetComponent<RectTransform>().anchorMin = Source.GetComponent<RectTransform>().anchorMin;
            Target.GetComponent<RectTransform>().anchorMax = Source.GetComponent<RectTransform>().anchorMax;
            Target.GetComponent<RectTransform>().pivot = Source.GetComponent<RectTransform>().pivot;

            var slp = Source.GetComponent<RectTransform>().localPosition;
            Target.GetComponent<RectTransform>().localPosition = slp;
            var lp = Target.GetComponent<RectTransform>().localPosition;

            Target.GetComponent<RectTransform>().localScale = Source.GetComponent<RectTransform>().localScale;
            Target.GetComponent<RectTransform>().localRotation = Source.GetComponent<RectTransform>().localRotation;
            Target.GetComponent<RectTransform>().ForceUpdateRectTransforms();
        }

        public static Entity CreateKillableEntity(EntityManager manager)
        {
            var entity = manager.CreateEntity();
            manager.AddComponentData(entity, default(KillableTag));
            return entity;
        }
        public static Entity CreateKillableEntity()
        {
            var manager = ClientWorld.Instance.EntityManager;
            return CreateKillableEntity(manager);
        }

        public static float GetNiceValue(float value, byte charsAfterComma = 0)
        {
            float multiplier = Mathf.Pow(10.0f, charsAfterComma);
            float result = Mathf.Floor(value * multiplier) / multiplier;
            return result;
        }

        public static float GetMinionValue(float basicValue, ushort level)
        {
            float multipliedValue = basicValue * Mathf.Pow(1.1f, level);
            float normalizedValue = Mathf.Ceil(multipliedValue * 100) / 100;
            return normalizedValue;
        }

        public static int GetMinionValue(int basicValue, ushort level)
        {
            int multipliedValue = (int)((float)basicValue * (float)Mathf.Pow(1.1f, level - 1));
            return multipliedValue;
        }

        public static bool SetParent(Entity item_entity, Entity container_entity)
        {
            return SetParent(ClientWorld.Instance.EntityManager, item_entity, container_entity);
        }

        public static bool SetParent(EntityManager manager, Entity item_entity, Entity container_entity)
        {
            GameObject item_go = null;
            if (manager.HasComponent<Transform>(item_entity))
                item_go = manager.GetComponentObject<Transform>(item_entity).gameObject;
            if (manager.HasComponent<RectTransform>(item_entity))
                item_go = manager.GetComponentObject<RectTransform>(item_entity).gameObject;
            if (item_go == null)
                return false;

            GameObject container_go = null;
            if (manager.HasComponent<Transform>(container_entity))
                container_go = manager.GetComponentObject<Transform>(container_entity).gameObject;
            if (manager.HasComponent<RectTransform>(container_entity))
                container_go = manager.GetComponentObject<RectTransform>(container_entity).gameObject;
            if (container_go == null)
                return false;

            item_go.transform.SetParent(container_go.transform);
            return true;
        }

        public static void DisableNonBattleHeroComponents(GameObject hero)
        {
            SafeEnableComponent<DamageEffect>(hero, false);
            SafeEnableComponent<MinionPanel>(hero, false);
            SafeEnableComponent<AscaliaBehaviour>(hero, false);
            SafeEnableComponent<RangeHitEffect>(hero, false);
            SafeEnableComponent<SimpleHitEffect>(hero, false);
            SafeEnableComponent<ZacZarBehaviour>(hero, false);
            SafeEnableComponent<UnderUnitCircle>(hero, false);
            var list = hero.GetComponentsInChildren<ParticleSystem>();
            foreach (var p in list)
            {
                p.Stop();
                //GameObject.Destroy(p);
            }
        }

        public static void SafeEnableComponent<T>(GameObject go, bool enable) where T : MonoBehaviour
        {
            var list = go.GetComponentsInChildren<T>();
            foreach (var c in list)
                c.enabled = enable;
        }

        public static string FormatByDigits(string text)
        {
            /*
            var pattern = @"[0-9]+";
            var rgx = new Regex(pattern);
            string output = text;
            foreach (Match match in rgx.Matches(text))
            {
                 output = String.Format("{0:### ### ### ###}", Convert.ToInt64( match.Value));
            }
            return output;
            */
            MatchEvaluator evaluator = new MatchEvaluator(AddSpace);
            return Regex.Replace(text, @"(\d)(?=(\d{3})+$)", evaluator);
        }

        private static string AddSpace(Match match)
        {
            string result = match.Value + " ";
            return result;
        }

        public static string FormatTime(int time)
        {
            string result = "";
            result += ((int)((float)time / 1000f)).ToString() + "s";
            return result;
        }

        private static int DEFAULT_MANA_RESTORE = 3000;
        public static string FormatMana(int time)
        {
            string result = "";

            result += "x" + Mathf.Round(time / DEFAULT_MANA_RESTORE).ToString();
            return result;
        }

        public static string BeautifullTimeText(TimeSpan time)
        {
            return BeautifullTimeText(time, true);
        }
        public static string BeautifullTimeText(TimeSpan time, bool absolute)
        {
            string result = "";
            var i = 0;
            TimeSpan absTime;
            if (absolute)
            {
                absTime = new TimeSpan(Math.Abs(time.Ticks));
            }
            else
            {
                absTime = new TimeSpan(time.Ticks);
            }
            if (absTime.Days > 0)
            {
                result += absTime.Days + "d. ";
                i++;
            }
            if (absTime.Hours > 0)
            {
                result += absTime.Hours + "h. ";
                i++;
            }
            if (i < 2 && absTime.Minutes > 0)
            {
                i++;
                result += absTime.Minutes + "m. ";
            }
            if (i < 2 && absTime.Seconds > 0)
            {
                result += absTime.Seconds + "s.";
            }
            //Timer.text = result;
            return result;
        }

        public static Rect getChildBounds(RectTransform rect)
        {
            Rect result = new Rect();
            var childs = rect.GetComponentsInChildren<RectTransform>();
            foreach (var c in childs)
            {
                if (c == rect) continue;
                var childRectData = GetRootRect(rect, c);
                result.min = new Vector2(Math.Min(result.min.x, childRectData.min.x), Math.Min(result.min.y, childRectData.min.y));
                result.max = new Vector2(Math.Min(result.max.x, childRectData.max.x), Math.Min(result.max.y, childRectData.max.y));
            }
            return result;
        }

        private static Rect GetRootRect(RectTransform rect, RectTransform childRect)
        {
            Rect result = new Rect();
            var rc = childRect;
            while (rc != rect)
            {
                RectTransform parent = (RectTransform)rc.parent;
                result.min = rc.localPosition + parent.localPosition;
                result.max = result.min + rc.sizeDelta * rc.localScale;
                rc = parent;
            }
            return result;
        }

        public static void TurnParticlesOn(GameObject gameObject)
        {
            var list = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var l in list)
                l.Play();
        }

        public static void TurnParticlesOff(GameObject gameObject)
        {
            var list = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var l in list)
                l.Stop();
        }

        public static void SetAlpha(GameObject gameObject, float alpha)
        {
            if (gameObject != null)
            {
                var rds = gameObject.GetComponentsInChildren<MeshRenderer>(true);
                foreach (var r in rds)
                {
                    foreach (var m in r.sharedMaterials)
                    {
                        var c = m.GetColor("_BaseColor");
                        c.a = alpha;
                        m.SetColor("_BaseColor", c);
                    }
                }
            }
        }
        public static void SetAlpha(MeshRenderer renderer, float alpha)
        {
            foreach (var m in renderer.sharedMaterials)
            {
                var c = m.GetColor("_BaseColor");
                c.a = alpha;
                m.SetColor("_BaseColor", c);
            }
        }

        public static void SetAlpha(GameObject gameObject, float alpha, Material material)
        {
            var rds = gameObject.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var r in rds)
            {
                r.material = material;
                foreach (var m in r.sharedMaterials)
                {
                    var c = m.GetColor("_BaseColor");
                    c.a = alpha;
                    m.SetColor("_BaseColor", c);
                }
            }
        }
    }
}