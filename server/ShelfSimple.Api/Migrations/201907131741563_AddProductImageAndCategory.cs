namespace ShelfSimple.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductImageAndCategory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ImageUrl", c => c.String(maxLength: 500));
            AddColumn("dbo.Products", "Category", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Category");
            DropColumn("dbo.Products", "ImageUrl");
        }
    }
}
