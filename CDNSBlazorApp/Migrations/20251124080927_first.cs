using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CDNSBlazorApp.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "INSTITUATION",
                columns: table => new
                {
                    INSTITUATION_ID = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    INSTITUATION_NAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CONTACT_EMAIL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CREATED_AT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CREATED_AT_VALUE_DATE = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INSTITUATION", x => x.INSTITUATION_ID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instrument",
                columns: table => new
                {
                    INSTRUMENT_ID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NAME = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LONG_NAME = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ORIGINAL_INST_ID = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ISIN_CODE = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    CD_DUR_CAT = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CD_LO_STATUS = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    D_SIGN = table.Column<DateTime>(type: "datetime2", nullable: true),
                    D_FINAL_MATURITY = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MATURITY_PERIOD = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    AMT = table.Column<decimal>(type: "decimal(21,3)", precision: 21, scale: 3, nullable: false),
                    AMT_DISCOUNTED = table.Column<decimal>(type: "decimal(21,3)", precision: 21, scale: 3, nullable: false),
                    CU_BASE = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    AMT_NET = table.Column<decimal>(type: "decimal(21,3)", precision: 21, scale: 3, nullable: false),
                    CD_DEBT_SOURCE = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CD_DEBT_TYPE = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CD_INSTRUMENT_TYPE = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CD_DEBT_SEC_INTR_TYPE = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CD_REORG_GRP = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CD_LO_PRP = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CD_ECON_SECT = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CD_SOURCE_MODULE = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CD_USER_CODE_1 = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    INSTITUATION_ID = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    CREDITOR_ID = table.Column<int>(type: "int", nullable: true),
                    DEBTOR_ID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instrument", x => x.INSTRUMENT_ID);
                    table.ForeignKey(
                        name: "FK_Instrument_INSTITUATION_INSTITUATION_ID",
                        column: x => x.INSTITUATION_ID,
                        principalTable: "INSTITUATION",
                        principalColumn: "INSTITUATION_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PAYMENTS",
                columns: table => new
                {
                    PaymentId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentTraNo = table.Column<int>(type: "int", nullable: false),
                    SCH_PAYMENT_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MADE_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RECEIVED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LOCAL_EXCH_RATE_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CD_PAYMENT_MODE = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    AMOUNT = table.Column<decimal>(type: "decimal(21,3)", precision: 21, scale: 3, nullable: false),
                    CU_BASE = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CD_TRANSACTION_TYPE = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CD_AMOUNT_DIFF = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PAYMENTS", x => new { x.PaymentId, x.PaymentTraNo });
                    table.ForeignKey(
                        name: "FK_PAYMENTS_Instrument_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Instrument",
                        principalColumn: "INSTRUMENT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    SerieId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SerieTraNo = table.Column<int>(type: "int", nullable: false),
                    SERIE_AMT = table.Column<decimal>(type: "decimal(21,3)", precision: 21, scale: 3, nullable: false),
                    SERIE_CURRENCY = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CD_REORG_GRP = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => new { x.SerieId, x.SerieTraNo });
                    table.ForeignKey(
                        name: "FK_Series_Instrument_SerieId",
                        column: x => x.SerieId,
                        principalTable: "Instrument",
                        principalColumn: "INSTRUMENT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SERIES_PATTERN",
                columns: table => new
                {
                    SeriePatId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SeriePatTraNo = table.Column<int>(type: "int", nullable: false),
                    CD_PAT_TYPE = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CD_PERIODICITY = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    CG_PERIODICITY = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    D_FIRST_PAYMENT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    D_LAST_PAYMENT = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AMT = table.Column<decimal>(type: "decimal(21,3)", precision: 21, scale: 3, nullable: true),
                    PERCENTAGE = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SERIES_PATTERN", x => new { x.SeriePatId, x.SeriePatTraNo });
                    table.ForeignKey(
                        name: "FK_SERIES_PATTERN_Instrument_SeriePatId",
                        column: x => x.SeriePatId,
                        principalTable: "Instrument",
                        principalColumn: "INSTRUMENT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SUBSCRIPTIONS",
                columns: table => new
                {
                    SubscriptionId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SubscriptionIdTraNo = table.Column<int>(type: "int", nullable: false),
                    TR_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RECEIVED_DATE = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CD_TRANSACTION_TYPE = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    LOCAL_EXCHANGE_DATE = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CU_BASE = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    RECEIPTS_AMOUNT = table.Column<decimal>(type: "decimal(21,3)", precision: 21, scale: 3, nullable: false),
                    CD_EXEC_MODE = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SUBSCRIPTIONS", x => new { x.SubscriptionId, x.SubscriptionIdTraNo });
                    table.ForeignKey(
                        name: "FK_SUBSCRIPTIONS_Instrument_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Instrument",
                        principalColumn: "INSTRUMENT_ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Instrument_INSTITUATION_ID",
                table: "Instrument",
                column: "INSTITUATION_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "PAYMENTS");

            migrationBuilder.DropTable(
                name: "Series");

            migrationBuilder.DropTable(
                name: "SERIES_PATTERN");

            migrationBuilder.DropTable(
                name: "SUBSCRIPTIONS");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Instrument");

            migrationBuilder.DropTable(
                name: "INSTITUATION");
        }
    }
}
