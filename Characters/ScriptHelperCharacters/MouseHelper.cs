using System.Diagnostics;
using Godot;

namespace GameHelperCharacters
{
    public static class MouseHelper
    {
        public static Vector2 HandlePlayerMovementToMouse(Vector2 MousePosition, Vector2 CharacterPosition, float Speed)
        {
            Vector2 velocity;

            velocity.X = (MousePosition - CharacterPosition).Normalized().X * Speed;
			velocity.Y = (MousePosition - CharacterPosition).Normalized().Y * Speed;

            return velocity;
        }
    }
}