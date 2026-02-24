using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Promotions.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixCodDivSchemaV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This migration used to alter CODDIV on TB0042RELATIONS_CUST,
            // TA8012PARTICIPANTS and TA501DELIVERYPOINTS. Those schema changes
            // are now handled safely in later migrations (especially
            // FixCodDivSchemaFinal) which drop/recreate the necessary
            // foreign keys and indexes first.
            //
            // Keeping the original operations here causes runtime failures
            // on databases that have already been renamed or don't yet have
            // the intermediate table/index names (e.g. TA8012PARTICIPANTS).
            //
            // To avoid double-applying the same schema change and to allow
            // migrations to run cleanly on all environments, this migration
            // is now intentionally a NO-OP.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op: Up no longer performs any schema changes, so there is
            // nothing to roll back here. The real schema is defined by the
            // later FixCodDivSchemaFinal migration.
        }
    }
}
