using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlayer.Data.Migrations
{
    public partial class ChangeAlbumEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ArtistId",
                table: "Albums",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "AlbumArtFileName",
                table: "Albums",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlbumArtFileName",
                table: "Albums");

            migrationBuilder.AlterColumn<long>(
                name: "ArtistId",
                table: "Albums",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
