namespace InstantTutors.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsession : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tutors", "Sessions_Id", c => c.Int());
            CreateIndex("dbo.Tutors", "Sessions_Id");
            AddForeignKey("dbo.Tutors", "Sessions_Id", "dbo.Sessions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tutors", "Sessions_Id", "dbo.Sessions");
            DropIndex("dbo.Tutors", new[] { "Sessions_Id" });
            DropColumn("dbo.Tutors", "Sessions_Id");
        }
    }
}
