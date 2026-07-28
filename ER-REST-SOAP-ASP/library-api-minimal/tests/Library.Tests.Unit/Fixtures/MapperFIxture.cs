using AutoMapper;
using Library.ControllerApi.Mapping;
using Microsoft.Extensions.Logging.Abstractions;

namespace Library.Tests.Unit.Fixtures;

public class MapperFixture
{
    public IMapper Mapper { get; }

    public MapperFixture()
    {
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance);

        Mapper = config.CreateMapper();
    }
}