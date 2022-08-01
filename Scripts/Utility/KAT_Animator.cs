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

#if VRC_SDK_VRCSDK3
	using Driver = VRC.SDK3.Avatars.Components.VRCAvatarParameterDriver;
	using DriverParameter = VRC.SDKBase.VRC_AvatarParameterDriver.Parameter;
	using LocomotionControl = VRC.SDK3.Avatars.Components.VRCAnimatorLocomotionControl;
#endif

using KillFrenzy.AvatarTextTools.Settings;


namespace KillFrenzy.AvatarTextTools.Utility
{
	public static class KatAnimatorInstaller
	{
		public static bool InstallToAnimator(AnimatorController controller, KatSettings settings)
		{
			KatAnimations animations = new KatAnimations(settings);

			if (animations.valid == false) {
				Debug.LogError("Failed: Resources/" + settings.CharacterAnimationFolder + " is missing some animations.");
				return false;
			}

			// Add parameters
			// controller.AddParameter("IsLocal", AnimatorControllerParameterType.Bool);
			controller.AddParameter(settings.ParamTextVisible, AnimatorControllerParameterType.Bool);
			controller.AddParameter(settings.ParamTextPointer, AnimatorControllerParameterType.Int);

			for (int i = 0; i < settings.SyncParamsSize; i++) {
				controller.AddParameter(settings.ParamTextSyncPrefix + i.ToString(), AnimatorControllerParameterType.Float);
			}

			#if VRC_SDK_VRCSDK3
				if (settings.InstallKeyboard) {
					AddParameterIfMissing(controller, "IsLocal", AnimatorControllerParameterType.Bool);
					AddParameterIfMissing(controller, "VRMode", AnimatorControllerParameterType.Int);
					controller.AddParameter(settings.ParamKeyboardPrefix, AnimatorControllerParameterType.Bool);
					controller.AddParameter(settings.ParamKeyboardProximity, AnimatorControllerParameterType.Float);
					controller.AddParameter(settings.ParamKeyboardProximityExit, AnimatorControllerParameterType.Float);
					controller.AddParameter(settings.ParamKeyboardHighlight, AnimatorControllerParameterType.Float);
					controller.AddParameter(settings.ParamKeyboardPressedClear, AnimatorControllerParameterType.Float);
					controller.AddParameter(settings.ParamKeyboardPressedBackspace, AnimatorControllerParameterType.Float);
					controller.AddParameter(settings.ParamKeyboardPressedShiftL, AnimatorControllerParameterType.Float);
					controller.AddParameter(settings.ParamKeyboardPressedShiftR, AnimatorControllerParameterType.Float);

					for (int i = 0; i < settings.KeyboardKeysCount; i++) {
						controller.AddParameter(settings.ParamKeyboardPressed + i.ToString(), AnimatorControllerParameterType.Float);
					}
				}

				// Add Layers
				controller.AddLayer(CreateToggleLayer(controller, settings, animations));

				if (settings.InstallKeyboard) {
					controller.AddLayer(CreateKeyboardLayer(controller, settings, animations));
					controller.AddLayer(CreateKeyboardProximityLayer(controller, settings, animations));
					controller.AddLayer(CreateKeyboardShiftLayer(controller, settings, animations));
					controller.AddLayer(CreateKeyboardHighlightLayer(controller, settings, animations));
				}
			#endif

			for (int i = 0; i < settings.SyncParamsSize; i++) {
				controller.AddLayer(CreateSyncLayer(controller, settings, animations, i));
			}

			EditorUtility.SetDirty(controller);
			Debug.Log("Installation to animator completed.");
			return true;
		}

		public static bool RemoveFromAnimator(AnimatorController controller, KatSettings settings) {
			// Remove Blend Trees
			// AssetDatabase.RemoveObjectFromAsset();
			// BlendTree[] blendTrees = controller.GetStateEffectiveMotion();

			// Remove Layers
			for (int i = controller.layers.Length - 1; i >= 0; i--) {
				AnimatorControllerLayer layer = controller.layers[i];
				if (
					layer.name.StartsWith(settings.ParamTextVisible) ||
					layer.name.StartsWith(settings.ParamKeyboardPrefix) ||
					layer.name.StartsWith(settings.ParamTextSyncPrefix)
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
					parameter.name.StartsWith(settings.ParamTextVisible) ||
					parameter.name.StartsWith(settings.ParamKeyboardPrefix) ||
					parameter.name.StartsWith(settings.ParamKeyboardPressed) ||
					parameter.name.StartsWith(settings.ParamTextPointer) ||
					parameter.name.StartsWith(settings.ParamTextSyncPrefix)
				) {
					controller.RemoveParameter(i);
				}
			}

			EditorUtility.SetDirty(controller);
			Debug.Log("Removal from animator completed.");
			return true;
		}

