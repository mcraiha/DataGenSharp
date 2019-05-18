
namespace DatagenSharp
{
	/// <summary>
	/// Class that keeps track of name and how common it is (how much weight it has)
	/// </summary>
	public class NameWeightPair
	{
		public readonly string name;
		public readonly int weight;

		public NameWeightPair(string name, int weight)
		{
			this.name = name;
			this.weight = weight;
		}
	}
}