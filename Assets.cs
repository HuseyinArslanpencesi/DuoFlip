using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

public static class Kaynaklar
{
    public static Texture2D OyuncuResmi, DikenResmi, ArkaPlanResmi, BlokResmi;
    public static SpriteFont YaziTipi;
    public static Song ArkaPlanMuzigi;
    public static SoundEffect ZiplamaSesi;
    
    // Tüm oyun varlıklarını İçerik (Content) yöneticisinden yükleyen metot
    public static void Yukle(ContentManager icerik)
    {
        OyuncuResmi = icerik.Load<Texture2D>("mono_player");
        DikenResmi = icerik.Load<Texture2D>("diken");
        ArkaPlanResmi = icerik.Load<Texture2D>("mono_bg");
        BlokResmi = icerik.Load<Texture2D>("mono_block");
        YaziTipi = icerik.Load<SpriteFont>("font");
        ArkaPlanMuzigi = icerik.Load<Song>("MonoMusic");
        ZiplamaSesi = icerik.Load<SoundEffect>("Jump");
    }
}