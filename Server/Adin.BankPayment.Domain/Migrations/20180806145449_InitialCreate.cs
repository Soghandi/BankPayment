using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Adin.BankPayment.Domain.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Application",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Status = table.Column<byte>(),
                    CreationDate = table.Column<DateTime>(),
                    CreatedBy = table.Column<int>(),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Title = table.Column<string>(maxLength: 32, nullable: true),
                    Description = table.Column<string>(maxLength: 256, nullable: true),
                    PublicKey = table.Column<string>(maxLength: 128, nullable: true),
                    PrivateKey = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Application", x => x.Id); });

            migrationBuilder.CreateTable(
                "Bank",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Status = table.Column<byte>(),
                    CreationDate = table.Column<DateTime>(),
                    CreatedBy = table.Column<int>(),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    Title = table.Column<string>(maxLength: 16, nullable: true),
                    PostUrl = table.Column<string>(maxLength: 64, nullable: true),
                    Code = table.Column<byte>()
                },
                constraints: table => { table.PrimaryKey("PK_Bank", x => x.Id); });

            migrationBuilder.CreateTable(
                "ApplicationBank",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Status = table.Column<byte>(),
                    CreationDate = table.Column<DateTime>(),
                    CreatedBy = table.Column<int>(),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    ApplicationId = table.Column<Guid>(),
                    BankId = table.Column<Guid>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationBank", x => x.Id);
                    table.ForeignKey(
                        "FK_ApplicationBank_Application_ApplicationId",
                        x => x.ApplicationId,
                        "Application",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_ApplicationBank_Bank_BankId",
                        x => x.BankId,
                        "Bank",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "Transaction",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Status = table.Column<byte>(),
                    CreationDate = table.Column<DateTime>(),
                    CreatedBy = table.Column<int>(),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    ApplicationId = table.Column<Guid>(),
                    BankId = table.Column<Guid>(),
                    Amount = table.Column<decimal>(),
                    Mobile = table.Column<long>(nullable: true),
                    UserTrackCode = table.Column<string>(maxLength: 64, nullable: true),
                    BankTrackCode = table.Column<string>(maxLength: 64, nullable: true),
                    ReferenceNumber = table.Column<string>(maxLength: 64, nullable: true),
                    BankErrorCode = table.Column<int>(),
                    BankRedirectUrl = table.Column<string>(maxLength: 128, nullable: true),
                    CallbackUrl = table.Column<string>(maxLength: 128, nullable: true),
                    ExpirationTime = table.Column<DateTime>(nullable: true),
                    BankErrorMessage = table.Column<string>(maxLength: 64, nullable: true),
                    ErrorCode = table.Column<byte>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                    table.ForeignKey(
                        "FK_Transaction_Application_ApplicationId",
                        x => x.ApplicationId,
                        "Application",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        "FK_Transaction_Bank_BankId",
                        x => x.BankId,
                        "Bank",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                "ApplicationBankParam",
                table => new
                {
                    Id = table.Column<Guid>(),
                    Status = table.Column<byte>(),
                    CreationDate = table.Column<DateTime>(),
                    CreatedBy = table.Column<int>(),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false, defaultValue: false),
                    ApplicationBankId = table.Column<Guid>(),
                    ParamKey = table.Column<string>(maxLength: 32, nullable: true),
                    ParamValue = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationBankParam", x => x.Id);
                    table.ForeignKey(
                        "FK_ApplicationBankParam_ApplicationBank_ApplicationBankId",
                        x => x.ApplicationBankId,
                        "ApplicationBank",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                "Bank",
                new[]
                {
                    "Id", "Code", "CreatedBy", "CreationDate", "IsDeleted", "ModifiedBy", "ModifiedOn", "PostUrl",
                    "RowVersion", "Status", "Title"
                },
                new object[]
                {
                    new Guid("482a591e-7536-4f47-a544-e9d4342586bd"), (byte) 1, 1,
                    new DateTime(2018, 8, 6, 19, 24, 49, 523, DateTimeKind.Local), false, null, null,
                    "https://sep.shaparak.ir/MobilePG/MobilePayment", null, (byte) 0, "سامان"
                });

            migrationBuilder.InsertData(
                "Bank",
                new[]
                {
                    "Id", "Code", "CreatedBy", "CreationDate", "IsDeleted", "ModifiedBy", "ModifiedOn", "PostUrl",
                    "RowVersion", "Status", "Title"
                },
                new object[]
                {
                    new Guid("ab3f226a-be56-4092-bbd0-2ae8ffbce131"), (byte) 2, 1,
                    new DateTime(2018, 8, 6, 19, 24, 49, 525, DateTimeKind.Local), false, null, null,
                    "https://pec.shaparak.ir/pecpaymentgateway/default.aspx?au={0}", null, (byte) 0, "پارسیان"
                });

            migrationBuilder.InsertData(
                "Bank",
                new[]
                {
                    "Id", "Code", "CreatedBy", "CreationDate", "IsDeleted", "ModifiedBy", "ModifiedOn", "PostUrl",
                    "RowVersion", "Status", "Title"
                },
                new object[]
                {
                    new Guid("98504148-3d89-4abb-9fb5-281bed8714e3"), (byte) 3, 1,
                    new DateTime(2018, 8, 6, 19, 24, 49, 525, DateTimeKind.Local), false, null, null,
                    "https://bpm.shaparak.ir/pgwchannel/startpay.mellat", null, (byte) 0, "ملت"
                });

            migrationBuilder.CreateIndex(
                "IX_ApplicationBank_ApplicationId",
                "ApplicationBank",
                "ApplicationId");

            migrationBuilder.CreateIndex(
                "IX_ApplicationBank_BankId",
                "ApplicationBank",
                "BankId");

            migrationBuilder.CreateIndex(
                "IX_ApplicationBankParam_ApplicationBankId",
                "ApplicationBankParam",
                "ApplicationBankId");

            migrationBuilder.CreateIndex(
                "IX_Transaction_ApplicationId",
                "Transaction",
                "ApplicationId");

            migrationBuilder.CreateIndex(
                "IX_Transaction_BankId",
                "Transaction",
                "BankId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "ApplicationBankParam");

            migrationBuilder.DropTable(
                "Transaction");

            migrationBuilder.DropTable(
                "ApplicationBank");

            migrationBuilder.DropTable(
                "Application");

            migrationBuilder.DropTable(
                "Bank");
        }
    }
}