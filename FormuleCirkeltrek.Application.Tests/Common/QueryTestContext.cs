using AutoMapper;
using FormuleCirkeltrek.Application.Common.Mappings;
using FormuleCirkeltrek.Persistence;
using System;
using System.Linq;

namespace FormuleCirkeltrek.Application.Tests.Common
{
    public class QueryTestContext : IDisposable
    {
        readonly  DbContextFactory _contextFactory;
        readonly IMapper _mapper;

        public QueryTestContext()
        {
            var configurationProvider = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = configurationProvider.CreateMapper();
            _contextFactory = new DbContextFactory();
        }

        public FormuleCirkeltrekDbContext GetNewDataLayer()
            => _contextFactory.Create();

        public IMapper GetMapper()
            => _mapper;

        public void Dispose()
        {
            _contextFactory.Dispose();
        }
    }
}
