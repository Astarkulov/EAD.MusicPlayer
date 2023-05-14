using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlayer.Data.Migrations
{
    public partial class AddGenresInTrackEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genres",
                table: "Tracks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ArtistArtFileName",
                table: "Artists",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genres",
                table: "Tracks");

            migrationBuilder.DropColumn(
                name: "ArtistArtFileName",
                table: "Artists");
        }
    }
}
