using System.Threading;

using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Core.Helpers
{
    public class LabelService : ILabelService
    {
        private readonly IYamlRepository yamlRepository;

        public string CurrentCultureName => Thread.CurrentThread.CurrentCulture.Name;

        public LabelService(IYamlRepository yamlRepository)
        {
            this.yamlRepository = yamlRepository;
        }

        public Metadata<TLabels> GetMetadata<TLabels>(string baseDirectory) where TLabels : new()
        {
            var yamlPath = $"{DirectoryHelper.GetExecutingDirectory()}\\{baseDirectory}\\Metadata\\{CurrentCultureName}.yaml";

            var labels = yamlRepository.Deserialize<Metadata<TLabels>>(yamlPath);

            return labels;
        }
    }
}