namespace MyStorageAPI.Models.Data
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Color { get; set; } = "gray"; // TODO: retrieved from UI color picker in the future
		public int WarehouseId { get; set; }
		public Warehouse Warehouse { get; set; }
		public ICollection<Product> Products { get; set; } = new List<Product>(); // Initialized with an empty list to avoid NullReferenceException when adding items. Ensures that the collection is never null
		public bool IsVisible { get; set; } = true;
	}
}