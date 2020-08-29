namespace InstantTutors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Instatutor : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Sessions", "TutorUserId");
            DropColumn("dbo.Sessions", "Status");
            DropColumn("dbo.Sessions", "ApproveDeclineBy");
            DropColumn("dbo.Sessions", "CreatedBy");
            DropTable("dbo.Configurations");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Configurations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Value = c.String(),
                        DefaultValue = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Sessions", "CreatedBy", c => c.String());
            AddColumn("dbo.Sessions", "ApproveDeclineBy", c => c.String());
            AddColumn("dbo.Sessions", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Sessions", "TutorUserId", c => c.String(nullable: false));
        }
    }
}
