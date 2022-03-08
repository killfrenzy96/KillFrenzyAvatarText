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

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

using System.Collections.Generic;

using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using AnimatorController = UnityEditor.Animations.AnimatorController;

using Driver = VRC.SDK3.Avatars.Components.VRCAvatarParameterDriver;
using DriverParameter = VRC.SDKBase.VRC_AvatarParameterDriver.Parameter;

using KillFrenzy.AvatarTextTools.Settings;


namespace KillFrenzy.AvatarTextTools.Utility
{
	public static class KatAnimatorInstaller
	{
		public static bool InstallToAnimator(AnimatorController controller, bool installKeyboard)
		{
			AnimationClip[] animationChars = GetCharAnimations(false);
			AnimationClip[] animationCharsEnd = GetCharAnimations(true);
			AnimationClip animationDisable = Resources.Load<AnimationClip>(KatSettings.CharacterAnimationFolder + "KAT_Disable");
			AnimationClip animationEnable = Resources.Load<AnimationClip>(KatSettings.CharacterAnimationFolder + "KAT_Enable");
			if (animationChars == null || animationCharsEnd == null || animationDisable == null || animationEnable == null) {
				Debug.LogError("Failed: Resources/" + KatSettings.CharacterAnimationFolder + " is missing some animations.");
				return false;
			}

			// Add parameters
			// controller.AddParameter("IsLocal", AnimatorControllerParameterType.Bool);
			controller.AddParameter(KatSettings.ParamTextVisible, AnimatorControllerParameterType.Bool);
			controller.AddParameter(KatSettings.ParamKeyboardPrefix, AnimatorControllerParameterType.Bool);
			controller.AddParameter(KatSettings.ParamTextPointer, AnimatorControllerParameterType.Int);

			if (installKeyboard) {
				for (int i = 0; i < KatSettings.KeyboardKeysCount; i++) {
					controller.AddParameter(KatSettings.ParamKeyboardPressed + i.ToString(), AnimatorControllerParameterType.Float);
				}
			}

			for (int i = 0; i < KatSettings.SyncParamsSize; i++) {
				controller.AddParameter(KatSettings.ParamTextSyncPrefix + i.ToString(), AnimatorControllerParameterType.Float);
			}

			// Add Layers
			controller.AddLayer(CreateToggleLayer(controller, animationDisable, animationEnable, installKeyboard));

			if (installKeyboard) {
				controller.AddLayer(CreateKeyboardLayer(controller));
			}

			for (int i = 0; i < KatSettings.SyncParamsSize; i++) {
				controller.AddLayer(CreateSyncLayer(controller, i, animationChars, animationCharsEnd, installKeyboard));
			}

			return true;
		}

		public static bool RemoveFromAnimator(AnimatorController controller) {
			// Remove Blend Trees
			// AssetDatabase.RemoveObjectFromAsset();
			// BlendTree[] blendTrees = controller.GetStateEffectiveMotion();

			// Remove Layers
			for (int i = controller.layers.Length - 1; i >= 0; i--) {
				AnimatorControllerLayer layer = controller.layers[i];
				if (
					layer.name.StartsWith(KatSettings.ParamTextVisible) ||
					layer.name.StartsWith(KatSettings.ParamKeyboardPrefix) ||
					layer.name.StartsWith(KatSettings.ParamKeyboardPressed) ||
					layer.name.StartsWith(KatSettings.ParamTextSyncPrefix)
				) {
					// Remove Blend Trees
					foreach (ChildAnimatorState childState in layer.stateMachine.states) {
						if (childState.state.motion != null && childState.state.motion is BlendTree) {
							AssetDatabase.RemoveObjectFromAsset(childState.state.motion);
						}
					}
					controller.RemoveLayer(i);
				}
			}

			// Remove Parameters
			for (int i = controller.parameters.Length - 1; i >= 0; i--) {
				AnimatorControllerParameter parameter = controller.parameters[i];
				if (
					parameter.name.StartsWith(KatSettings.ParamTextVisible) ||
					parameter.name.StartsWith(KatSettings.ParamKeyboardPrefix) ||
					parameter.name.StartsWith(KatSettings.ParamKeyboardPressed) ||
					parameter.name.StartsWith(KatSettings.ParamTextPointer) ||
					parameter.name.StartsWith(KatSettings.ParamTextSyncPrefix)
				) {
					controller.RemoveParameter(i);
				}
			}

			return true;
		}

