namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class f3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tags", "TagCategoryID", "dbo.TagCategories");
            DropIndex("dbo.Tags", new[] { "TagCategoryID" });
            DropColumn("dbo.Tags", "TagCategoryID");
            DropTable("dbo.TagCategories");
        }
        
        public override void Down()
        {
        }
    }
}
