namespace WVN.Barcodes.Code128
{
    internal sealed class TextEncoding
    {
        public string TextValue { get; internal set; }
        public bool IsSwitch { get; internal set; }
        public Switch SwitchValue { get; internal set; }
        public int Encoding { get; internal set; }

        public TextEncoding(string value, int enc)
        {
            TextValue = value;
            IsSwitch = false;
            Encoding = enc;
        }

        public TextEncoding(Switch value, int enc)
        {
            SwitchValue = value;
            IsSwitch = true;
            Encoding = enc;
        }
    }
}
