namespace ShelfSimple.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProductIsArchived : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "IsArchived", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "IsArchived");
        }
    }
}
