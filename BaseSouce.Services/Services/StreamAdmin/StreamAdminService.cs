using AutoMapper;
using BaseSouce.Data.EF;
using BaseSource.Data.Entities;
using BaseSource.ViewModels.Stream;
using EFCore.UnitOfWork;
using EFCore.UnitOfWork.PageList;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseSouce.Services.Services.StreamAdmin
{
    public class StreamAdminService : IStreamAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StreamAdminService> _logger;
        private readonly IMapper _mapper;
        public StreamAdminService(
            IUnitOfWork<BaseSourceDbContext> unitOfWork,
            ILogger<StreamAdminService> logger,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;

        }
        public async Task<IPagedList<StreamAdminInfoDto>> GetByFilterAsync(StreamAdminRequestDto model)
        {
            var _repository = _unitOfWork.GetRepository<StreamHistory>();

            var query = _repository.Queryable().AsNoTracking();

            return await _repository.GetStagesPagedListAsync(
                stages: query,
                selector: x => _mapper.Map<StreamAdminInfoDto>(x),
                include: x => x.Include(i => i.AppUser),
               orderBy: x => x.OrderByDescending(i => i.CreatedTime),
               pageIndex: model.Page, pageSize: model.PageSize
                );
        }
    }
}
