using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common
{
    public class EConatinerChecksum
    {
        public bool IsChecksumMatched = false;
        public int ChecksumDigit = -1;
    }
}