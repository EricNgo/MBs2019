namespace uStora.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatetest : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TagCategories",
                c => new
                {
                    ID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 256),
               
                })
                .PrimaryKey(t => t.ID);

            AddColumn("dbo.Tags", "TagCategoryID", c => c.Int(nullable: true));
            CreateIndex("dbo.Tags", "TagCategoryID");
            AddForeignKey("dbo.Tags", "TagCategoryID", "dbo.TagCategories", "ID", cascadeDelete: true);
        }

        public override void Down()
        {
        }
    }
}
