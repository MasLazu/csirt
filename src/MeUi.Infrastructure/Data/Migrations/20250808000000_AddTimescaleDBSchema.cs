using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using System.Net;

#nullable disable

namespace MeUi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimescaleDBSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create ASN Info lookup table
            migrationBuilder.CreateTable(
                name: "asn_info",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    asn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asn_info", x => x.id);
                });

            // Create Countries lookup table
            migrationBuilder.CreateTable(
                name: "countries",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "char(2)", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_countries", x => x.id);
                });

            // Create Protocols lookup table
            migrationBuilder.CreateTable(
                name: "protocols",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_protocols", x => x.id);
                });

            // Create Malware Families lookup table
            migrationBuilder.CreateTable(
                name: "malware_families",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_malware_families", x => x.id);
                });

            // Create Threat Intelligence main table
            migrationBuilder.CreateTable(
                name: "threat_intelligence",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    timestamp = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    asn_id = table.Column<int>(type: "integer", nullable: false),
                    source_address = table.Column<IPAddress>(type: "inet", nullable: false),
                    source_country_id = table.Column<int>(type: "integer", nullable: true),
                    destination_address = table.Column<IPAddress>(type: "inet", nullable: true),
                    destination_country_id = table.Column<int>(type: "integer", nullable: true),
                    source_port = table.Column<int>(type: "integer", nullable: true),
                    destination_port = table.Column<int>(type: "integer", nullable: true),
                    protocol_id = table.Column<int>(type: "integer", nullable: true),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    malware_family_id = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "NOW()"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_threat_intelligence", x => new { x.id, x.timestamp });
                    table.ForeignKey(
                        name: "fk_threat_asn",
                        column: x => x.asn_id,
                        principalTable: "asn_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_threat_source_country",
                        column: x => x.source_country_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_threat_dest_country",
                        column: x => x.destination_country_id,
                        principalTable: "countries",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_threat_protocol",
                        column: x => x.protocol_id,
                        principalTable: "protocols",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_threat_malware_family",
                        column: x => x.malware_family_id,
                        principalTable: "malware_families",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            // Create unique indexes for lookup tables
            migrationBuilder.CreateIndex(
                name: "idx_asn_info_asn_unique",
                table: "asn_info",
                column: "asn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_countries_code_unique",
                table: "countries",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_protocols_name_unique",
                table: "protocols",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_malware_families_name_unique",
                table: "malware_families",
                column: "name",
                unique: true);

            // Create time-series optimized indexes for threat_intelligence
            migrationBuilder.CreateIndex(
                name: "idx_threat_timestamp_source",
                table: "threat_intelligence",
                columns: new[] { "timestamp", "source_address" });

            migrationBuilder.CreateIndex(
                name: "idx_threat_timestamp_asn",
                table: "threat_intelligence",
                columns: new[] { "timestamp", "asn_id" });

            // Create hash indexes for IP address lookups
            migrationBuilder.Sql("CREATE INDEX idx_threat_source_addr ON threat_intelligence USING hash (source_address);");
            migrationBuilder.Sql("CREATE INDEX idx_threat_dest_addr ON threat_intelligence USING hash (destination_address);");

            // Create category index with timestamp for filtering
            migrationBuilder.CreateIndex(
                name: "idx_threat_category_time",
                table: "threat_intelligence",
                columns: new[] { "category", "timestamp" });

            // Create country-based indexes for geographical queries
            migrationBuilder.CreateIndex(
                name: "idx_threat_source_country_time",
                table: "threat_intelligence",
                columns: new[] { "source_country_id", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "idx_threat_dest_country_time",
                table: "threat_intelligence",
                columns: new[] { "destination_country_id", "timestamp" });

            // Create composite indexes for common query patterns
            migrationBuilder.CreateIndex(
                name: "idx_threat_asn_category_time",
                table: "threat_intelligence",
                columns: new[] { "asn_id", "category", "timestamp" });

            migrationBuilder.CreateIndex(
                name: "idx_threat_country_protocol_time",
                table: "threat_intelligence",
                columns: new[] { "source_country_id", "protocol_id", "timestamp" });

            // Create soft delete index for BaseEntity
            migrationBuilder.Sql("CREATE INDEX idx_threat_deleted_at ON threat_intelligence (deleted_at) WHERE deleted_at IS NOT NULL;");

            // Create performance index for time range queries (descending)
            migrationBuilder.Sql("CREATE INDEX idx_threat_timestamp_desc ON threat_intelligence (timestamp DESC);");

            // Create filtered indexes for optional fields
            migrationBuilder.Sql("CREATE INDEX idx_threat_malware_time ON threat_intelligence (malware_family_id, timestamp) WHERE malware_family_id IS NOT NULL;");
            migrationBuilder.Sql("CREATE INDEX idx_threat_protocol_time ON threat_intelligence (protocol_id, timestamp) WHERE protocol_id IS NOT NULL;");
            migrationBuilder.Sql("CREATE INDEX idx_threat_ports_time ON threat_intelligence (source_port, destination_port, timestamp) WHERE source_port IS NOT NULL OR destination_port IS NOT NULL;");

            // Create TimescaleDB hypertable
            // This converts the regular PostgreSQL table into a TimescaleDB hypertable partitioned by timestamp
            migrationBuilder.Sql("SELECT create_hypertable('threat_intelligence', 'timestamp', chunk_time_interval => INTERVAL '1 day');");

            // Note: Continuous aggregates will be created separately after migration
            // due to PostgreSQL transaction limitations with CREATE MATERIALIZED VIEW

            // Set up compression policy for data older than 7 days
            migrationBuilder.Sql(@"
                ALTER TABLE threat_intelligence SET (
                    timescaledb.compress,
                    timescaledb.compress_segmentby = 'asn_id, source_country_id',
                    timescaledb.compress_orderby = 'timestamp DESC'
                );
            ");

            migrationBuilder.Sql("SELECT add_compression_policy('threat_intelligence', INTERVAL '7 days');");

            // Set up retention policy for data older than 2 years
            migrationBuilder.Sql("SELECT add_retention_policy('threat_intelligence', INTERVAL '2 years');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop retention and compression policies
            migrationBuilder.Sql("SELECT remove_retention_policy('threat_intelligence');");
            migrationBuilder.Sql("SELECT remove_compression_policy('threat_intelligence');");

            // Drop continuous aggregates
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS threat_daily_summary;");
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS threat_hourly_summary;");

            // Drop the main threat intelligence table (this will also drop the hypertable)
            migrationBuilder.DropTable(
                name: "threat_intelligence");

            // Drop lookup tables
            migrationBuilder.DropTable(
                name: "malware_families");

            migrationBuilder.DropTable(
                name: "protocols");

            migrationBuilder.DropTable(
                name: "countries");

            migrationBuilder.DropTable(
                name: "asn_info");
        }
    }
}