namespace DIT.Core
{
	public class GetRequest
	{
		public Guid? category { get; set; }
		public int? page { get; set; }
		public int? size { get; set; }

		public string? q { get; set; }

	}
}
