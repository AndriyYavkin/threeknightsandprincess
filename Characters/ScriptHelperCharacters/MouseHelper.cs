using System;
using Godot;

namespace GameHelperCharacters
{
    public static class MouseHelper
    {
        ///<summary>
        ///<para>Takes 3 parameters: Camera3D, mouse position(viewport) and speed.</para>
        ///<para>Returns normalized Vector3 velocity.</para>
        ///<para> <b>Note:</b> Should be used only with CharacterBody3D and player controlled characters. </para>
        ///<para> <b>Note 2: Moving camera away from the character will break movements</b></para>
        ///</summary>
        public static Vector3 HandlePlayerMovementToMouse(Camera3D camera, Vector2 mouseViewport, float Speed)
        {
            Vector3 from = camera.ProjectRayOrigin(mouseViewport);
            Vector3 to = from + camera.ProjectRayNormal(mouseViewport);

            Vector3 velocity = Vector3.Zero;
            
            velocity.X = to.X - from.X;
            velocity.Z = to.Z - from.Z;

            return velocity.Normalized() * Speed;

        }

        ///<summary>
        ///<para>Takes 4 parameters: global mouse position, position of character, speed, limit of pixels where we need acceleration.</para>
        ///<para>Returns normalized Vector2 velocity.</para>
        ///<para> <b>Note:</b> Should be used only with CharacterBody2D and player controlled characters. </para>
        ///<para> <b>Note 2:</b> It is not implemented yet! Using this method will result in NotImplementedException</para>
        ///</summary>
        public static Vector2 HandlePlayerMovementToMouseWithAcceleration(Vector2 MousePosition, Vector2 CharacterPosition, float Speed)
        {
            throw new NotImplementedException();

            Vector2 velocity;
            Vector2 dif = MousePosition - CharacterPosition;
            int lim = 300;
            int MaxDif = 1;

            if (Math.Abs(dif.X) < lim && Math.Abs(dif.Y) < lim)
            {   
                GD.Print("Before: ", dif);
                dif /= 30; //If our difference in any coordinates less than 100, we translate it in %
                GD.Print("After: ", dif);
                velocity = dif.Normalized() * Speed;

                if(Math.Abs(dif.X) <= MaxDif && Math.Abs(dif.X) > 0)
                {
                    velocity *= Math.Abs(dif.X);
                }

                if(Math.Abs(dif.Y) <= MaxDif && Math.Abs(dif.Y) > 0)
                {
                    velocity *= Math.Abs(dif.Y);
                }

                if(dif.X == 0 || dif.Y == 0)
                {
                    velocity.X *= 0.1f;
                    velocity.Y *= 0.1f;
                }
            }
            else
            {
                velocity = dif.Normalized() * Speed;
            }
            GD.Print(velocity);

            return velocity;
        }
    }
}