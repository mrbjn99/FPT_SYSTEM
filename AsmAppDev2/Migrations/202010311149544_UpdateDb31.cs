namespace AsmAppDev2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDb31 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Full_Name", c => c.String());
            AddColumn("dbo.AspNetUsers", "Education", c => c.String());
            AddColumn("dbo.AspNetUsers", "Programming_Language", c => c.String());
            AddColumn("dbo.AspNetUsers", "Experience_Details", c => c.String());
            AddColumn("dbo.AspNetUsers", "Department", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Department");
            DropColumn("dbo.AspNetUsers", "Experience_Details");
            DropColumn("dbo.AspNetUsers", "Programming_Language");
            DropColumn("dbo.AspNetUsers", "Education");
            DropColumn("dbo.AspNetUsers", "Full_Name");
        }
    }
}
