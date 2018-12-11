namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQrCodeColunm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ApplicationUsers", "QrCode", c => c.String(maxLength: 700));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ApplicationUsers", "QrCode");
        }
    }
}
