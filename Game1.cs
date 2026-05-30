using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

public class Game1 : Game
{   
    private readonly GraphicsDeviceManager grafikYoneticisi;
    private SpriteBatch spriteBatch;
    private OyunDurumu mevcutOyunDurumu = OyunDurumu.Menu;
    private Menu menu;
    private Harita oyunHaritasi; 
    private MouseState eskiFareDurumu;
    private Oyuncu o1, o2;
    private float geriSayimSayaci;
    private int kazananOyuncuNumarasi; 

    // Ekranı kilitli bir ekran boyutu sınırına sabitleyen yapıcı metot (Constructor)
    public Game1()
    {
        grafikYoneticisi = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        // Grafiklerin laptoplarda bozulmasını önlemek için kilitliyoruz
        grafikYoneticisi.PreferredBackBufferWidth = OyunAyarlari.Genislik;
        grafikYoneticisi.PreferredBackBufferHeight = OyunAyarlari.Yukseklik;
        grafikYoneticisi.HardwareModeSwitch = false;
        grafikYoneticisi.IsFullScreen = true;
        grafikYoneticisi.ApplyChanges();
    }
    protected override void Initialize()
    {
        oyunHaritasi = new Harita(); 
        base.Initialize();
    }

    // Temiz bir oyun döngüsü tekrarı için pozisyonları sıfırlar ve bölüm yapılarını temizler
    private void SeviyeyiSifirla()
    {
        kazananOyuncuNumarasi = 0;
        oyunHaritasi.Kurulum(OyunAyarlari.Genislik, OyunAyarlari.Yukseklik); 
        o1 = new Oyuncu(Kaynaklar.OyuncuResmi, oyunHaritasi.o1Baslangic, Keys.D); 
        o2 = new Oyuncu(Kaynaklar.OyuncuResmi, oyunHaritasi.o2Baslangic, Keys.K); 
    }
    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        Kaynaklar.Yukle(Content);
        menu = new Menu(Kaynaklar.YaziTipi, OyunAyarlari.Genislik, OyunAyarlari.Yukseklik);
        SeviyeyiSifirla(); 

