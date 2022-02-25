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

// using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using AnimatorController = UnityEditor.Animations.AnimatorController;

// using VRC.SDK3.Avatars.Components;
// using static VRC.SDKBase.VRC_AvatarParameterDriver;
// using Parameter = VRC.SDKBase.VRC_AvatarParameterDriver.Parameter;

using KillFrenzy.AvatarTextTools.Utility;


namespace KillFrenzy.AvatarTextTools
{
	public class KillFrenzyAvatarText : EditorWindow
	{
		private AnimatorController targetController = null;
		private int tab = 1;


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
					break;
				}

				case 1: { // Modular installer
					// Animation controller installer
					EditorGUILayout.LabelField("Install to Animation Controller", subtitleStyle);

					targetController = EditorGUILayout.ObjectField("Animator", targetController, typeof(AnimatorController), true) as AnimatorController;
					EditorGUILayout.Space();

					if (GUILayout.Button("Install/Update KAT animations")) {
						if (targetController == null) {
							Debug.Log("Failed: No animator has been selected.");
						} else {
							Debug.Log("KAT install started.");
							if (!KatAnimatorInstaller.RemoveFromAnimator(targetController)) {
								Debug.Log("Warning: KAT removal failed. This is done before installation.");
							}
							if (!KatAnimatorInstaller.InstallToAnimator(targetController)) {
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
							if (!KatAnimatorInstaller.RemoveFromAnimator(targetController)) {
								Debug.Log("KAT removal complete.");
							} else {
								Debug.Log("KAT install failed.");
							}
						}
					}

					// Parameter installer

					// Menu installer

					break;
				}

				default: {
					break;
				}
			}
		}

	}
}
