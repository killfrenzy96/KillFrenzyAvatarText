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
