namespace WebDraw.Migrations.WebDrawDbContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chains",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartID = c.Int(nullable: false),
                        Open = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StartSuggestions", t => t.StartID, cascadeDelete: true)
                .Index(t => t.StartID);
            
            CreateTable(
                "dbo.Entries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChainId = c.Int(nullable: false),
                        entryType = c.Int(nullable: false),
                        Value = c.String(),
                        Active = c.Boolean(nullable: false),
                        created = c.DateTime(),
                        lastShown = c.DateTime(),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Chains", t => t.ChainId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.ChainId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdentityId = c.Guid(),
                        VisibleName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StartSuggestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Chains", "StartID", "dbo.StartSuggestions");
            DropForeignKey("dbo.Entries", "UserId", "dbo.Users");
            DropForeignKey("dbo.Entries", "ChainId", "dbo.Chains");
            DropIndex("dbo.Entries", new[] { "UserId" });
            DropIndex("dbo.Entries", new[] { "ChainId" });
            DropIndex("dbo.Chains", new[] { "StartID" });
            DropTable("dbo.StartSuggestions");
            DropTable("dbo.Users");
            DropTable("dbo.Entries");
            DropTable("dbo.Chains");
        }
    }
}
