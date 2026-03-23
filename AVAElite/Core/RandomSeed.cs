namespace AVAElite.Core;

public struct EliteSeed
{
    public ushort W0, W1, W2;

    public EliteSeed(ushort w0, ushort w1, ushort w2)
    {
        W0 = w0; W1 = w1; W2 = w2;
    }

    public static EliteSeed Galaxy1 => new(0x5A4A, 0x0248, 0xB753);

    public void Twist()
    {
        ushort temp = (ushort)((W0 + W1 + W2) & 0xFFFF);
        W0 = W1;
        W1 = W2;
        W2 = temp;
    }

    public void NextGalaxy()
    {
        W0 = RotateWord(W0);
        W1 = RotateWord(W1);
        W2 = RotateWord(W2);
    }

    private static ushort RotateWord(ushort val)
    {
        byte hi = (byte)(val >> 8);
        byte lo = (byte)(val & 0xFF);
        hi = RotateByte(hi);
        lo = RotateByte(lo);
        return (ushort)((hi << 8) | lo);
    }

    private static byte RotateByte(byte b)
    {
        // Rotate left by 1 bit within the byte
        return (byte)(((b << 1) | (b >> 7)) & 0xFF);
    }

    public byte Hi0 => (byte)(W0 >> 8);
    public byte Lo0 => (byte)(W0 & 0xFF);
    public byte Hi1 => (byte)(W1 >> 8);
    public byte Lo1 => (byte)(W1 & 0xFF);
    public byte Hi2 => (byte)(W2 >> 8);
    public byte Lo2 => (byte)(W2 & 0xFF);
}
