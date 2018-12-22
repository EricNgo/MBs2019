namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addToaDoKHang : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "CustomerLat", c => c.String());
            AddColumn("dbo.Orders", "CustomerLong", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "CustomerLong");
            DropColumn("dbo.Orders", "CustomerLat");
        }
    }
}
