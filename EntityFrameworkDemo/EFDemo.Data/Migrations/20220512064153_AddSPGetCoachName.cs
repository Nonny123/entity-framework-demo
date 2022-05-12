using Microsoft.EntityFrameworkCore.Migrations;

namespace EFDemo.Data.Migrations
{
    public partial class AddSPGetCoachName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE PROCEDURE sp_GetTeamCoach
                                     @teamId int
                                    AS
                                    BEGIN                                        
                                        SELECT * from Coaches where TeamId = @teamId                                      
                                    END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP PROCEDURE [dbo].[CoachName]");
        }
    }
}
