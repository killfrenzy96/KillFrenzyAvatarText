# KillFrenzy Avatar Text (KAT)
A text display system designed to be used on VRChat Avatars. This takes advantage of the new OSC (Open Sound Protocol) system that lets other programs interact with your VRChat avatar.

Keep in mind that there is no in-game menu or keyboard for this system. It is designed to be used by OSC. When VRChat avatar dynamics releases, I will consider creating an in-game keyboard for this.

![Demonstration](/Images/KAT_Demonstration.gif)

# Recommended OSC programs
You will need a program to use this avatar text by using VRChat OSC.

[English - ASCII] Keyboard App by harunadev\
harunadev has created an OSC program for the avatar text system. Check it out and download here:\
https://harunadev.booth.pm/items/3691327

[英語＆日本語] KillFrenzy's Avatar Text (KAT) 日本語入力プログラム by harunadev\
This is the Japanese version of harunadev's app:\
https://harunadev.booth.pm/items/3692774

# Prerequisites
- Unity 2019.4.31f1
- VRCSDK3
- An existing working avatar (if installing to your own custom avatar). Your avatar needs to have at least 41 bits of parameter space available.

# Installation (Simple)
1. Download the latest unity package from the releases: https://github.com/killfrenzy96/KillFrenzyAvatarText/releases
2. Use the KAT installer to add this to your avatar as shown below:\
![InstallInstructions](/Images/KAT_Install_Simple.gif)
3. Upload the avatar.
4. Be sure to enable OSC once you are in VRChat.\
![EnableOSC](/Images/EnableOSC.gif)

# Installation (Advanced)
Example files have been included in "Assets/KillFrenzy/AvatarText/Examples/"

This includes the animator controller, material, parameters, a prefab for the KAT setup, and debug expression menu.

An example avatar that has KAT installed is also included.

# Developer Information
An example will be available soon. It is a work in progress.

This is some additional information if you would like to develop your own OSC software.

I recommend uploading the example avatar (YumiExample) and playing with the debug menu first to get a better understanding of how the KAT system works. Make sure "KAT Visible" is turned on, then select a pointer position, then edit the characters.

Here's a short overview of how the KAT works:
- The keyboard supports a limit of 128 letters in length.
- Make sure to set (KAT_Visible) to true to reveal the keyboard.
- It uses a pointer (KAT_Pointer) to mark which section of text you would like to edit.
	- The pointer position ranges from 1 to 32.
	- Pointer position 1 edits letters 1 to 4. Pointer postion 2 edits letters 5 to 8. Pointer position 3 edits letters 9 to 12. It keeps going for 32 pointers.
	- Setting the pointer position at 255 will clear all the text.
- There are 4 sync variables, each editing a different letter.
	- The values can be set between -1.0 and 1.0, with each (1 / 127) increment representing a different character.
	- The values from 0.0 to 1.0 will represent the characters from 0 to 127.
	- The values from -1.0 to 0.0 will represent the characters from 129 to 255.

There are 6 syncronised expression parameters. Here's a short overview of what they do:
- KAT_Visible (Bool): Used to show or hide the text.
- KAT_Pointer (Int): Used to indicate which section of text you are editing. This ranges from 1 to 32.
- KAT_CharSync0 (Float): The 1st letter within the pointer that is being edited. Each (1 / 127) increment represents a different character.
- KAT_CharSync1 (Float): The 2nd letter within the pointer that is being edited. Each (1 / 127) increment represents a different character.
- KAT_CharSync2 (Float): The 3rd letter within the pointer that is being edited. Each (1 / 127) increment represents a different character.
- KAT_CharSync3 (Float): The 4th letter within the pointer that is being edited. Each (1 / 127) increment represents a different character.

If you require additional information, please join the official VRChat server (https://discord.gg/vrchat) and send me a message directly (KillFrenzy#7777).

# Why have I made this?
This will directly replace my old VRC Avatar Keyboard system. It replaces the particle text on the old keyboard with a shader. This greatly improves performance and even makes it possible to place this on an excellent performance ranked avatar.

On top of that, installation is automated and much more simple to setup. This allows a greater playerbase to take advantage of this.

I have plans to implement an in-game keyboard with VRC Avatar Dynamics. I will have to determine that is possible to do so when Avatar Dynamics is available.

# Donation
If you like this software and feel like gifting me something, you can donate to me here:

https://www.paypal.com/donate/?business=U8UQQPADHCQVN&no_recurring=0&item_name=Thank+you+for+your+donation%2C+it+is+much+appreciated+and+will+help+maintain+a+better+living+situation.&currency_code=AUD