/*
Copyright 2023 KillFrenzy / Evan Tran

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

using System.Collections.Generic;

using AnimatorController = UnityEditor.Animations.AnimatorController;

#if VRC_SDK_VRCSDK3
	using VRCAvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
	using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
	using ExpressionParameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter;
	using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
	using Control = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control;
	using ControlParameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter;
#endif

#if CVR_CCK_EXISTS
	using CVRAvatar = ABI.CCK.Components.CVRAvatar;
#endif

using KillFrenzy.AvatarTextTools.Settings;


namespace KillFrenzy.AvatarTextTools.Utility
{
	public static class KatAvatarInstaller
	{
		#if VRC_SDK_VRCSDK3
			public static bool InstallToAvatar(VRCAvatarDescriptor avatarDescriptor, KatSettings settings)
			{
				AnimatorController animatorController = null;
				VRCAvatarDescriptor.CustomAnimLayer animatorLayer;
				bool animatorLayerFound = false;
				ExpressionParameters expressionParameters = avatarDescriptor.expressionParameters;
				ExpressionsMenu expressionsMenu = avatarDescriptor.expressionsMenu;

				foreach (VRCAvatarDescriptor.CustomAnimLayer animLayer in avatarDescriptor.baseAnimationLayers) {
					if (animLayer.type == VRCAvatarDescriptor.AnimLayerType.FX) {
						if (animLayer.animatorController != null) {
							animatorController = (AnimatorController)animLayer.animatorController;
						}
						animatorLayer = animLayer;
						animatorLayerFound = true;
						break;
					}
				}

				// No animator found, create a new animator
				if (!animatorController) {
					if (!animatorLayerFound) {
						Debug.LogError("Failed: Could not find FX layer on the VRC Avatar Descriptor.");
						return false;
					}

					avatarDescriptor.customizeAnimationLayers = true;

					animatorController = new AnimatorController();
					VRCAvatarDescriptor.CustomAnimLayer newLayer = new VRCAvatarDescriptor.CustomAnimLayer();

					animatorLayer.isDefault = false;
					animatorLayer.animatorController = animatorController;

					newLayer.animatorController = animatorController;
					newLayer.isDefault = false;
					newLayer.type = VRCAvatarDescriptor.AnimLayerType.FX;
					newLayer.mask = null;

					for (int i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++) {
						VRCAvatarDescriptor.CustomAnimLayer layer = avatarDescriptor.baseAnimationLayers[i];
						if (layer.type == VRCAvatarDescriptor.AnimLayerType.FX) {
							avatarDescriptor.baseAnimationLayers[i] = newLayer;
						}
					}

					CreateOutputFolder(settings);
					string assetPath = "Assets/" + settings.GeneratedOutputFolderName + "/KAT_AnimatorFX_" + System.Guid.NewGuid().ToString() + ".controller";
					AssetDatabase.CreateAsset(animatorController, assetPath);
					Debug.Log("Created new AnimatorController at " + assetPath);
				}

				// No parameters found, create new parameters
				if (!expressionParameters) {
					// expressionParameters = new ExpressionParameters();
					expressionParameters = (ExpressionParameters)ScriptableObject.CreateInstance(typeof(ExpressionParameters));
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

					CreateOutputFolder(settings);
					string assetPath = "Assets/" + settings.GeneratedOutputFolderName + "/KAT_ExpressionParameters_" + System.Guid.NewGuid().ToString() + ".asset";
					AssetDatabase.CreateAsset(expressionParameters, assetPath);
					Debug.Log("Created new ExpressionParameters at " + assetPath);
				}

				// No menu found, create new menu
				if (!expressionsMenu) {
					ExpressionsMenu expressionsEmotes = Resources.Load<ExpressionsMenu>("KAT_Misc/DefaultEmotes");
					// if (expressionsMenu) {
					// 	avatarDescriptor.expressionsMenu = expressionsMenu;
					// } else {
					// 	Debug.LogWarning("Warning: Could not insert default expressions menu.");
					// 	Debug.Log(expressionsMenu);
					// }

					// expressionsMenu = new ExpressionsMenu();
					expressionsMenu = (ExpressionsMenu)ScriptableObject.CreateInstance(typeof(ExpressionsMenu));
					avatarDescriptor.expressionsMenu = expressionsMenu;
					avatarDescriptor.customExpressions = true;
					expressionsMenu.controls = new List<ExpressionsMenu.Control>();

					Control control = new Control();
					control.name = "Emotes";
					control.subMenu = expressionsEmotes;
					control.type = Control.ControlType.SubMenu;
					expressionsMenu.controls.Add(control);

					CreateOutputFolder(settings);
					string assetPath = "Assets/" + settings.GeneratedOutputFolderName + "/KAT_ExpressionMenu_" + System.Guid.NewGuid().ToString() + ".asset";
					AssetDatabase.CreateAsset(expressionsMenu, assetPath);
					Debug.Log("Created new ExpressionMenu at " + assetPath);
				}

				if (
					KatAnimatorInstaller.InstallToAnimator(animatorController, settings) &&
					KatMenuInstaller.InstallToMenu(expressionsMenu, settings) &&
					KatParametersInstaller.InstallToParameters(expressionParameters, settings) &&
					KatObjectsInstaller.InstallObjectsToAvatar(avatarDescriptor, settings)
				) {
					EditorUtility.SetDirty(avatarDescriptor);
					return true;
				} else {
					EditorUtility.SetDirty(avatarDescriptor);
					return false;
				}
			}

			public static bool RemoveFromAvatar(VRCAvatarDescriptor avatarDescriptor, KatSettings settings)
			{
				AnimatorController animatorControllerFx = null;
				ExpressionsMenu expressionsMenu = avatarDescriptor.expressionsMenu;
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
					KatAnimatorInstaller.RemoveFromAnimator(animatorControllerFx, settings);
				}

				if (expressionsMenu != null) {
					KatMenuInstaller.RemoveFromMenu(expressionsMenu, settings);
				}

				if (expressionParameters != null) {
					KatParametersInstaller.RemoveFromParameters(expressionParameters, settings);
				}

				KatObjectsInstaller.RemoveObjectsFromAvatar(avatarDescriptor, settings);
				return true;
			}
		#endif

		#if CVR_CCK_EXISTS
			const string CCK_SOURCE_ANIMATOR_PATH = "Assets/ABI.CCK/Animations/AvatarAnimator.controller";

			public static bool InstallToAvatar(CVRAvatar avatarDescriptor, KatSettings settings)
			{
				avatarDescriptor.avatarUsesAdvancedSettings = true;
				AnimatorController animatorController = null;
				if (avatarDescriptor.avatarSettings.baseController) {
					animatorController = (AnimatorController)avatarDescriptor.avatarSettings.baseController;
				}

				if (animatorController) {
					string assetPath = AssetDatabase.GetAssetPath(animatorController);
					if (assetPath == CCK_SOURCE_ANIMATOR_PATH) {
						// Default controller selected, do not use this
						animatorController = null;
					}
				}

				// No animator found, create a new animator
				if (!animatorController) {
					// animatorController = new AnimatorController();
					string assetPath = "Assets/" + settings.GeneratedOutputFolderName + "/KAT_AnimatorCVR_" + System.Guid.NewGuid().ToString() + ".controller";

					CreateOutputFolder(settings);
					if (AssetDatabase.CopyAsset(CCK_SOURCE_ANIMATOR_PATH, assetPath)) {
						Debug.Log("Created new AnimatorController at " + assetPath);
						animatorController = (AnimatorController)AssetDatabase.LoadAssetAtPath(assetPath, typeof(AnimatorController));
						avatarDescriptor.avatarSettings.baseController = animatorController;
					} else {
						Debug.LogError("Failed: Could not make duplicate of CVR Avatar Animator at " + assetPath);
						return false;
					}
				}

				if (
					KatAnimatorInstaller.InstallToAnimator(animatorController, settings) &&
					KatObjectsInstaller.InstallObjectsToAvatar(avatarDescriptor, settings)
				) {
					EditorUtility.SetDirty(avatarDescriptor);
					return true;
				} else {
					EditorUtility.SetDirty(avatarDescriptor);
					return false;
				}
			}

			public static bool RemoveFromAvatar(CVRAvatar avatarDescriptor, KatSettings settings)
			{
				AnimatorController animatorController = null;
				if (avatarDescriptor.avatarSettings.baseController) {
					animatorController = (AnimatorController)avatarDescriptor.avatarSettings.baseController;
				}

				if (animatorController) {
					string assetPath = AssetDatabase.GetAssetPath(animatorController);
					if (assetPath == CCK_SOURCE_ANIMATOR_PATH) {
						// Default controller selected, do not use this
						animatorController = null;
					}
				}

				if (animatorController != null) {
					KatAnimatorInstaller.RemoveFromAnimator(animatorController, settings);
				}

				KatObjectsInstaller.RemoveObjectsFromAvatar(avatarDescriptor, settings);
				return true;
			}
		#endif

		public static void CreateOutputFolder(KatSettings settings) {
			if (AssetDatabase.AssetPathToGUID("Assets/" + settings.GeneratedOutputFolderName) == "") {
				AssetDatabase.CreateFolder("Assets", settings.GeneratedOutputFolderName);
			}
		}

	}
}

#endif
