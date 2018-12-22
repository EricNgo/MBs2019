namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTrackorderAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrackOrders", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.TrackOrders", "StatusPickup", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrackOrders", "StatusPickup");
            DropColumn("dbo.TrackOrders", "CreatedDate");
        }
    }
}
