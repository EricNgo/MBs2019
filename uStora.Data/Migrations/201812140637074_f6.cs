namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class f6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagCategories",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 256),
                    CreatedDate = c.DateTime(),
                    CreatedBy = c.String(maxLength: 50),
                    UpdatedDate = c.DateTime(),
                    UpdatedBy = c.String(maxLength: 50),

                })
                .PrimaryKey(t => t.ID);

            AddColumn("dbo.Tags", "TagCategoryID", c => c.Int(nullable: false));
            CreateIndex("dbo.Tags", "TagCategoryID");
            AddForeignKey("dbo.Tags", "TagCategoryID", "dbo.TagCategories", "ID", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Tags", "TagCategoryID", "dbo.TagCategories");
            DropIndex("dbo.Tags", new[] { "TagCategoryID" });
            DropColumn("dbo.Tags", "TagCategoryID");
            DropTable("dbo.TagCategories");

        }
    }
}
