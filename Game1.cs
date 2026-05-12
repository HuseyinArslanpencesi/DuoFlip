using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace Hoppala;

public enum GameState { Menu, Countdown, Playing } 

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
    Texture2D bgResmi;
    Texture2D blokResmi;
    List<Rectangle> dikenListesi = new List<Rectangle>();
    List<Rectangle> blokListesi = new List<Rectangle>();
    
    Song arkaPlanMuzigi;
    SoundEffect jumpSound;

    SpriteFont font;
    float countdownTimer = 0f;
    
    float kaydirmaHizi = 150f;
    float kaydirmaBirikimi = 0f;

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

        int blokBoyutu = 16;
        int altZeminY = yuk - 120; 
        int ustZeminY = 120;       

        for (int x = 16; x < gen - 16; x += blokBoyutu)
        {
            blokListesi.Add(new Rectangle(x, altZeminY, blokBoyutu, blokBoyutu));
            blokListesi.Add(new Rectangle(x, ustZeminY, blokBoyutu, blokBoyutu));
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        
        font = Content.Load<SpriteFont>("font");
        menu = new Menu(font, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

        Texture2D pTex = Content.Load<Texture2D>("mono_player");
        dikenResmi = Content.Load<Texture2D>("diken");
        bgResmi = Content.Load<Texture2D>("mono_bg");
        blokResmi = Content.Load<Texture2D>("mono_block");

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
            if (action == "PLAY") 
            {
                currentState = GameState.Countdown;
                countdownTimer = 4f; 
            }
            if (action == "EXIT") Exit();
        }
        else if (currentState == GameState.Countdown)
        {
            countdownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (countdownTimer <= 0)
            {
                currentState = GameState.Playing; 
            }

            if (keys.IsKeyDown(Keys.Escape)) currentState = GameState.Menu;
        }
        else if (currentState == GameState.Playing)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            kaydirmaBirikimi += kaydirmaHizi * dt;
            int artis = (int)kaydirmaBirikimi;
            
            if (artis > 0)
            {
                kaydirmaBirikimi -= artis;
                
                for (int i = 0; i < blokListesi.Count; i++)
                {
                    Rectangle b = blokListesi[i];
                    b.X -= artis;
                    blokListesi[i] = b;
                }
            }

            int h = graphics.PreferredBackBufferHeight;
            p1.Guncelle(gameTime, h, dikenListesi, blokListesi);
            p2.Guncelle(gameTime, h, dikenListesi, blokListesi);

            if (keys.IsKeyDown(Keys.D) && oldKeys.IsKeyUp(Keys.D)) jumpSound.Play(menu.SfxVolume, 0f, 0f);
            if (keys.IsKeyDown(Keys.K) && oldKeys.IsKeyUp(Keys.K)) jumpSound.Play(menu.SfxVolume, 0f, 0f);

            if (keys.IsKeyDown(Keys.Escape)) currentState = GameState.Menu;
        }

        oldMouse = mouse;
        oldKeys = keys;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        int w = graphics.PreferredBackBufferWidth;
        int h = graphics.PreferredBackBufferHeight;

        spriteBatch.Draw(bgResmi, new Rectangle(0, 0, w, h), Color.White);
        
        if (currentState == GameState.Menu)
        {
            menu.Draw(spriteBatch, w, h);
        }
        else
        {
            foreach (var b in blokListesi)
            {
                spriteBatch.Draw(blokResmi, b, Color.White);
            }

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

            if (currentState == GameState.Countdown)
            {
                string txt = "";
                if (countdownTimer > 3) txt = "3";
                else if (countdownTimer > 2) txt = "2";
                else if (countdownTimer > 1) txt = "1";
                else if (countdownTimer > 0) txt = "GO!";
                spriteBatch.DrawString(font, txt, new Vector2(w / 2 - 20, 200), Color.Yellow);
            }
        }

        spriteBatch.End();
        base.Draw(gameTime);
    }
}