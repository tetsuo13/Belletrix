﻿using Bennett.AbroadAdvisor.Core;
using Npgsql;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Bennett.AbroadAdvisor.Models
{
    /// <summary>
    /// Student model related to promos.
    /// </summary>
    public class StudentPromoModel : StudentBaseModel, IStudentModel
    {
        [Display(Name = "Are you a Pell Grant Recipient?")]
        new public bool? PellGrantRecipient { get; set; }

        [Display(Name = "Do you have a passport?")]
        new public bool? HasPassport { get; set; }

        [Display(Name = "Desired Language Abroad")]
        public new IEnumerable<int> SelectedDesiredLanguages { get; set; }

        public new void Save(int? userId, string promoCode)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(Connections.Database.Dsn))
            {
                connection.ValidateRemoteCertificateCallback += Connections.Database.connection_ValidateRemoteCertificateCallback;
                connection.Open();

                using (NpgsqlTransaction transaction = connection.BeginTransaction())
                {
                    base.Save(connection, userId);

                    EventLogModel eventLog = new EventLogModel();
                    eventLog.Student = this;
                    eventLog.AddStudentEvent(connection, Id, EventLogModel.EventType.AddStudent);

                    StudentPromoLog log = new StudentPromoLog();
                    log.Save(connection, Id, promoCode);

                    transaction.Commit();
                }
            }
        }
    }
}
