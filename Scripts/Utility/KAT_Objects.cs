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
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Animations;

#if VRC_SDK_VRCSDK3
	using VRCAvatarDescriptor = VRC.SDK3.Avatars.Components.VRCAvatarDescriptor;
#endif

#if CVR_CCK_EXISTS
	using CVRAvatar = ABI.CCK.Components.CVRAvatar;
#endif

using KillFrenzy.AvatarTextTools.Settings;


namespace KillFrenzy.AvatarTextTools.Utility
{
	public static class KatObjectsInstaller
	{
		#if VRC_SDK_VRCSDK3
			public static bool InstallObjectsToAvatar(VRCAvatarDescriptor avatarDescriptor, KatSettings settings)
			{
				Transform transform = avatarDescriptor.gameObject.transform;
				return InstallObjectsToAvatar(transform, settings);
			}

			public static bool RemoveObjectsFromAvatar(VRCAvatarDescriptor avatarDescriptor, KatSettings settings)
			{
				Transform transform = avatarDescriptor.gameObject.transform;
				return RemoveObjectsFromAvatar(transform, settings);
			}
		#endif

		#if CVR_CCK_EXISTS
			public static bool InstallObjectsToAvatar(CVRAvatar avatarDescriptor, KatSettings settings)
			{
				Transform transform = avatarDescriptor.gameObject.transform;
				return InstallObjectsToAvatar(transform, settings);
			}

			public static bool RemoveObjectsFromAvatar(CVRAvatar avatarDescriptor, KatSettings settings)
			{
				Transform transform = avatarDescriptor.gameObject.transform;
				return RemoveObjectsFromAvatar(transform, settings);
			}
		#endif

