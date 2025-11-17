using System.Collections.Generic;

public static class PhonemeDictionary
{
    // Easy words phonemes
    public static readonly Dictionary<string, string> easyPhonemes = new()
    {
        {"fan","F AE N"},
        {"food","F UW D"},
        {"fish","F IH SH"},
        {"fairy","F EH R IY"},
        {"very","V EH R IY"},
        {"vote","V OW T"},
        {"van","V AE N"},
        {"this","DH IH S"},
        {"that","DH AE T"},
        {"then","DH EH N"},
        {"thin","TH IH N"},
        {"three","TH R IY"},
        {"zip","Z IH P"},
        {"zoo","Z UW"},
        {"buzz","B AH Z"},
        {"rice","R AY S"},
        {"sun","S AH N"},
        {"moon","M UW N"},
        {"king","K IH NG"},
        {"queen","K W IY N"},
        {"dragon","D R AE G AH N"}
    };

    // Medium words phonemes
    public static readonly Dictionary<string, string> mediumPhonemes = new()
    {
        {"fantasy","F AE N T AH S IY"},
        {"festival","F EH S T IH V AH L"},
        {"fortress","F AO R T R AH S"},
        {"fearful","F IH R F UH L"},
        {"forever","F AO R EH V ER"},
        {"victory","V IH K T AH R IY"},
        {"villain","V IH L AH N"},
        {"voyage","V OY AH JH"},
        {"vanish","V AE N IH SH"},
        {"velvet","V EH L V AH T"},
        {"thunder","TH AH N D ER"},
        {"thousand","TH AW Z AH N D"},
        {"brother","B R AH DH ER"},
        {"mother","M AH DH ER"},
        {"gather","G AE DH ER"},
        {"another","AH N AH DH ER"},
        {"puzzle","P AH Z AH L"},
        {"blizzard","B L IH Z ER D"},
        {"frozen","F R OW Z AH N"},
        {"horizon","HH AH R AY Z AH N"},
        {"amazing","AH M EY Z IH NG"},
        {"discover","D IH S K AH V ER"},
        {"adventure","AH D V EH N CH ER"},
        {"wizard","W IH Z ER D"},
        {"lantern","L AE N T ER N"}
    };

    // Hard words phonemes
    public static readonly Dictionary<string, string> hardPhonemes = new()
    {
        {"responsibility","R IH S P AA N S AH B IH L IH T IY"},
        {"pronunciation","P R AH N AH N S IY EY SH AH N"},
        {"opportunity","AO P ER T UW N IH T IY"},
        {"vocabulary","V AH K AE B Y UH L EH R IY"},
        {"unbelievable","AH N B IH L IY V AH B AH L"},
        {"transformation","T R AE N S F ER M EY SH AH N"},
        {"determination","D IH T ER M AH N EY SH AH N"},
        {"extraordinary","EH K S T R AH AO R D AH N EH R IY"},
        {"electricity","IH L EH K T R IH S IH T IY"},
        {"imagination","IH M AE J IH N EY SH AH N"},
        {"communication","K AH M Y UW N AH K EY SH AH N"},
        {"information","IH N F ER M EY SH AH N"},
        {"celebration","S EH L AH B R EY SH AH N"},
        {"investigation","IH N V EH S T AH G EY SH AH N"},
        {"civilization","S IH V AH L AH Z EY SH AH N"},
        {"federation","F EH D AH R EY SH AH N"},
        {"verification","V EH R AH F AH K EY SH AH N"},
        {"visualization","V IH ZH UH W AH L AH Z EY SH AH N"},
        {"adventurous","AH D V EH N CH ER AH S"},
        {"victorious","V IH K T AO R IY AH S"},
        {"perseverance","P ER S IH V IH R AH N S"},
        {"bewilderment","B IH W IH L D ER M AH N T"},
        {"appreciation","AH P R IY SH IY EY SH AH N"},
        {"exaggeration","IH G Z AE J ER EY SH AH N"},
        {"manifestation","M AE N AH F EH S T EY SH AH N"}
    };
}
