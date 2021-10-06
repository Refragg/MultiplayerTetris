using Microsoft.Xna.Framework;

namespace MultiplayerTetris
{
    public class AudioEmitter
    {
        
        
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        Vector3 position;

        public Vector3 Forward
        {
            get { return forward; }
            set { forward = value; }
        }

        Vector3 forward;


        public Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }

        Vector3 up;


        public Vector3 Velocity
        {
            get { return velocity; }
            protected set { velocity = value; }
        }

        Vector3 velocity;
        
        
        
        
        public void Update(Vector3 pos)
        {

            Position = pos;

        }
    }
}
