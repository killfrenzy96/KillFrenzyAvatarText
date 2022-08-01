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

using System.Collections.Generic;

#if VRC_SDK_VRCSDK3
	using ExpressionsMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
	using Control = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control;
	using Parameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter;
#endif

using KillFrenzy.AvatarTextTools.Settings;


namespace KillFrenzy.AvatarTextTools.Utility
{
	public static class KatMenuInstaller
	{
		#if VRC_SDK_VRCSDK3
			public static bool InstallToMenu(ExpressionsMenu targetMenu, KatSettings settings)
			{
				if (!settings.InstallKeyboard) {
					Debug.Log("Installation to menu skipped - not required without keyboard.");
					return true;
				}

				if (targetMenu.controls.Count >= 8) {
					Debug.LogError("Error: Avatar controls list is already full. Please add '" + settings.ParamKeyboardPrefix + "' to you avatar manually.");
					return false;
				}

				Control control = new Control();
				control.name = "Keyboard";
				control.parameter = new Parameter();
				control.parameter.name = settings.ParamKeyboardPrefix;
				control.type = Control.ControlType.Toggle;
				targetMenu.controls.Add(control);

				EditorUtility.SetDirty(targetMenu);
				Debug.Log("Installation to menu completed.");
				return true;
			}

			public static bool RemoveFromMenu(ExpressionsMenu targetMenu, KatSettings settings)
			{
				for (int i = 0; i < targetMenu.controls.Count; i++) {
					Control control = targetMenu.controls[i];
					if (control.parameter != null && control.parameter.name == settings.ParamKeyboardPrefix) {
						targetMenu.controls.Remove(control);
						break;
					}
				}

				EditorUtility.SetDirty(targetMenu);
				Debug.Log("Removal from menu completed.");
				return true;
			}
		#endif
	}
}

#endif
