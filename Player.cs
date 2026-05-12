using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Hoppala;

public class Player
{
    public Vector2 pozisyon;
    public Vector2 hizVektoru;
    public Texture2D karakterResmi;
    
    public int can = 100;
    public bool hayattaMi = true;

    private Keys _yercekimiTusu; 
    private float _yercekimiGucu = 2000f;
    private float _yercekimiYonu = 1f;

    private float _beklemeSuresi = 0.3f; 
    private float _zamanSayaci = 0.3f; 

    public Player(Texture2D gelenResim, Vector2 gelenPozisyon, Keys gelenTus)
    {
        karakterResmi = gelenResim;
        pozisyon = gelenPozisyon;
        _yercekimiTusu = gelenTus;
    }

    public void Guncelle(GameTime oyunSuresi, int ekranYuksekligi, List<Rectangle> dikenler, List<Rectangle> bloklar)
    {
        if (!hayattaMi) return;

        float deltaZaman = (float)oyunSuresi.ElapsedGameTime.TotalSeconds;
        var tuslar = Keyboard.GetState();

        _zamanSayaci += deltaZaman;

        if (tuslar.IsKeyDown(_yercekimiTusu) && _zamanSayaci >= _beklemeSuresi)
        {
            _yercekimiYonu *= -1f; 
            hizVektoru.Y = 0;      
            _zamanSayaci = 0f;     
        }

        hizVektoru.Y += (_yercekimiGucu * _yercekimiYonu) * deltaZaman;
        pozisyon.Y += hizVektoru.Y * deltaZaman;

        Rectangle oyuncuKutusu = new Rectangle((int)pozisyon.X, (int)pozisyon.Y, karakterResmi.Width, karakterResmi.Height);

        foreach (var blok in bloklar)
        {
            if (oyuncuKutusu.Intersects(blok))
            {
                if (_yercekimiYonu > 0 && hizVektoru.Y > 0) 
                {
                    pozisyon.Y = blok.Top - karakterResmi.Height;
                    hizVektoru.Y = 0f;
                }
                else if (_yercekimiYonu < 0 && hizVektoru.Y < 0) 
                {
                    pozisyon.Y = blok.Bottom;
                    hizVektoru.Y = 0f;
                }
                
                oyuncuKutusu.Y = (int)pozisyon.Y;
            }
        }

        if (pozisyon.Y > ekranYuksekligi - karakterResmi.Height) pozisyon.Y = ekranYuksekligi - karakterResmi.Height;
        if (pozisyon.Y < 0) pozisyon.Y = 0;

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

    public void Ciz(SpriteBatch cizici, Color renk)
    {
        if (!hayattaMi) return;

        SpriteEffects efekt = _yercekimiYonu < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
        cizici.Draw(karakterResmi, pozisyon, null, renk, 0f, Vector2.Zero, 1f, efekt, 0f);
    }
}