using Belletrix.Core;
using Belletrix.Entity.ViewModel;
using Dapper;
using StackExchange.Exceptional;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Belletrix.DAL
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly IUnitOfWork UnitOfWork;

        public DocumentRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DocumentViewModel>> GetActivityLogDocumentsList(int id)
        {
            const string sql = @"
                SELECT  [Guid] AS PublicId, [Title], [Size], [MimeType], [Created]
                FROM    [dbo].[Documents]
                WHERE   [ActivityLogId] = @Id AND
                        [Deleted] IS NULL";

            try
            {
                List<DocumentViewModel> documents = (await UnitOfWork.Context().QueryAsync<DocumentViewModel>(sql,
                    new { Id = id })).ToList();
                documents.ForEach(x => x.Created = DateTimeFilter.UtcToLocal(x.Created));
                return documents;
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return Enumerable.Empty<DocumentViewModel>();
        }

        public async Task<bool> InsertActivityLogDocument(int activityLogId, int userId,
            HttpPostedFileBase document)
        {
            const string sql = @"
                UPDATE  [dbo].[Documents]
                SET     [Content] = @Content,
                        [Size] = @Size,
                        [MimeType] = @MimeType,
                        [LastModified] = @LastModified,
                        [LastModifiedBy] = @LastModifiedBy
                WHERE   [ActivityLogId] = @ActivityLogId AND
                        [Title] = @Title

                IF @@ROWCOUNT = 0
                    INSERT INTO [dbo].[Documents]
                    (
                        [Guid], [Created], [CreatedBy], [ActivityLogId],
                        [Title], [Size], [MimeType], [Content]
                    )
                    VALUES
                    (
                        @Guid, @Created, @CreatedBy, @ActivityLogId,
                        @Title, @Size, @MimeType, @Content
                    );";

            try
            {
                await UnitOfWork.Context().ExecuteAsync(sql, new
                {
                    Content = GetDocumentFromPostedFileBase(document.InputStream),
                    Size = document.ContentLength, // TODO: This is always inserted/updated as zero
                    ActivityLogId = activityLogId,
                    Title = document.FileName,
                    Guid = Guid.NewGuid(),
                    Created = DateTime.Now.ToUniversalTime(),
                    CreatedBy = userId,
                    MimeType = document.ContentType,
                    LastModified = DateTime.Now.ToUniversalTime(),
                    LastModifiedBy = userId
                });

                return true;
            }
            catch (Exception e)
            {
                e.Data["SQL"] = sql;
                ErrorStore.LogException(e, HttpContext.Current);
            }

            return false;
        }

        private byte[] GetDocumentFromPostedFileBase(Stream fileStream)
        {
            using (BinaryReader reader = new BinaryReader(fileStream))
            {
                return reader.ReadBytes((int)fileStream.Length);
            }
        }
    }
}