		private static AnimatorControllerLayer CreateToggleLayer(AnimatorController controller, KatSettings settings, KatAnimations animations)
		{
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			AnimatorStateMachine stateMachine;

			layer.name = settings.ParamTextVisible;
			layer.defaultWeight = 1f;
			layer.stateMachine = stateMachine = new AnimatorStateMachine();
			layer.stateMachine.name = layer.name;
			AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

			// Hide KAT state
			AnimatorState stateDisabled = CreateState(stateMachine, "Disabled", new Vector3(300f, 200f, 0f), animations.disable);

			// Show KAT state
			AnimatorState stateEnabled = CreateState(stateMachine, "Enabled", new Vector3(300f, 300f, 0f), animations.enable);
			AnimatorStateTransition transitionEnable = CreateTransition(stateDisabled, stateEnabled);
			transitionEnable.AddCondition(AnimatorConditionMode.If, 0, settings.ParamTextVisible);
			AnimatorStateTransition transitionDisable = CreateTransition(stateEnabled, stateDisabled);
			transitionDisable.AddCondition(AnimatorConditionMode.IfNot, 0, settings.ParamTextVisible);

			if (settings.InstallKeyboard) {
				// Add extra transitions to keep text visible if keyboard is visible
				transitionDisable.AddCondition(AnimatorConditionMode.IfNot, 0, settings.ParamKeyboardPrefix);

				AnimatorStateTransition transitionEnable2 = CreateTransition(stateDisabled, stateEnabled);
				transitionEnable2.AddCondition(AnimatorConditionMode.If, 0, settings.ParamKeyboardPrefix);
			}

			#if CVR_CCK_EXISTS
				if (settings.SDK == KatSDK.ChilloutVR) {
					layer.avatarMask = animations.avatarMask;
				}
			#endif

			return layer;
		}

		private static AnimatorControllerLayer CreateSyncLayer(AnimatorController controller, KatSettings settings, KatAnimations animations, int logicId)
		{
			AnimatorControllerLayer layer = new AnimatorControllerLayer();
			AnimatorStateMachine stateMachine;

			layer.name = settings.ParamTextSyncPrefix + logicId.ToString();
			layer.defaultWeight = 1f;
			layer.stateMachine = stateMachine = new AnimatorStateMachine();
			layer.stateMachine.name = layer.name;
			AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

			// Default state
			AnimatorState stateDisabled = CreateState(stateMachine, "Disabled", new Vector3(300f, 200f, 0f), animations.nothing);
			AnimatorStateTransition transitionDisabled = CreateTransition(stateMachine, stateDisabled);
			transitionDisabled.AddCondition(AnimatorConditionMode.Equals, 0, settings.ParamTextPointer);

			// Standby state - waits for updates to the pointer
			AnimatorState stateStandby = CreateState(stateMachine, "StandBy", new Vector3(300f, 500f, 0f), animations.nothing);
			AnimatorStateTransition transitionStandby = CreateTransition(stateDisabled, stateStandby);
			transitionStandby.AddCondition(AnimatorConditionMode.NotEqual, 0, settings.ParamTextPointer);

			if (settings.InstallKeyboard) {
				AnimatorStateTransition transitionDisabled2 = CreateTransition(stateMachine, stateDisabled);
				transitionDisabled2.AddCondition(AnimatorConditionMode.Greater, settings.KeyboardBackspaceMode - 0.005f, settings.ParamTextSyncPrefix + "0");
				transitionDisabled2.AddCondition(AnimatorConditionMode.Less, settings.KeyboardBackspaceMode + 0.005f, settings.ParamTextSyncPrefix + "0");

				transitionStandby.AddCondition(AnimatorConditionMode.Less, settings.KeyboardBackspaceMode - 0.005f, settings.ParamTextSyncPrefix + "0");

				AnimatorStateTransition transitionStandby2 = CreateTransition(stateDisabled, stateStandby);
				transitionStandby2.AddCondition(AnimatorConditionMode.NotEqual, 0, settings.ParamTextPointer);
				transitionStandby2.AddCondition(AnimatorConditionMode.Greater, settings.KeyboardBackspaceMode + 0.005f, settings.ParamTextSyncPrefix + "0");
			}

			// Create pointer animations states
			for (int i = 0; i < settings.PointerCount; i++) {
				int pointerIndex = i + 1;
				int charIndex = settings.SyncParamsSize * i + logicId;

				float pointerPosOffsetY = 500f + 50f * i;

				BlendTree blendTree = new BlendTree();
				// controller.CreateBlendTreeInController("Char" + charIndex + " BlendTree", out blendTree);
				blendTree.name = "Char" + charIndex + " BlendTree";
				blendTree.blendParameter = settings.ParamTextSyncPrefix + logicId.ToString();
				blendTree.blendType = BlendTreeType.Simple1D;
				blendTree.useAutomaticThresholds = false;
				blendTree.AddChild(animations.charsStart[charIndex], -1.0f);
				blendTree.AddChild(animations.charsEnd[charIndex], 1.0f);
				AssetDatabase.AddObjectToAsset(blendTree, AssetDatabase.GetAssetPath(controller));

				AnimatorState statePointerChar = CreateState(stateMachine, "Pointer" + pointerIndex.ToString() + " Char" + charIndex, new Vector3(800f, pointerPosOffsetY, 0f), blendTree);
				statePointerChar.timeParameter = settings.ParamTextSyncPrefix + logicId.ToString();
				statePointerChar.timeParameterActive = true;

				// Create transitions to activate pointer
				AnimatorStateTransition transitionPointerStart = CreateTransition(stateStandby, statePointerChar);
				transitionPointerStart.AddCondition(AnimatorConditionMode.Equals, pointerIndex, settings.ParamTextPointer);
				AnimatorStateTransition transitionPointerExit = CreateTransition(statePointerChar, stateStandby);
				transitionPointerExit.AddCondition(AnimatorConditionMode.NotEqual, pointerIndex, settings.ParamTextPointer);

				// Create alternate transitions to point to a single character at a time (for in-game keyboard)
				if (settings.InstallKeyboard && settings.PointerAltSyncOffset != 0) {
					AnimatorStateTransition transitionAltStart = CreateTransition(stateStandby, statePointerChar);
					transitionAltStart.AddCondition(AnimatorConditionMode.Equals, settings.PointerAltSyncOffset + charIndex + 1, settings.ParamTextPointer);

					transitionPointerExit.AddCondition(AnimatorConditionMode.NotEqual, settings.PointerAltSyncOffset + charIndex + 1, settings.ParamTextPointer);
				}
			}

			// Create clear state
			AnimatorState stateClear = CreateState(stateMachine, "Clear Text", new Vector3(800f, 500f - 50f, 0f), animations.clear);
			stateClear.timeParameter = settings.ParamTextSyncPrefix + logicId.ToString();
			stateClear.timeParameterActive = true;

			AnimatorStateTransition clearStartTransition = CreateTransition(stateStandby, stateClear);
			clearStartTransition.AddCondition(AnimatorConditionMode.Equals, 255, settings.ParamTextPointer);
			AnimatorStateTransition clearReturnTransition = CreateTransition(stateClear, stateStandby);
			clearReturnTransition.AddCondition(AnimatorConditionMode.NotEqual, 255, settings.ParamTextPointer);

			#if CVR_CCK_EXISTS
				if (settings.SDK == KatSDK.ChilloutVR) {
					layer.avatarMask = animations.avatarMask;
				}
			#endif

			return layer;
		}

