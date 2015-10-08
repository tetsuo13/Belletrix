using Belletrix.Core;
using Belletrix.DAL;
using Belletrix.Domain;
using Microsoft.Practices.Unity;
using System;

namespace Belletrix.App_Start
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

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
