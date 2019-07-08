using KenticoInspector.Core.Constants;
using System.Collections.Generic;

namespace KenticoInspector.Core.Services.Interfaces
{
    public interface ICmsFileService : IService
    {
        Dictionary<string, string> GetResourceStringsFromResx(string instanceRoot, string relativeResxFilePath = DefaultKenticoPaths.PrimaryResxFile);
    }
}
