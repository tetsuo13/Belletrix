using Belletrix.Core;
using Belletrix.DAL;
using Belletrix.Domain;
using Belletrix.Infrastructure;
using Microsoft.Practices.Unity;
using System.Web.Mvc;

namespace Belletrix.App_Start
{
    public static class UnityConfig
    {
        public static void ConfigureIocUnityContainer()
        {
            IUnityContainer container = new UnityContainer();
            registerServices(container);
            DependencyResolver.SetResolver(new BelletrixDependencyResolver(container));
        }

        private static void registerServices(IUnityContainer container)
        {
            // One concrete instance of IUnitOfWork per request. This provides
            // scope transactional support on a per-request basis.
            container.RegisterType<IUnitOfWork, UnitOfWork>(new HierarchicalLifetimeManager(),
                new InjectionConstructor(Connections.Database.Dsn));

            container.RegisterType<IActivityService, ActivityService>();
            container.RegisterType<IEventLogService, EventLogService>();
            container.RegisterType<IPingService, PingService>();
            container.RegisterType<IPromoService, PromoService>();
            container.RegisterType<IStudentNoteService, StudentNoteService>();
            container.RegisterType<IStudentService, StudentService>();
            container.RegisterType<IStudyAbroadService, StudyAbroadService>();
            container.RegisterType<IUserService, UserService>();

            container.RegisterType<IActivityLogRepository, ActivityLogRepository>();
            container.RegisterType<IActivityLogPersonRepository, ActivityLogPersonRepository>();
            container.RegisterType<IEventLogRepository, EventLogRepository>();
            container.RegisterType<IPingRepository, PingRepository>();
            container.RegisterType<IPromoRepository, PromoRepository>();
            container.RegisterType<IStudentNoteRepository, StudentNoteRepository>();
            container.RegisterType<IStudentRepository, StudentRepository>();
            container.RegisterType<IStudyAbroadRepository, StudyAbroadRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
        }
    }
}
