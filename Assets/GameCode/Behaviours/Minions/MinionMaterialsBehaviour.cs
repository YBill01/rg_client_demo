using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Legacy.Client
{
    public class MinionMaterialsBehaviour : MonoBehaviour
    {
        private Material[] _defaultMats;
        private Material[] _transparentMats;
        private Material[] _dmgMatsRed;
        private Material[] _dmgMatsBlue;

        private SkinnedMeshRenderer[] _renderers;
        private MinionInitBehaviour _minionInitBehaviour;

        private enum DefaoultMatNames 
        {
            atlas_Blue,
            atlas_Blue_2,
            atlas_Blue_2side,
            atlas_Blue_2side_2
        }
        private void Awake()
        {
            _minionInitBehaviour = GetComponent<MinionInitBehaviour>();
            InitMaterials();
        }
        private void InitMaterials()
        {
            _renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            if (_renderers != null)
            {
                _defaultMats = _renderers[0].materials;
            }
            else
            {
                GameDebug.Log($"-=- No renderers in mob {name} WTF???");
            }

            var len             = _defaultMats.Length;
            _transparentMats    = new Material[len];
            _dmgMatsRed         = new Material[len];
            _dmgMatsBlue        = new Material[len];

            for (int i = 0; i < len; i++)
            {
                _transparentMats[i] = VisualContent.Instance.SpawnUnitMaterial;
            }
            if (_minionInitBehaviour.isHero)
            {
                InitHeroDmgMaterials();
            }
            else
            {
                InitMinionsDmgMaterials();
            }
        }
        private void InitHeroDmgMaterials()
        {
            for (int i = 0; i < _renderers[0].materials.Length; i++)
            {
                _dmgMatsRed[i] = VisualContent.Instance.DamageMaterialsEnemyHero[i];
                _dmgMatsBlue[i] = VisualContent.Instance.DamageMaterialsAllyHero[i];
            }
        }
        private void InitMinionsDmgMaterials()
        {
            for (int i = 0; i < _renderers[0].materials.Length; i++)
            {
                var name = _renderers[0].materials[i].name.Split(new char[] { ' ' })[0];
                if (name.Equals(DefaoultMatNames.atlas_Blue.ToString()))
                {
                    _dmgMatsRed[i] = VisualContent.Instance.DamageMaterialsEnemy[0];
                    _dmgMatsBlue[i] = VisualContent.Instance.DamageMaterialsAlly[0];
                }
                else if (name.Equals(DefaoultMatNames.atlas_Blue_2.ToString()))
                {
                    _dmgMatsRed[i] = VisualContent.Instance.DamageMaterialsEnemy[1];
                    _dmgMatsBlue[i] = VisualContent.Instance.DamageMaterialsAlly[1];
                }
                else if (name.Equals(DefaoultMatNames.atlas_Blue_2side.ToString()))
                {
                    _dmgMatsRed[i] = VisualContent.Instance.DamageMaterialsEnemy[2];
                    _dmgMatsBlue[i] = VisualContent.Instance.DamageMaterialsAlly[2];
                }
                else if (name.Equals(DefaoultMatNames.atlas_Blue_2side_2.ToString()))
                {
                    _dmgMatsRed[i] = VisualContent.Instance.DamageMaterialsEnemy[3];
                    _dmgMatsBlue[i] = VisualContent.Instance.DamageMaterialsAlly[3];
                }
                else
                {
                    GameDebug.Log($"-=- No such a material in registry {name}");
                    _dmgMatsRed[i] = VisualContent.Instance.DamageMaterialsEnemy[i];
                    _dmgMatsBlue[i] = VisualContent.Instance.DamageMaterialsAlly[i];
                }
            }
        }
        public void SetTransparentMaterials()
        {
            SetMaterial(_transparentMats);
        }
        public void SetDefaultMaterials()
        {
            SetMaterial(_defaultMats);
        }
        public void SetDamageMaterials(bool isEnemy)
        {
            if (isEnemy)
            {
                SetMaterial(_dmgMatsRed);
            }
            else
            {
                SetMaterial(_dmgMatsBlue);
            }
        }
        private void SetMaterial(Material[] materials)
        {
            foreach (var rend in _renderers)
            {
                rend.materials= materials;
            }
        }
    }
}