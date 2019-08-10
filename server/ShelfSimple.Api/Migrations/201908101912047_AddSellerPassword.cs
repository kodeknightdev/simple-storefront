namespace ShelfSimple.Api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSellerPassword : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sellers", "PasswordHash", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sellers", "PasswordHash");
        }
    }
}
