using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace JSONCapital.Data.Migrations
{
    public partial class UpdateTradeTableKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Trades",
                table: "Trades");

            migrationBuilder.AlterColumn<string>(
                name: "TradeType",
                table: "Trades",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Trades_CoinTrackingTradeID_TradeID",
                table: "Trades",
                columns: new[] { "CoinTrackingTradeID", "TradeID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trades",
                table: "Trades",
                columns: new[] { "TradeID", "CoinTrackingTradeID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Trades_CoinTrackingTradeID_TradeID",
                table: "Trades");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trades",
                table: "Trades");

            migrationBuilder.AlterColumn<int>(
                name: "TradeType",
                table: "Trades",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trades",
                table: "Trades",
                column: "TradeID");
        }
    }
}
