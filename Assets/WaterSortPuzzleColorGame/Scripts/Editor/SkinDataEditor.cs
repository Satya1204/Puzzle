using UnityEditor;
using UnityEngine;

namespace WaterSortPuzzleGame
{
    [CustomPropertyDrawer(typeof(SkinData), true)]
    public class SkinDataEditor : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) + 70 + 5;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw all default fields
            position.height = EditorGUI.GetPropertyHeight(property, label, true);
            EditorGUI.PropertyField(position, property, label, true);

            // Draw Sprite preview
            SerializedProperty imageProp = property.FindPropertyRelative("image");
            if (imageProp != null && imageProp.objectReferenceValue != null)
            {
                Sprite sprite = imageProp.objectReferenceValue as Sprite;
                if (sprite != null)
                {
                    Rect texRect = sprite.textureRect;

                    // Calculate aspect ratio
                    float aspect = texRect.width / texRect.height;
                    float previewSize = 64f;
                    Rect previewRect = new Rect(position.x + 15, position.y + position.height + 5, previewSize, previewSize);

                    // Adjust rect to keep aspect ratio
                    if (aspect > 1f) // wider
                        previewRect.height = previewSize / aspect;
                    else // taller
                        previewRect.width = previewSize * aspect;

                    // Center it
                    previewRect.x += (64 - previewRect.width) * 0.5f;
                    previewRect.y += (64 - previewRect.height) * 0.5f;

                    // Draw only the sprite portion of the texture
                    GUI.DrawTextureWithTexCoords(previewRect, sprite.texture,
                        new Rect(texRect.x / sprite.texture.width,
                                 texRect.y / sprite.texture.height,
                                 texRect.width / sprite.texture.width,
                                 texRect.height / sprite.texture.height));
                }
            }
        }
    }
}
