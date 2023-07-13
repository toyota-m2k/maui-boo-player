using System.Security.Cryptography;
using System.Text;

namespace BooPlayer.Utils;

public enum HashAlgorithmId {
    Md5,
    Sha1,
    Sha256,
    Sha384,
    Sha512,
}

internal interface IHashResult {
    byte[] Hash { get; }
    string AsHexString { get; }
    string AsBase64String { get; }

    }
internal class HashBuilder : IHashResult {
    private HashAlgorithm builder;
    private byte[] hash = null!;

    public static HashBuilder Create(HashAlgorithm algorithm) { return new HashBuilder(algorithm); }
    public static HashBuilder MD5 => Create(System.Security.Cryptography.MD5.Create());
    public static HashBuilder SHA1 => Create(System.Security.Cryptography.SHA1.Create());
    public static HashBuilder SHA256 => Create(System.Security.Cryptography.SHA256.Create());
    public static HashBuilder SHA384 => Create(System.Security.Cryptography.SHA384.Create());
    public static HashBuilder SHA512 => Create(System.Security.Cryptography.SHA512.Create());


    private HashBuilder(HashAlgorithm algorithm)
    {
        builder = algorithm;
        //workBuffer = new byte[builder.HashSize / 8];
    }

    public HashBuilder Append(string src)
    {
        return Append(Encoding.UTF8.GetBytes(src));
    }
    public HashBuilder Append(byte[] src)
    {
        builder.TransformBlock(src, 0, src.Length, src, 0);
        return this;
    }

    public HashBuilder Build()
    {
        builder.TransformFinalBlock(new byte[0], 0, 0);
        hash = builder.Hash!;
        return this;
    }

    public byte[] Hash => hash;
    public string AsHexString => hash.EncodeHexString();
    public string AsBase64String => hash.EncodeBase64();
}
