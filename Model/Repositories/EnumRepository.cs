using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Extensions;
using TimeOffTracker.Model.DTO;
using TimeOffTracker.Model.Enum;

namespace TimeOffTracker.Model.Repositories
{
    public class EnumRepository
    {
        public bool Contains<T>(T val) where T : System.Enum
        {
            return System.Enum.IsDefined(typeof(T), val.ToString() ?? string.Empty);
        }

        public EnumDto GetById<T>(int id) where T : System.Enum
        {
            if (!System.Enum.IsDefined(typeof(T), id))
            {
                return null;
            }

            return new EnumDto()
            {
                Id = id,
                Type = System.Enum.Parse(typeof(UserRoles), id.ToString(), ignoreCase: true).ToString()
            };
        }

        public List<EnumDto> GetAll<T>() where T : System.Enum
        {
            return (System.Enum.GetValues(typeof(T))
                .Cast<object>()
                .Select(v => new EnumDto() {Id = (int) v, Type = v.ToString()})).ToList();
        }
    }
}