namespace MyStorageAPI.Models.Data
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int Quantity { get; set; } = 1;
		public int CategoryId { get; set; }
		public Category Category { get; set; }
		public string? ImagePath { get; set; }
		public bool IsVisible { get; set; } = true;

		public ICollection<ProductLog> ProductLogs { get; set; } = new List<ProductLog>();
	}
}