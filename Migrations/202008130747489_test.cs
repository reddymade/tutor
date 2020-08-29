namespace InstantTutors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class test : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "TutorUserId", c => c.String());
            AddColumn("dbo.Sessions", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.Sessions", "ApproveDeclineBy", c => c.String());
            AddColumn("dbo.Sessions", "CreatedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "CreatedBy");
            DropColumn("dbo.Sessions", "ApproveDeclineBy");
            DropColumn("dbo.Sessions", "Status");
            DropColumn("dbo.Sessions", "TutorUserId");
        }
    }
}
