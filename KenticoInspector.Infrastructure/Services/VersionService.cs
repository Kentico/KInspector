using System;
using System.Collections.Generic;

using KenticoInspector.Core;
using KenticoInspector.Core.Models;
using KenticoInspector.Core.Repositories.Interfaces;
using KenticoInspector.Core.Services.Interfaces;

namespace KenticoInspector.Infrastructure.Services
{
    public class VersionService : IVersionService
    {
        private readonly IVersionRepository versionRepository;

        public VersionService(IVersionRepository versionRepository)
        {
            this.versionRepository = versionRepository;
        }

        public string GetCoreProductVersion()
        {
            var coreV = versionRepository.GetCoreProductVersion();
            return coreV ?? "";
        }
    }
}