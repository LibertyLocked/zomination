using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FAZEngine
{
    public class Camera2D
    {
        protected float _zoom; // Camera Zoom
        protected Matrix _transform; // Matrix Transform
        protected Vector2 _pos; // Camera Position
        protected float _rotation; // Camera Rotation
        private const float minZoom = 0.1f;
        private const float maxZoom = 10f;

        public Camera2D()
        {
            _zoom = 1.0f;
            _rotation = 0.0f;
            _pos = Vector2.Zero;
        }

        // Sets and gets zoom
        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = value;
                if (_zoom < minZoom) _zoom = minZoom; // Negative zoom will flip image
                if (_zoom > maxZoom) _zoom = maxZoom;
            }
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        // Auxiliary function to move the camera
        public void Move(Vector2 amount)
        {
            _pos += amount;
        }

        // Get set position
        public Vector2 Pos
        {
            get { return _pos; }
            set { _pos = value; }
        }

        public Matrix get_transformation(GraphicsDevice graphicsDevice)
        {
            return get_transformation(graphicsDevice.Viewport);
        }

        public Matrix get_transformation(Viewport viewport)
        {
            _transform =
              Matrix.CreateTranslation(new Vector3(-_pos.X, -_pos.Y, 0)) *
              Matrix.CreateRotationZ(Rotation) *
              Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
              Matrix.CreateTranslation(new Vector3(viewport.Width * 0.5f, viewport.Height * 0.5f, 0));
            return _transform;
        }

        public void ForceSetZoom(float zoom)
        {
            this._zoom = zoom;
        }
    }
}
