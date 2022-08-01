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
using UnityEditor;

using AnimatorController = UnityEditor.Animations.AnimatorController;

#if VRC_SDK_VRCSDK3
	using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
	using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
	using VRCAvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
#endif

#if CVR_CCK_EXISTS
	using CVRAvatar = ABI.CCK.Components.CVRAvatar;
#endif

using KillFrenzy.AvatarTextTools.Utility;
using KillFrenzy.AvatarTextTools.Settings;


namespace KillFrenzy.AvatarTextTools
{
	public class KillFrenzyAvatarText : EditorWindow
	{
		private AnimatorController targetController = null;

		#if VRC_SDK_VRCSDK3
			private ExpressionsMenu targetMenu = null;
			private ExpressionParameters targetParameters = null;
			private VRCAvatarDescriptor targetAvatar = null;
		#endif
		#if CVR_CCK_EXISTS
			private CVRAvatar targetCVRAvatar = null;
		#endif
		private int attachmentPoint = KatAttachmentPoint.Chest;
		private int installKeyboard = 1;
		private int syncParamSize = 2;

		private int tab = 0;
		private bool optionsExpand = false;
		private bool optionsExpandVRC = false;
		private bool optionsExpandCVR = false;

		private GUIStyle titleStyle = null;
		private GUIStyle subtitleStyle = null;
		private GUIStyle dropdownStyle = null;

		[MenuItem("Tools/KillFrenzy Avatar Text (KAT)")]
		public static void Open()
		{
			GetWindow<KillFrenzyAvatarText>("KAT");
		}

		private void OnGUI()
		{
			titleStyle = new GUIStyle() {
				fontSize = 14,
				fixedHeight = 28,
				fontStyle = FontStyle.Bold,
				normal = {
					textColor = Color.white
				}
			};

			subtitleStyle = new GUIStyle() {
				fontSize = 13,
				fixedHeight = 26,
				fontStyle = FontStyle.Bold,
				normal = {
					textColor = Color.white
				}
			};

			dropdownStyle = new GUIStyle("ToolbarDropDown") {
				fontSize = 12,
				fixedHeight = 22,
				fontStyle = FontStyle.Bold
			};

			EditorGUILayout.LabelField("KillFrenzy Avatar Text (KAT) v1.3.0", titleStyle);

			// Tab to switch between simple installer or modular installer
			#if VRC_SDK_VRCSDK3 && CVR_CCK_EXISTS
				tab = GUILayout.Toolbar(tab, new string[]{"VRChat Avatar", "ChilloutVR Avatar", "Advanced"});

				EditorGUILayout.Space();

				switch (tab) {
					case 0: { // Simple installer
						DrawSimpleVRC();
						break;
					}

					case 1: { // Modular installer
						DrawSimpleCVR();
						break;
					}

					case 2: { // Modular installer
						DrawAdvanced();
						break;
					}
				}
				#else
					#if VRC_SDK_VRCSDK3
						tab = GUILayout.Toolbar(tab, new string[]{"VRChat Avatar Installer", "Advanced"});

						EditorGUILayout.Space();

						switch (tab) {
							case 0: { // Simple installer
								DrawSimpleVRC();
								break;
							}

							case 1: { // Modular installer
								DrawAdvanced();
								break;
							}
						}
					#endif
					#if CVR_CCK_EXISTS
						tab = GUILayout.Toolbar(tab, new string[]{"ChilloutVR Avatar Installer", "Advanced"});

						EditorGUILayout.Space();

						switch (tab) {
							case 0: { // Simple installer
								DrawSimpleCVR();
								break;
							}

							case 1: { // Modular installer
								DrawAdvanced();
								break;
							}
						}
					#endif
			#endif
		}

