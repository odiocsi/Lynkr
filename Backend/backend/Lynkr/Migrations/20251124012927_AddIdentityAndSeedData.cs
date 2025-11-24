using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lynkr.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Friendships",
                columns: new[] { "Id", "ActionUserId", "CreatedAt", "Status", "UpdatedAt", "User1Id", "User2Id" },
                values: new object[] { 1, 1, new DateTimeOffset(new DateTime(2025, 11, 21, 1, 29, 26, 892, DateTimeKind.Unspecified).AddTicks(5143), new TimeSpan(0, 0, 0, 0, 0)), "ACCEPTED", new DateTimeOffset(new DateTime(2025, 11, 21, 1, 29, 26, 892, DateTimeKind.Unspecified).AddTicks(5292), new TimeSpan(0, 0, 0, 0, 0)), 1, 2 });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 11, 19, 1, 29, 26, 892, DateTimeKind.Unspecified).AddTicks(4023), new TimeSpan(0, 0, 0, 0, 0)), "https://placehold.co/600x400/4CAF50/white?text=Alice+Post" });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 11, 22, 1, 29, 26, 892, DateTimeKind.Unspecified).AddTicks(4412), new TimeSpan(0, 0, 0, 0, 0)), "https://placehold.co/600x400/2196F3/white?text=Bob+Post" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "EmailConfirmed", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "c26d504f-d046-42b2-8cf9-e9342a181720", new DateTimeOffset(new DateTime(2025, 11, 14, 1, 29, 26, 759, DateTimeKind.Unspecified).AddTicks(2430), new TimeSpan(0, 0, 0, 0, 0)), true, "ALICE@TEST.COM", "ALICE@TEST.COM", "AQAAAAIAAYagAAAAEEMGXvBf4iZJWSduXdYtglIHF+knyOhbj21m/VcyIUM2jWzG1covyMnHwPMWXJ78ww==", "da3089f9-8fa8-42f0-b457-1389196f7a83", "alice@test.com" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "EmailConfirmed", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "ca71dcd6-3b6e-45e9-9846-812a6f9a70f5", new DateTimeOffset(new DateTime(2025, 11, 19, 1, 29, 26, 805, DateTimeKind.Unspecified).AddTicks(3285), new TimeSpan(0, 0, 0, 0, 0)), true, "BOB@TEST.COM", "BOB@TEST.COM", "AQAAAAIAAYagAAAAEPsfLg67AwkS3HcQhLlsiymzzKBfKyHgAVASpF30xUJe+URckWwr446K0KfzntwNUA==", "baabe55c-1d4b-4905-80a2-48aadd4a75c3", "bob@test.com" });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedAt", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureUrl", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { 3, 0, "8f311642-bc43-4cfe-9ae8-da6d2833b5f6", new DateTimeOffset(new DateTime(2025, 11, 22, 1, 29, 26, 849, DateTimeKind.Unspecified).AddTicks(216), new TimeSpan(0, 0, 0, 0, 0)), "charlie@test.com", true, false, null, "Charlie Chat", "CHARLIE@TEST.COM", "CHARLIE@TEST.COM", "AQAAAAIAAYagAAAAEEqTog4WoAjBPuSeVHkFIkEkn9DG3A7KyXPb06tZI7qhhcOzPdyGs3MGE0pKKxKqNQ==", null, false, "https://placehold.co/100x100/FF9800/white?text=C", "da23659c-6134-4107-8e9e-e2e1367fbe2c", false, "charlie@test.com" });

            migrationBuilder.InsertData(
                table: "Friendships",
                columns: new[] { "Id", "ActionUserId", "CreatedAt", "Status", "UpdatedAt", "User1Id", "User2Id" },
                values: new object[] { 2, 2, new DateTimeOffset(new DateTime(2025, 11, 24, 0, 29, 26, 892, DateTimeKind.Unspecified).AddTicks(5678), new TimeSpan(0, 0, 0, 0, 0)), "PENDING", null, 2, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 1, 10, 12, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "https://placehold.co/100x100/2196F3/white?text=B" });

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "ImageUrl" },
                values: new object[] { new DateTimeOffset(new DateTime(2025, 1, 11, 14, 30, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "https://placehold.co/100x100/2196F3/white?text=B" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "EmailConfirmed", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "53724b19-0491-4cb2-a3cc-47eff02bb085", new DateTimeOffset(new DateTime(2025, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, null, null, "AQAAAAIAAYagAAAAENrrc8o9WrAWf69luRiKyUTX7c85/B9FUxr9ge54/lvT2zSxa3DhRWm00uS6HE8vig==", null, null });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "EmailConfirmed", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "772ad8eb-f36b-45b8-a02b-b9c99127a34c", new DateTimeOffset(new DateTime(2025, 1, 5, 11, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), false, null, null, "AQAAAAIAAYagAAAAEMBwcnjryGNkSWivlPofv6VmLV2y4zJOK/3OjIQsoxU2Dr9qKqxAJ+iTQvwnHZh5eQ==", null, null });
        }
    }
}
