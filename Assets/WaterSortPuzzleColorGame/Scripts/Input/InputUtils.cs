using UnityEngine;
using UnityEngine.InputSystem;

namespace WaterSortPuzzleGame
{
    public static class InputUtils
    {
        /// <summary>
        /// Returns true if a click or touch happened this frame.
        /// </summary>
        public static bool IsClickThisFrame()
        {
            return (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
        }

        /// <summary>
        /// Returns current screen position of mouse or touch.
        /// </summary>
        public static Vector2 GetScreenPosition()
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                return Touchscreen.current.primaryTouch.position.ReadValue();

            if (Mouse.current != null)
                return Mouse.current.position.ReadValue();

            return Vector2.zero;
        }

        /// <summary>
        /// Converts screen position to world position.
        /// </summary>
        public static Vector2 GetWorldPosition(Camera camera)
        {
            Vector2 screenPos = GetScreenPosition();
            Vector3 worldPos = camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, camera.nearClipPlane));
            return new Vector2(worldPos.x, worldPos.y);
        }

        /// <summary>
        /// Performs a 2D Raycast at the current pointer position.
        /// </summary>
        public static RaycastHit2D Raycast2D(Camera camera)
        {
            var worldPos = GetWorldPosition(camera);
            return Physics2D.Raycast(worldPos, Vector2.zero);
        }
    }
}
