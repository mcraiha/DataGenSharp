
namespace DatagenSharp
{
	public static class UniqueIdMaker
    {
        private static long currentId = 0;
        public static long GetId()
        {
            currentId++;
            return currentId;
        }
    }
}