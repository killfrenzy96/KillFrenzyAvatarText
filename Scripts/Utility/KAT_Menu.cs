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
