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
        public static string ToHash(string code)
        {
            StringBuilder sb = new StringBuilder();
            var bytes = GetByte(code);
            foreach (var item in bytes)
            {
                sb.Append(String.Format("{0:x}", Convert.ToInt32(item)));
            }
            return sb.ToString();
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