		#if VRC_SDK_VRCSDK3
			private static AnimatorControllerLayer CreateKeyboardLayer(AnimatorController controller, KatSettings settings, KatAnimations animations)
			{
				AnimatorControllerLayer layer = new AnimatorControllerLayer();
				AnimatorStateMachine stateMachine;

				layer.name = settings.ParamKeyboardPrefix;
				layer.defaultWeight = 1f;
				layer.stateMachine = stateMachine = new AnimatorStateMachine();
				layer.stateMachine.name = layer.name;
				AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

				// Default state
				AnimatorState stateDisabled = CreateState(stateMachine, "Disabled", new Vector3(300f, 200f, 0f), animations.keyboardDisable);
				AnimatorStateTransition transitionDisabled = CreateTransition(stateMachine, stateDisabled);
				transitionDisabled.AddCondition(AnimatorConditionMode.IfNot, 0, settings.ParamKeyboardPrefix);

				Driver driverDisabled = stateDisabled.AddStateMachineBehaviour<Driver>();
				driverDisabled.localOnly = true;
				driverDisabled.parameters = new List<DriverParameter>();

				// Init state - initializes keyboard
				AnimatorState stateInit = CreateState(stateMachine, "Init", new Vector3(300f, 400f, 0f), animations.keyboardInit);
				stateInit.speed = 1.0f;
				AnimatorStateTransition transitionInit = CreateTransition(stateDisabled, stateInit);
				transitionInit.AddCondition(AnimatorConditionMode.If, 0, settings.ParamKeyboardPrefix);
				transitionInit.AddCondition(AnimatorConditionMode.IfNot, 0, settings.ParamTextVisible);

				Driver driverInit = stateInit.AddStateMachineBehaviour<Driver>();
				driverInit.localOnly = true;
				driverInit.parameters = new List<DriverParameter>();

				DriverParameter driverStateInit1 = new DriverParameter();
				driverStateInit1.name = settings.ParamTextPointer;
				driverStateInit1.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
				driverStateInit1.value = 255;
				driverInit.parameters.Add(driverStateInit1);

				DriverParameter driverStateInit2 = new DriverParameter();
				driverStateInit2.name = settings.ParamKeyboardHighlight;
				driverStateInit2.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
				driverStateInit2.value = ConvertKeyToFloat(100);
				driverInit.parameters.Add(driverStateInit2);

				for (int i = 0; i < settings.SyncParamsSize; i++) {
					DriverParameter driverParameterChar = new DriverParameter();
					driverParameterChar.name = settings.ParamTextSyncPrefix + i.ToString();
					driverParameterChar.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
					driverParameterChar.value = 0.0f;
					driverInit.parameters.Add(driverParameterChar);
				}

				// Alternate init state - initializes keyboard without touching the text if OSC is taking priority
				AnimatorState stateInitAlt = CreateState(stateMachine, "Init App Active", new Vector3(600f, 300f, 0f), animations.keyboardInit);
				stateInitAlt.speed = 1.0f;
				AnimatorStateTransition transitionInitAlt = CreateTransition(stateDisabled, stateInitAlt);
				transitionInitAlt.AddCondition(AnimatorConditionMode.If, 0, settings.ParamKeyboardPrefix);
				transitionInitAlt.AddCondition(AnimatorConditionMode.If, 0, settings.ParamTextVisible);

				Driver driverInitAlt = stateInitAlt.AddStateMachineBehaviour<Driver>();
				driverInitAlt.localOnly = true;
				driverInitAlt.parameters = new List<DriverParameter>();

				DriverParameter driverStateInitAlt = new DriverParameter();
				driverStateInitAlt.name = settings.ParamKeyboardHighlight;
				driverStateInitAlt.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
				driverStateInitAlt.value = ConvertKeyToFloat(100);
				driverInitAlt.parameters.Add(driverStateInitAlt);

				// Standby state - waits for updates to the pointer
				AnimatorState stateStandby = CreateState(stateMachine, "StandBy", new Vector3(300f, 500f, 0f), animations.keyboardEnable);
				AnimatorStateTransition transitionEnabled = CreateTransition(stateInit, stateStandby);
				transitionEnabled.AddCondition(AnimatorConditionMode.If, 0, settings.ParamKeyboardPrefix);
				transitionEnabled.hasExitTime = true;
				transitionEnabled.exitTime = 0.2f;

				Driver driverStandBy = stateStandby.AddStateMachineBehaviour<Driver>();
				driverStandBy.localOnly = true;
				driverStandBy.parameters = new List<DriverParameter>();

				DriverParameter driverResetHighlight = new DriverParameter();
				driverResetHighlight.name = settings.ParamKeyboardHighlight;
				driverResetHighlight.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
				driverResetHighlight.value = ConvertKeyToFloat(100);
				driverStandBy.parameters.Add(driverResetHighlight);

				// App active state - app is active, disable keyboard input
				AnimatorState stateAppActive = CreateState(stateMachine, "App Active", new Vector3(600f, 400f, 0f), animations.keyboardEnable);
				AnimatorStateTransition transitionAppActiveEnter = CreateTransition(stateStandby, stateAppActive);
				transitionAppActiveEnter.AddCondition(AnimatorConditionMode.If, 0, settings.ParamTextVisible);
				AnimatorStateTransition transitionAppActiveEnter2 = CreateTransition(stateStandby, stateAppActive);
				transitionAppActiveEnter2.AddCondition(AnimatorConditionMode.IfNot, 0, "IsLocal");
				AnimatorStateTransition transitionAppActiveExit = CreateTransition(stateAppActive, stateStandby);
				transitionAppActiveExit.AddCondition(AnimatorConditionMode.IfNot, 0, settings.ParamTextVisible);

				AnimatorStateTransition transitionEnabled2 = CreateTransition(stateInitAlt, stateAppActive);
				transitionEnabled2.AddCondition(AnimatorConditionMode.If, 0, settings.ParamKeyboardPrefix);
				transitionEnabled2.hasExitTime = true;
				transitionEnabled2.exitTime = 0.2f;

				// Fix Pointer - places pointer within offset if out of range
				AnimatorState stateFix = CreateState(stateMachine, "Fix Pointer", new Vector3(0f, 500f, 0f), animations.nothing);
				AnimatorStateTransition transitionFixEnter1 = CreateTransition(stateStandby, stateFix);
				transitionFixEnter1.AddCondition(AnimatorConditionMode.Greater, settings.PointerAltSyncOffset + settings.TextLength + 1, settings.ParamTextPointer);
				transitionFixEnter1.duration = 0.2f;
				transitionFixEnter1.hasFixedDuration = true;
				AnimatorStateTransition transitionFixEnter2 = CreateTransition(stateStandby, stateFix);
				transitionFixEnter2.AddCondition(AnimatorConditionMode.Less, settings.PointerAltSyncOffset, settings.ParamTextPointer);
				transitionFixEnter2.duration = 0.2f;
				transitionFixEnter2.hasFixedDuration = true;
				AnimatorStateTransition transitionFixExit = CreateTransition(stateFix, stateStandby);
				transitionFixExit.AddCondition(AnimatorConditionMode.If, 0, settings.ParamKeyboardPrefix);

				Driver driverFix = stateFix.AddStateMachineBehaviour<Driver>();
				driverFix.localOnly = true;
				driverFix.parameters = new List<DriverParameter>();

				DriverParameter driverParameterFix = new DriverParameter();
				driverParameterFix.name = settings.ParamTextPointer;
				driverParameterFix.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
				driverParameterFix.value = settings.PointerAltSyncOffset;
				driverFix.parameters.Add(driverParameterFix);

				// Create keyboard presses
				for (int i = 0; i < settings.KeyboardKeysCount; i++) {
					int key = i;
					string keyString = key.ToString();
					float positionY = 500f + 50f * i;

					AnimatorState stateKeyPress = CreateState(stateMachine, "Key" + keyString, new Vector3(600f, positionY, 0f), animations.nothing);
					AnimatorStateTransition transitionKeyDown = CreateTransition(stateStandby, stateKeyPress);
					transitionKeyDown.AddCondition(AnimatorConditionMode.Greater, 0.5f, settings.ParamKeyboardPressed + keyString);
					transitionKeyDown.duration = 0.2f;
					transitionKeyDown.hasFixedDuration = true;
					AnimatorStateTransition transitionKeyUp = CreateTransition(stateKeyPress, stateStandby);
					transitionKeyUp.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardPressed + keyString);

					Driver driver = stateKeyPress.AddStateMachineBehaviour<Driver>();
					driver.localOnly = true;
					driver.parameters = new List<DriverParameter>();

					DriverParameter driverParameterPointer = new DriverParameter();
					driverParameterPointer.name = settings.ParamTextPointer;
					driverParameterPointer.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Add;
					driverParameterPointer.value = 1;
					driver.parameters.Add(driverParameterPointer);

					DriverParameter driverHighlight = new DriverParameter();
					driverHighlight.name = settings.ParamKeyboardHighlight;
					driverHighlight.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
					driverHighlight.value = ConvertKeyToFloat(key);
					driver.parameters.Add(driverHighlight);

					for (int j = 0; j < settings.SyncParamsSize; j++) {
						DriverParameter driverParameterChar = new DriverParameter();
						driverParameterChar.name = settings.ParamTextSyncPrefix + j.ToString();
						driverParameterChar.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
						driverParameterChar.value = ConvertKeyToFloat(key);
						driver.parameters.Add(driverParameterChar);
					}
				}

