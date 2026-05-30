using System.Collections.Generic;

public static class BolumVerisi
{
    // Haritanın başlangıç kısmı. Oyuncu doğma noktalarını (1, 2) and bariyerleri (B) içerir
    public static string[] Baslangic = new string[]
    {
        "ddddddddddddddddddddddddddddddddddddddddddddddddddddd",
        "                                                     ",
        "OOOOOOOOOOOOOOOOOOOOOOO      OOOOOO      OOOOOOOO    ",
        "          B                                          ",
        "          B     KKK                KKK               ",
        "        1 B     KKK                KKK               ",
        "OOOOOOOOOOOO    KKK     OOOOOOOOOOOOOOOOOOOOOOOOOO   ",
        "                                   KKK               ",
        "                           KKK     KKK     KKK       ",
        "                           KKK             KKK       ",
        "OOOOOOOOOOOO    KKK     OOOOOOOOOOOOOOOOOOOOOOOOOO   ",
        "          B     KKK                KKK               ",
        "          B     KKK                KKK               ",
        "        2 B                                          ",
        "OOOOOOOOOOOOOOOOOOOOOO      OOOOOO      OOOOOOOO     ",
        "                                                     ",
        "ddddddddddddddddddddddddddddddddddddddddddddddddddddd",
    };

    // Haritanın bitiş kısmı. Bitiş çizgisi bloklarını (F) içerir
    public static string[] Bitis = new string[]
    {
        "ddddddddddddddddddddddddddddddddddddd",
        "                                     ",
        "    OOOOOOOOOOOOOOKKKKOOOOOOOOOOOOOOO",
        "       d          KKKK     d        F",
        "                  KKKK              F",
        "            d                  d    F",
        "      OOOOOOOOOOOOKKKKOOOOOOOOOOOOOOO",
        "            d     KKKK     d        F",
        "                  KKKK              F",
        "                               d    F",
        "      OOOOOOOOOOOOKKKKOOOOOOOOOOOOOOO",
        "        d         KKKK         d    F",
        "                  KKKK              F",
        "            d              d        F",
        "    OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO",
        "                                     ",
        "ddddddddddddddddddddddddddddddddddddd",
    };

    // Oluşturma sırasında rastgele seçilen farklı engel platform düzenlerinin listesi
    public static List<string[]> OrtaModuller = new List<string[]>()
    {
        new string[]
        {
            "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
            "                                                                                          ",
            "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO",
            "   OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO   ",
            "   OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO     ",
            "               OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO     ",
            "                OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO       ",
            "                           d       d       d       d       d       d        d             ",
            "                                                                                          ",
            "                       d       d       d       d       d       d       d        d         ",
            "                OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO       ",
            "               OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO     ",
            "   OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO     ",
            "   OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO   ",
            "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO",
            "                                                                                          ",
            "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
        },
        new string[]
        {
            "dddddddddddddddddddddddddddddddddddddddddddddddddddddd",
            "                                                      ",
            "    OOOOOOOOOOOOOOOOOOO    OOOOOOO       OOOOOOOO     ",
            "            d                                         ",
            "                KKK                 KKK               ",
            "       d        KKK                 KKK               ",
            "    OOOOOOOO    KKK     OOOOOOOOOOOOOOOOOOOOOOOOOO    ",
            "                                    KKK               ",
            "                             KKK    KKK    KKK        ",
            "                             KKK           KKK        ",
            "    OOOOOOOO    KKK     OOOOOOOOOOOOOOOOOOOOOOOOOO    ",
            "       d        KKK                KKKK               ",
            "                KKK                KKKK               ",
            "           d                                          ",
            "    OOOOOOOOOOOOOOOOOO     OOOOOOO      OOOOOOOO      ",
            "                                                      ",
            "dddddddddddddddddddddddddddddddddddddddddddddddddddddd",
        },
        new string[]
        {
            "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
            "                                                            ",
            "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO",
            "                  d       d       d       d       d         ",
            "                      d       d       d       d             ",
            "                OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO     d   ",
            "   OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO",
            "         KKK     KKK     KKK     KKK     KKK     KKK        ",
            "          K   K   K   K   K   K   K   K   K   K   K   K     ",
            "             KKK     KKK     KKK     KKK     KKK     KKK    ",
            "   OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO",
            "                OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO         ",
            "                      d         d         d                 ",
            "                 d         d         d         d       d    ",
            "OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO",
            "                                                            ",
            "dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd",
        }
    };
}