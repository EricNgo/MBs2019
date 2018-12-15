namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_add_tagcategory : DbMigration
    {
        public override void Up()
        {
            //DropColumn("dbo.TagCategories", "CreatedDate");
            //DropColumn("dbo.TagCategories", "CreatedBy");
            //DropColumn("dbo.TagCategories", "UpdatedDate");
            //DropColumn("dbo.TagCategories", "UpdatedBy");
            //DropColumn("dbo.TagCategories", "MetaKeyword");
            //DropColumn("dbo.TagCategories", "MetaDescription");
            //DropColumn("dbo.TagCategories", "Status");
        }
        
        public override void Down()
        {
            //AddColumn("dbo.TagCategories", "Status", c => c.Boolean(nullable: false));
            //AddColumn("dbo.TagCategories", "MetaDescription", c => c.String(maxLength: 150));
            //AddColumn("dbo.TagCategories", "MetaKeyword", c => c.String(maxLength: 150));
            //AddColumn("dbo.TagCategories", "UpdatedBy", c => c.String(maxLength: 50));
            //AddColumn("dbo.TagCategories", "UpdatedDate", c => c.DateTime());
            //AddColumn("dbo.TagCategories", "CreatedBy", c => c.String(maxLength: 50));
            //AddColumn("dbo.TagCategories", "CreatedDate", c => c.DateTime());
        }
    }
}