        // Arka planda sürekli dönecek olan ortam müziğini başlatma
        if (Kaynaklar.ArkaPlanMuzigi != null)
        {
            MediaPlayer.IsRepeating = true; 
            MediaPlayer.Play(Kaynaklar.ArkaPlanMuzigi); 
        }
    }

    protected override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();
        var keys = Keyboard.GetState();
        
        // Arayüz (UI) ayar alanlarını anında doğrudan küresel ses donanımı motorlarına bağlar
        MediaPlayer.Volume = menu.MuzikSesSeviyesi;          
        SoundEffect.MasterVolume = menu.EfektSesSeviyesi;      

        if (mevcutOyunDurumu == OyunDurumu.Kazandi)
        {
            if (keys.IsKeyDown(Keys.Space)) mevcutOyunDurumu = OyunDurumu.Menu; 
        }
        else if (mevcutOyunDurumu == OyunDurumu.Menu)
        {
            string act = menu.Guncelle(mouse, eskiFareDurumu);
            if (act == "PLAY") { SeviyeyiSifirla(); mevcutOyunDurumu = OyunDurumu.GeriSayim; geriSayimSayaci = 4f; }
            if (act == "EXIT") Exit();
        }
        else OynanisiGuncelle(gameTime, keys);
        eskiFareDurumu = mouse;
        base.Update(gameTime);
    }

    private void OynanisiGuncelle(GameTime gameTime, KeyboardState keys)
    {
        // Escape kısayolu doğrudan ana menü görünümüne geri döner
        if (keys.IsKeyDown(Keys.Escape)) { mevcutOyunDurumu = OyunDurumu.Menu; return; }

        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        int size = OyunAyarlari.BlokBoyutu;

        // Nesneleri yalnızca geri sayım bittiğinde ve tur tamamen başladığında sola kaydırır
        if (mevcutOyunDurumu == OyunDurumu.Oynaniyor) oyunHaritasi.Kaydir(dt);

        var p1Box = new Rectangle((int)o1.pozisyon.X, (int)o1.pozisyon.Y, size, size);
        var p2Box = new Rectangle((int)o2.pozisyon.X, (int)o2.pozisyon.Y, size, size);

        // Her iki oyuncu için de aktif fizik kare vektörlerini işler
        o1.Guncelle(gameTime, OyunAyarlari.Yukseklik, oyunHaritasi.dikenListesi, oyunHaritasi.GetKatiCisimler(mevcutOyunDurumu), p2Box);
        o2.Guncelle(gameTime, OyunAyarlari.Yukseklik, oyunHaritasi.dikenListesi, oyunHaritasi.GetKatiCisimler(mevcutOyunDurumu), p1Box);

        // Eğer bekleme/ısınma sayım aşamasındaysa, geçen süreyi düşer ve kamera kaymasını kilitler
        if (mevcutOyunDurumu == OyunDurumu.GeriSayim)
        {
            geriSayimSayaci -= dt;
            if (geriSayimSayaci <= 0) mevcutOyunDurumu = OyunDurumu.Oynaniyor;
            return;
        }

        // ELENME KURALI 1: Eğer oyuncu ekranın sol sınırının dışına itilirse, ölümü tetikler
        if (o1.hayattaMi && o1.pozisyon.X + size < 0) { o1.can = 0; o1.hayattaMi = false; }
        if (o2.hayattaMi && o2.pozisyon.X + size < 0) { o2.can = 0; o2.hayattaMi = false; }

        // STANDART KAZANAN TESPİTİ: Bitiş çizgisi kare düğümleri üzerindeki kesişmeleri kontrol eder
        foreach (var f in oyunHaritasi.bitisListesi)
        {
            if (o1.hayattaMi && p1Box.Intersects(f)) { kazananOyuncuNumarasi = 1; mevcutOyunDurumu = OyunDurumu.Kazandi; }
            if (o2.hayattaMi && p2Box.Intersects(f)) { kazananOyuncuNumarasi = 2; mevcutOyunDurumu = OyunDurumu.Kazandi; }
        }

        // ELENME KURALI 2 (HAYATTA KALMA MODU): Biri öldüğünde, hayatta kalan diğer oyuncu anında kazanır
        if (!o1.hayattaMi && o2.hayattaMi) { kazananOyuncuNumarasi = 2; mevcutOyunDurumu = OyunDurumu.Kazandi; }
        else if (!o2.hayattaMi && o1.hayattaMi) { kazananOyuncuNumarasi = 1; mevcutOyunDurumu = OyunDurumu.Kazandi; }
        else if (!o1.hayattaMi && !o2.hayattaMi) mevcutOyunDurumu = OyunDurumu.Menu; // Her ikisinin de aynı karede ölmesi durumunda güvenli geri dönüş seçeneği
    }

    // Renkli pikselleri ekrana çizmek için güncellemeden hemen sonra çalışan çizim (Draw) metodu
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        // PointClamp filtresi, grafik nesnelerini büyütürken retro sert piksel kenarı stilini korur
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        int w = OyunAyarlari.Genislik, h = OyunAyarlari.Yukseklik;
        oyunHaritasi.Ciz(spriteBatch, w, h, mevcutOyunDurumu);

        // Motor durum işaretçilerine göre ekrana metin kartları yazdıran koşullu blok
        if (mevcutOyunDurumu == OyunDurumu.Kazandi)
        {
            string txt = $"OYUNCU {kazananOyuncuNumarasi} KAZANDI!";
            Vector2 size = Kaynaklar.YaziTipi.MeasureString(txt);
            spriteBatch.DrawString(Kaynaklar.YaziTipi, txt, new Vector2((w - size.X) / 2, (h - size.Y) / 2), Color.Gold);
            
            string txt2 = "SIFIRLAMAK ICIN SPACE'E BAS";
            Vector2 size2 = Kaynaklar.YaziTipi.MeasureString(txt2);
            spriteBatch.DrawString(Kaynaklar.YaziTipi, txt2, new Vector2((w - size2.X) / 2, (h - size2.Y) / 2 + 50), Color.White);
        }
        else if (mevcutOyunDurumu == OyunDurumu.Menu) menu.Ciz(spriteBatch, w, h);
        else
        {
            o1.Ciz(spriteBatch, Color.White);
            o2.Ciz(spriteBatch, Color.Cyan);

            // Başlangıç zamanlayıcıları sırasında ekranda yüzen merkez sayım numaralarını gösterir
            if (mevcutOyunDurumu == OyunDurumu.GeriSayim)
            {
                string txt = geriSayimSayaci > 3 ? "3" : geriSayimSayaci > 2 ? "2" : geriSayimSayaci > 1 ? "1" : geriSayimSayaci > 0 ? "GO!" : "";
                spriteBatch.DrawString(Kaynaklar.YaziTipi, txt, new Vector2(w / 2 - 20, 200), Color.Yellow);
            }
        }

        spriteBatch.End();
        base.Draw(gameTime);
    }
}