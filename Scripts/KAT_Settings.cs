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

namespace KillFrenzy.AvatarTextTools.Settings
{
	public class KatSettings
	{
		public readonly int SDK = KatSDK.VRChat;
		public readonly string ParamTextVisible = "KAT_Visible";
		public readonly string ParamTextPointer = "KAT_Pointer";
		public readonly string ParamTextSyncPrefix = "KAT_CharSync";

		public readonly string ParamKeyboardPrefix = "KAT_Keyboard";
		public readonly string ParamKeyboardPressed = "KAT_KeyboardPressed";
		public readonly string ParamKeyboardShift = "KAT_KeyboardShift";
		public readonly string ParamKeyboardProximity = "KAT_KeyboardProximity";
		public readonly string ParamKeyboardProximityExit = "KAT_KeyboardProximityExit";
		public readonly string ParamKeyboardHighlight = "KAT_KeyboardHighlight";
		public readonly string ParamKeyboardPressedClear = "KAT_KeyboardPressedClear";
		public readonly string ParamKeyboardPressedBackspace = "KAT_KeyboardPressedBksp";
		public readonly string ParamKeyboardPressedShiftL = "KAT_KeyboardPressedShiftL";
		public readonly string ParamKeyboardPressedShiftR = "KAT_KeyboardPressedShiftR";

		public readonly int KeyboardKeyBackspace = 97;
		public readonly int KeyboardKeyClear = 96;
		public readonly int KeyboardKeyShiftL = 98;
		public readonly int KeyboardKeyShiftR = 99;
		public readonly float KeyboardBackspaceMode = 97f / 127f;

		public readonly int KeyboardKeysCount = 96;

		public readonly string CharacterAnimationFolder = "KAT_CharAnimations/";
		public readonly string CharacterAnimationClipNamePrefix = "Char";
		public readonly int TextLength = 128;
		public readonly int SyncParamsSize = 4;
		public readonly int PointerCount = 32; // = TextLength / SyncParamsSize;
		public readonly int PointerAltSyncOffset = 100;
		public readonly bool InstallKeyboard = true;

		public readonly string GeneratedOutputFolderName = "KAT_GeneratedOutput";

		public readonly string TextAttachmentPointName = "KAT_Text_AttachmentPoint";
		public readonly string KeyboardAttachmentPointName = "KAT_Keyboard_AttachmentPoint";
		public readonly int AttachmentPoint = KatAttachmentPoint.Chest;

		public KatSettings(int sdk = KatSDK.VRChat, bool installKeyboard = true, int attachmentPoint = KatAttachmentPoint.Chest, int syncParamsSize = 4)
		{
			SDK = sdk;
			InstallKeyboard = sdk == KatSDK.VRChat ? installKeyboard : false;
			SyncParamsSize = syncParamsSize;
			PointerCount = TextLength / SyncParamsSize;
			AttachmentPoint = attachmentPoint;
			if (SyncParamsSize == 1) {
				PointerAltSyncOffset = 0;
			}
		}

		public int GetPointerCount()
		{
			return TextLength / SyncParamsSize;
		}
	}

	public static class KatAttachmentPoint
	{
		public const int None = 0;
		public const int Head = 1;
		public const int Chest = 2;
	}

	public static class KatSDK
	{
		public const int VRChat = 0;
		public const int ChilloutVR = 1;
	}
}

#endif
