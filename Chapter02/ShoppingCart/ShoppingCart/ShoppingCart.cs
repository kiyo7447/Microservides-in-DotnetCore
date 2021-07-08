namespace ShoppingCart.ShoppingCart
{
	using System.Collections.Generic;
	using System.Linq;
	using EventFeed;

	public class ShoppingCart
	{
		private readonly HashSet<ShoppingCartItem> items = new();

		public int UserId { get; }
		public IEnumerable<ShoppingCartItem> Items => this.items;

		public ShoppingCart(int userId) => this.UserId = userId;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="shoppingCartItems"></param>
		/// <param name="eventStore"></param>
		public void AddItems(IEnumerable<ShoppingCartItem> shoppingCartItems, IEventStore eventStore)
		{
			foreach (var item in shoppingCartItems)
				if (this.items.Add(item))
					eventStore.Raise("ShoppingCartItemAdded", new { this.UserId, item });
		}

		public void RemoveItems(int[] productCatalogueIds, IEventStore eventStore) =>
		  this.items.RemoveWhere(i => productCatalogueIds.Contains(i.ProductCatalogueId));
	}

	public record ShoppingCartItem(
	  int ProductCatalogueId,
	  string ProductName,
	  string Description,
	  Money Price)
	{
		public virtual bool Equals(ShoppingCartItem? obj) =>
		  obj != null && this.ProductCatalogueId.Equals(obj.ProductCatalogueId);

		public override int GetHashCode() => this.ProductCatalogueId.GetHashCode();
	}

	public record Money(string Currency, decimal Amount);

}

namespace Abe
{
	public class Test
	{

		public void Test2()
		{
			var m = new Money(123304, TaxTypeEnum.In);

			
		}
	}

	public record Money(decimal Amount, TaxTypeEnum Tax);

	public enum TaxTypeEnum
	{
		In = 1,
		Out = 2
	}

	public enum TaxEnum
	{
		U8 = 108,
		U10 = 110,
		S8 = 208,
		S10 = 210,
		H = 0
	}

}