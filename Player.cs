using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public class Oyuncu
{
    public Vector2 pozisyon;
    public Vector2 hizVektoru;
    public Texture2D karakterResmi;
    public int can = 100;
    public bool hayattaMi = true;
    private readonly Keys yercekimiTusu; 
    private float yercekimiYonu = 1f;
    private float zamanSayaci = OyunAyarlari.BeklemeSuresi; 
    public Oyuncu(Texture2D resim, Vector2 pos, Keys tus)
    {
        karakterResmi = resim;
        pozisyon = pos;
        yercekimiTusu = tus;
    }

    // Fizik ve çarpışmaları hesaplamak için her karede çağrılan ana güncelleme (Update) metot
    public void Guncelle(GameTime oyunSuresi, int h, List<Rectangle> dikenler, List<Rectangle> bloklar, Rectangle digerOyuncuKutusu)
    {
        // Eğer oyuncu ölüyse, tüm hareket ve çarpışma kodlarını atlar
        if (!hayattaMi) return;

        // Son kareden bu yana geçen delta zamanını alır
        float dt = (float)oyunSuresi.ElapsedGameTime.TotalSeconds;
        zamanSayaci += dt;

        // Girdi olup olmadığını kontrol eder ve yer çekimini tersine çevirmeden önce bekleme süresinin bittiğinden emin olur
        if (Keyboard.GetState().IsKeyDown(yercekimiTusu) && zamanSayaci >= OyunAyarlari.BeklemeSuresi)
        {
            yercekimiYonu *= -1f;  // Yer çekimi yönünü tersine çevir
            hizVektoru.Y = 0;      // Temiz bir düşüş geçişi için dikey hızı sıfırlama
            zamanSayaci = 0f;      // Bekleme süresi zamanlayıcısını sıfırlama
            Kaynaklar.ZiplamaSesi?.Play(); // Takla/Zıplama ses efektini güvenli bir şekilde çalma
        }

        int size = OyunAyarlari.BlokBoyutu;
        
        // Sabit blokları ve diğer oyuncuyu birleşik çarpışma kontrolleri için tek bir listede birleştirir
        var hedefler = new List<Rectangle>(bloklar) { digerOyuncuKutusu };

        // YATAY (X) ÇARPIŞMA TESPİTÇİSİ
        var xKutusu = new Rectangle((int)pozisyon.X, (int)pozisyon.Y, size, size);
        foreach (var n in hedefler)
        {
            // Eğer öndeki bir engelle çarpışıyorsa, oyuncuyu bloğun sol kenarına geri iter
            if (xKutusu.Intersects(n) && n.Center.X > xKutusu.Center.X)
            {
                pozisyon.X = n.Left - size;
                xKutusu.X = (int)pozisyon.X; 
            }
        }

        // DİKEY (Y) FİZİK VE ÇARPIŞMA TESPİTÇİSİ
        hizVektoru.Y += OyunAyarlari.Yercekimi * yercekimiYonu * dt; // Zamanla yer çekimi kuvveti uygula
        pozisyon.Y += hizVektoru.Y * dt; // Oyuncuyu Y ekseninde hareket ettir

        var yKutusu = new Rectangle((int)pozisyon.X, (int)pozisyon.Y, size, size);
        foreach (var n in hedefler)
        {
            // Kutular kesişmiyorsa kontrolü atla
            if (!yKutusu.Intersects(n)) continue;
            
            // Eğer normal yer çekimiyse ve katı bir bloğun üstüne iniliyorsa
            if (yercekimiYonu > 0 && hizVektoru.Y >= 0 && yKutusu.Bottom > n.Top && yKutusu.Top < n.Top)
            {
                pozisyon.Y = n.Top - size; // Ayakları bloğun üstüne sabitle
                hizVektoru.Y = 0f; // Düşme kuvvetini durdur
                yKutusu.Y = (int)pozisyon.Y;
            }
            // Eğer ters yer çekimiyse ve katı bir bloğun tavan tabanına çarpılıyorsa
            else if (yercekimiYonu < 0 && hizVektoru.Y <= 0 && yKutusu.Top < n.Bottom && yKutusu.Bottom > n.Bottom)
            {
                pozisyon.Y = n.Bottom; // Kafayı bloğun altına sabitle
                hizVektoru.Y = 0f; // Yükselme kuvvetini durdur
                yKutusu.Y = (int)pozisyon.Y;
            }
        }

        // DİKEN (ÖLÜM) ÇARPIŞMA TESPİTÇİSİ
        var sonKutu = new Rectangle((int)pozisyon.X, (int)pozisyon.Y, size, size);
        foreach (var d in dikenler)
        {
            // Eğer kesişme gerçekleşirse, oyuncuyu anında öldür
            if (sonKutu.Intersects(d))
            {
                can = 0;
                hayattaMi = false;
                break; // Diğer dikenleri kontrol etmeyi durdur
            }
        }
    }

    // Oyuncu dokusunu ekrana kare kare çizme metodu
    public void Ciz(SpriteBatch cizici, Color renk)
    {
        // Eğer ölüyse ekrana çizme (Render etme)
        if (!hayattaMi) return;
        
        // Yer çekimi ters çevrilmişse dokuyu dikey olarak ters çevir
        var fx = yercekimiYonu < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
        
        // Pixel Art görünümünü korumak için dokuyu 4 kat büyüterek çizer
        cizici.Draw(karakterResmi, pozisyon, null, renk, 0f, Vector2.Zero, 4f, fx, 0f);
    }
}