		private static AnimationClip[] GetCharAnimations(bool end = false) {
			AnimationClip[] animationClips = Resources.LoadAll<AnimationClip>(KatSettings.CharacterAnimationFolder);
			AnimationClip[] orderedAnimationClips = new AnimationClip[KatSettings.TextLength + 1];

			// Order the animation clips so they match their index
			for (int i = 0; i < KatSettings.TextLength; i++) {
				orderedAnimationClips[i] = null;
				foreach (var animationClip in animationClips) {
					string name;
					if (end) {
						name = KatSettings.CharacterAnimationClipNamePrefix + "End" + i.ToString();
					} else {
						name = KatSettings.CharacterAnimationClipNamePrefix + i.ToString();
					}
					if (animationClip.name == name) {
						orderedAnimationClips[i] = animationClip;
						break;
					}
				}
				if (orderedAnimationClips[i] == null) {
					return null;
				}
			}

			// Add the clear animation clip to the end
			orderedAnimationClips[KatSettings.TextLength] = null;
			foreach (var animationClip in animationClips) {
				if (animationClip.name == KatSettings.CharacterAnimationClipNamePrefix + "Clear") {
					orderedAnimationClips[KatSettings.TextLength] = animationClip;
					break;
				}
			}
			if (orderedAnimationClips[KatSettings.TextLength] == null) {
				return null;
			}

			return orderedAnimationClips;
		}

		private static AnimatorControllerLayer CreateToggleLayer(AnimatorController controller, AnimationClip animationDisable, AnimationClip animationEnable, bool installKeyboard)
		{
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			AnimatorStateMachine stateMachine;

			layer.name = KatSettings.ParamTextVisible;
			layer.defaultWeight = 1f;
			layer.stateMachine = stateMachine = new AnimatorStateMachine();
			layer.stateMachine.name = layer.name;
			AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

			// Hide KAT state
			AnimatorState stateDisabled = stateMachine.AddState("Disabled", new Vector3(300f, 200f, 0f));
			stateDisabled.motion = animationDisable;
			stateDisabled.writeDefaultValues = false;
			stateDisabled.speed = 0f;

			// Show KAT state
			AnimatorState stateEnabled = stateMachine.AddState("Enabled", new Vector3(300f, 300f, 0f));
			stateEnabled.motion = animationEnable;
			stateEnabled.writeDefaultValues = false;
			stateEnabled.speed = 0f;

			AnimatorStateTransition enableTransition = stateDisabled.AddExitTransition(false);
			enableTransition.destinationState = stateEnabled;
			enableTransition.AddCondition(AnimatorConditionMode.If, 0, KatSettings.ParamTextVisible);
			enableTransition.duration = 0;

			AnimatorStateTransition disabledTransition = stateEnabled.AddExitTransition(false);
			disabledTransition.destinationState = stateDisabled;
			disabledTransition.AddCondition(AnimatorConditionMode.IfNot, 0, KatSettings.ParamTextVisible);
			disabledTransition.duration = 0;

			if (installKeyboard) {
				disabledTransition.AddCondition(AnimatorConditionMode.IfNot, 0, KatSettings.ParamKeyboardPrefix);

				AnimatorStateTransition enableTransition2 = stateDisabled.AddExitTransition(false);
				enableTransition2.destinationState = stateEnabled;
				enableTransition2.AddCondition(AnimatorConditionMode.If, 0, KatSettings.ParamKeyboardPrefix);
				enableTransition2.duration = 0;
			}

			return layer;
		}

