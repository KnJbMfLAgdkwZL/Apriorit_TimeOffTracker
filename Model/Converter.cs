using TimeOffTracker.Model.DTO;
using TimeOffTracker.Model.Enum;

namespace TimeOffTracker.Model
{
    public static class Converter
    {
        public static Request DtoToEntity(RequestDto dto)
        {
            return new Request()
            {
                Id = dto.Id,
                RequestTypeId = (int) dto.RequestTypeId,
                Reason = dto.Reason,
                ProjectRoleComment = dto.ProjectRoleComment,
                ProjectRoleTypeId = (int) dto.ProjectRoleTypeId,
                UserId = dto.UserId,
                StateDetailId = (int) dto.StateDetailId,
                DateTimeFrom = dto.DateTimeFrom,
                DateTimeTo = dto.DateTimeTo
            };
        }

        public static User DtoToEntity(UserDto dto)
        {
            return new User()
            {
                Id = dto.Id,
                Email = dto.Email,
                Login = dto.Login,
                FirstName = dto.FirstName,
                SecondName = dto.SecondName,
                Password = dto.Password,
                RoleId = (int) dto.RoleId,
                Deleted = dto.Deleted
            };
        }

        public static UserSignature DtoToEntity(UserSignatureDto dto)
        {
            return new UserSignature()
            {
                Id = dto.Id,
                NInQueue = dto.NInQueue,
                RequestId = dto.RequestId,
                UserId = dto.UserId,
                Approved = dto.Approved,
                Deleted = dto.Deleted
            };
        }

        public static RequestDto EntityToDto(Request entity)
        {
            return new RequestDto()
            {
                Id = entity.Id,
                RequestTypeId = (RequestTypes) entity.RequestTypeId,
                Reason = entity.Reason,
                ProjectRoleComment = entity.ProjectRoleComment,
                ProjectRoleTypeId = (ProjectRoleTypes) entity.ProjectRoleTypeId,
                UserId = entity.UserId,
                StateDetailId = (StateDetails) entity.StateDetailId,
                DateTimeFrom = entity.DateTimeFrom,
                DateTimeTo = entity.DateTimeTo
            };
        }

        public static UserDto EntityToDto(User entity)
        {
            return new UserDto()
            {
                Id = entity.Id,
                Email = entity.Email,
                Login = entity.Login,
                FirstName = entity.FirstName,
                SecondName = entity.SecondName,
                Password = entity.Password,
                RoleId = (UserRoles) entity.RoleId,
                Deleted = entity.Deleted
            };
        }

        public static UserSignatureDto EntityToDto(UserSignature entity)
        {
            return new UserSignatureDto()
            {
                Id = entity.Id,
                NInQueue = entity.NInQueue,
                RequestId = entity.RequestId,
                UserId = entity.UserId,
                Approved = entity.Approved,
                Deleted = entity.Deleted
            };
        }
    }
}