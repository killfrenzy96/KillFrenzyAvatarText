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

namespace KillFrenzy.AvatarTextTools.Settings
{
	public static class KatSettings
	{
		public const string ParamTextLogicPrefix = "KAT_Logic";
		public const string ParamTextVisible = "KAT_Visible";
		public const string ParamTextPointer = "KAT_Pointer";
		public const string ParamTextSyncPrefix = "KAT_CharSync";
		public const string ParamCharPrefix = "KAT_Char";

		public const string CharacterAnimationFolder = "KAT_CharAnimations/";
		public const string CharacterAnimationClipNamePrefix = "Char";
		public const int CharacterLimit = 128;
		public const int CharacterCount = 96;
		public const int CharacterSyncParamsSize = 4;
		public const int PointerCount = CharacterLimit / CharacterSyncParamsSize;
	}
}