		private static AnimatorControllerLayer CreateSyncLayer(AnimatorController controller, int logicId, AnimationClip[] animationChars, AnimationClip[] animationCharsEnd, bool installKeyboard)
		{
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			AnimatorStateMachine stateMachine;

			layer.name = KatSettings.ParamTextSyncPrefix + logicId.ToString();
			layer.defaultWeight = 1f;
			layer.stateMachine = stateMachine = new AnimatorStateMachine();
			layer.stateMachine.name = layer.name;
			AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

			// Default state
			AnimatorState stateDisabled = stateMachine.AddState("Disabled", new Vector3(300f, 200f, 0f));
			stateDisabled.writeDefaultValues = false;
			stateDisabled.speed = 0f;

			AnimatorStateTransition disabledTransition = stateMachine.AddAnyStateTransition(stateDisabled);
			disabledTransition.AddCondition(AnimatorConditionMode.Equals, 0, KatSettings.ParamTextPointer);
			disabledTransition.duration = 0;
			disabledTransition.canTransitionToSelf = false;

			// Standby state - waits for updates to the pointer
			AnimatorState stateStandby = stateMachine.AddState("StandBy", new Vector3(300f, 500f, 0f));
			stateStandby.writeDefaultValues = false;
			stateStandby.speed = 0f;

			AnimatorStateTransition enabledTransition = stateDisabled.AddExitTransition(false);
			enabledTransition.destinationState = stateStandby;
			enabledTransition.AddCondition(AnimatorConditionMode.NotEqual, 0, KatSettings.ParamTextPointer);
			enabledTransition.duration = 0;

			// Create pointer animations states
			for (int i = 0; i < KatSettings.PointerCount; i++) {
				int pointerIndex = i + 1;
				int charIndex = KatSettings.SyncParamsSize * i + logicId;

				float pointerPosOffsetY = 500f + 50f * i;

				BlendTree blendTree = new BlendTree();
				// controller.CreateBlendTreeInController("Char" + charIndex + " BlendTree", out blendTree);
				blendTree.name = "Char" + charIndex + " BlendTree";
				blendTree.blendParameter = KatSettings.ParamTextSyncPrefix + logicId.ToString();
				blendTree.blendType = BlendTreeType.Simple1D;
				blendTree.useAutomaticThresholds = false;
				blendTree.AddChild(animationChars[charIndex], -1.0f);
				blendTree.AddChild(animationCharsEnd[charIndex], 1.0f);
				AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(controller));

				AnimatorState statePointerChar = stateMachine.AddState("Pointer" + pointerIndex.ToString() + " Char" + charIndex, new Vector3(800f, pointerPosOffsetY, 0f));
				statePointerChar.motion = blendTree;
				statePointerChar.timeParameter = KatSettings.ParamTextSyncPrefix + logicId.ToString();
				statePointerChar.timeParameterActive = true;
				statePointerChar.writeDefaultValues = false;
				statePointerChar.speed = 0f;

				// Create transitions to activate pointer
				AnimatorStateTransition pointerStartTransition = stateStandby.AddExitTransition(false);
				pointerStartTransition.destinationState = statePointerChar;
				pointerStartTransition.AddCondition(AnimatorConditionMode.Equals, pointerIndex, KatSettings.ParamTextPointer);
				pointerStartTransition.duration = 0;

				AnimatorStateTransition pointerReturnTransition = statePointerChar.AddExitTransition(false);
				pointerReturnTransition.destinationState = stateStandby;
				pointerReturnTransition.AddCondition(AnimatorConditionMode.NotEqual, pointerIndex, KatSettings.ParamTextPointer);
				pointerReturnTransition.duration = 0;

				// Create alternate transitions to point to a single character at a time (for in-game keyboard)
				if (installKeyboard) {
					AnimatorStateTransition altStartTransition = stateStandby.AddExitTransition(false);
					altStartTransition.destinationState = statePointerChar;
					altStartTransition.AddCondition(AnimatorConditionMode.Equals, KatSettings.PointerAltSyncOffset + charIndex + 1, KatSettings.ParamTextPointer);
					altStartTransition.duration = 0;

					pointerReturnTransition.AddCondition(AnimatorConditionMode.NotEqual, KatSettings.PointerAltSyncOffset + charIndex + 1, KatSettings.ParamTextPointer);
				}
			}

			// Create clear state
			AnimatorState stateClear = stateMachine.AddState("Clear Text", new Vector3(800f, 500f - 50f, 0f));
			stateClear.motion = animationChars[KatSettings.TextLength];
			stateClear.timeParameter = KatSettings.ParamTextSyncPrefix + logicId.ToString();
			stateClear.timeParameterActive = true;
			stateClear.writeDefaultValues = false;
			stateClear.speed = 0f;

			AnimatorStateTransition clearStartTransition = stateStandby.AddExitTransition(false);
			clearStartTransition.destinationState = stateClear;
			clearStartTransition.AddCondition(AnimatorConditionMode.Equals, 255, KatSettings.ParamTextPointer);
			clearStartTransition.duration = 0;

			AnimatorStateTransition clearReturnTransition = stateClear.AddExitTransition(false);
			clearReturnTransition.destinationState = stateStandby;
			clearReturnTransition.AddCondition(AnimatorConditionMode.NotEqual, 255, KatSettings.ParamTextPointer);
			clearReturnTransition.duration = 0;

			return layer;
		}

