using System.Security.Cryptography;
using System.Text;

namespace WebAppCmvc
{
    public static class Sidekick
    {
        static Random random = new Random();

        public static List<T> ShuffleList<T>(List<T> list)
        {
            List<T> newShuffledList = new List<T>();
            int listcCount = list.Count;
            for (int i = 0; i < listcCount; i++)
            {
                int randomElementInList = random.Next(0, list.Count);
                newShuffledList.Add(list[randomElementInList]);
                list.Remove(list[randomElementInList]);
            }
            return newShuffledList;
        }


        public static string Decrypt(string s)
        {
            byte[] b;
            string decrypted;
            try
            {
                b = Convert.FromBase64String(s);
                decrypted = Encoding.ASCII.GetString(b);
            }
            catch (FormatException)
            {
                decrypted = "";
            }
            return decrypted;
        }

        public static string Crypt(string s)
        {
            byte[] b = Encoding.ASCII.GetBytes(s);
            string encrypted = Convert.ToBase64String(b);
            return encrypted;
        }
    }
}
