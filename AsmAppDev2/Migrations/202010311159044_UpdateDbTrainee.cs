namespace AsmAppDev2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDbTrainee : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Full_Name");
            DropColumn("dbo.AspNetUsers", "Education");
            DropColumn("dbo.AspNetUsers", "Programming_Language");
            DropColumn("dbo.AspNetUsers", "Experience_Details");
            DropColumn("dbo.AspNetUsers", "Department");
            DropColumn("dbo.TraineeUsers", "isVerified");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TraineeUsers", "isVerified", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Department", c => c.String());
            AddColumn("dbo.AspNetUsers", "Experience_Details", c => c.String());
            AddColumn("dbo.AspNetUsers", "Programming_Language", c => c.String());
            AddColumn("dbo.AspNetUsers", "Education", c => c.String());
            AddColumn("dbo.AspNetUsers", "Full_Name", c => c.String());
        }
    }
}
