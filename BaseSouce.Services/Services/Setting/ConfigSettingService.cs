using AutoMapper;
using BaseSouce.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.Setting;
using EFCore.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.Services.Services.Setting
{
    public class ConfigSettingService : IConfigSettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ConfigSettingService> _logger;
        private readonly IMapper _mapper;

        public ConfigSettingService(IUnitOfWork<BaseSourceDbContext> unitOfWork,
            ILogger<ConfigSettingService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<ConfigSettingVm> GetSettingAsync()
        {
            var _repository = _unitOfWork.GetRepository<ConfigSystem>();
            return await _repository.GetFirstOrDefaultAsync(
                selector: x => _mapper.Map<ConfigSettingVm>(x));
        }
    }
}
