using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

using Newtonsoft.Json;
using System.Reflection;

namespace KenticoInspector.Core
{
    public abstract class AbstractAction<TTerms,TOptions>
        : AbstractModule<TTerms>, IAction
        where TTerms : new()
        where TOptions: new()
    {
        public TOptions Options => new TOptions();

        protected AbstractAction(IModuleMetadataService moduleMetadataService)
            : base(moduleMetadataService) { }

        public ActionResults Execute(string optionsJson) {
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

                if (!ValidateOptions(options))
                {
                    return GetInvalidOptionsResult();
                }

                return Execute(options);
            }
            catch
            {
                return GetInvalidOptionsResult();
            }
        }

        /// <summary>
        /// Executed when all options are populated and validated.
        /// </summary>
        public abstract ActionResults Execute(TOptions options);

        /// <summary>
        /// Executed when at least one option is populated, but not validated.
        /// This could be a valid scenario where an option is hidden and not needed when another option
        /// is set to a specific value.
        /// </summary>
        public abstract ActionResults ExecutePartial(TOptions options);

        /// <summary>
        /// Executed when no options are provided. Should display a list of data that can be modified by
        /// the action.
        /// </summary>
        public abstract ActionResults ExecuteListing();

        /// <summary>
        /// Returned when <see cref="ValidateOptions(TOptions)"/> fails, or there is an exception during
        /// <see cref="Execute(TOptions)"/>.
        /// </summary>
        /// <returns></returns>
        public abstract ActionResults GetInvalidOptionsResult();

        /// <summary>
        /// Returns <c>true</c> if all options are valid before calling <see cref="Execute(TOptions)"/>.
        /// </summary>
        public abstract bool ValidateOptions(TOptions options);

        /// <summary>
        /// Returns <c>true</c> if at least one option has a value and one doesn't.
        /// </summary>
        private bool OptionsPartial(TOptions options)
        {
            var hasNull = false;
            var hasValue = false;
            PropertyInfo[] properties = typeof(TOptions).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(options) == null)
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
        private bool OptionsNull(TOptions options)
        {
            PropertyInfo[] properties = typeof(TOptions).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.GetValue(options) != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}