		public static bool InstallObjectsToAvatar(Transform avatarRootTransform, KatSettings settings)
		{
			Material textMaterial = Resources.Load<Material>("KAT_Misc/KAT_Text");;
			Transform avatarAttachmentTransform = null;
			Vector3 avatarAttachmentOffset = new Vector3(0.0f, 1.0f, 0.4f);

			switch (settings.AttachmentPoint) {
				case KatAttachmentPoint.Head: {
					avatarAttachmentTransform = FindAvatarHead(avatarRootTransform);
					if (avatarAttachmentTransform == null) {
						Debug.LogWarning("Warning: Avatar head not found.");
					} else {
						avatarAttachmentOffset = new Vector3(-0.4f, 0.15f, 0.0f);
					}
					break;
				}
				case KatAttachmentPoint.Chest: {
					avatarAttachmentTransform = FindAvatarChest(avatarRootTransform);
					if (avatarAttachmentTransform == null) {
						Debug.LogWarning("Warning: Avatar chest not found.");
					} else {
						avatarAttachmentOffset = new Vector3(0.0f, 0.0f, 0.4f);
					}
					break;
				}
			}

			if (textMaterial == null) {
				Debug.LogError("Error: Material for Text not found.");
				return false;
			}

			GameObject keyboardObject = null;
			if (settings.InstallKeyboard) {
				Object keyboardPrefab = Resources.Load<GameObject>("KAT_Misc/KillFrenzyAvatarKeyboard");
				if (keyboardPrefab == null) {
					Debug.LogError("Error: Prefab for keyboard not found.");
					return false;
				}

				keyboardObject = (GameObject)PrefabUtility.InstantiatePrefab(keyboardPrefab, avatarRootTransform);
				keyboardObject.SetActive(false);

				try {
					Transform avatarChest = FindAvatarChest(avatarRootTransform);

					// Create constraint on avatar to stabilize rotation
					GameObject attachmentPoint = new GameObject(settings.KeyboardAttachmentPointName);
					attachmentPoint.transform.SetParent(avatarChest);
					attachmentPoint.transform.localPosition = new Vector3(0, 0, 0);

					RotationConstraint constraintAvatar = attachmentPoint.AddComponent<RotationConstraint>();
					constraintAvatar.locked = true;
					constraintAvatar.rotationAxis = Axis.X | Axis.Z;

					ConstraintSource constraintAvatarSource = new ConstraintSource();
					constraintAvatarSource.sourceTransform = keyboardObject.transform;
					constraintAvatarSource.weight = 1f;
					constraintAvatar.AddSource(constraintAvatarSource);
					constraintAvatar.constraintActive = true;

					GameObject attachmentTarget = new GameObject(settings.KeyboardAttachmentPointName + "_Target");
					attachmentTarget.transform.SetParent(attachmentPoint.transform);
					attachmentTarget.transform.localPosition = new Vector3(0.0f, 0.0f, 0.4f);

					// Create constraint on keyboard to position keyboard over the avatar
					Transform keyboardConstraintTransform = FindTransformRecursive(keyboardObject.transform, "ConstraintChild");
					ParentConstraint parentConstraint = keyboardConstraintTransform.gameObject.GetComponent<ParentConstraint>();
					ConstraintSource constraintSource = parentConstraint.GetSource(0);
					constraintSource.sourceTransform = attachmentTarget.transform;
					parentConstraint.SetSource(0, constraintSource);

				} catch {
					Debug.LogWarning("Warning: Could not attach keyboard 'ConstraintChild' to '" + settings.KeyboardAttachmentPointName + "'.");
				}
			}

			GameObject katObject = new GameObject("KillFrenzyAvatarText");
			katObject.transform.SetParent(avatarRootTransform);
			katObject.SetActive(false);

			GameObject constraintObject = new GameObject("Constraint");
			constraintObject.transform.SetParent(katObject.transform);

			if (avatarAttachmentTransform != null) {
				// Create constraint on avatar to stabilize rotation
				GameObject attachmentPoint = new GameObject(settings.TextAttachmentPointName);
				attachmentPoint.transform.SetParent(avatarAttachmentTransform);
				attachmentPoint.transform.localPosition = new Vector3(0, 0, 0);

				RotationConstraint constraintAvatar = attachmentPoint.AddComponent<RotationConstraint>();
				constraintAvatar.locked = true;
				constraintAvatar.rotationAxis = Axis.X | Axis.Z;

				ConstraintSource constraintAvatarSource = new ConstraintSource();
				constraintAvatarSource.sourceTransform = katObject.transform;
				constraintAvatarSource.weight = 1f;
				constraintAvatar.AddSource(constraintAvatarSource);
				constraintAvatar.constraintActive = true;

				GameObject attachmentTarget = new GameObject(settings.TextAttachmentPointName + "_Target");
				attachmentTarget.transform.SetParent(attachmentPoint.transform);
				attachmentTarget.transform.localPosition = avatarAttachmentOffset;

				// Create constraint on KAT to position text over the avatar
				ParentConstraint constraintText = constraintObject.AddComponent<ParentConstraint>();
				constraintText.locked = true;

				ConstraintSource constraintTextSource = new ConstraintSource();
				constraintTextSource.sourceTransform = attachmentTarget.transform;
				constraintTextSource.weight = 1f;
				constraintText.AddSource(constraintTextSource);

				if (settings.AttachmentPoint == KatAttachmentPoint.Chest) {
					constraintText.rotationAxis = Axis.Y;
				}

				if (settings.InstallKeyboard) {
					Transform avatarChest = FindAvatarChest(avatarRootTransform);

					if (avatarChest == null) {
						Debug.LogWarning("Warning: Avatar chest not found.");
					} else {
						Transform constraintKeyboardAttachment = FindTransformRecursive(keyboardObject.transform, "KeyboardText_AttachmentPoint");
						if (constraintKeyboardAttachment == null) {
							Debug.Log(keyboardObject);
							Debug.LogWarning("Warning: Prefab for keyboard does not contain 'KeyboardText_AttachmentPoint'.");
						} else {
							ConstraintSource constraintSource2 = new ConstraintSource();
							constraintSource2.sourceTransform = constraintKeyboardAttachment;
							constraintSource2.weight = 0f;
							constraintText.AddSource(constraintSource2);
						}
					}
				}

				constraintText.constraintActive = true;
			}

			GameObject textObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
			textObject.name = "Text";
			textObject.transform.SetParent(constraintObject.transform);
			textObject.transform.localScale = new Vector3(0.5f, 1f / 6f, 1.0f);

			GameObject.DestroyImmediate(textObject.GetComponent<MeshCollider>());

			MeshRenderer textRenderer = textObject.GetComponent<MeshRenderer>();
			textRenderer.material = textMaterial;

			// EditorSceneManager.MarkSceneDirty(avatarDescriptor.gameObject.scene);
			EditorSceneManager.MarkSceneDirty(avatarRootTransform.gameObject.scene);
			Debug.Log("Installation of objects to avatar completed.");
			return true;
		}

