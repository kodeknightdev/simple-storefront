namespace ShelfSimple.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSellerProfile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sellers", "Slug", c => c.String(maxLength: 60));
            AddColumn("dbo.Sellers", "Bio", c => c.String(maxLength: 1000));

            // existing sellers need a unique slug before the column can be required + indexed,
            // they can change it from their profile afterwards
            Sql("UPDATE dbo.Sellers SET Slug = 'shop-' + CAST(Id AS nvarchar(10)) WHERE Slug IS NULL");
            AlterColumn("dbo.Sellers", "Slug", c => c.String(nullable: false, maxLength: 60));
            CreateIndex("dbo.Sellers", "Slug", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Sellers", new[] { "Slug" });
            DropColumn("dbo.Sellers", "Bio");
            DropColumn("dbo.Sellers", "Slug");
        }
    }
}