				// Create clear key
				AnimatorState stateKeyClear = CreateState(stateMachine, "KeyClear", new Vector3(0f, 550f, 0f), animations.nothing);
				AnimatorStateTransition transitionKeyClearDown = CreateTransition(stateStandby, stateKeyClear);
				transitionKeyClearDown.AddCondition(AnimatorConditionMode.Greater, 0.5f, settings.ParamKeyboardPressedClear);
				transitionKeyClearDown.duration = 0.2f;
				transitionKeyClearDown.hasFixedDuration = true;
				AnimatorStateTransition transitionKeyClearUp = CreateTransition(stateKeyClear, stateStandby);
				transitionKeyClearUp.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardPressedClear);

				Driver driveKeyClear = stateKeyClear.AddStateMachineBehaviour<Driver>();
				driveKeyClear.localOnly = true;
				driveKeyClear.parameters = new List<DriverParameter>();

				DriverParameter driverKeyClearParameter = new DriverParameter();
				driverKeyClearParameter.name = settings.ParamTextPointer;
				driverKeyClearParameter.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
				driverKeyClearParameter.value = 255;
				driveKeyClear.parameters.Add(driverKeyClearParameter);

				DriverParameter driverHighlightClear = new DriverParameter();
				driverHighlightClear.name = settings.ParamKeyboardHighlight;
				driverHighlightClear.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
				driverHighlightClear.value = ConvertKeyToFloat(settings.KeyboardKeyClear);
				driveKeyClear.parameters.Add(driverHighlightClear);

