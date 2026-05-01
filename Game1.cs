using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Hoppala;

public enum GameState { Menu, Playing }

public class Game1 : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;

    GameState currentState = GameState.Menu;
    Menu menu;
    MouseState oldMouse;
    KeyboardState oldKeys;

    Player p1, p2;
    Texture2D dikenResmi;
    List<Rectangle> dikenListesi = new List<Rectangle>();
    
    Song arkaPlanMuzigi;
    SoundEffect jumpSound;

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        graphics.HardwareModeSwitch = false;
        graphics.IsFullScreen = true;
        graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        int gen = graphics.PreferredBackBufferWidth;
        int yuk = graphics.PreferredBackBufferHeight;

        for (int x = 0; x < gen; x += 16)
        {
            dikenListesi.Add(new Rectangle(x, 0, 16, 16));
            dikenListesi.Add(new Rectangle(x, yuk - 16, 16, 16));
        }
        for (int y = 16; y < yuk - 16; y += 16)
        {
            dikenListesi.Add(new Rectangle(0, y, 16, 16));
            dikenListesi.Add(new Rectangle(gen - 16, y, 16, 16));
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        
        SpriteFont font = Content.Load<SpriteFont>("font");
        menu = new Menu(font, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

        Texture2D pTex = Content.Load<Texture2D>("mono_player");
        dikenResmi = Content.Load<Texture2D>("diken");

        arkaPlanMuzigi = Content.Load<Song>("MonoMusic");
        jumpSound = Content.Load<SoundEffect>("Jump");
        
        MediaPlayer.Play(arkaPlanMuzigi);
        MediaPlayer.IsRepeating = true;

        int w = graphics.PreferredBackBufferWidth;
        int h = graphics.PreferredBackBufferHeight;

        p1 = new Player(pTex, new Vector2(w / 3f, h / 2f), Keys.D);
        p2 = new Player(pTex, new Vector2(w / 1.5f, h / 2f), Keys.K);
    }

    protected override void Update(GameTime gameTime)
    {
        MouseState mouse = Mouse.GetState();
        KeyboardState keys = Keyboard.GetState();
        
        MediaPlayer.Volume = menu.MusicVolume;

        if (currentState == GameState.Menu)
        {
            string action = menu.Update(mouse, oldMouse);
            if (action == "PLAY") currentState = GameState.Playing;
            if (action == "EXIT") Exit();
        }
        else
        {
            int h = graphics.PreferredBackBufferHeight;
            p1.Guncelle(gameTime, h, dikenListesi);
            p2.Guncelle(gameTime, h, dikenListesi);

            if (keys.IsKeyDown(Keys.D) && oldKeys.IsKeyUp(Keys.D))
            {
                jumpSound.Play(menu.SfxVolume, 0f, 0f);
            }
            if (keys.IsKeyDown(Keys.K) && oldKeys.IsKeyUp(Keys.K))
            {
                jumpSound.Play(menu.SfxVolume, 0f, 0f);
            }

            if (keys.IsKeyDown(Keys.Escape)) currentState = GameState.Menu;
        }

        oldMouse = mouse;
        oldKeys = keys;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        if (currentState == GameState.Menu)
        {
            menu.Draw(spriteBatch, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }
        else
        {
            int w = graphics.PreferredBackBufferWidth;
            int h = graphics.PreferredBackBufferHeight;

            foreach (var d in dikenListesi)
            {
                SpriteEffects efekt = SpriteEffects.None;
                float rotasyon = 0f;
                Vector2 origin = Vector2.Zero;

                if (d.Y == 0) efekt = SpriteEffects.FlipVertically;
                else if (d.X == 0) { rotasyon = MathHelper.ToRadians(90); origin = new Vector2(0, 16); }
                else if (d.X >= w - 16) { rotasyon = MathHelper.ToRadians(-90); origin = new Vector2(16, 0); }

                spriteBatch.Draw(dikenResmi, d, null, Color.White, rotasyon, origin, efekt, 0f);
            }

            p1.Ciz(spriteBatch, Color.White);
            p2.Ciz(spriteBatch, Color.Cyan);
        }

        spriteBatch.End();
        base.Draw(gameTime);
    }
}