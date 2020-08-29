namespace InstantTutors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StartDateendDateadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "StartDate", c => c.DateTime());
            AddColumn("dbo.Sessions", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "EndDate");
            DropColumn("dbo.Sessions", "StartDate");
        }
    }
}
