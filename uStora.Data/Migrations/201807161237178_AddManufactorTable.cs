namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManufactorTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.InventoryStocks", "ProductId", "dbo.Products");
            DropIndex("dbo.InventoryStocks", new[] { "ProductId" });
            CreateTable(
                "dbo.Manufactors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        LogoUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Products", "ManufactorId", c => c.Int());
            CreateIndex("dbo.Products", "ManufactorId");
            AddForeignKey("dbo.Products", "ManufactorId", "dbo.Manufactors", "Id");
            DropTable("dbo.InventoryStocks");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.InventoryStocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Long(nullable: false),
                        Quantity = c.Int(nullable: false),
                        AdjustedQuantity = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("dbo.Products", "ManufactorId", "dbo.Manufactors");
            DropIndex("dbo.Products", new[] { "ManufactorId" });
            DropColumn("dbo.Products", "ManufactorId");
            DropTable("dbo.Manufactors");
            CreateIndex("dbo.InventoryStocks", "ProductId");
            AddForeignKey("dbo.InventoryStocks", "ProductId", "dbo.Products", "ID", cascadeDelete: true);
        }
    }
}
