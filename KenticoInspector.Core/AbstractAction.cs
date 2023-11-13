using KenticoInspector.Core.Models;
using KenticoInspector.Core.Modules;
using KenticoInspector.Core.Services.Interfaces;

using Newtonsoft.Json;

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

        public ActionResults Execute(string OptionsJson) {
            try
            {
                var options = JsonConvert.DeserializeObject<TOptions>(OptionsJson);
                return Execute(options);
            }
            catch
            {
                return GetInvalidOptionsResult();
            }
        }

        public abstract ActionResults Execute(TOptions Options);

        public abstract ActionResults GetInvalidOptionsResult();
    }
}