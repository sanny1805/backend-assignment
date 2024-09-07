using AutoMapper;
using OT.Assessment.App.ViewModels;
using OT.Assessment.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OT.Assessment.Infrastructure
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CasinoWagerViewModel, CasinoWager>();
        }
    }
}
