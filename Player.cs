using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Hoppala;

public class Player
{
    // --- PROPERTIES & VARIABLES ---
    public Vector2 pozisyon;
    public Vector2 hizVektoru;
    public Texture2D karakterResmi;
    
    // Player Health & Status
    public int can = 100;
    public bool hayattaMi = true;

    // Movement & Gravity Settings
    private Keys _yercekimiTusu; 
    private float _yercekimiGucu = 2000f;
    private float _yercekimiYonu = 1f; // 1 = Down, -1 = Up

    // Cooldown Mechanism (0.3 seconds)
    private float _beklemeSuresi = 0.3f; 
    private float _zamanSayaci = 0.3f; 

    // --- CONSTRUCTOR ---
    public Player(Texture2D gelenResim, Vector2 gelenPozisyon, Keys gelenTus)
    {
        karakterResmi = gelenResim;
        pozisyon = gelenPozisyon;
        _yercekimiTusu = gelenTus;
    }

    // --- UPDATE LOGIC ---
    public void Guncelle(GameTime oyunSuresi, int ekranYuksekligi, List<Rectangle> dikenler)
    {
        if (!hayattaMi) return;

        float deltaZaman = (float)oyunSuresi.ElapsedGameTime.TotalSeconds;
        var tuslar = Keyboard.GetState();

        _zamanSayaci += deltaZaman;

        // Gravity Flip Input with Cooldown
        if (tuslar.IsKeyDown(_yercekimiTusu) && _zamanSayaci >= _beklemeSuresi)
        {
            _yercekimiYonu *= -1f; 
            hizVektoru.Y = 0;      
            _zamanSayaci = 0f;     
        }

        // Apply Physics
        hizVektoru.Y += (_yercekimiGucu * _yercekimiYonu) * deltaZaman;
        pozisyon.Y += hizVektoru.Y * deltaZaman;


        // Screen Boundaries (Fallback)
        if (pozisyon.Y > ekranYuksekligi - karakterResmi.Height) pozisyon.Y = ekranYuksekligi - karakterResmi.Height;
        if (pozisyon.Y < 0) pozisyon.Y = 0;

        // Spike Collision Detection (Death Trigger)
        Rectangle oyuncuKutusu = new Rectangle((int)pozisyon.X, (int)pozisyon.Y, karakterResmi.Width, karakterResmi.Height);

        
        foreach (var diken in dikenler)
        {
            if (oyuncuKutusu.Intersects(diken))
            {
                can -= 100;
                if (can <= 0) 
                {
                    can = 0;
                    hayattaMi = false;
                }
            }
        }
    }

    // --- DRAW LOGIC ---
    public void Ciz(SpriteBatch cizici, Color renk)
    {
        if (!hayattaMi) return;

        // Visual Feedback: Flip vertically when falling upwards
        SpriteEffects efekt = _yercekimiYonu < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
        cizici.Draw(karakterResmi, pozisyon, null, renk, 0f, Vector2.Zero, 1f, efekt, 0f);
    }
}