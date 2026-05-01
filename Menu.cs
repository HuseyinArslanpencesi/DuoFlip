using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Hoppala;

public enum MenuState { Main, Settings }

public class Menu
{
    private SpriteFont _font;
    public MenuState CurrentState = MenuState.Main;
    public float MusicVolume = 0.5f;
    public float SfxVolume = 0.8f;

    private Rectangle playBtn, settingsBtn, exitBtn, backBtn;

    public Menu(SpriteFont font, int w, int h)
    {
        _font = font;
        int cx = w / 2 - 80;
        playBtn = new Rectangle(cx, 400, 200, 50);
        settingsBtn = new Rectangle(cx, 500, 200, 50);
        exitBtn = new Rectangle(cx, 600, 200, 50);
        backBtn = new Rectangle(cx, 600, 200, 50);
    }

    public string Update(MouseState mouse, MouseState oldMouse)
    {
        bool clicked = mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released;

        if (CurrentState == MenuState.Main)
        {
            if (clicked && playBtn.Contains(mouse.Position)) return "PLAY";
            if (clicked && settingsBtn.Contains(mouse.Position)) CurrentState = MenuState.Settings;
            if (clicked && exitBtn.Contains(mouse.Position)) return "EXIT";
        }
        else if (CurrentState == MenuState.Settings)
        {
            if (clicked && backBtn.Contains(mouse.Position)) CurrentState = MenuState.Main;
            
            if (clicked && new Rectangle(playBtn.X, 350, 250, 50).Contains(mouse.Position)) MusicVolume = (MusicVolume + 0.1f) % 1.1f;
            if (clicked && new Rectangle(playBtn.X, 450, 250, 50).Contains(mouse.Position)) SfxVolume = (SfxVolume + 0.1f) % 1.1f;
        }
        return "NONE";
    }

    public void Draw(SpriteBatch sb, int w, int h)
    {
        sb.DrawString(_font, "DUO FLIP", new Vector2(w / 2 - 100, 200), Color.White);

        if (CurrentState == MenuState.Main)
        {
            sb.DrawString(_font, "Play Game", new Vector2(playBtn.X, playBtn.Y), Color.White);
            sb.DrawString(_font, "Settings", new Vector2(settingsBtn.X, settingsBtn.Y), Color.White);
            sb.DrawString(_font, "Exit", new Vector2(exitBtn.X, exitBtn.Y), Color.White);
        }
        else
        {
            sb.DrawString(_font, $"Music: {(int)(MusicVolume * 100)}%", new Vector2(playBtn.X, 350), Color.Lime);
            sb.DrawString(_font, $"SFX: {(int)(SfxVolume * 100)}%", new Vector2(playBtn.X, 450), Color.Lime);
            sb.DrawString(_font, "Back", new Vector2(backBtn.X, backBtn.Y), Color.White);
        }
    }
}