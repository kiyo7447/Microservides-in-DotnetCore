﻿namespace ShoppingCart.Shoppingcart
{
	using System.Diagnostics;
	using System.Text.Json;
	using System.Threading.Tasks;
	using EventFeed;
	using Microsoft.AspNetCore.Mvc;
	using ShoppingCart;

	[Route("/shoppingcart")]
	public class ShoppingCartController : ControllerBase
	{
		private readonly IShoppingCartStore shoppingCartStore;
		private readonly IProductCatalogClient productCatalog;
		private readonly IEventStore eventStore;

		public ShoppingCartController(
		  IShoppingCartStore shoppingCartStore,
		  IProductCatalogClient productCatalog,
		  IEventStore eventStore)
		{
			this.shoppingCartStore = shoppingCartStore;
			this.productCatalog = productCatalog;
			this.eventStore = eventStore;
		}

		[HttpGet("{userId:int}")]
		public ShoppingCart Get(int userId) => this.shoppingCartStore.Get(userId);

		[HttpPost("{userId:int}/items")]
		public async Task<ShoppingCart> Post(int userId, [FromBody] int[] productIds)
		{
			var shoppingCart = shoppingCartStore.Get(userId);
			var shoppingCartItems = await this.productCatalog.GetShoppingCartItems(productIds);
			shoppingCart.AddItems(shoppingCartItems, eventStore);
			this.shoppingCartStore.Save(shoppingCart);

			return shoppingCart;
		}

		[HttpDelete("{userid:int}/items")]
		public ShoppingCart Delete(int userId, [FromBody] int[] productIds)
		{
			var shoppingCart = this.shoppingCartStore.Get(userId);
			shoppingCart.RemoveItems(productIds, this.eventStore);
			this.shoppingCartStore.Save(shoppingCart);
			return shoppingCart;
		}

		//応答データの書式設定
		//https://docs.microsoft.com/ja-jp/aspnet/core/web-api/advanced/formatting?view=aspnetcore-5.0

		[HttpGet("hoge/{str}")]
		public Hoge GetHoge(string str, [FromBody] Hoge hoge)
		{
			Debug.WriteLine($"str={str}");
			Debug.WriteLine($"hoge={hoge}");
			return new Hoge { I = 123, N = "abc" };
		}

		public record Hoge
		{
			public int I { get; set; }
			public string N { get; set; } = "";
		}
	}
}
