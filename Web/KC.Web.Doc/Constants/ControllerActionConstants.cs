using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using KC.Web.Constants;

namespace KC.Web.Doc.Constants
{
    public sealed class ControllerName : ControllerNameBase
    {
        public const string Document = "Document";
        public const string DocTemplate = "DocTemplate";
    }
    public sealed class ActionName : ActionNameBase
    {
        public sealed class Document
        {
            public const string LoadDocCategoryTree = "LoadDocCategoryTree";
            public const string RemoveDocCategory = "RemoveDocCategory";
            public const string GetDocCategory = "GetDocCategory";
            public const string SaveDocCategory = "SaveDocCategory";

            public const string FindDocuments = "FindDocuments";
            public const string GetDocumentForm = "GetDocumentForm";
            public const string SaveDocument = "SaveDocument";
            public const string DeleteDocument = "DeleteDocument";
            public const string MoveDocumentsToCategory = "MoveDocumentsToCategory";

            public const string GetUserTree = "GetUserTree";
            public const string GetOrganizationTree = "GetOrganizationTree";
            public const string GetRoleTree = "GetRoleTree";
            public const string DownloadDocument = "DownloadDocument";
            public const string GetSendEmailForm = "GetSendEmailForm";

            public const string ExistCategoryName = "ExistCategoryName";

            public const string DocumentLog = "DocumentLog";
            public const string LoadDocumentLogList = "LoadDocumentLogList";
        }

        public sealed class DocTemplate
        {
            public const string FindDocTemplates = "FindDocTemplates";
            public const string GetDocTemplateForm = "GetDocTemplateForm";
            public const string SaveDocTemplate = "SaveDocTemplate";
            public const string DeleteDocTemplate = "DeleteDocTemplate";

            public const string GetUserTree = "GetUserTree";
            public const string GetOrganizationTree = "GetOrganizationTree";
            public const string GetRoleTree = "GetRoleTree";
            public const string DownloadDocTemplate = "DownloadDocTemplate";
            public const string GetSendEmailForm = "GetSendEmailForm";

            public const string DocTemplateLog = "DocTemplateLog";
            public const string LoadDocTemplateLogList = "LoadDocTemplateLogList";
        }
    }
}