		#if VRC_SDK_VRCSDK3
			private void DrawSimpleVRC()
			{
				EditorGUILayout.LabelField("KAT VRChat Avatar Installer", subtitleStyle);

				targetAvatar = EditorGUILayout.ObjectField("VRChat Avatar", targetAvatar, typeof(VRCAvatarDescriptor), true) as VRCAvatarDescriptor;
				EditorGUILayout.Space();

				DrawOptions();

				EditorGUILayout.Space();

				if (GUILayout.Button("Install/Update KAT")) {
					if (targetAvatar == null) {
						Debug.LogError("Failed: No VRChat avatar has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.VRChat);
						if (!KatAvatarInstaller.RemoveFromAvatar(targetAvatar, settings)) {
							Debug.LogWarning("Warning: KAT VRChat removal failed. This is done before installation.");
						}
						if (!KatAvatarInstaller.InstallToAvatar(targetAvatar, settings)) {
							Debug.LogError("KAT VRChat install failed.");
						} else {
							Debug.Log("KAT VRChat install complete.");
						}
					}
				}

				if (GUILayout.Button("Remove KAT")) {
					if (targetAvatar == null) {
						Debug.LogError("Failed: No VRChat avatar has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.VRChat);
						if (!KatAvatarInstaller.RemoveFromAvatar(targetAvatar, settings)) {
							Debug.LogError("KAT VRChat removal failed.");
						} else {
							Debug.Log("KAT VRChat removal complete.");
						}
					}
				}
			}
		#endif

		#if CVR_CCK_EXISTS
			private void DrawSimpleCVR()
			{
				EditorGUILayout.LabelField("KAT ChilloutVR Avatar Installer (EXPERIMENTAL)", subtitleStyle);

				targetCVRAvatar = EditorGUILayout.ObjectField("ChilloutVR Avatar", targetCVRAvatar, typeof(CVRAvatar), true) as CVRAvatar;
				EditorGUILayout.Space();

				DrawOptions(true);

				if (GUILayout.Button("Install/Update KAT")) {
					if (targetCVRAvatar == null) {
						Debug.LogError("Failed: No ChilloutVR avatar has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.ChilloutVR);
						if (!KatAvatarInstaller.RemoveFromAvatar(targetCVRAvatar, settings)) {
							Debug.LogWarning("Warning: KAT ChilloutVR removal failed. This is done before installation.");
						}
						if (!KatAvatarInstaller.InstallToAvatar(targetCVRAvatar, settings)) {
							Debug.LogError("KAT ChilloutVR install failed.");
						} else {
							Debug.Log("KAT ChilloutVR install complete.");
						}
					}
				}

				if (GUILayout.Button("Remove KAT")) {
					if (targetCVRAvatar == null) {
						Debug.LogError("Failed: No ChilloutVR avatar has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.ChilloutVR);
						if (!KatAvatarInstaller.RemoveFromAvatar(targetCVRAvatar, settings)) {
							Debug.LogError("KAT ChilloutVR removal failed.");
						} else {
							Debug.Log("KAT ChilloutVR removal complete.");
						}
					}
				}
			}
		#endif

		private void DrawAdvanced()
		{
			EditorGUILayout.LabelField("Options", subtitleStyle);
			DrawOptions();

			EditorGUILayout.Space();

			#if VRC_SDK_VRCSDK3
				#if CVR_CCK_EXISTS
					if (optionsExpandVRC = GUILayout.Toggle(optionsExpandVRC, "VRChat Installers", dropdownStyle)) {
						DrawAdvancedVRC();
					}
				#else
					DrawAdvancedVRC();
				#endif
			#endif

			#if CVR_CCK_EXISTS
				#if VRC_SDK_VRCSDK3
					if (optionsExpandCVR = GUILayout.Toggle(optionsExpandCVR, "ChilloutVR Installers", dropdownStyle)) {
						DrawAdvancedCVR();
					}
				#else
					DrawAdvancedCVR();
				#endif
			#endif
		}

		#if VRC_SDK_VRCSDK3
			private void DrawAdvancedVRC()
			{
				// Animation controller installer
				EditorGUILayout.LabelField("Install to VRChat Animation Controller", subtitleStyle);

				targetController = EditorGUILayout.ObjectField("Animator Controller", targetController, typeof(AnimatorController), true) as AnimatorController;
				EditorGUILayout.Space();

				if (GUILayout.Button("Install/Update KAT VRChat animations")) {
					if (targetController == null) {
						Debug.LogError("Failed: No VRChat animator has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.VRChat);
						if (!KatAnimatorInstaller.RemoveFromAnimator(targetController, settings)) {
							Debug.LogWarning("Warning: KAT VRChat animator removal failed. This is done before installation.");
						}
						if (!KatAnimatorInstaller.InstallToAnimator(targetController, settings)) {
							Debug.LogError("KAT VRChat animator install failed.");
						} else {
							Debug.Log("KAT VRChat animator install complete.");
						}
					}
				}

				if (GUILayout.Button("Remove KAT VRChat animations")) {
					if (targetController == null) {
						Debug.LogError("Failed: No VRChat animator has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.VRChat);
						if (!KatAnimatorInstaller.RemoveFromAnimator(targetController, settings)) {
							Debug.LogError("KAT VRChat animator removal failed.");
						} else {
							Debug.Log("KAT VRChat animator removal complete.");
						}
					}
				}

				EditorGUILayout.Space();

				// Menu installer
				EditorGUILayout.LabelField("Install to VRChat Expressions Menu", subtitleStyle);

				targetMenu = EditorGUILayout.ObjectField("Expressions Menu", targetMenu, typeof(ExpressionsMenu), true) as ExpressionsMenu;
				EditorGUILayout.Space();

				if (GUILayout.Button("Install/Update KAT VRChat expressions menu")) {
					if (targetMenu == null) {
						Debug.LogError("Failed: No VRChat expressions menu has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.VRChat);
						if (!KatMenuInstaller.RemoveFromMenu(targetMenu, settings)) {
							Debug.LogWarning("Warning: KAT VRChat expressions menu removal failed. This is done before installation.");
						}
						if (!KatMenuInstaller.InstallToMenu(targetMenu, settings)) {
							Debug.LogError("KAT VRChat expressions menu install failed.");
						} else {
							Debug.Log("KAT VRChat expressions menu install complete.");
						}
					}
				}

				if (GUILayout.Button("Remove KAT VRChat expressions menu")) {
					if (targetMenu == null) {
						Debug.LogError("Failed: No VRChat expressions menu has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.VRChat);
						if (!KatMenuInstaller.RemoveFromMenu(targetMenu, settings)) {
							Debug.LogError("KAT VRChat expressions menu removal failed.");
						} else {
							Debug.Log("KAT VRChat expressions menu removal complete.");
						}
					}
				}

				EditorGUILayout.Space();

				// Parameter installer
				EditorGUILayout.LabelField("Install to VRChat Expression Parameters", subtitleStyle);

				targetParameters = EditorGUILayout.ObjectField("Expression Parameters", targetParameters, typeof(ExpressionParameters), true) as ExpressionParameters;
				EditorGUILayout.Space();

				if (GUILayout.Button("Install/Update KAT VRChat expression parameters")) {
					if (targetParameters == null) {
						Debug.LogError("Failed: No VRChat expression parameters has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.VRChat);
						if (!KatParametersInstaller.RemoveFromParameters(targetParameters, settings)) {
							Debug.LogWarning("Warning: KAT VRChat expression parameters removal failed. This is done before installation.");
						}
						if (!KatParametersInstaller.InstallToParameters(targetParameters, settings)) {
							Debug.LogError("KAT VRChat expression parameters install failed.");
						} else {
							Debug.Log("KAT VRChat expression parameters install complete.");
						}
					}
				}

				if (GUILayout.Button("Remove KAT VRChat expression parameters")) {
					if (targetParameters == null) {
						Debug.LogError("Failed: No VRChat expression parameters has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.VRChat);
						if (!KatParametersInstaller.RemoveFromParameters(targetParameters, settings)) {
							Debug.LogError("KAT VRChat expression parameters removal failed.");
						} else {
							Debug.Log("KAT VRChat expression parameters removal complete.");
						}
					}
				}

				EditorGUILayout.Space();

				// Objects installer
				EditorGUILayout.LabelField("Install VRChat Avatar Parts", subtitleStyle);

				targetAvatar = EditorGUILayout.ObjectField("Avatar Descriptor", targetAvatar, typeof(VRCAvatarDescriptor), true) as VRCAvatarDescriptor;
				EditorGUILayout.Space();

				if (GUILayout.Button("Install/Update KAT VRChat avatar parts")) {
					if (targetAvatar == null) {
						Debug.LogError("Failed: No VRChat avatar descriptor has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.VRChat);
						if (!KatObjectsInstaller.RemoveObjectsFromAvatar(targetAvatar, settings)) {
							Debug.LogWarning("Warning: KAT VRChat avatar parts removal failed. This is done before installation.");
						}
						if (!KatObjectsInstaller.InstallObjectsToAvatar(targetAvatar, settings)) {
							Debug.LogError("KAT VRChat avatar parts install failed.");
						} else {
							Debug.Log("KAT VRChat avatar parts install complete.");
						}
					}
				}

				if (GUILayout.Button("Remove KAT VRChat avatar parts")) {
					if (targetAvatar == null) {
						Debug.LogError("Failed: No VRChat avatar descriptor has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.VRChat);
						if (!KatObjectsInstaller.RemoveObjectsFromAvatar(targetAvatar, settings)) {
							Debug.LogError("KAT VRChat avatar parts removal failed.");
						} else {
							Debug.Log("KAT VRChat avatar parts removal complete.");
						}
					}
				}
			}
		#endif

		#if CVR_CCK_EXISTS
			private void DrawAdvancedCVR()
			{
				// Animation controller installer
				EditorGUILayout.LabelField("Install to ChilloutVR Animation Controller", subtitleStyle);

				targetController = EditorGUILayout.ObjectField("Animator Controller", targetController, typeof(AnimatorController), true) as AnimatorController;
				EditorGUILayout.Space();

				if (GUILayout.Button("Install/Update KAT ChilloutVR animations")) {
					if (targetController == null) {
						Debug.LogError("Failed: No ChilloutVR animator has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.ChilloutVR);
						if (!KatAnimatorInstaller.RemoveFromAnimator(targetController, settings)) {
							Debug.LogWarning("Warning: KAT ChilloutVR animator removal failed. This is done before installation.");
						}
						if (!KatAnimatorInstaller.InstallToAnimator(targetController, settings)) {
							Debug.LogError("KAT ChilloutVR animator install failed.");
						} else {
							Debug.Log("KAT ChilloutVR animator install complete.");
						}
					}
				}

				if (GUILayout.Button("Remove KAT ChilloutVR animations")) {
					if (targetController == null) {
						Debug.LogError("Failed: No ChilloutVR animator has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.ChilloutVR);
						if (!KatAnimatorInstaller.RemoveFromAnimator(targetController, settings)) {
							Debug.LogError("KAT ChilloutVR animator removal failed.");
						} else {
							Debug.Log("KAT ChilloutVR animator removal complete.");
						}
					}
				}

				EditorGUILayout.Space();

				// Objects installer
				EditorGUILayout.LabelField("Install ChilloutVR Avatar Parts", subtitleStyle);

				targetCVRAvatar = EditorGUILayout.ObjectField("Avatar Descriptor", targetCVRAvatar, typeof(CVRAvatar), true) as CVRAvatar;
				EditorGUILayout.Space();

				if (GUILayout.Button("Install/Update KAT ChilloutVR avatar parts")) {
					if (targetCVRAvatar == null) {
						Debug.LogError("Failed: No ChilloutVR avatar descriptor has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.ChilloutVR);
						if (!KatObjectsInstaller.RemoveObjectsFromAvatar(targetCVRAvatar, settings)) {
							Debug.LogWarning("Warning: KAT ChilloutVR avatar parts removal failed. This is done before installation.");
						}
						if (!KatObjectsInstaller.InstallObjectsToAvatar(targetCVRAvatar, settings)) {
							Debug.LogError("KAT ChilloutVR avatar parts install failed.");
						} else {
							Debug.Log("KAT ChilloutVR avatar parts install complete.");
						}
					}
				}

				if (GUILayout.Button("Remove KAT ChilloutVR avatar parts")) {
					if (targetCVRAvatar == null) {
						Debug.LogError("Failed: No ChilloutVR avatar descriptor has been selected.");
					} else {
						KatSettings settings = GetKatSettings(KatSDK.ChilloutVR);
						if (!KatObjectsInstaller.RemoveObjectsFromAvatar(targetCVRAvatar, settings)) {
							Debug.LogError("KAT ChilloutVR avatar parts removal failed.");
						} else {
							Debug.Log("KAT ChilloutVR avatar parts removal complete.");
						}
					}
				}
			}
		#endif

		private KatSettings GetKatSettings(int sdk) {
			return new KatSettings(sdk, installKeyboard != 0 ? true : false, attachmentPoint, (int)Math.Pow(2, syncParamSize));
		}

		private void DrawOptions(bool hideVRCOnly = false)
		{
			if (!hideVRCOnly) {
				#if VRC_SDK_VRCSDK3
					EditorGUILayout.Space();

					#if CVR_CCK_EXISTS
						EditorGUILayout.LabelField("Include In-game keyboard (VRChat Only)");
					#else
						EditorGUILayout.LabelField("Include In-game keyboard");
					#endif

					installKeyboard = EditorGUILayout.Popup(installKeyboard, new string[2] {
						"No",
						"Yes"
					});
				#endif
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Attachment Point");
			attachmentPoint = EditorGUILayout.Popup(attachmentPoint, new string[3] {
				"None",
				"Head",
				"Chest"
			});

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Sync Parameters (Lower = Less Parameters, Higher = Faster Sync)");
			syncParamSize = EditorGUILayout.Popup(syncParamSize, new string[5] {
				"1 (In-game keyboard speed)",
				"2 (Average typing speed)",
				"4 (Default)",
				"8 (Speech to text speed)",
				"16 (Fastest)"
			});

			EditorGUILayout.Space();
		}
	}
}

#endif