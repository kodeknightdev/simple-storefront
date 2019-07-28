namespace ShelfSimple.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrders : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        SellerId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 200),
                        UnitPrice = c.Decimal(nullable: false, precision: 10, scale: 2),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.OrderId)
                .Index(t => t.ProductId)
                .Index(t => t.SellerId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CartId = c.Guid(nullable: false),
                        Email = c.String(nullable: false, maxLength: 256),
                        Status = c.Int(nullable: false),
                        Total = c.Decimal(nullable: false, precision: 10, scale: 2),
                        Currency = c.String(nullable: false, maxLength: 3),
                        PaymentIntentId = c.String(maxLength: 100),
                        CreatedAt = c.DateTime(nullable: false),
                        PaidAt = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.PaymentIntentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropIndex("dbo.Orders", new[] { "PaymentIntentId" });
            DropIndex("dbo.OrderItems", new[] { "SellerId" });
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropTable("dbo.Orders");
            DropTable("dbo.OrderItems");
        }
    }
}
