using ElectroPi.TaskManager.Application.Features.Tasks.DTOs;
using ElectroPi.TaskManager.Domain.Entities;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPi.TaskManager.Application.Common.Mappings
{
    public sealed class TaskMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ProjectTask, TaskDto>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.Priority, src => src.Priority.ToString())
                .Map(dest => dest.DueDate, src => src.DueDate)
                .Map(dest => dest.ProjectId, src => src.ProjectId)
                .Map(dest => dest.CreatedAt, src => src.CreatedAt)
                .Map(dest => dest.IsOverdue, src => src.IsOverdue());
        }
    }
}