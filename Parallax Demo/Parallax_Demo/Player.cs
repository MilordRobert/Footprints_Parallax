using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Parallax_Demo
{
    class Player
    {
        Vector3 position;
        Vector3 rotation;
        Vector3 forward;
        Vector3 right;
        Vector3 up;
        float heading, elevation;
        MouseState last_mouse_state;
        KeyboardState kbs;
        const float da = 0.01f, dp = 0.1f;

        public Player()
        {
            reset_Position();
        }

        public void reset_Position()
        {
            position = new Vector3(0, 02f, 5);
            rotation = new Vector3(0, 0, 0);
            forward = new Vector3(0, 0, -1);
            right = new Vector3(1, 0, 0);
            up = new Vector3(0, 1, 0);
            heading = 0;
            elevation = 0;
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
        }

        public Vector3 Forward
        {
            get { return forward; }
        }

        public Vector3 Up
        {
            get { return up; }
        }

        public void Update()
        {
            float movement = dp;
            //if (Keyboard.GetState().IsKeyDown(Keys.Up)) elevation += da;
            //if (Keyboard.GetState().IsKeyDown(Keys.Down)) elevation -= da;
            //if (Keyboard.GetState().IsKeyDown(Keys.Left)) heading -= da;
            //if (Keyboard.GetState().IsKeyDown(Keys.Right)) heading += da;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftShift)) movement *= 3;

            // Version 3
            int mdx = Mouse.GetState().X - 100;
            int mdy = Mouse.GetState().Y - 100;
            heading += mdx / 1000.0f;
            elevation -= mdy / 1000.0f;
            Mouse.SetPosition(100, 100);

            last_mouse_state = Mouse.GetState();

            forward = Vector3.Transform(new Vector3(0, 0, -1),
                         Matrix.CreateRotationY(-heading)
                        );
            right = Vector3.Cross(forward, new Vector3(0, 1, 0));
            right.Normalize();
            up = Vector3.Cross(right, forward);

            if (Keyboard.GetState().IsKeyDown(Keys.W)) position += movement * forward;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) position -= movement * forward;
            if (Keyboard.GetState().IsKeyDown(Keys.A)) position -= movement * right;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) position += movement * right;

            forward = Vector3.Transform(new Vector3(0, 0, -1),
                        Matrix.CreateRotationX(elevation) * Matrix.CreateRotationY(-heading)
                        );
            rotation = new Vector3(0, -heading, 0);

        }
    }
}
