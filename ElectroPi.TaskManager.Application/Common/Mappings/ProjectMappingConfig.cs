using ElectroPi.TaskManager.Application.Features.Projects.DTOs;
using ElectroPi.TaskManager.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Mappings
{
    public sealed class ProjectMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Project, ProjectDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.TaskCount, src => src.Tasks.Count);
        }
    }
    }
