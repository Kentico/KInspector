using KInspector.Core.Models;
using KInspector.Core.Modules;
using KInspector.Core.Services.Interfaces;

using Newtonsoft.Json;

using System.Reflection;

namespace KInspector.Core
{
    public abstract class AbstractAction<TTerms,TOptions> : AbstractModule<TTerms>, IAction
        where TTerms : new()
        where TOptions: new()
    {
        public TOptions Options => new();

        protected AbstractAction(IModuleMetadataService moduleMetadataService)
            : base(moduleMetadataService) { }

        public Task<ModuleResults> Execute(string optionsJson)
        {
            try
            {
                var options = JsonConvert.DeserializeObject<TOptions>(optionsJson);
                if (OptionsNull(options))
                {
                    return ExecuteListing();
                }

                if (OptionsPartial(options))
                {
                    return ExecutePartial(options);
                }

                return Execute(options);
            }
            catch
            {
                return GetInvalidOptionsResult();
            }
        }

        public Type GetOptionsType()
        {
            return typeof(TOptions);
        }

        /// <summary>
        /// Executed when all options are populated.
        /// </summary>
        public abstract Task<ModuleResults> Execute(TOptions? options);

        /// <summary>
        /// Executed when at least one option has a value and one doesn't.
        /// This could be a valid scenario where an option is hidden and not needed when another option
        /// is set to a specific value.
        /// </summary>
        public abstract Task<ModuleResults> ExecutePartial(TOptions? options);

        /// <summary>
        /// Executed when no options are provided. Should display a list of data that can be modified by
        /// the action.
        /// </summary>
        public abstract Task<ModuleResults> ExecuteListing();

        public abstract Task<ModuleResults> GetInvalidOptionsResult();

        /// <summary>
        /// Returns <c>true</c> if at least one option has a value and one doesn't.
        /// </summary>
        private static bool OptionsPartial(TOptions? options)
        {
            var hasNull = false;
            var hasValue = false;
            PropertyInfo[] properties = typeof(TOptions).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(options) is null)
                {
                    hasNull = true;
                }
                else
                {
                    hasValue = true;
                }
            }

            return hasNull && hasValue;
        }

        /// <summary>
        /// Returns <c>true</c> if all options are null.
        /// </summary>
        private static bool OptionsNull(TOptions? options)
        {
            PropertyInfo[] properties = typeof(TOptions).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(options) is not null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
