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

using UnityEditor;
using UnityEngine;

using System.Collections.Generic;

#if VRC_SDK_VRCSDK3
	using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
	using ExpressionParameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter;
#endif

using KillFrenzy.AvatarTextTools.Settings;


namespace KillFrenzy.AvatarTextTools.Utility
{
	public static class KatParametersInstaller
	{
		#if VRC_SDK_VRCSDK3
			public static bool InstallToParameters(ExpressionParameters targetParameters, KatSettings settings)
			{
				List<ExpressionParameter> parameters = new List<ExpressionParameter>();
				parameters.AddRange(targetParameters.parameters);

				parameters.Add(new ExpressionParameter() {
					name = settings.ParamTextVisible,
					valueType = ExpressionParameters.ValueType.Bool,
					defaultValue = 0,
					saved = false
				});

				if (settings.InstallKeyboard) {
					parameters.Add(new ExpressionParameter() {
						name = settings.ParamKeyboardPrefix,
						valueType = ExpressionParameters.ValueType.Bool,
						defaultValue = 0,
						saved = false
					});
				}

				parameters.Add(new ExpressionParameter() {
					name = settings.ParamTextPointer,
					valueType = ExpressionParameters.ValueType.Int,
					defaultValue = 0,
					saved = false
				});

				for (int i = 0; i < settings.SyncParamsSize; i++) {
					parameters.Add(new ExpressionParameter() {
						name = settings.ParamTextSyncPrefix + i.ToString(),
						valueType = ExpressionParameters.ValueType.Float,
						defaultValue = 0,
						saved = false
					});
				}

				targetParameters.parameters = parameters.ToArray();
				EditorUtility.SetDirty(targetParameters);
				Debug.Log("Installation to parameters completed.");
				return true;
			}

			public static bool RemoveFromParameters(ExpressionParameters targetParameters, KatSettings settings)
			{
				List<ExpressionParameter> parameters = new List<ExpressionParameter>();
				parameters.AddRange(targetParameters.parameters);

				for (int i = 0; i < targetParameters.parameters.Length; i++) {
					ExpressionParameter parameter = targetParameters.parameters[i];
					if (
						parameter.name.StartsWith(settings.ParamTextVisible) ||
						parameter.name.StartsWith(settings.ParamKeyboardPrefix) ||
						parameter.name.StartsWith(settings.ParamTextPointer) ||
						parameter.name.StartsWith(settings.ParamTextSyncPrefix)
					) {
						parameters.Remove(parameter);
					}
				}

				targetParameters.parameters = parameters.ToArray();
				EditorUtility.SetDirty(targetParameters);
				Debug.Log("Removal from parameters completed.");
				return true;
			}
		#endif
	}
}

#endif
