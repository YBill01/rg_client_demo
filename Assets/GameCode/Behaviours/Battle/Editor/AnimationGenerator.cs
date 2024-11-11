using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Animations;
using System.Linq;
using System.IO;
using System;

namespace Legacy.Battle
{
	public class AnimationGenerator : EditorWindow
    {

		struct WorkInfo
		{
			public AnimationClip clip;
			public Animator animator;
			public GameObject animated;
			public GameObject prefab;
			public int frame;
			public int total;
			public bool started;
		}

		private WorkInfo _work;

		private static AnimationGenerator s_window;
		private GameObject generatedPrefab;
		private GameObject generatedObject;
		private Dictionary<string, AnimationClip> generateAnims = new Dictionary<string, AnimationClip>();
		private int selected_anim_index;
		private string selected_anim_name;

		private string selected_object;
		private Animator _selected_animator;
		private byte[] _bytes;
		private MemoryStream _memory;
		private BinaryWriter _binary;

		private void OnEnable()
        {
            EditorApplication.update += GenerateAnimation;
			_bytes = new byte[ushort.MaxValue];
			_memory = new MemoryStream(_bytes);
			_binary = new BinaryWriter(_memory);
        }

        void OnDisable()
        {
			_memory.Position = 0L;
			_binary.Dispose();
			EditorApplication.update -= GenerateAnimation;
        }

        private void Reset()
        {
			if (_memory != null)
			{
				_memory.Position = 0L;
			}			
		}

        void GenerateAnimation()
        {
			if(_work.started)
			{
				if (_work.frame == 0)
				{
					_binary.Write(_work.total);
				}

				if (_work.frame < _work.total)
				{
					_work.animator.Update(1f / _work.clip.frameRate);

					Debug.Log(_work.animated.transform.position);

					_binary.Write(_work.animated.transform.position.x);
					_binary.Write(_work.animated.transform.position.z);

					EditorUtility.DisplayProgressBar(
						"Generating Animations",
						string.Format("Animation '{0}' is Generating.", _work.clip.name),
						(float)_work.frame / _work.total
					);
					_work.frame++;
				} else
				{
					var _path = Path.Combine(Application.streamingAssetsPath, generatedPrefab.name + ".bytes");
					var _write_bytes = new byte[(int)_memory.Position];
					Array.Copy(_bytes, _write_bytes, _write_bytes.Length);
					File.WriteAllBytes(_path, _write_bytes);

					DestroyImmediate(_work.prefab);
					_work.started = false;
					EditorUtility.ClearProgressBar();
					Reset();
				}	
			}
        }

        [MenuItem("Legacy/Animation Generator", false)]
        static void MakeWindow()
        {
            s_window = GetWindow(typeof(AnimationGenerator)) as AnimationGenerator;
        }

        private void OnGUI()
        {
            GUI.skin.label.richText = true;
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            var prefab = EditorGUILayout.ObjectField("Prefab to Generate", generatedPrefab, typeof(GameObject), true) as GameObject;
            if (prefab != generatedPrefab)
            {
                generatedPrefab = prefab;
            }

            bool error = false;
            if (generatedPrefab)
            {
                Animator animator = generatedPrefab.GetComponentInChildren<Animator>();
                if (animator == null)
                {
                    EditorGUILayout.LabelField("Error: The prefab should have a Animator Component.");
                    return;
                }
                if (animator.runtimeAnimatorController == null)
                {
                    EditorGUILayout.LabelField("Error: The prefab's Animator should have a Animator Controller.");
                    return;
                }

				selected_object = EditorGUILayout.TextField("Bone:", selected_object);

				/*_prefab_objects = new List<string>();
				var _geometry = generatedPrefab.GetComponentsInChildren<Transform>();
				for (int i=0;i< _geometry.Length;++i)
				{
					var _transform = _geometry[i];
					if (_transform.parent != null)
					{
						_prefab_objects.Add(_transform.name);
					}
				}
				selected_object = EditorGUILayout.Popup("Bone", selected_object, _prefab_objects.ToArray());*/

				var clips = GetClips(animator);
				string[] clipNames = generateAnims.Keys.ToArray();
				var options = new string[clipNames.Length];
				var _names = new string[clipNames.Length];
				var _index = 0;
				foreach (var clipName in clipNames)
				{
					AnimationClip clip = clips.Find(delegate (AnimationClip c) {
						if (c != null)
							return c.name == clipName;
						return false;
					});
					int framesToBake = clip ? (int)(clip.length * 30f) : 1;
					framesToBake = Mathf.Clamp(framesToBake, 1, framesToBake);
					_names[_index] = clipName;
					options[_index++] = string.Format("({0}) {1} ", framesToBake, clipName);					
				}
				selected_anim_index = EditorGUILayout.Popup("Animation", selected_anim_index, options);
				selected_anim_name = _names[selected_anim_index];
			}

            if (generatedPrefab && !error)
            {
                if (GUILayout.Button(string.Format("Generate")))
                {
                    BakeWithAnimator();
                }
            }
        }

		void BakeWithAnimator()
		{
			if (generatedPrefab != null)
			{
				generatedObject = Instantiate(generatedPrefab);
				Selection.activeGameObject = generatedObject;
				generatedObject.transform.position = Vector3.zero;
				generatedObject.transform.rotation = Quaternion.identity;

				_selected_animator = generatedObject.GetComponentInChildren<Animator>();
				var _clip = generateAnims[selected_anim_name];

				_selected_animator.Play(_clip.name);
				_selected_animator.Update(0);

				//Axe_RHand_Joint
				var _animated_object = GameObject.Find(selected_object);
				if (_animated_object == null)
				{
					EditorGUILayout.LabelField("Error: The prefab bone["+ selected_object + "] dont find.");
					DestroyImmediate(generatedObject);
					return;
				}

				_work = new WorkInfo
				{
					animated = _animated_object,
					animator = _selected_animator,
					clip = _clip,
					total = (int)(_clip.length * _clip.frameRate),
					prefab = generatedObject,
					started = true
				};
			}
		}

		private List<AnimationClip> GetClips(Animator animator)
        {
            var controller = animator.runtimeAnimatorController as AnimatorController;
            return GetClipsFromStatemachine(controller.layers[0].stateMachine);
        }

		private List<AnimationClip> GetClipsFromStatemachine(AnimatorStateMachine stateMachine)
		{
			List<AnimationClip> list = new List<AnimationClip>();
			for (int i = 0; i != stateMachine.states.Length; ++i)
			{
				ChildAnimatorState state = stateMachine.states[i];
				if (state.state.motion is BlendTree)
				{
					BlendTree blendTree = state.state.motion as BlendTree;
					ChildMotion[] childMotion = blendTree.children;
					for (int j = 0; j != childMotion.Length; ++j)
					{
						list.Add(childMotion[j].motion as AnimationClip);
					}
				}
				else if (state.state.motion != null)
					list.Add(state.state.motion as AnimationClip);
			}
			
			var distinctClips = list.Select(q => (AnimationClip)q).Distinct().ToList();
			for (int i = 0; i < distinctClips.Count; i++)
			{
				if (distinctClips[i] && generateAnims.ContainsKey(distinctClips[i].name) == false)
					generateAnims.Add(distinctClips[i].name, distinctClips[i]);
			}
			return list;
		}

	}
}