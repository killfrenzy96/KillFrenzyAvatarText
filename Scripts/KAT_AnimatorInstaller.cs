using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections;
using System.Collections.Generic;

using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using Parameter = VRC.SDKBase.VRC_AvatarParameterDriver.Parameter;

using VRC.SDK3.Avatars.Components;
using static VRC.SDKBase.VRC_AvatarParameterDriver;


namespace KillFrenzy.AvatarTextTools
{
	public class KillFrenzyAvatarTextInstaller : EditorWindow
	{
		private const string ParamTextLogic = "KAT_Logic";
		private const string ParamTextSync1 = "KAT_CharSync1";
		private const string ParamTextSync2 = "KAT_CharSync2";
		private const string ParamCharPrefix = "KAT_Char";

		private const string CharacterAnimationFolder = "CharacterAnimations/";
		private const string CharacterAnimationClipNamePrefix = "Char";
		private const string CharacterVariablePrefix = "KAT_Char";
		private const int CharacterLimit = 64;
		private const int CharacterCount = 96;

		private AnimatorController targetController = null;
		private int tab = 1;


		[MenuItem("Tools/KillFrenzy Avatar Text (KAT)")]
		public static void Open()
		{
			GetWindow<KillFrenzyAvatarTextInstaller>("KAT");
		}

		private void OnGUI()
		{
			GUIStyle titleStyle = new GUIStyle("Title") {
				fontSize = 14,
				fixedHeight = 28,
				fontStyle = FontStyle.Bold,
				normal = {
					textColor = Color.white
				}
			};

			GUIStyle subtitleStyle = new GUIStyle("Subtitle") {
				fontSize = 13,
				fixedHeight = 26,
				fontStyle = FontStyle.Bold,
				normal = {
					textColor = Color.white
				}
			};

			EditorGUILayout.LabelField("KillFrenzy Avatar Text (KAT)", titleStyle);

			tab = GUILayout.Toolbar(tab, new string[]{"Avatar Installer", "Advanced"});
			EditorGUILayout.Space();

			switch (tab) {
				case 0: {
					break;
				}
				case 1: {
					EditorGUILayout.LabelField("Install to Animation Controller", subtitleStyle);

					targetController = EditorGUILayout.ObjectField("Animator", targetController, typeof(AnimatorController), true) as AnimatorController;
					EditorGUILayout.Space();

					if (GUILayout.Button("Install KAT animations")) {
						if (targetController == null) {
							Debug.Log("Failed: No animator has been selected.");
						} else {
							Debug.Log("KAT install started.");
							if (!RemoveFromAnimator(targetController)) {
								Debug.Log("Warning: KAT removal failed. This is done before installation.");
							}
							if (!InstallToAnimator(targetController)) {
								Debug.Log("KAT install failed.");
							} else {
								Debug.Log("KAT install complete.");
							}
						}
					}

					if (GUILayout.Button("Remove KAT animations")) {
						if (targetController == null) {
							Debug.Log("Failed: No animator has been selected.");
						} else {
							Debug.Log("KAT removal started.");
							if (!RemoveFromAnimator(targetController)) {
								Debug.Log("KAT removal complete.");
							} else {
								Debug.Log("KAT install failed.");
							}
						}
					}
					break;
				}
				default: {
					break;
				}
			}
		}

		private bool InstallToAnimator(AnimatorController controller)
		{
			AnimationClip[] charAnimations = GetCharAnimations();
			if (charAnimations == null) {
				Debug.Log("Failed: Resources/" + CharacterAnimationFolder + " is missing some animations.");
				return false;
			}

			// Add parameters
			controller.AddParameter(ParamTextLogic, AnimatorControllerParameterType.Bool);
			controller.AddParameter(ParamTextSync1, AnimatorControllerParameterType.Int);
			controller.AddParameter(ParamTextSync2, AnimatorControllerParameterType.Int);

			for (int i = 0; i < CharacterLimit; i++) {
				controller.AddParameter(ParamCharPrefix + i.ToString(), AnimatorControllerParameterType.Float);
			}

			// Add Layers
			controller.AddLayer(ParamTextLogic);

			for (int i = 0; i < CharacterLimit; i++) {
				Debug.Log(charAnimations[i]);
				controller.AddLayer(CreateCharLayer(i, charAnimations[i]));
			}

			return true;
		}

		private bool RemoveFromAnimator(AnimatorController controller) {
			// Remove Layers
			for (int i = controller.layers.Length - 1; i >= 0; i--) {
				AnimatorControllerLayer layer = controller.layers[i];
				if (
					layer.name.StartsWith(ParamCharPrefix) ||
					layer.name.StartsWith(ParamTextLogic)
				) {
					controller.RemoveLayer(i);
				}
			}

			// Remove Parameters
			for (int i = controller.parameters.Length - 1; i >= 0; i--) {
				AnimatorControllerParameter parameter = controller.parameters[i];
				if (
					parameter.name.StartsWith(ParamCharPrefix) ||
					parameter.name.StartsWith(ParamTextLogic) ||
					parameter.name.StartsWith(ParamTextSync1) ||
					parameter.name.StartsWith(ParamTextSync2)
				) {
					controller.RemoveParameter(i);
				}
			}

			return true;
		}

		private AnimationClip[] GetCharAnimations() {
			AnimationClip[] animationClips = Resources.LoadAll<AnimationClip>(CharacterAnimationFolder);
			AnimationClip[] orderedAnimationClips = new AnimationClip[64];

			for (int i = 0; i < CharacterLimit; i++) {
				orderedAnimationClips[i] = null;
				foreach (var animationClip in animationClips) {
					if (animationClip.name == CharacterAnimationClipNamePrefix + i.ToString()) {
						orderedAnimationClips[i] = animationClip;
						break;
					}
				}
				if (orderedAnimationClips[i] == null) {
					return null;
				}
			}

			return orderedAnimationClips;
		}

		private AnimatorControllerLayer CreateLogicLayer() {
			AnimatorControllerLayer layer = new AnimatorControllerLayer();

			layer.name = ParamTextLogic;
			layer.defaultWeight = 0;
			layer.stateMachine = new AnimatorStateMachine();

			// StateMachineBehaviour behaviour = state.AddStateMachineBehaviour(typeof(AnimationClip));

			return layer;
		}

		private AnimatorControllerLayer CreateCharLayer(int charId, AnimationClip animation)
		{
			AnimatorControllerLayer layer = new AnimatorControllerLayer();

			layer.name = ParamCharPrefix + charId.ToString();
			layer.defaultWeight = 1;
			layer.stateMachine = new AnimatorStateMachine();

			AnimatorState state = layer.stateMachine.AddState(CharacterVariablePrefix + charId.ToString(), new Vector3(300f, 100f, 0f));
			state.name = CharacterVariablePrefix + charId.ToString();
			state.motion = animation;
			state.timeParameter = CharacterVariablePrefix + charId.ToString();
			state.timeParameterActive = true;

			return layer;
		}
	}
}
