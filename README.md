# KillFrenzy Avatar Text (KAT)
A text display system designed to be used on VRChat Avatars. This takes advantage of the new OSC (Open Sound Protocol) system that lets other programs interact with your VRChat avatar.

![Demonstration](/Images/KAT_Demonstration.gif)

An in-game keyboard is now also included using VRChat Avatar Dynamics. You can type with an OSC app, or use the in-game keyboard. (Keep in mind you can't use the in-game keyboard while the OSC app is open).

![KeyboardDemonstration](/Images/KAT_Keyboard_Demonstration.gif)

# Recommended OSC apps
You will need an app to write avatar text using VRChat OSC.

**KillFrenzy Avatar Text OSC App**\
My own simple OSC app for KAT.\
https://github.com/killfrenzy96/KatOscApp

**TTS Voice Wizard**\
An OSC app by VRCWizard that supports speech-to-text-to-speech.\
https://github.com/VRCWizard/TTS-Voice-Wizard

# Prerequisites
- Unity 2019.4.31f1
- VRCSDK3. Make sure it is updated to support avatar dynamics.
- An existing working avatar (if installing to your own custom avatar).
- Your avatar needs to have at least 18 bits of parameter space available for the slowest setting of 1 sync parameter. You can use up to 74 bits of parameter space with the fastest setting of 8 sync parameters.
- Your avatar animator should be designed using write defaults off to avoid any issues.

# Installation (Simple)
1. Download the latest unity package from the releases: https://github.com/killfrenzy96/KillFrenzyAvatarText/releases
2. Use the KAT installer to add this to your avatar as shown below:\
![InstallInstructions](/Images/KAT_Install_Simple.gif)
3. Upload the avatar.
4. If you are using an OSC app, make sure you enable OSC in-game.\
![EnableOSC](/Images/EnableOSC.gif)

# Installation (Advanced)
Example files have been included in "Assets/KillFrenzy/AvatarText/Examples/"

This includes the animator controller, material, menu, parameters, a prefab for the KAT setup. A debugging expression menu is also included.

An example avatar that has KAT installed is also included.

# Developer Information
I have an open source KAT app written in python. You can have a look here:\
https://github.com/killfrenzy96/KatOscApp

This is some additional information if you would like to develop your own OSC software.

I recommend uploading the example avatar (YumiExample) and playing with the debug menu first to get a better understanding of how the KAT system works. Make sure "KAT Visible" is turned on, then select a pointer position, then edit the characters.

Here's a short overview of how the KAT works. This is an example with 4 sync parameters.
- The keyboard supports a limit of 128 letters in length.
- Make sure to set (KAT_Visible) to true to reveal the keyboard.
- It uses a pointer (KAT_Pointer) to mark which section of text you would like to edit.
	- The pointer position ranges from 1 to 32 (128 letters / 4 sync parameters = 32 pointer positions).
	- Pointer position 1 edits letters 1 to 4. Pointer postion 2 edits letters 5 to 8. Pointer position 3 edits letters 9 to 12. It keeps going for 32 pointers.
	- Setting the pointer position at 255 will clear all the text.
- In this example, there are 4 sync parameters each editing a different letter.
	- The values can be set between -1.0 and 1.0, with each (1 / 127) increment representing a different character.
	- The values from 0.0 to 1.0 will represent the characters from 0 to 127.
	- The values from -1.0 to 0.0 will represent the characters from 129 to 255.

There are 6 syncronised expression parameters. Here's a short overview of what they do:
- KAT_Visible (Bool): Used to show or hide the text.
- KAT_Pointer (Int): Used to indicate which section of text you are editing. The range of this depends on how many sync parameters there are. (Equal to 128 letters / X sync parameters)
- KAT_CharSyncX (Float): The X letter within the pointer that is being edited. Each (1 / 127) increment represents a different character.

If you require additional information, please join the official VRChat server (https://discord.gg/vrchat) or Alpha Blend Interactive server (https://discord.gg/abi) and send me a message directly (KillFrenzy#7777).

# Why have I made this?
This will directly replace my old VRC Avatar Keyboard system. It replaces the particle text on the old keyboard with a shader setup. This greatly improves performance and even makes it possible to place this on an excellent performance ranked avatar.

On top of that, installation is automated and much more simple to setup. This allows a greater playerbase to take advantage of this.
