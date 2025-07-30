using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace KC.Database.EFRepository
{
    public class SqlServerSchemaAwareMigrationSqlGenerator : SqlServerMigrationsSqlGenerator
    {
        protected string _schema;
        public SqlServerSchemaAwareMigrationSqlGenerator(
            MigrationsSqlGeneratorDependencies dependencies,
            IRelationalAnnotationProvider migrationsAnnotations,
            ICurrentDbContext context)
        : base(dependencies, migrationsAnnotations)
        {
            if (context.Context is MultiTenantDataContext)
            {
                _schema = ((MultiTenantDataContext)context.Context).TenantName;
            }
            //else
            //{
            //    _schema = TenantConstant.TestTenantName;
            //}
        }

        /// <summary>
        ///     <para>
        ///         Builds commands for the given <see cref="MigrationOperation" /> by making calls on the given
        ///         <see cref="MigrationCommandListBuilder" />.
        ///     </para>
        ///     <para>
        ///         This method uses a double-dispatch mechanism to call one of the 'Generate' methods that are
        ///         specific to a certain subtype of <see cref="MigrationOperation" />. Typically database providers
        ///         will override these specific methods rather than this method. However, providers can override
        ///         this methods to handle provider-specific operations.
        ///     </para>
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            MigrationOperation operation, 
            IModel model, 
            MigrationCommandListBuilder builder)
        {
            //operation.Schema = _schema;
            //Framework.Util.LogUtil.LogDebug("----MigrationOperation: " + this._schema);
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            AddUniqueConstraintOperation operation, 
            IModel model, 
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }
        protected override void Generate(
            DropUniqueConstraintOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            AddColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(
            AddForeignKeyOperation operation,
            IModel model,
            MigrationCommandListBuilder builder, 
            bool terminate = true)
        {
            operation.Schema = _schema;
            operation.PrincipalSchema = _schema;
            //Framework.Util.LogUtil.LogDebug("----MigrationOperation: " + this._schema);
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(
            AddPrimaryKeyOperation operation,
            IModel model,
            MigrationCommandListBuilder builder
            , bool terminate = true)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(
            AlterColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            RenameIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            RenameSequenceOperation operation, 
            IModel model, 
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            RestartSequenceOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            CreateTableOperation operation,
            IModel model,
            MigrationCommandListBuilder builder, 
            bool terminate = true)
        {
            operation.Schema = _schema;
            foreach(var foreignKey in operation.ForeignKeys)
            {
                foreignKey.Schema = _schema;
                foreignKey.PrincipalSchema = _schema;
            }
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(
            RenameTableOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            DropTableOperation operation, 
            IModel model, 
            MigrationCommandListBuilder builder, 
            bool terminate)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(
            CreateIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(
            DropPrimaryKeyOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(
            CreateSequenceOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            AlterTableOperation operation, 
            IModel model, 
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            DropForeignKeyOperation operation, 
            IModel model, 
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(
            DropIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(
            DropColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(
            RenameColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            SqlOperation operation, 
            IModel model, 
            MigrationCommandListBuilder builder)
        {
            operation.Sql = _GetNameWithReplacedSqlSchema(operation.Sql);
            //Framework.Util.LogUtil.LogDebug("---sql:" + operation.Sql);
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            InsertDataOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder, terminate);
        }

        private string _GetNameWithReplacedSchema(string name)
        {
            string[] nameParts = name.Split('.');
            string newName;

            switch (nameParts.Length)
            {
                case 1:
                    newName = string.Format("{0}.{1}", _schema, nameParts[0]);
                    break;

                case 2:
                    newName = string.Format("{0}.{1}", _schema, nameParts[1]);
                    break;

                case 3:
                    newName = string.Format("{0}.{1}.{2}", _schema, nameParts[1], nameParts[2]);
                    break;

                default:
                    throw new NotSupportedException();
            }

            return newName;
        }

        private string _GetNameWithReplacedSqlSchema(string sql)
        {
            return sql.Replace("[" + TenantConstant.TestTenantName + "]", "[" + _schema + "]")
                    .Replace("'" + TenantConstant.TestTenantName + "'", "'" + _schema + "'");
        }
    }
}
