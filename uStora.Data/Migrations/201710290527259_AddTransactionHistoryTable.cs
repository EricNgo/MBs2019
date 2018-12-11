namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTransactionHistoryTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TransactionHistories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatedAt = c.DateTime(nullable: false),
                        Description = c.String(),
                        Money = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MoneyInSite = c.Int(nullable: false),
                        CardOwner = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUsers", t => t.CardOwner)
                .Index(t => t.CardOwner);
            
            AddColumn("dbo.ApplicationUsers", "Coin", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TransactionHistories", "CardOwner", "dbo.ApplicationUsers");
            DropIndex("dbo.TransactionHistories", new[] { "CardOwner" });
            DropColumn("dbo.ApplicationUsers", "Coin");
            DropTable("dbo.TransactionHistories");
        }
    }
}