				for (int i = 0; i < settings.SyncParamsSize; i++) {
					DriverParameter driverParameterChar = new DriverParameter();
					driverParameterChar.name = settings.ParamTextSyncPrefix + i.ToString();
					driverParameterChar.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
					driverParameterChar.value = 0.0f;
					driveKeyClear.parameters.Add(driverParameterChar);
				}

				// Create backspace start key
				AnimatorState stateKeyBkspStart = CreateState(stateMachine, "KeyBackspaceStart", new Vector3(0f, 600f, 0f), animations.nothing);
				AnimatorStateTransition transitionKeyBkspDownStart = CreateTransition(stateStandby, stateKeyBkspStart);
				transitionKeyBkspDownStart.AddCondition(AnimatorConditionMode.Greater, 0.5f, settings.ParamKeyboardPressedBackspace);
				transitionKeyBkspDownStart.duration = 0.2f;
				transitionKeyBkspDownStart.hasFixedDuration = true;

				Driver driverBkspStart = stateKeyBkspStart.AddStateMachineBehaviour<Driver>();
				driverBkspStart.localOnly = true;
				driverBkspStart.parameters = new List<DriverParameter>();

				DriverParameter driverHighlightBksp = new DriverParameter();
				driverHighlightBksp.name = settings.ParamKeyboardHighlight;
				driverHighlightBksp.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
				driverHighlightBksp.value = ConvertKeyToFloat(settings.KeyboardKeyBackspace);
				driverBkspStart.parameters.Add(driverHighlightBksp);

