/*
	Avatar Text for VRChat
	Copyright (C) 2022 KillFrenzy / Evan Tran

	This program is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
	the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
	GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with this program. If not, see <https://www.gnu.org/licenses/>.
*/

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
		private const string ParamTextLogicPrefix = "KAT_Logic";
		private const string ParamTextVisible = "KAT_Visible";
		private const string ParamTextPointer = "KAT_Pointer";
		private const string ParamTextSyncPrefix = "KAT_CharSync";
		private const string ParamCharPrefix = "KAT_Char";

		private const string CharacterAnimationFolder = "KAT_CharAnimations/";
		private const string CharacterAnimationClipNamePrefix = "Char";
		private const int CharacterLimit = 128;
		private const int CharacterCount = 96;
		private const int CharacterSyncParamsSize = 4;
		const int PointerCount = CharacterLimit / CharacterSyncParamsSize;

		private AnimatorController targetController = null;
		private int tab = 1;


		[MenuItem("Tools/KillFrenzy Avatar Text (KAT)")]
		public static void Open()
		{
			GetWindow<KillFrenzyAvatarTextInstaller>("KAT");
		}

		private void OnGUI()
		{
			GUIStyle titleStyle = new GUIStyle() {
				fontSize = 14,
				fixedHeight = 28,
				fontStyle = FontStyle.Bold,
				normal = {
					textColor = Color.white
				}
			};

			GUIStyle subtitleStyle = new GUIStyle() {
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

					if (GUILayout.Button("Install/Update KAT animations")) {
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
			controller.AddParameter(ParamTextVisible, AnimatorControllerParameterType.Bool);
			controller.AddParameter(ParamTextPointer, AnimatorControllerParameterType.Int);

			for (int i = 0; i < CharacterSyncParamsSize; i++) {
				controller.AddParameter(ParamTextSyncPrefix + i.ToString(), AnimatorControllerParameterType.Int);
			}

			for (int i = 0; i < CharacterLimit; i++) {
				controller.AddParameter(ParamCharPrefix + i.ToString(), AnimatorControllerParameterType.Float);
			}

			// Add Layers
			for (int i = 0; i < CharacterSyncParamsSize; i++) {
				controller.AddLayer(CreateLogicLayer(controller, i));
			}

			for (int i = 0; i < CharacterLimit; i++) {
				controller.AddLayer(CreateCharLayer(controller, i, charAnimations[i]));
			}

			return true;
		}

		private bool RemoveFromAnimator(AnimatorController controller) {
			// Remove Layers
			for (int i = controller.layers.Length - 1; i >= 0; i--) {
				AnimatorControllerLayer layer = controller.layers[i];
				if (
					layer.name.StartsWith(ParamCharPrefix) ||
					layer.name.StartsWith(ParamTextLogicPrefix)
				) {
					controller.RemoveLayer(i);
				}
			}

			// Remove Parameters
			for (int i = controller.parameters.Length - 1; i >= 0; i--) {
				AnimatorControllerParameter parameter = controller.parameters[i];
				if (
					parameter.name.StartsWith(ParamTextVisible) ||
					parameter.name.StartsWith(ParamTextPointer) ||
					parameter.name.StartsWith(ParamTextLogicPrefix) ||
					parameter.name.StartsWith(ParamTextSyncPrefix) ||
					parameter.name.StartsWith(ParamCharPrefix)
				) {
					controller.RemoveParameter(i);
				}
			}

			return true;
		}

		private AnimationClip[] GetCharAnimations() {
			AnimationClip[] animationClips = Resources.LoadAll<AnimationClip>(CharacterAnimationFolder);
			AnimationClip[] orderedAnimationClips = new AnimationClip[CharacterLimit];

			// Order the animation clips so they match their index
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

		private AnimatorControllerLayer CreateLogicLayer(AnimatorController controller, int logicId)
		{
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			AnimatorStateMachine stateMachine;

			layer.name = ParamTextLogicPrefix + logicId.ToString();
			layer.defaultWeight = 0;
			layer.stateMachine = stateMachine = new AnimatorStateMachine();
			layer.stateMachine.name = layer.name;
			AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

			// Default state
			AnimatorState stateDisabled = stateMachine.AddState("Disabled", new Vector3(0f, 200f, 0f));
			stateDisabled.writeDefaultValues = false;

			// Standby state - waits for updates to the pointer
			AnimatorState stateStandby = stateMachine.AddState("StandBy", new Vector3(0f, 500f, 0f));
			stateStandby.writeDefaultValues = false;

			AnimatorStateTransition enabledTransition = stateDisabled.AddExitTransition(false);
			enabledTransition.destinationState = stateStandby;
			enabledTransition.AddCondition(AnimatorConditionMode.NotEqual, 0, ParamTextPointer);
			enabledTransition.duration = 0;

			AnimatorStateTransition disabledTransition = stateMachine.AddAnyStateTransition(stateDisabled);
			disabledTransition.AddCondition(AnimatorConditionMode.Equals, 0, ParamTextPointer);
			disabledTransition.duration = 0;

			// Create Character states
			AnimatorState[] stateChars = new AnimatorState[CharacterCount];
			for (int i = logicId; i < CharacterCount; i += CharacterSyncParamsSize) {
				int charIndex = i;

				AnimatorState stateChar = stateChars[charIndex] = stateMachine.AddState("Char" + charIndex.ToString(), new Vector3(1000f, 500f + 50f * charIndex, 0f));
				stateChar.writeDefaultValues = false;

				VRCAvatarParameterDriver parameterDriver = stateChar.AddStateMachineBehaviour<VRCAvatarParameterDriver>();
				parameterDriver.parameters.Add(new Parameter() {
					name = ParamCharPrefix + charIndex.ToString(),
					value = (float)charIndex * 0.01f
				});
			}

			// Create Pointer states
			for (int i = 0; i < PointerCount; i++) {
				int pointerIndex = i + 1;

				float pointerPosOffsetY = 500f;

				AnimatorState statePointerStart = stateMachine.AddState("PointerStart" + pointerIndex.ToString(), new Vector3(500f, pointerPosOffsetY + 100f * i, 0f));
				statePointerStart.writeDefaultValues = false;

				AnimatorStateTransition pointerStartTransition = stateStandby.AddExitTransition(false);
				pointerStartTransition.destinationState = statePointerStart;
				pointerStartTransition.AddCondition(AnimatorConditionMode.Equals, pointerIndex, ParamTextPointer);
				pointerStartTransition.duration = 0;

				AnimatorStateTransition pointerReturnTransition = statePointerStart.AddExitTransition(false);
				pointerReturnTransition.destinationState = stateStandby;
				pointerReturnTransition.AddCondition(AnimatorConditionMode.NotEqual, pointerIndex, ParamTextPointer);
				pointerReturnTransition.duration = 0;

				for (int j = logicId; j < CharacterCount; j += CharacterSyncParamsSize) {
					int charIndex = j;
					AnimatorState stateChar = stateChars[charIndex];

					AnimatorStateTransition charEntryTransition1 = statePointerStart.AddExitTransition(false);
					charEntryTransition1.destinationState = stateChar;
					charEntryTransition1.AddCondition(AnimatorConditionMode.Equals, charIndex, ParamTextSyncPrefix + logicId.ToString());
					charEntryTransition1.AddCondition(AnimatorConditionMode.Less, (float)charIndex * 0.01f - 0.005f, ParamCharPrefix + charIndex.ToString());
					charEntryTransition1.duration = 0;

					AnimatorStateTransition charEntryTransition2 = statePointerStart.AddExitTransition(false);
					charEntryTransition2.destinationState = stateChar;
					charEntryTransition2.AddCondition(AnimatorConditionMode.Equals, charIndex, ParamTextSyncPrefix + logicId.ToString());
					charEntryTransition2.AddCondition(AnimatorConditionMode.Greater, (float)charIndex * 0.01f + 0.005f, ParamCharPrefix + charIndex.ToString());
					charEntryTransition2.duration = 0;

					AnimatorStateTransition charExitTransition = stateChar.AddExitTransition(false);
					charExitTransition.destinationState = statePointerStart;
					charExitTransition.AddCondition(AnimatorConditionMode.Equals, pointerIndex, ParamTextPointer);
					charExitTransition.duration = 0;
				}
			}

			return layer;
		}

		private AnimatorControllerLayer CreateCharLayer(AnimatorController controller, int charId, AnimationClip animation)
		{
			AnimatorControllerLayer layer = new AnimatorControllerLayer();

			layer.name = ParamCharPrefix + charId.ToString();
			layer.defaultWeight = 1f;
			layer.stateMachine = new AnimatorStateMachine();
			layer.stateMachine.name = layer.name;
			AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

			AnimatorState state = layer.stateMachine.AddState(ParamCharPrefix + charId.ToString(), new Vector3(300f, 100f, 0f));
			state.motion = animation;
			state.timeParameter = ParamCharPrefix + charId.ToString();
			state.timeParameterActive = true;
			state.writeDefaultValues = false;

			return layer;
		}
	}
}
