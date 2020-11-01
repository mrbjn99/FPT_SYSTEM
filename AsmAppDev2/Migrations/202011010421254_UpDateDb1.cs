namespace AsmAppDev2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpDateDb1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TraineeUsers", "Phone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TraineeUsers", "Phone", c => c.Int(nullable: false));
        }
    }
}