		private static AnimatorControllerLayer CreateKeyboardLayer(AnimatorController controller)
		{
			AnimationClip animationKeyboardDisable = Resources.Load<AnimationClip>(KatSettings.CharacterAnimationFolder + "KAT_Keyboard_Disable");
			AnimationClip animationKeyboardEnable = Resources.Load<AnimationClip>(KatSettings.CharacterAnimationFolder + "KAT_Keyboard_Enable");
			AnimationClip animationKeyboardInit = Resources.Load<AnimationClip>(KatSettings.CharacterAnimationFolder + "KAT_Keyboard_Init");

			if (animationKeyboardDisable == null || animationKeyboardEnable == null || animationKeyboardInit == null) {
				Debug.LogError("Failed: Resources/" + KatSettings.CharacterAnimationFolder + " is missing keyboard animations.");
				return null;
			}

			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			AnimatorStateMachine stateMachine;

			layer.name = KatSettings.ParamKeyboardPrefix;
			layer.defaultWeight = 1f;
			layer.stateMachine = stateMachine = new AnimatorStateMachine();
			layer.stateMachine.name = layer.name;
			AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

			// Default state
			AnimatorState stateDisabled = stateMachine.AddState("Disabled", new Vector3(300f, 200f, 0f));
			stateDisabled.motion = animationKeyboardDisable;
			stateDisabled.writeDefaultValues = false;
			stateDisabled.speed = 0f;

			AnimatorStateTransition disabledTransition = stateMachine.AddAnyStateTransition(stateDisabled);
			disabledTransition.AddCondition(AnimatorConditionMode.IfNot, 0, KatSettings.ParamKeyboardPrefix);
			disabledTransition.duration = 0;
			disabledTransition.canTransitionToSelf = false;

			// Init state - initializes keyboard
			AnimatorState stateInit = stateMachine.AddState("Init", new Vector3(300f, 400f, 0f));
			stateInit.motion = animationKeyboardInit;
			stateInit.writeDefaultValues = false;

			AnimatorStateTransition initTransition = stateDisabled.AddExitTransition(false);
			initTransition.destinationState = stateInit;
			initTransition.AddCondition(AnimatorConditionMode.If, 0, KatSettings.ParamKeyboardPrefix);
			initTransition.duration = 0;

			Driver driverInit = stateInit.AddStateMachineBehaviour<Driver>();
			driverInit.localOnly = true;
			driverInit.parameters = new List<DriverParameter>();

			DriverParameter driverStateInit = new DriverParameter();
			driverStateInit.name = KatSettings.ParamTextPointer;
			driverStateInit.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
			driverStateInit.value = KatSettings.PointerAltSyncOffset;
			driverInit.parameters.Add(driverStateInit);

			// Standby state - waits for updates to the pointer
			AnimatorState stateStandby = stateMachine.AddState("StandBy", new Vector3(300f, 500f, 0f));
			stateStandby.motion = animationKeyboardEnable;
			stateStandby.writeDefaultValues = false;
			stateStandby.speed = 0f;

			AnimatorStateTransition enabledTransition = stateInit.AddExitTransition(false);
			enabledTransition.destinationState = stateStandby;
			enabledTransition.AddCondition(AnimatorConditionMode.If, 0, KatSettings.ParamKeyboardPrefix);
			enabledTransition.duration = 0;
			enabledTransition.hasExitTime = true;
			enabledTransition.exitTime = 0.1f;

			// Fix Pointer - places pointer within offset if out of range
			AnimatorState stateFix = stateMachine.AddState("Fix Pointer", new Vector3(0f, 500f, 0f));
			// stateFix.motion = animationKeyboardEnable;
			stateFix.writeDefaultValues = false;
			stateFix.speed = 0f;

			AnimatorStateTransition stateFixEnter1 = stateStandby.AddExitTransition(false);
			stateFixEnter1.destinationState = stateFix;
			stateFixEnter1.AddCondition(AnimatorConditionMode.Greater, KatSettings.PointerAltSyncOffset + KatSettings.TextLength + 1, KatSettings.ParamTextPointer);
			stateFixEnter1.duration = 0;

			AnimatorStateTransition stateFixEnter2 = stateStandby.AddExitTransition(false);
			stateFixEnter2.destinationState = stateFix;
			stateFixEnter2.AddCondition(AnimatorConditionMode.Less, KatSettings.PointerAltSyncOffset, KatSettings.ParamTextPointer);
			stateFixEnter2.duration = 0;

			AnimatorStateTransition stateFixExit = stateFix.AddExitTransition(false);
			stateFixExit.destinationState = stateStandby;
			stateFixExit.AddCondition(AnimatorConditionMode.If, 0, KatSettings.ParamKeyboardPrefix);
			stateFixExit.duration = 0;

			Driver driverFix = stateFix.AddStateMachineBehaviour<Driver>();
			driverFix.localOnly = true;
			driverFix.parameters = new List<DriverParameter>();

			DriverParameter driverParameterFix = new DriverParameter();
			driverParameterFix.name = KatSettings.ParamTextPointer;
			driverParameterFix.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
			driverParameterFix.value = KatSettings.PointerAltSyncOffset;
			driverFix.parameters.Add(driverParameterFix);

			// Create keyboard presses
			for (int i = 0; i < KatSettings.KeyboardKeysCount; i++) {
				int key = i;
				string keyString = key.ToString();
				float positionY = 500f + 50f * i;

				AnimatorState stateKeyPress = stateMachine.AddState("Key" + keyString, new Vector3(600f, positionY, 0f));
				// stateKeyPress.motion = animationKeyboardEnable;
				stateKeyPress.writeDefaultValues = false;
				stateKeyPress.speed = 0f;

				AnimatorStateTransition stateKeyDown = stateStandby.AddExitTransition(false);
				stateKeyDown.destinationState = stateKeyPress;
				stateKeyDown.AddCondition(AnimatorConditionMode.Greater, 0.5f, KatSettings.ParamKeyboardPressed + keyString);
				stateKeyDown.duration = 0;

				AnimatorStateTransition stateKeyUp = stateKeyPress.AddExitTransition(false);
				stateKeyUp.destinationState = stateStandby;
				stateKeyUp.AddCondition(AnimatorConditionMode.Less, 0.5f, KatSettings.ParamKeyboardPressed + keyString);
				stateKeyUp.duration = 0;
				// stateKeyUp.hasExitTime = true;
				// stateKeyUp.exitTime = 0.2f;

				Driver driver = stateKeyPress.AddStateMachineBehaviour<Driver>();
				driver.localOnly = true;
				driver.parameters = new List<DriverParameter>();

				DriverParameter driverParameterPointer = new DriverParameter();
				driverParameterPointer.name = KatSettings.ParamTextPointer;
				driverParameterPointer.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Add;
				driverParameterPointer.value = 1;
				driver.parameters.Add(driverParameterPointer);

				for (int j = 0; j < KatSettings.SyncParamsSize; j++) {
					DriverParameter driverParameterChar = new DriverParameter();
					driverParameterChar.name = KatSettings.ParamTextSyncPrefix + j.ToString();
					driverParameterChar.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
					driverParameterChar.value = ConvertKeyToFloat(key);
					driver.parameters.Add(driverParameterChar);
				}
			}

			// Create clear key
			/*AnimatorState stateClearKey = stateMachine.AddState("Clear Key", new Vector3(0f, 500f, 0f));
			stateClearKey.motion = animationKeyboardEnable;
			stateClearKey.writeDefaultValues = false;
			stateClearKey.speed = 0f;

			AnimatorStateTransition stateClearPressed = stateStandby.AddExitTransition(false);
			stateClearPressed.destinationState = stateClearKey;
			stateClearPressed.AddCondition(AnimatorConditionMode.Greater, 0.5f, KatSettings.ParamKeyboardPressed + "Clear");
			stateClearPressed.duration = 0;

			AnimatorStateTransition stateClearPressedReturn = stateClearKey.AddExitTransition(false);
			stateClearPressedReturn.destinationState = stateStandby;
			stateClearPressedReturn.AddCondition(AnimatorConditionMode.Less, 0.05f, KatSettings.ParamKeyboardPressed);
			stateClearPressedReturn.duration = 0;

			Driver driverClearKey = stateClearKey.AddStateMachineBehaviour<Driver>();
			driverClearKey.localOnly = true;
			driverClearKey.parameters = new List<DriverParameter>();

			DriverParameter driverParameterClearKey = new DriverParameter();
			driverParameterClearKey.name = KatSettings.ParamTextPointer;
			driverParameterClearKey.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
			driverParameterClearKey.value = 255;
			driverClearKey.parameters.Add(driverParameterClearKey);*/

			// Create backspace state

			// Create

			return layer;
		}

		private static float ConvertKeyToFloat(int key)
		{
			float value = (float)key;
			if (value > 127.5f) {
				value = value - 256.0f;
			}
			value = value / 127.0f;
			return value;
		}
	}
}

#endif
