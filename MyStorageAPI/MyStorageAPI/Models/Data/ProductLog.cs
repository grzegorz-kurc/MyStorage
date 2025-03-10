namespace MyStorageAPI.Models.Data
{
	public class ProductLog
	{
		public int Id { get; set; }
		public int ProductId { get; set; }
		public Product Product { get; set; }
		public string UserId { get; set; }
		public User User { get; set; }
		public int PreviousQuantity { get; set; }
		public int NewQuantity { get; set; }
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	}
}