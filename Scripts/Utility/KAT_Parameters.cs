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

using System.Collections.Generic;

using ExpressionParameters = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
using ExpressionParameter = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter;

using KillFrenzy.AvatarTextTools.Settings;


namespace KillFrenzy.AvatarTextTools.Utility
{
	public static class KatParametersInstaller
	{
		public static bool InstallToParameters(ExpressionParameters targetParameters, bool installKeyboard)
		{
			List<ExpressionParameter> parameters = new List<ExpressionParameter>();
			parameters.AddRange(targetParameters.parameters);

			parameters.Add(new ExpressionParameter() {
				name = KatSettings.ParamTextVisible,
				valueType = ExpressionParameters.ValueType.Bool,
				defaultValue = 0,
				saved = false
			});

			if (installKeyboard) {
				parameters.Add(new ExpressionParameter() {
					name = KatSettings.ParamKeyboardPrefix,
					valueType = ExpressionParameters.ValueType.Bool,
					defaultValue = 0,
					saved = false
				});
			}

			parameters.Add(new ExpressionParameter() {
				name = KatSettings.ParamTextPointer,
				valueType = ExpressionParameters.ValueType.Int,
				defaultValue = 0,
				saved = false
			});

			for (int i = 0; i < KatSettings.SyncParamsSize; i++) {
				parameters.Add(new ExpressionParameter() {
					name = KatSettings.ParamTextSyncPrefix + i.ToString(),
					valueType = ExpressionParameters.ValueType.Float,
					defaultValue = 0,
					saved = false
				});
			}

			targetParameters.parameters = parameters.ToArray();
			return true;
		}

		public static bool RemoveFromParameters(ExpressionParameters targetParameters)
		{
			List<ExpressionParameter> parameters = new List<ExpressionParameter>();
			parameters.AddRange(targetParameters.parameters);

			for (int i = 0; i < targetParameters.parameters.Length; i++) {
				ExpressionParameter parameter = targetParameters.parameters[i];
				if (
					parameter.name.StartsWith(KatSettings.ParamTextVisible) ||
					parameter.name.StartsWith(KatSettings.ParamKeyboardPrefix) ||
					parameter.name.StartsWith(KatSettings.ParamTextPointer) ||
					parameter.name.StartsWith(KatSettings.ParamTextSyncPrefix)
				) {
					parameters.Remove(parameter);
				}
			}

			targetParameters.parameters = parameters.ToArray();
			return true;
		}
	}
}

#endif
