using System.Security.Cryptography;
using System.Text;

namespace Biblioteca.Common
{
    public static class Encryption
    {
        //create a string MD5  
        public static string GetMD5(string str)
        {
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = MD5.HashData(fromData);
            StringBuilder byte2String = new();

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String.Append(targetData[i].ToString("x2"));
            }
            return byte2String.ToString();
        }
    }
}
