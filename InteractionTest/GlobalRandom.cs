using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Interaction
{
    public static class GlobalRandom
    {
        private static RNGCryptoServiceProvider _r = new RNGCryptoServiceProvider();

        public static int Next(int min, int max)
        {
            byte[] buffer = new byte[4];
            _r.GetBytes(buffer);
            UInt32 rand = BitConverter.ToUInt32(buffer, 0);
            return (int)(rand % (max - min) + min);
        }
    }
}
