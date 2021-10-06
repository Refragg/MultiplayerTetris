using Microsoft.Xna.Framework.Input;

namespace MultiplayerTetris
{
    public static class GamePadStateExtensions
    {
        public static bool IsButtonDown(this GamePadState state, Buttons stickButton, float stickDeadZone, float triggerDeadZone)
        {
            switch (stickButton)
            {
                case Buttons.LeftThumbstickLeft:
                case Buttons.LeftThumbstickRight:
                case Buttons.LeftThumbstickDown:
                case Buttons.LeftThumbstickUp:
                case Buttons.RightThumbstickLeft:
                case Buttons.RightThumbstickRight:
                case Buttons.RightThumbstickDown:
                case Buttons.RightThumbstickUp:
                    return IsStickButtonPressed(state, stickButton, stickDeadZone, stickDeadZone);
                case Buttons.LeftTrigger:
                    return state.Triggers.Left > triggerDeadZone;
                case Buttons.RightTrigger:
                    return state.Triggers.Right > triggerDeadZone;
                default:
                    return state.IsButtonDown(stickButton);
            }
        }
        
        public static bool IsStickButtonPressed(this GamePadState state, Buttons stickButton, float leftDeadZone, float rightDeadZone)
        {
            switch (stickButton)
            {
                case Buttons.LeftThumbstickLeft:
                    return state.ThumbSticks.Left.X < -leftDeadZone;
                case Buttons.LeftThumbstickRight:
                    return state.ThumbSticks.Left.X > leftDeadZone;
                
                case Buttons.LeftThumbstickDown:
                    return state.ThumbSticks.Left.Y < -leftDeadZone;
                case Buttons.LeftThumbstickUp:
                    return state.ThumbSticks.Left.Y > leftDeadZone;
                
                case Buttons.RightThumbstickLeft:
                    return state.ThumbSticks.Right.X < -rightDeadZone;
                case Buttons.RightThumbstickRight:
                    return state.ThumbSticks.Right.X > rightDeadZone;
                
                case Buttons.RightThumbstickDown:
                    return state.ThumbSticks.Right.Y < -rightDeadZone;
                case Buttons.RightThumbstickUp:
                    return state.ThumbSticks.Right.Y > rightDeadZone;
                default:
                    return false;
            }
        }
    }
    
}