		public static bool RemoveObjectsFromAvatar(Transform avatarRootTransform, KatSettings settings)
		{
			Transform katObjectTransform = avatarRootTransform.transform.Find("KillFrenzyAvatarText");
			if (katObjectTransform) {
				// Remove constraint target
				try {
					Transform katConstraintTransform = katObjectTransform.transform.Find("Constraint");
					ParentConstraint constraint = katConstraintTransform.GetComponent<ParentConstraint>();
					ConstraintSource constraintSource = constraint.GetSource(0);
					if (constraintSource.sourceTransform.parent.gameObject.name == settings.TextAttachmentPointName) {
						GameObject.DestroyImmediate(constraintSource.sourceTransform.parent.gameObject);
					}
				} catch {}

				// Remove parent object
				try {
					GameObject.DestroyImmediate(katObjectTransform.gameObject);
				} catch {
					Debug.LogWarning("Warning: Unable to destroy the KillFrenzyAvatarText object.");
					return false;
				}
			}

			Transform keyboardObjectTransform = avatarRootTransform.transform.Find("KillFrenzyAvatarKeyboard");
			if (keyboardObjectTransform) {
				// Remove constraint target
				try {
					Transform keyboardConstraintTransform = FindTransformRecursive(keyboardObjectTransform, "ConstraintChild");
					ParentConstraint constraint = keyboardConstraintTransform.GetComponent<ParentConstraint>();
					ConstraintSource constraintSource = constraint.GetSource(0);
					if (constraintSource.sourceTransform.parent.gameObject.name == settings.KeyboardAttachmentPointName) {
						GameObject.DestroyImmediate(constraintSource.sourceTransform.parent.gameObject);
					}
				} catch {}

				// Remove parent object
				try {
					GameObject.DestroyImmediate(keyboardObjectTransform.gameObject);
				} catch {
					Debug.LogWarning("Warning: Unable to destroy the KillFrenzyAvatarKeyboard object.");
					EditorSceneManager.MarkSceneDirty(avatarRootTransform.gameObject.scene);
					return false;
				}
			}

			EditorSceneManager.MarkSceneDirty(avatarRootTransform.gameObject.scene);
			Debug.Log("Removal of objects from avatar completed.");
			return true;
		}

		private static Transform FindAvatarChest(Transform transform)
		{
			Transform chest = null;

			// Get chest from animator bone
			Animator animator = transform.GetComponent<Animator>();
			if (animator != null) {
				chest = animator.GetBoneTransform(HumanBodyBones.Chest);
				if (chest != null) {
					return chest;
				}
			}

			// Otherwise check bone transform names to find the head
			chest = FindTransformRecursive(transform, "chest");
			if (chest != null) {
				Transform neck = FindTransformRecursive(transform, "neck");
				if (neck != null) {
					return chest;
				}
			}

			return null;
		}

		private static Transform FindAvatarHead(Transform transform)
		{
			Transform head = null;

			// Get neck from animator bone
			Animator animator = transform.GetComponent<Animator>();
			if (animator != null) {
				head = animator.GetBoneTransform(HumanBodyBones.Head);
				if (head != null) {
					return head;
				}
			}

			// Otherwise check bone transform names to find the head
			Transform neck = FindTransformRecursive(transform, "neck");
			if (neck != null) {
				head = FindTransformRecursive(transform, "head");
				return head;
			}

			return null;
		}

		private static Transform FindTransformRecursive(Transform transform, string name)
		{
			foreach (Transform child in transform) {
				if (child.name.ToLower().Contains(name.ToLower())) {
					return child;
				} else {
					Transform found = FindTransformRecursive(child, name);
					if (found != null) {
						return found;
					}
				}
			}
			return null;
		}
	}
}

#endif
