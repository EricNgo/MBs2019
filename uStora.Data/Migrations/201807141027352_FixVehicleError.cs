namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixVehicleError : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InventoryStocks", "Quantity", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InventoryStocks", "Quantity");
        }
    }
}
