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
    }
}
