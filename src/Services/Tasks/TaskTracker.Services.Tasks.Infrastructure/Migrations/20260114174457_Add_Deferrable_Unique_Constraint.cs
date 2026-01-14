using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Services.Tasks.Infrastructure.Migrations
{
    public partial class Add_Deferrable_Unique_Constraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE public.""Tasks""
                ADD CONSTRAINT uq_tasks_user_state_order
                UNIQUE (""UserId"", ""TaskState"", ""SortOrder"")
                DEFERRABLE INITIALLY DEFERRED;
            ");
        }
        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE public.""Tasks""
                DROP CONSTRAINT uq_tasks_user_state_order;
            ");
        }
    }
}
