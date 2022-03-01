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

using AnimatorController = UnityEditor.Animations.AnimatorController;
using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
using VRCAvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;

using KillFrenzy.AvatarTextTools.Utility;
using KillFrenzy.AvatarTextTools.Settings;


namespace KillFrenzy.AvatarTextTools
{
	public class KillFrenzyAvatarText : EditorWindow
	{
		private AnimatorController targetController = null;
		private ExpressionParameters targetParameters = null;
		private VRCAvatarDescriptor targetAvatar = null;
		private int attachmentPoint = KatAttachmentPoint.Head;
		private int tab = 0;
		private bool optionsExpand = false;


		[MenuItem("Tools/KillFrenzy Avatar Text (KAT)")]
		public static void Open()
		{
			GetWindow<KillFrenzyAvatarText>("KAT");
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

			// Tab to switch between simple installer or modular installer
			tab = GUILayout.Toolbar(tab, new string[]{"Avatar Installer", "Advanced"});
			EditorGUILayout.Space();

			switch (tab) {
				case 0: { // Simple installer
					EditorGUILayout.LabelField("KAT Avatar Installer", subtitleStyle);

					targetAvatar = EditorGUILayout.ObjectField("Avatar", targetAvatar, typeof(VRCAvatarDescriptor), true) as VRCAvatarDescriptor;
					EditorGUILayout.Space();

					if (optionsExpand = GUILayout.Toggle(optionsExpand, "Options", new GUIStyle("ToolbarDropDown") {
						fontSize = 12,
						fixedHeight = 22,
						fontStyle = FontStyle.Bold
					})) {
						EditorGUILayout.Space();

						EditorGUILayout.LabelField("Attachment Point");
						attachmentPoint = EditorGUILayout.Popup(attachmentPoint, new string[3] {
							"None",
							"Head",
							"Chest"
						});

						EditorGUILayout.Space();
					}

					EditorGUILayout.Space();

					if (GUILayout.Button("Install/Update KAT")) {
						if (targetAvatar == null) {
							Debug.LogError("Failed: No VRC avatar has been selected.");
						} else {
							if (!KatAvatarInstaller.RemoveFromAvatar(targetAvatar)) {
								Debug.LogWarning("Warning: KAT removal failed. This is done before installation.");
							}
							if (!KatAvatarInstaller.InstallToAvatar(targetAvatar, attachmentPoint)) {
								Debug.LogError("KAT install failed.");
							} else {
								Debug.Log("KAT install complete.");
							}
						}
					}

					if (GUILayout.Button("Remove KAT")) {
						if (targetAvatar == null) {
							Debug.LogError("Failed: No avatar has been selected.");
						} else {
							if (!KatAvatarInstaller.RemoveFromAvatar(targetAvatar)) {
								Debug.LogError("KAT removal failed.");
							} else {
								Debug.Log("KAT install complete.");
							}
						}
					}
					break;
				}

				case 1: { // Modular installer
					// Animation controller installer
					EditorGUILayout.LabelField("Install to Animation Controller", subtitleStyle);

					targetController = EditorGUILayout.ObjectField("Animator Controller", targetController, typeof(AnimatorController), true) as AnimatorController;
					EditorGUILayout.Space();

					if (GUILayout.Button("Install/Update KAT animations")) {
						if (targetController == null) {
							Debug.LogError("Failed: No animator has been selected.");
						} else {
							if (!KatAnimatorInstaller.RemoveFromAnimator(targetController)) {
								Debug.LogWarning("Warning: KAT animator removal failed. This is done before installation.");
							}
							if (!KatAnimatorInstaller.InstallToAnimator(targetController)) {
								Debug.LogError("KAT animator install failed.");
							} else {
								Debug.Log("KAT animator install complete.");
							}
						}
					}

					if (GUILayout.Button("Remove KAT animations")) {
						if (targetController == null) {
							Debug.LogError("Failed: No animator has been selected.");
						} else {
							if (!KatAnimatorInstaller.RemoveFromAnimator(targetController)) {
								Debug.LogError("KAT animator removal failed.");
							} else {
								Debug.Log("KAT animator install complete.");
							}
						}
					}

					EditorGUILayout.Space();

					// Parameter installer
					EditorGUILayout.LabelField("Install to Expression Parameters", subtitleStyle);

					targetParameters = EditorGUILayout.ObjectField("Expression Parameters", targetParameters, typeof(ExpressionParameters), true) as ExpressionParameters;
					EditorGUILayout.Space();

					if (GUILayout.Button("Install/Update KAT expression parameters")) {
						if (targetParameters == null) {
							Debug.LogError("Failed: No expression parameters has been selected.");
						} else {
							if (!KatParametersInstaller.RemoveFromParameters(targetParameters)) {
								Debug.LogWarning("Warning: KAT expression parameters removal failed. This is done before installation.");
							}
							if (!KatParametersInstaller.InstallToParameters(targetParameters)) {
								Debug.LogError("KAT expression parameters install failed.");
							} else {
								Debug.Log("KAT expression parameters install complete.");
							}
						}
					}

					if (GUILayout.Button("Remove KAT expression parameters")) {
						if (targetParameters == null) {
							Debug.LogError("Failed: No expression parameters has been selected.");
						} else {
							if (!KatParametersInstaller.RemoveFromParameters(targetParameters)) {
								Debug.LogError("KAT expression parameters removal failed.");
							} else {
								Debug.Log("KAT expression parameters install complete.");
							}
						}
					}

					EditorGUILayout.Space();

					// Objects installer
					EditorGUILayout.LabelField("Install Avatar Parts", subtitleStyle);

					targetAvatar = EditorGUILayout.ObjectField("Avatar Descriptor", targetAvatar, typeof(VRCAvatarDescriptor), true) as VRCAvatarDescriptor;
					EditorGUILayout.Space();

					EditorGUILayout.LabelField("Attachment Point");
					attachmentPoint = EditorGUILayout.Popup(attachmentPoint, new string[3] {
						"None",
						"Head",
						"Chest"
					});
					EditorGUILayout.Space();

					if (GUILayout.Button("Install/Update KAT avatar parts")) {
						if (targetAvatar == null) {
							Debug.LogError("Failed: No VRC avatar descriptor has been selected.");
						} else {
							if (!KatObjectsInstaller.RemoveObjectsFromAvatar(targetAvatar)) {
								Debug.LogWarning("Warning: KAT avatar parts removal failed. This is done before installation.");
							}
							if (!KatObjectsInstaller.InstallObjectsToAvatar(targetAvatar, attachmentPoint)) {
								Debug.LogError("KAT avatar parts install failed.");
							} else {
								Debug.Log("KAT avatar parts install complete.");
							}
						}
					}

					if (GUILayout.Button("Remove KAT avatar parts")) {
						if (targetAvatar == null) {
							Debug.LogError("Failed: No avatar descriptor has been selected.");
						} else {
							if (!KatObjectsInstaller.RemoveObjectsFromAvatar(targetAvatar)) {
								Debug.LogError("KAT avatar parts removal failed.");
							} else {
								Debug.Log("KAT avatar parts install complete.");
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

	}
}

#endif