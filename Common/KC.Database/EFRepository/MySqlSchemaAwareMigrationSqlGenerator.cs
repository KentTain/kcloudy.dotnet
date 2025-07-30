
using KC.Framework.Tenant;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Migrations;
using System;
using System.Diagnostics.CodeAnalysis;

namespace KC.Database.EFRepository
{
    public class MySqlSchemaAwareMigrationSqlGenerator : MySqlMigrationsSqlGenerator
    {
        private string _schema;

        public MySqlSchemaAwareMigrationSqlGenerator([NotNullAttribute]MigrationsSqlGeneratorDependencies dependencies, [NotNullAttribute] IRelationalAnnotationProvider migrationsAnnotations, [NotNullAttribute] IMySqlOptions options, string schema)
            : base(dependencies, migrationsAnnotations, options)
        {
            _schema = schema;
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
        protected override void Generate(MigrationOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            base.Generate(operation, model, builder);
        }

        protected override void Generate(
            [NotNull] CreateTableOperation operation,
            IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            operation.Schema = _schema;
            foreach (var foreignKey in operation.ForeignKeys)
            {
                foreignKey.Schema = _schema;
                foreignKey.PrincipalSchema = _schema;
            }

            base.Generate(operation, model, builder, terminate);
        }

        protected override void Generate(AlterTableOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;

            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="AlterColumnOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            AlterColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;

            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RenameIndexOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RenameIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RestartSequenceOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RestartSequenceOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RenameTableOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RenameTableOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="CreateIndexOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        /// <param name="terminate"> Indicates whether or not to terminate the command after generating SQL for the operation. </param>
        protected override void Generate(
            CreateIndexOperation operation,
            IModel model,
            MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        /// /// <summary>
        ///     Ignored, since schemas are not supported by MySQL and are silently ignored to improve testing compatibility.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(EnsureSchemaOperation operation, IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Name = _schema;
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Ignored, since schemas are not supported by MySQL and are silently ignored to improve testing compatibility.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(DropSchemaOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            operation.Name = _schema;
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="CreateSequenceOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            CreateSequenceOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="MySqlCreateDatabaseOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            [NotNull] MySqlCreateDatabaseOperation operation,
            IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="MySqlDropDatabaseOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            [NotNull] MySqlDropDatabaseOperation operation,
            IModel model,
            [NotNull] MigrationCommandListBuilder builder)
        {
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="DropIndexOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            [NotNull] DropIndexOperation operation,
            IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate = true)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="DropUniqueConstraintOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />, and then terminates the final command.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            DropUniqueConstraintOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="DropForeignKeyOperation" /> by making calls on the given
        ///     <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        /// <param name="terminate"> Indicates whether or not to terminate the command after generating SQL for the operation. </param>
        protected override void Generate(
            [NotNull] DropForeignKeyOperation operation,
            IModel model,
            [NotNull] MigrationCommandListBuilder builder,
            bool terminate)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Builds commands for the given <see cref="RenameColumnOperation" />
        ///     by making calls on the given <see cref="MigrationCommandListBuilder" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to build the commands. </param>
        protected override void Generate(
            RenameColumnOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(AddPrimaryKeyOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        protected override void Generate(DropPrimaryKeyOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate = true)
        {
            operation.Schema = _schema;
            base.Generate(operation, model, builder);
        }

        /// <summary>
        ///     Generates a SQL fragment for a foreign key constraint of an <see cref="AddForeignKeyOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void ForeignKeyConstraint(
            AddForeignKeyOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            operation.Schema = _schema;
            base.ForeignKeyConstraint(operation, model, builder);
        }

        /// <summary>
        ///     Generates a SQL fragment for traits of an index from a <see cref="CreateIndexOperation" />,
        ///     <see cref="AddPrimaryKeyOperation" />, or <see cref="AddUniqueConstraintOperation" />.
        /// </summary>
        /// <param name="operation"> The operation. </param>
        /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
        /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
        protected override void IndexTraits(MigrationOperation operation, IModel model,
            MigrationCommandListBuilder builder)
        {
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
