namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStockStatusColunm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "StockStatus", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "StockStatus");
        }
    }
}
