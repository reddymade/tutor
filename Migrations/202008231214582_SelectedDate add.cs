namespace InstantTutors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SelectedDateadd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SessionSchedule", "SelectedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SessionSchedule", "SelectedDate");
        }
    }
}