				for (int i = 0; i < settings.SyncParamsSize; i++) {
					DriverParameter driverParameterChar = new DriverParameter();
					driverParameterChar.name = settings.ParamTextSyncPrefix + i.ToString();
					driverParameterChar.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
					driverParameterChar.value = 0.0f;
					driverBkspStart.parameters.Add(driverParameterChar);
				}

				// Create backspace end key
				AnimatorState stateKeyBkspEnd = CreateState(stateMachine, "KeyBackspaceEnd", new Vector3(0f, 650f, 0f), animations.nothing);
				AnimatorStateTransition transitionKeyBkspEnd = CreateTransition(stateKeyBkspEnd, stateStandby);
				transitionKeyBkspEnd.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardPressedBackspace);

				AnimatorStateTransition transitionKeyBkspUpFirst = CreateTransition(stateKeyBkspStart, stateKeyBkspEnd);
				transitionKeyBkspUpFirst.AddCondition(AnimatorConditionMode.If, 0, settings.ParamKeyboardPrefix);
				transitionKeyBkspUpFirst.duration = 0.2f;
				transitionKeyBkspUpFirst.hasFixedDuration = true;

				Driver driverBksp = stateKeyBkspEnd.AddStateMachineBehaviour<Driver>();
				driverBksp.localOnly = true;
				driverBksp.parameters = new List<DriverParameter>();

				DriverParameter driverParameterBksp = new DriverParameter();
				driverParameterBksp.name = settings.ParamTextPointer;
				driverParameterBksp.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Add;
				driverParameterBksp.value = -1;
				driverBksp.parameters.Add(driverParameterBksp);

				for (int j = 0; j < settings.SyncParamsSize; j++) {
					DriverParameter driverParameterChar = new DriverParameter();
					driverParameterChar.name = settings.ParamTextSyncPrefix + j.ToString();
					driverParameterChar.type = VRC.SDKBase.VRC_AvatarParameterDriver.ChangeType.Set;
					driverParameterChar.value = settings.KeyboardBackspaceMode;
					driverBksp.parameters.Add(driverParameterChar);
				}

				return layer;
			}

			private static AnimatorControllerLayer CreateKeyboardShiftLayer(AnimatorController controller, KatSettings settings, KatAnimations animations)
			{
				AnimatorControllerLayer layer = new AnimatorControllerLayer();
				AnimatorStateMachine stateMachine;

				layer.name = settings.ParamKeyboardShift;
				layer.defaultWeight = 1f;
				layer.stateMachine = stateMachine = new AnimatorStateMachine();
				layer.stateMachine.name = layer.name;
				AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

				// Create states
				AnimatorState stateDisabledStart = CreateState(stateMachine, "Disabled Start", new Vector3(300f, 200f, 0f), animations.keyboardShiftRelease);
				AnimatorState stateDisabledEnd = CreateState(stateMachine, "Disabled End", new Vector3(600f, 200f, 0f), animations.keyboardShiftRelease);
				AnimatorState stateEnabledStart = CreateState(stateMachine, "Enabled Start", new Vector3(600f, 300f, 0f), animations.keyboardShiftPress);
				AnimatorState stateEnabledEnd = CreateState(stateMachine, "Enabled End", new Vector3(300f, 300f, 0f), animations.keyboardShiftPress);

				// Create transitions
				AnimatorStateTransition transition;

				transition = CreateTransition(stateDisabledStart, stateDisabledEnd);
				transition.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardPressedShiftL);
				transition.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardPressedShiftR);
				transition.AddCondition(AnimatorConditionMode.If, 0, "IsLocal");

				transition = CreateTransition(stateDisabledEnd, stateEnabledStart);
				transition.AddCondition(AnimatorConditionMode.Greater, 0.5f, settings.ParamKeyboardPressedShiftL);
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;
				transition = CreateTransition(stateDisabledEnd, stateEnabledStart);
				transition.AddCondition(AnimatorConditionMode.Greater, 0.5f, settings.ParamKeyboardPressedShiftR);
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;

				transition = CreateTransition(stateEnabledStart, stateEnabledEnd);
				transition.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardPressedShiftL);
				transition.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardPressedShiftR);

				transition = CreateTransition(stateEnabledEnd, stateDisabledStart);
				transition.AddCondition(AnimatorConditionMode.Greater, 0.5f, settings.ParamKeyboardPressedShiftL);
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;
				transition = CreateTransition(stateEnabledEnd, stateDisabledStart);
				transition.AddCondition(AnimatorConditionMode.Greater, 0.5f, settings.ParamKeyboardPressedShiftR);
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;

				return layer;
			}

			private static AnimatorControllerLayer CreateKeyboardProximityLayer(AnimatorController controller, KatSettings settings, KatAnimations animations)
			{
				AnimatorControllerLayer layer = new AnimatorControllerLayer();
				AnimatorStateMachine stateMachine;

				layer.name = settings.ParamKeyboardProximity;
				layer.defaultWeight = 1f;
				layer.stateMachine = stateMachine = new AnimatorStateMachine();
				layer.stateMachine.name = layer.name;
				AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

				// Create states
				AnimatorState stateProximityFar = CreateState(stateMachine, "Proximity Far", new Vector3(300f, 200f, 0f), animations.keyboardProximityFar);
				AnimatorState stateProximityClose = CreateState(stateMachine, "Proximity Close", new Vector3(600f, 200f, 0f), animations.keyboardProximityClose);
				AnimatorState stateProximityCloseDesktop = CreateState(stateMachine, "Proximity Close Desktop", new Vector3(300f, 300f, 0f), animations.keyboardProximityClose);

				// Create transitions
				AnimatorStateTransition transition;

				transition = CreateTransition(stateProximityClose, stateProximityFar);
				transition.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardProximity);
				transition.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardProximityExit);
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;

				transition = CreateTransition(stateProximityClose, stateProximityFar);
				transition.AddCondition(AnimatorConditionMode.If, 0, settings.ParamTextVisible);
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;

				transition = CreateTransition(stateProximityClose, stateProximityFar);
				transition.AddCondition(AnimatorConditionMode.IfNot, 0, settings.ParamKeyboardPrefix);
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;

				transition = CreateTransition(stateProximityFar, stateProximityClose);
				transition.AddCondition(AnimatorConditionMode.Greater, 0.5f, settings.ParamKeyboardProximity);
				transition.AddCondition(AnimatorConditionMode.IfNot, 0, settings.ParamTextVisible);
				transition.AddCondition(AnimatorConditionMode.If, 0, settings.ParamKeyboardPrefix);
				transition.AddCondition(AnimatorConditionMode.Equals, 1, "VRMode");
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;

				// Creat desktop mode transitions (currently just visual)
				transition = CreateTransition(stateProximityCloseDesktop, stateProximityFar);
				transition.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardProximity);
				transition.AddCondition(AnimatorConditionMode.Less, 0.5f, settings.ParamKeyboardProximityExit);
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;

				transition = CreateTransition(stateProximityCloseDesktop, stateProximityFar);
				transition.AddCondition(AnimatorConditionMode.If, 0, settings.ParamTextVisible);
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;

				transition = CreateTransition(stateProximityCloseDesktop, stateProximityFar);
				transition.AddCondition(AnimatorConditionMode.IfNot, 0, settings.ParamKeyboardPrefix);
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;

				transition = CreateTransition(stateProximityFar, stateProximityCloseDesktop);
				transition.AddCondition(AnimatorConditionMode.Greater, 0.5f, settings.ParamKeyboardProximity);
				transition.AddCondition(AnimatorConditionMode.IfNot, 0, settings.ParamTextVisible);
				transition.AddCondition(AnimatorConditionMode.If, 0, settings.ParamKeyboardPrefix);
				transition.AddCondition(AnimatorConditionMode.Equals, 0, "VRMode");
				transition.hasFixedDuration = true;
				transition.duration = 0.1f;

				// Create locomotion lock
				LocomotionControl locomotionControlClose = stateProximityClose.AddStateMachineBehaviour<LocomotionControl>();
				locomotionControlClose.disableLocomotion = true;

				LocomotionControl locomotionControlFar = stateProximityFar.AddStateMachineBehaviour<LocomotionControl>();
				locomotionControlFar.disableLocomotion = false;

				return layer;
			}

			private static AnimatorControllerLayer CreateKeyboardHighlightLayer(AnimatorController controller, KatSettings settings, KatAnimations animations)
			{
				AnimatorControllerLayer layer = new AnimatorControllerLayer();
				AnimatorStateMachine stateMachine;

				layer.name = settings.ParamKeyboardHighlight;
				layer.defaultWeight = 1f;
				layer.stateMachine = stateMachine = new AnimatorStateMachine();
				layer.stateMachine.name = layer.name;
				AssetDatabase.AddObjectToAsset(layer.stateMachine, AssetDatabase.GetAssetPath(controller));

				AnimatorState stateHighlight = CreateState(stateMachine, "Highlight", new Vector3(300f, 200f, 0f), animations.keyboardHighlightOn);
				stateHighlight.timeParameter = settings.ParamKeyboardHighlight;
				stateHighlight.timeParameterActive = true;

				return layer;
			}
		#endif

		public static AnimatorState CreateState(AnimatorStateMachine stateMachine, string name, Vector3 position, Motion animation)
		{
			AnimatorState state = stateMachine.AddState(name, position);
			state.motion = animation;
			state.writeDefaultValues = false;
			state.speed = 0f;
			return state;
		}

		public static AnimatorStateTransition CreateTransition(AnimatorState source, AnimatorState destination)
		{
			AnimatorStateTransition transition = source.AddExitTransition(false);
			transition.destinationState = destination;
			transition.duration = 0;
			return transition;
		}

		public static AnimatorStateTransition CreateTransition(AnimatorStateMachine source, AnimatorState destination)
		{
			AnimatorStateTransition transition = source.AddAnyStateTransition(destination);
			transition.duration = 0;
			transition.canTransitionToSelf = false;
			return transition;
		}

		private static bool AddParameterIfMissing(AnimatorController controller, string name, AnimatorControllerParameterType type)
		{
			foreach (AnimatorControllerParameter parameter in controller.parameters) {
				if (parameter.name == name) {
					if (parameter.type == type) {
						return true;
					} else {
						return false;
					}
				}
			}
			controller.AddParameter(name, type);
			return true;
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

	public class KatAnimations
	{
		public readonly AnimationClip[] charsStart;
		public readonly AnimationClip[] charsEnd;
		public readonly AnimationClip clear;
		public readonly AnimationClip disable;
		public readonly AnimationClip enable;
		public readonly AnimationClip nothing;

		public readonly AnimationClip keyboardDisable;
		public readonly AnimationClip keyboardEnable;
		public readonly AnimationClip keyboardInit;
		public readonly AnimationClip keyboardShiftPress;
		public readonly AnimationClip keyboardShiftRelease;
		public readonly AnimationClip keyboardProximityClose;
		public readonly AnimationClip keyboardProximityFar;
		public readonly AnimationClip keyboardHighlightOn;
		public readonly AnimationClip keyboardHighlightOff;

		#if CVR_CCK_EXISTS
			public readonly AvatarMask avatarMask;
		#endif


		public readonly bool valid;

		public KatAnimations(KatSettings settings)
		{
			valid = true;

			charsStart = GetCharAnimations(settings, false);
			charsEnd = GetCharAnimations(settings, true);
			disable = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Disable");
			enable = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Enable");
			nothing = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Nothing");
			clear = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + settings.CharacterAnimationClipNamePrefix + "Clear");

			if (
				charsStart == null ||
				charsEnd == null ||
				disable == null ||
				enable == null ||
				nothing == null
			) {
				valid = false;
			}

			if (settings.InstallKeyboard) {
				keyboardDisable = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Keyboard_Disable");
				keyboardEnable = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Keyboard_Enable");
				keyboardInit = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Keyboard_Init");
				keyboardShiftPress = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Keyboard_Shift_Press");
				keyboardShiftRelease = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Keyboard_Shift_Release");
				keyboardProximityClose = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Keyboard_Proximity_Close");
				keyboardProximityFar = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Keyboard_Proximity_Far");
				keyboardHighlightOn = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Keyboard_Highlight_On");
				keyboardHighlightOff = Resources.Load<AnimationClip>(settings.CharacterAnimationFolder + "KAT_Keyboard_Highlight_Off");

				if (
					keyboardDisable == null ||
					keyboardEnable == null ||
					keyboardInit == null ||
					keyboardShiftPress == null ||
					keyboardShiftRelease == null ||
					keyboardProximityClose == null ||
					keyboardProximityFar == null ||
					keyboardHighlightOn == null ||
					keyboardHighlightOff == null
				) {
					valid = false;
				}
			}

			#if CVR_CCK_EXISTS
				if (settings.SDK == KatSDK.ChilloutVR) {
					avatarMask = (AvatarMask)AssetDatabase.LoadAssetAtPath("Assets/ABI.CCK/Animations/Toggles.mask", typeof(AvatarMask));
				}
			#endif
		}

		private static AnimationClip[] GetCharAnimations(KatSettings settings, bool end = false)
		{
			AnimationClip[] animationClips = Resources.LoadAll<AnimationClip>(settings.CharacterAnimationFolder);
			AnimationClip[] orderedAnimationClips = new AnimationClip[settings.TextLength];

			// Order the animation clips so they match their index
			for (int i = 0; i < settings.TextLength; i++) {
				orderedAnimationClips[i] = null;
				foreach (var animationClip in animationClips) {
					string name;
					if (end) {
						name = settings.CharacterAnimationClipNamePrefix + "End" + i.ToString();
					} else {
						name = settings.CharacterAnimationClipNamePrefix + i.ToString();
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

			return orderedAnimationClips;
		}
	}
}

#endif
