using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

public class Harita
{
    public string[] SeviyeTasarimi; 
    public List<Rectangle> blokListesi = new();
    public List<Rectangle> engelListesi = new(); 
    public List<Rectangle> bariyerListesi = new(); 
    public List<Rectangle> dikenListesi = new(); 
    public List<Rectangle> bitisListesi = new(); 
    public Vector2 o1Baslangic, o2Baslangic;
    public int parkurUzunlugu;
    private float kaydirmaBirikimi;
    private readonly List<Rectangle> katiCisimlerTumu = new();

    public void HaritayiBirlestir()
    {
        var rnd = new Random();
        int rows = BolumVerisi.Baslangic.Length;
        SeviyeTasarimi = new string[rows]; 

        // Rastgele seçilen 3 orta modülü saklamak için dizi
        var secilenOrtalar = new string[3][];
        for (int i = 0; i < 3; i++)
            secilenOrtalar[i] = BolumVerisi.OrtaModuller[rnd.Next(BolumVerisi.OrtaModuller.Count)];

        // Metin karakterlerini yan yana birleştirmek için satır satır dönen döngü
        for (int s = 0; s < rows; s++)
        {
            string satir = BolumVerisi.Baslangic[s]; // Başlangıç alanı satırını ekler
            for (int p = 0; p < 3; p++) satir += secilenOrtalar[p][s]; // Orta engel satırlarını ekler
            SeviyeTasarimi[s] = satir + BolumVerisi.Bitis[s]; // Bitiş bölgesi satırını sonuna ekler
        }
    }

    // Oluşturulan metin düzeninden fiziksel dikdörtgen çarpışma sınırları oluşturur
    public void Kurulum(int gen, int yuk)
    {
        HaritayiBirlestir(); // İlk olarak harita düzenini prosedürel olarak oluşturur
        
        // Durumu güvenli bir şekilde sıfırlamak için eski listeleri temizler
        blokListesi.Clear(); engelListesi.Clear(); bariyerListesi.Clear(); dikenListesi.Clear(); bitisListesi.Clear();
        kaydirmaBirikimi = 0f;

        int size = OyunAyarlari.BlokBoyutu; 
        int hGrid = SeviyeTasarimi.Length * size;
        
        // Tüm kare ızgarayı 1080p ekranlarda dikey olarak ortalamak için Y kaydırma formülü
        int offsetY = (yuk - hGrid) / 2;
        parkurUzunlugu = SeviyeTasarimi[0].Length * size;

        // Metin ızgara dizisinin satır ve sütunları arasında döner
        for (int r = 0; r < SeviyeTasarimi.Length; r++)
        {
            for (int c = 0; c < SeviyeTasarimi[r].Length; c++)
            {
                char ch = SeviyeTasarimi[r][c];
                int x = c * size, y = offsetY + (r * size);

                // Harfleri işlevsel geometri sınıflarına dönüştürür
                if (ch == 'O') blokListesi.Add(new Rectangle(x, y, size, size));
                else if (ch == 'K' || ch == 'k') engelListesi.Add(new Rectangle(x, y, size, size));
                else if (ch == 'B') bariyerListesi.Add(new Rectangle(x, y, size, size));
                else if (ch == '1') o1Baslangic = new Vector2(x, y);
                else if (ch == '2') o2Baslangic = new Vector2(x, y);
                else if (ch == 'd') dikenListesi.Add(new Rectangle(x, y, size, size)); 
                else if (ch == 'F') bitisListesi.Add(new Rectangle(x, y, size, size)); 
            }
        }
    }

    // Koşucuların ileri doğru hareketini simüle etmek için aktif haritanın her öğesini sola kaydırır
    public void Kaydir(float dt)
    {
        kaydirmaBirikimi += OyunAyarlari.KaydirmaHizi * dt;
        int shift = (int)kaydirmaBirikimi;
        if (shift <= 0) return; // Biriken mesafe 1 pikselin altındaysa kaydırmayı atlar

        kaydirmaBirikimi -= shift; // İşlenen tam sayı piksel miktarını düşer
        
        // Tüm nesne dikdörtgenlerini sola doğru iter
        ListeyiKaydir(blokListesi, shift);
        ListeyiKaydir(engelListesi, shift);
        ListeyiKaydir(bariyerListesi, shift);
        ListeyiKaydir(dikenListesi, shift);
        ListeyiKaydir(bitisListesi, shift);
    }

    // Liste içindeki X pozisyonlarını doğrudan güncellemek için yardımcı metot
    private static void ListeyiKaydir(List<Rectangle> liste, int miktar)
    {
        for (int i = 0; i < liste.Count; i++) { var r = liste[i]; r.X -= miktar; liste[i] = r; }
    }

    // Katı öğelerin performans optimizasyonlu birleştirilmiş bir ana koleksiyonunu döndürür
    public List<Rectangle> GetKatiCisimler(OyunDurumu durum)
    {
        katiCisimlerTumu.Clear(); // Bellek şişmesini önlemek için tamponu temizler
        katiCisimlerTumu.AddRange(blokListesi);
        katiCisimlerTumu.AddRange(engelListesi);
        
        // Bariyer duvar çizgilerini yalnızca oyun henüz yüklenirken/hazırlanırken ekler
        if (durum is OyunDurumu.GeriSayim or OyunDurumu.Menu) katiCisimlerTumu.AddRange(bariyerListesi);
        return katiCisimlerTumu;
    }

    // Arka plan dokusunu ve tüm yapısal öğeleri ekranda işler/çizer
    public void Ciz(SpriteBatch sb, int gen, int yuk, OyunDurumu durum)
    {
        // Arka planı her zaman tüm pencere ekran genişliği ve yüksekliği boyunca çizer
        sb.Draw(Kaynaklar.ArkaPlanResmi, new Rectangle(0, 0, gen, yuk), Color.White);
        if (durum is OyunDurumu.Menu or OyunDurumu.Kazandi) return; // Belirtilen ekranlarda oynanış varlıklarını gizler

        // Temel platformları ve ölüm duvarlarını çizer
        foreach (var b in blokListesi) sb.Draw(Kaynaklar.BlokResmi, b, Color.White);
        foreach (var e in engelListesi) sb.Draw(Kaynaklar.BlokResmi, e, Color.Red);
        if (durum == OyunDurumu.GeriSayim)
            foreach (var ba in bariyerListesi) sb.Draw(Kaynaklar.BlokResmi, ba, Color.SaddleBrown);

        // Özel siyah bitiş çizgisi bloklarını çizer
        foreach (var f in bitisListesi) sb.Draw(Kaynaklar.BlokResmi, f, Color.Black); 

        // Ekranın üst yarısına yakın konumlandırılmışsa, diken tehlikelerini dikey olarak ters çevirerek çizer
        foreach (var d in dikenListesi)
        {
            var fx = d.Y < yuk / 2 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            sb.Draw(Kaynaklar.DikenResmi, d, null, Color.White, 0f, Vector2.Zero, fx, 0f); 
        }
    }
}