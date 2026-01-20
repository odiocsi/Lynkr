using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lynkr.Migrations
{
    /// <inheritdoc />
    public partial class FixPostsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Posts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6db75d8b-9e22-4c3e-9b4d-1b252beb5fad", new DateTimeOffset(new DateTime(2026, 1, 10, 18, 11, 6, 279, DateTimeKind.Unspecified).AddTicks(2623), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEEoQXuQHZKxIeECOysYZJsh2kqlC7f6I18SclHEyLaUK7dgPmmRltMC2vLJE8Rpzdg==", "3bc4dfbd-5053-446e-a1a8-645fffbcb2c2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e325a98f-da1c-4904-b85e-9538c93f9329", new DateTimeOffset(new DateTime(2026, 1, 15, 18, 11, 6, 326, DateTimeKind.Unspecified).AddTicks(1211), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEIYPM8FjLj/AcOj85yk/2MotFao/B47kHbvD8QwXkAg9aTJKuDUcikTjCNZj+9fBhA==", "6e4753ec-049d-423d-861d-e26b75e7be5d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aaf8e677-eff5-4248-86ba-669fcdb0ee0d", new DateTimeOffset(new DateTime(2026, 1, 18, 18, 11, 6, 368, DateTimeKind.Unspecified).AddTicks(1374), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEKfS7FyueJYwo53c0LJk4EnIq0hOzIiKuSf/tG6PFNES4jQExuM+fik2U/B/hfatIg==", "14940510-3364-4093-93ab-20edb6295072" });

            migrationBuilder.InsertData(
                table: "Conversations",
                columns: new[] { "Id", "CreatedAt", "User1Id", "User2Id" },
                values: new object[,]
                {
                    { 1, new DateTimeOffset(new DateTime(2026, 1, 16, 18, 11, 6, 410, DateTimeKind.Unspecified).AddTicks(5565), new TimeSpan(0, 0, 0, 0, 0)), 1, 2 },
                    { 2, new DateTimeOffset(new DateTime(2026, 1, 20, 16, 11, 6, 410, DateTimeKind.Unspecified).AddTicks(5707), new TimeSpan(0, 0, 0, 0, 0)), 1, 3 }
                });

            migrationBuilder.UpdateData(
                table: "Friendships",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2026, 1, 17, 18, 11, 6, 410, DateTimeKind.Unspecified).AddTicks(4466), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2026, 1, 17, 18, 11, 6, 410, DateTimeKind.Unspecified).AddTicks(4614), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Friendships",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 20, 17, 11, 6, 410, DateTimeKind.Unspecified).AddTicks(4988), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 15, 18, 11, 6, 410, DateTimeKind.Unspecified).AddTicks(3509), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 18, 18, 11, 6, 410, DateTimeKind.Unspecified).AddTicks(3741), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Messages",
                columns: new[] { "Id", "Content", "ConversationId", "SenderId", "SentAt" },
                values: new object[,]
                {
                    { 1, "Szia Bob! Láttad a tegnapi meccset?", 1, 1, new DateTimeOffset(new DateTime(2026, 1, 17, 4, 11, 6, 410, DateTimeKind.Unspecified).AddTicks(6412), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 2, "Szia! Persze, elképesztő volt a vége.", 1, 2, new DateTimeOffset(new DateTime(2026, 1, 17, 4, 16, 6, 410, DateTimeKind.Unspecified).AddTicks(6558), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 3, "Hihetetlen, hogy megfordították. Mikor érsz rá egy kávéra?", 1, 1, new DateTimeOffset(new DateTime(2026, 1, 18, 3, 11, 6, 410, DateTimeKind.Unspecified).AddTicks(6569), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 4, "Holnap délután jó lehet. 4 körül?", 1, 2, new DateTimeOffset(new DateTime(2026, 1, 18, 3, 41, 6, 410, DateTimeKind.Unspecified).AddTicks(6570), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 5, "Helló Alice! Láttam a posztodat, nagyon jó lett a kép.", 2, 3, new DateTimeOffset(new DateTime(2026, 1, 20, 16, 11, 6, 410, DateTimeKind.Unspecified).AddTicks(6572), new TimeSpan(0, 0, 0, 0, 0)) },
                    { 6, "Köszi Charlie! :)", 2, 1, new DateTimeOffset(new DateTime(2026, 1, 20, 18, 6, 6, 410, DateTimeKind.Unspecified).AddTicks(6573), new TimeSpan(0, 0, 0, 0, 0)) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Messages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Conversations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Conversations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7a72c8bb-5252-48b2-848c-481c7b48863c", new DateTimeOffset(new DateTime(2025, 11, 14, 1, 45, 16, 373, DateTimeKind.Unspecified).AddTicks(9160), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEBHRqTCwqTKYZM9+JL9NUxQATgkLZberriDg1osqjdyV7DqNZxBvXLhJI3Kjxo+qxQ==", "7dab6c15-e2eb-4cfe-b1e3-6480319f4a53" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "265a8e88-dd9c-4cac-993a-d8a38c9a0569", new DateTimeOffset(new DateTime(2025, 11, 19, 1, 45, 16, 416, DateTimeKind.Unspecified).AddTicks(5209), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEMjM/2wtpMqJRYuQaX6v1StilKB4d5PR70txLGBrhAQL0vYSjhPDoe97tcB+G7Xyyw==", "c5d7b05d-fd07-4345-93f3-6fa664c32746" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c98240fa-997f-4f65-acab-11dadc1563b2", new DateTimeOffset(new DateTime(2025, 11, 22, 1, 45, 16, 457, DateTimeKind.Unspecified).AddTicks(4995), new TimeSpan(0, 0, 0, 0, 0)), "AQAAAAIAAYagAAAAEDFKHrPaNZzo0HfPtEMjk1pKlnr8iytH46BdfQqY0p+JxTOPU8xCPWKlSDA23lCuig==", "56d6814a-fd0f-4b0f-9ab6-d5db73b4c8bb" });

            migrationBuilder.UpdateData(
                table: "Friendships",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 11, 21, 1, 45, 16, 498, DateTimeKind.Unspecified).AddTicks(5908), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(2025, 11, 21, 1, 45, 16, 498, DateTimeKind.Unspecified).AddTicks(6050), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.UpdateData(
                table: "Friendships",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 11, 24, 0, 45, 16, 498, DateTimeKind.Unspecified).AddTicks(6417), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 11, 19, 1, 45, 16, 498, DateTimeKind.Unspecified).AddTicks(4819), new TimeSpan(0, 0, 0, 0, 0)), "https://placehold.co/600x400/4CAF50/white?text=Alice+Post" });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 11, 22, 1, 45, 16, 498, DateTimeKind.Unspecified).AddTicks(5183), new TimeSpan(0, 0, 0, 0, 0)), "https://placehold.co/600x400/2196F3/white?text=Bob+Post" });
        }
    }
}
