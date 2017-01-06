using Belletrix.Entity.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;

namespace Belletrix.DAL
{
    public interface IDocumentRepository
    {
        Task<IEnumerable<DocumentViewModel>> GetActivityLogDocumentsList(int id);
        Task<bool> InsertActivityLogDocument(int activityLogId, int userId, HttpPostedFileBase document);
    }
}
