namespace BooPlayer.Utils;
internal static class HashHelper
{
    //public static IHashResult SHA256(string password, string seed)
    //{
    //    return HashBuilder
    //        .SHA256
    //        .Append(seed)
    //        .Append(password)
    //        .Build();
    //}
    //public static IHashResult MD5(string password, string seed) {
    //    return HashBuilder
    //        .MD5
    //        .Append(seed)
    //        .Append(password)
    //        .Build();
    //}

    public static string EncodeBase64(this byte[] source) {
        return Convert.ToBase64String(source);
    }
    public static string EncodeHexString(this byte[] source) {
        return BitConverter.ToString(source).Replace("-", "").ToLower();
    }

    public static byte[] DecodeBase64(this string source) {
        return Convert.FromBase64String(source);
    }

    public static byte[] FromHexString(string source) {
        int length = source.Length / 2;
        byte[] bytes = new byte[length];

        for (int i = 0; i < length; i++) {
            // 文字列から2文字切り出す
            string hex = source.Substring(i * 2, 2);
            // 16進数の文字列を整数に変換する
            int value = Convert.ToInt32(hex, 16);
            // 整数をバイトにキャストして、バイト配列に代入する
            bytes[i] = (byte)value;
        }
        return bytes;
    }
    public static byte[] DecodeHexString(this string source) {
        return FromHexString(source);
    }
}
