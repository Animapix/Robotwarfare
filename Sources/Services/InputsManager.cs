using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Services
{
    public enum MouseButton { Left, Right }

    public interface InputsService
    {
        public bool IsJustPressed(Keys key);
        public bool IsPressed(Keys key);

        public bool IsJustPressed(MouseButton button, ButtonState state = ButtonState.Pressed);
        public bool IsPressed(MouseButton button, ButtonState state = ButtonState.Pressed);
        public Vector2 MousePosition();
    }

    public sealed class InputsManager : InputsService
    {
        private KeyboardState _oldKeybordState;
        private MouseState _oldMouseState;

        public InputsManager()
        {
            ServiceLocator.RegisterService<InputsManager>(this);
        }

        public void UpdateState()
        {
            _oldKeybordState = Keyboard.GetState();
            _oldMouseState = Mouse.GetState();
        }

        public bool IsJustPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) && !_oldKeybordState.IsKeyDown(key);
        }

        public bool IsPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public bool IsJustPressed(MouseButton button, ButtonState state = ButtonState.Pressed)
        {
            bool buttonState = false;
            if (button == MouseButton.Left) buttonState = Mouse.GetState().LeftButton == state && _oldMouseState.LeftButton != state;
            if (button == MouseButton.Right) buttonState = Mouse.GetState().RightButton == state && _oldMouseState.RightButton != state;
            return buttonState;
        }

        public bool IsPressed(MouseButton button, ButtonState state = ButtonState.Pressed)
        {
            bool buttonState = false;
            if (button == MouseButton.Left) buttonState = Mouse.GetState().LeftButton == state; 
            if (button == MouseButton.Right) buttonState = Mouse.GetState().RightButton == state;
            return buttonState;
        }

        public Vector2 MousePosition()
        {
            Point mousePoint = Mouse.GetState().Position;
            return new Vector2(mousePoint.X, mousePoint.Y);
        }
    }
}
