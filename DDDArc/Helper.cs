﻿using Org.BouncyCastle.Utilities.Zlib;

namespace DDDArc
{
    class Helper
    {
        const int Z_DEFAULT_COMPRESSION = -1;

        // Reverse byte order
        public static short ReverseBytesInt16(short value)
        {
            return (short)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        // Compression related
        // partially from http://stackoverflow.com/a/6627194/5343630
        public static void CompressData(string compLevel, byte[] inData, out byte[] outData)
        {
            int cmp_lvl = 0;
            if (compLevel == "789C")
                cmp_lvl = 6;
            else if (compLevel == "7801" || compLevel == "78DA")
                cmp_lvl = 0;
            else
            {
                Console.WriteLine("\nERROR: Unsupported compression flag used.");
                Console.WriteLine("Falling back to default.\n");
                cmp_lvl = Z_DEFAULT_COMPRESSION;
            }

            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream, cmp_lvl))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.Finish();
                outData = outMemoryStream.ToArray();
            }
        }

        public static void DecompressData(byte[] inData, out byte[] outData)
        {
            using (MemoryStream outMemoryStream = new MemoryStream())
            using (ZOutputStream outZStream = new ZOutputStream(outMemoryStream))
            using (Stream inMemoryStream = new MemoryStream(inData))
            {
                CopyStream(inMemoryStream, outZStream);
                outZStream.Finish();
                outData = outMemoryStream.ToArray();
            }
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[1000000]; // 1MB buffer, should be sufficient
            int len;
            while ((len = input.Read(buffer, 0, 1000000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }
    }
}
