using System;
using System.Collections.Generic;
using System.Data.HashFunction.MurmurHash;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Akeem.Web.Tools
{
    public static class MurmurHashUtil
    {
        const string Str = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public static string ToHash1(string code)
        {
            StringBuilder sb = new StringBuilder();
            var bytes = GetByte(code);
            foreach (var item in bytes)
            {
                sb.Append(String.Format("{0:x}", Convert.ToInt32(item)));
            } 
            return sb.ToString();
        }

        public static string ToHash(string code)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random r = new Random(BitConverter.ToInt32(b, 0));
            string s = null;
        
            for (int i = 0; i < 6; i++)
            {
                s += Str.Substring(r.Next(0, Str.Length - 1), 1);
            }
            return s;
        }

        public static byte [] GetByte(string code)
        {
            byte[] srcBytes = Encoding.UTF8.GetBytes(code);
            var cfg = new MurmurHash3Config() { HashSizeInBits = 32, Seed = 0 };
            var mur = MurmurHash3Factory.Instance.Create(cfg);
            var hv = mur.ComputeHash(srcBytes);
            //var base64 = hv.AsBase64String();
            var hashBytes = hv.Hash;
            return hashBytes;
        }
    }
}
