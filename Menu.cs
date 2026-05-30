using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Menu
{
    private readonly SpriteFont fonts;
    public MenuDurumu MevcutDurum = MenuDurumu.Ana;
    public float MuzikSesSeviyesi = 0.5f, EfektSesSeviyesi = 0.8f;
    private readonly Rectangle oynaButonu, ayarlarButonu, cikisButonu, geriButonu;

    // Butonlar için koordinat sınırlarını belirleyen arayüz (UI) menü düzeni yapıcı metodu
    public Menu(SpriteFont font, int w, int h)
    {
        fonts = font;
        int cx = w / 2 - 80; // Yatay merkez çizgisi hizalama noktasını hesaplar
        oynaButonu = new Rectangle(cx, 400, 200, 50);
        ayarlarButonu = new Rectangle(cx, 500, 200, 50);
        cikisButonu = new Rectangle(cx, 600, 200, 50);
        geriButonu = new Rectangle(cx, 600, 200, 50);
    }

    // Metin seçimleri üzerindeki fare tıklama isabetlerini işler ve mantıksal eylemleri yönlendirir
    public string Guncelle(MouseState mouse, MouseState oldMouse)
    {
        bool clicked = mouse.LeftButton == ButtonState.Pressed && oldMouse.LeftButton == ButtonState.Released;

        // Hangi düzen ekranının görünür olduğunu kontrol ederek isabetleri işler
        if (MevcutDurum == MenuDurumu.Ana)
        {
            if (clicked && oynaButonu.Contains(mouse.Position)) return "PLAY";
            if (clicked && ayarlarButonu.Contains(mouse.Position)) MevcutDurum = MenuDurumu.Ayarlar;
            if (clicked && cikisButonu.Contains(mouse.Position)) return "EXIT";
        }
        else
        {
            if (clicked && geriButonu.Contains(mouse.Position)) MevcutDurum = MenuDurumu.Ana;
            
            // Eğer kaydırıcılara tıklanırsa, seviyeyi %10 artırır; sınır değerleri aşılırsa tekrar 0'a döndürür
            if (clicked && new Rectangle(oynaButonu.X, 350, 250, 50).Contains(mouse.Position)) MuzikSesSeviyesi = (MuzikSesSeviyesi + 0.1f) % 1.1f;
            if (clicked && new Rectangle(oynaButonu.X, 450, 250, 50).Contains(mouse.Position)) EfektSesSeviyesi = (EfektSesSeviyesi + 0.1f) % 1.1f;
        }
        return "NONE";
    }

    // Ekranın merkez konumunda arayüz (UI) metin varlıklarını görüntüler
    public void Ciz(SpriteBatch sb, int w, int h)
    {
        sb.DrawString(fonts, "DUO FLIP", new Vector2(w / 2 - 100, 200), Color.Black);

        if (MevcutDurum == MenuDurumu.Ana)
        {
            sb.DrawString(fonts, "Play Game", new Vector2(oynaButonu.X, oynaButonu.Y), Color.Black);
            sb.DrawString(fonts, "Settings", new Vector2(ayarlarButonu.X, ayarlarButonu.Y), Color.Black);
            sb.DrawString(fonts, "Exit", new Vector2(cikisButonu.X, cikisButonu.Y), Color.Black);
        }
        else
        {
            sb.DrawString(fonts, $"Music: {(int)(MuzikSesSeviyesi * 100)}%", new Vector2(oynaButonu.X, 350), Color.Lime);
            sb.DrawString(fonts, $"SFX: {(int)(EfektSesSeviyesi * 100)}%", new Vector2(oynaButonu.X, 450), Color.Lime);
            sb.DrawString(fonts, "Back", new Vector2(geriButonu.X, geriButonu.Y), Color.Black);
        }
    }
}