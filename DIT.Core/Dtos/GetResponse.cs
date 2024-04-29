namespace DIT.Core
{
	public class GetResponse<T> where T : class
	{
		public IEnumerable<T> content { get; set; }
		public bool last { get; set; }
		public int totalElements { get; set; }

		public Pagination pageable { get; set; }

	}
}
