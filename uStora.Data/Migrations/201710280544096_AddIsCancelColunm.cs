namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsCancelColunm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsCancel", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "IsCancel");
        }
    }
}
