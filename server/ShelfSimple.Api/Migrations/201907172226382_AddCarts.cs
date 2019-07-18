namespace ShelfSimple.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCarts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CartItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CartId = c.Guid(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Carts", t => t.CartId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => new { t.CartId, t.ProductId }, unique: true, name: "IX_CartItem_CartProduct");
            
            CreateTable(
                "dbo.Carts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                        UpdatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CartItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.CartItems", "CartId", "dbo.Carts");
            DropIndex("dbo.CartItems", "IX_CartItem_CartProduct");
            DropTable("dbo.Carts");
            DropTable("dbo.CartItems");
        }
    }
}
