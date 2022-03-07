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

using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEditor;

using AnimatorController = UnityEditor.Animations.AnimatorController;

using VRCAvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
using ExpressionParameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter;
using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;

using KillFrenzy.AvatarTextTools.Settings;


namespace KillFrenzy.AvatarTextTools.Utility
{
	public static class KatAvatarInstaller
	{
		public static bool InstallToAvatar(VRCAvatarDescriptor avatarDescriptor, int attachmentPoint, bool installKeyboard)
		{
			AnimatorController animatorControllerFx = null;
			VRCAvatarDescriptor.CustomAnimLayer animatorLayer;
			bool animatorLayerFound = false;
			ExpressionParameters expressionParameters = avatarDescriptor.expressionParameters;
			ExpressionsMenu expressionsMenu = avatarDescriptor.expressionsMenu;

			foreach (VRCAvatarDescriptor.CustomAnimLayer animLayer in avatarDescriptor.baseAnimationLayers) {
				if (animLayer.type == VRCAvatarDescriptor.AnimLayerType.FX) {
					if (animLayer.animatorController != null) {
						animatorControllerFx = (AnimatorController)animLayer.animatorController;
					}
					animatorLayer = animLayer;
					animatorLayerFound = true;
					break;
				}
			}

			// No animator found, create a new animator
			if (!animatorControllerFx) {
				if (!animatorLayerFound) {
					Debug.LogError("Failed: Could not find FX layer on the VRC Avatar Descriptor.");
					return false;
				}

				avatarDescriptor.customizeAnimationLayers = true;

				animatorControllerFx = new AnimatorController();
				VRCAvatarDescriptor.CustomAnimLayer newLayer = new VRCAvatarDescriptor.CustomAnimLayer();

				animatorLayer.isDefault = false;
				animatorLayer.animatorController = animatorControllerFx;

				newLayer.animatorController = animatorControllerFx;
				newLayer.isDefault = false;
				newLayer.type = VRCAvatarDescriptor.AnimLayerType.FX;
				newLayer.mask = null;

				for (int i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++) {
					VRCAvatarDescriptor.CustomAnimLayer layer = avatarDescriptor.baseAnimationLayers[i];
					if (layer.type == VRCAvatarDescriptor.AnimLayerType.FX) {
						avatarDescriptor.baseAnimationLayers[i] = newLayer;
					}
				}

				CreateOutputFolder();
				AssetDatabase.CreateAsset(animatorControllerFx, "Assets/" + KatSettings.GeneratedOutputFolderName + "/KAT_AnimatorFX_" + System.Guid.NewGuid().ToString() + ".controller");
			}

			// No parameters found, create new parameters
			if (!expressionParameters) {
				expressionParameters = new ExpressionParameters();
				avatarDescriptor.expressionParameters = expressionParameters;
				avatarDescriptor.customExpressions = true;
				expressionParameters.parameters = new ExpressionParameter[1] {
					new ExpressionParameter() {
						name = "VRCEmote",
						valueType = ExpressionParameters.ValueType.Int,
						defaultValue = 0,
						saved = false
					}
				};

				CreateOutputFolder();
				AssetDatabase.CreateAsset(expressionParameters, "Assets/" + KatSettings.GeneratedOutputFolderName + "/KAT_ExpressionParameters_" + System.Guid.NewGuid().ToString() + ".asset");
			}

			// No menu found, insert default menu
			if (!expressionsMenu) {
				expressionsMenu = Resources.Load<ExpressionsMenu>("KAT_Misc/DefaultEmotes");
				if (expressionsMenu) {
					avatarDescriptor.expressionsMenu = expressionsMenu;
				} else {
					Debug.LogWarning("Warning: Could not insert default expressions menu.");
					Debug.Log(expressionsMenu);
				}
			}

			if (
				KatAnimatorInstaller.InstallToAnimator(animatorControllerFx, installKeyboard) &&
				KatParametersInstaller.InstallToParameters(expressionParameters) &&
				KatObjectsInstaller.InstallObjectsToAvatar(avatarDescriptor, attachmentPoint)
			) {
				return true;
			} else {
				return false;
			}
		}

		public static bool RemoveFromAvatar(VRCAvatarDescriptor avatarDescriptor)
		{
			AnimatorController animatorControllerFx = null;
			ExpressionParameters expressionParameters = avatarDescriptor.expressionParameters;

			foreach (VRCAvatarDescriptor.CustomAnimLayer animLayer in avatarDescriptor.baseAnimationLayers) {
				if (animLayer.type == VRCAvatarDescriptor.AnimLayerType.FX) {
					if (animLayer.animatorController != null) {
						animatorControllerFx = (AnimatorController)animLayer.animatorController;
					}
					break;
				}
			}

			if (animatorControllerFx != null) {
				KatAnimatorInstaller.RemoveFromAnimator(animatorControllerFx);
			}

			if (expressionParameters != null) {
				KatParametersInstaller.RemoveFromParameters(expressionParameters);
			}

			KatObjectsInstaller.RemoveObjectsFromAvatar(avatarDescriptor);

			return true;
		}

		public static void CreateOutputFolder() {
			if (AssetDatabase.AssetPathToGUID("Assets/" + KatSettings.GeneratedOutputFolderName) == "") {
				AssetDatabase.CreateFolder("Assets", KatSettings.GeneratedOutputFolderName);
			}
		}

	}
}

#endif
