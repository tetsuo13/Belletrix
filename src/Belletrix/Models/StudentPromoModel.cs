using Belletrix.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;

namespace Belletrix.Models
{
    /// <summary>
    /// Student model related to promos.
    /// </summary>
    /// <remarks>
    /// Some of the fields are copied from the base class just to provide a
    /// different display name than the base class defines.
    /// </remarks>
    public class StudentPromoModel : StudentBaseModel, IStudentModel
    {
        [Display(Name = "Are you a Pell Grant Recipient?")]
        new public bool? PellGrantRecipient { get; set; }

        [Display(Name = "Do you have a passport?")]
        new public bool? HasPassport { get; set; }

        [Display(Name = "Desired Language Abroad")]
        public new IEnumerable<int> SelectedDesiredLanguages { get; set; }

        public void Save(int? userId, string promoCode)
        {
            using (SqlConnection connection = new SqlConnection(Connections.Database.Dsn))
            {
                connection.Open();

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    base.Save(connection, transaction, userId);

                    EventLogModel eventLog = new EventLogModel();
                    eventLog.Student = this;
                    eventLog.AddStudentEvent(connection, transaction, Id, EventLogModel.EventType.AddStudent);

                    StudentPromoLog.Save(connection, transaction, Id, promoCode);

                    transaction.Commit();
                }
            }
        }
    }
}
