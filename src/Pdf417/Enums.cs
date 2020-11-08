namespace WVN.Barcodes.Pdf417
{
    internal enum Switch
    {
        Punctuation = 25,
        Lowercase = 27,
        UppercaseNextOnly = 27,
        Mixed = 28,
        Uppercase = 29,
        PunctuationNextOnly = 29,
    }

    internal enum SubTypes
    {
        Uppercase = 0,
        Lowercase = 1,
        Mixed = 2,
        Punctuation = 3,
    }

    internal enum BarcodeSide
    {
        Left, Right
    }
}
