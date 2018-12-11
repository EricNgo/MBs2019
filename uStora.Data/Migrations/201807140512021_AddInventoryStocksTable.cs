namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInventoryStocksTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InventoryStocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Long(nullable: false),
                        AdjustedQuantity = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InventoryStocks", "ProductId", "dbo.Products");
            DropIndex("dbo.InventoryStocks", new[] { "ProductId" });
            DropTable("dbo.InventoryStocks");
        }
    }
}
