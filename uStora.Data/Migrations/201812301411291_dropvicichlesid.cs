namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dropvicichlesid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TrackOrders", "VehicleId", "dbo.Vehicles");
        }
        
        public override void Down()
        {
        }
    }
}
