using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeUi.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateContinuousAggregates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create continuous aggregates for performance optimization
            // Note: These operations need to be run outside of a transaction

            // Drop existing hourly summary if exists
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS threat_hourly_summary;");

            // Create hourly summary continuous aggregate
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW threat_hourly_summary
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket('1 hour', timestamp) AS hour,
                    asn_id,
                    source_country_id,
                    category,
                    COUNT(*) as threat_count,
                    COUNT(DISTINCT source_address) as unique_sources
                FROM threat_intelligence
                GROUP BY hour, asn_id, source_country_id, category;
            ", suppressTransaction: true);

            // Add refresh policy for continuous aggregate
            migrationBuilder.Sql(@"
                SELECT add_continuous_aggregate_policy('threat_hourly_summary',
                    start_offset => INTERVAL '1 day',
                    end_offset => INTERVAL '1 hour',
                    schedule_interval => INTERVAL '1 hour');
            ");

            // Drop existing daily summary if exists
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS threat_daily_summary;");

            // Create daily summary continuous aggregate
            migrationBuilder.Sql(@"
                CREATE MATERIALIZED VIEW threat_daily_summary
                WITH (timescaledb.continuous) AS
                SELECT
                    time_bucket('1 day', timestamp) AS day,
                    asn_id,
                    source_country_id,
                    category,
                    COUNT(*) as threat_count,
                    COUNT(DISTINCT source_address) as unique_sources,
                    MIN(timestamp) as first_seen,
                    MAX(timestamp) as last_seen
                FROM threat_intelligence
                GROUP BY day, asn_id, source_country_id, category;
            ", suppressTransaction: true);

            // Add refresh policy for daily summary
            migrationBuilder.Sql(@"
                SELECT add_continuous_aggregate_policy('threat_daily_summary',
                    start_offset => INTERVAL '7 days',
                    end_offset => INTERVAL '1 day',
                    schedule_interval => INTERVAL '1 day');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop continuous aggregates
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS threat_daily_summary;");
            migrationBuilder.Sql("DROP MATERIALIZED VIEW IF EXISTS threat_hourly_summary;");
        }
    }
}