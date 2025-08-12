using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeUi.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ConvertThreatEventsToHypertable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Ensure TimescaleDB extension exists
        migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS timescaledb;");

        // Convert existing table to hypertable (idempotent)
        migrationBuilder.Sql(@"SELECT create_hypertable('""ThreatEvents""','Timestamp', if_not_exists => TRUE, migrate_data => TRUE);");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // NOTE: Converting a hypertable back to a plain table is non-trivial and not performed automatically here.
        // If required, manual steps would be: SELECT * INTO copy, DROP TABLE, CREATE TABLE, COPY DATA BACK.
    }
}
