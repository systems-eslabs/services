using System;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common
{
    public static partial class CommonFunctions
    {
        public static EConatinerChecksum calculateChecksum(string containerNo)
        {
            var containerArray = containerNo.Trim().ToCharArray();
            int containerSum = 0;
            int containerDivision = 0;
            int checkDigit = -1;
            bool IsCheckSummatched = false;

            for (int i = 0; i < 10; i++)
            {
                containerSum = containerSum + getAlphabetWeight(containerArray[i]) * (2 ^ i);
            }

            containerDivision = containerSum / 11; // Round off towards zero
            containerDivision = containerDivision * 11;
            checkDigit = containerSum - containerDivision;

            IsCheckSummatched = checkDigit == (int)containerArray[10] ? true : false;

            return new EConatinerChecksum
            {
                IsChecksumMatched = IsCheckSummatched,
                ChecksumDigit = checkDigit
            };
        }


        static int getAlphabetWeight(char val)
        {
            int charWeight = 0;
            switch (val)
            {
                case 'A':
                    charWeight = 10;
                    break;
                case 'B':
                    charWeight = 12;
                    break;
                case 'C':
                    charWeight = 13;
                    break;
                case 'D':
                    charWeight = 14;
                    break;
                case 'E':
                    charWeight = 15;
                    break;
                case 'F':
                    charWeight = 16;
                    break;
                case 'G':
                    charWeight = 17;
                    break;
                case 'H':
                    charWeight = 18;
                    break;
                case 'I':
                    charWeight = 19;
                    break;
                case 'J':
                    charWeight = 20;
                    break;
                case 'K':
                    charWeight = 21;
                    break;
                case 'L':
                    charWeight = 22;
                    break;
                case 'M':
                    charWeight = 23;
                    break;
                case 'N':
                    charWeight = 24;
                    break;
                case 'O':
                    charWeight = 25;
                    break;
                case 'P':
                    charWeight = 26;
                    break;
                case 'Q':
                    charWeight = 27;
                    break;
                case 'R':
                    charWeight = 28;
                    break;
                case 'S':
                    charWeight = 29;
                    break;
                case 'T':
                    charWeight = 30;
                    break;
                case 'U':
                    charWeight = 31;
                    break;
                case 'V':
                    charWeight = 32;
                    break;
                case 'W':
                    charWeight = 33;
                    break;
                case 'X':
                    charWeight = 34;
                    break;
                case 'Y':
                    charWeight = 35;
                    break;
                case 'Z':
                    charWeight = 36;
                    break;
            }
            return charWeight;
        }




    }
}