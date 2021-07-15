using TimeOffTracker.Model.DTO;

namespace TimeOffTracker.Model
{
    public static class Converter
    {
        public static ProjectRoleType DtoToEntity(ProjectRoleTypeDto dto)
        {
            return new ProjectRoleType()
            {
                Id = dto.Id,
                Type = dto.Type,
                Comments = dto.Comments,
                Deleted = dto.Deleted
            };
        }

        public static Request DtoToEntity(RequestDto dto)
        {
            return new Request()
            {
                Id = dto.Id,
                RequestTypeId = dto.RequestTypeId,
                Reason = dto.Reason,
                ProjectRoleComment = dto.ProjectRoleComment,
                ProjectRoleTypeId = dto.ProjectRoleTypeId,
                UserId = dto.UserId,
                StateDetailId = dto.StateDetailId,
                DateTimeFrom = dto.DateTimeFrom,
                DateTimeTo = dto.DateTimeTo
            };
        }

        public static RequestType DtoToEntity(RequestTypeDto dto)
        {
            return new RequestType()
            {
                Id = dto.Id,
                Type = dto.Type,
                Comments = dto.Comments,
                Deleted = dto.Deleted
            };
        }

        public static StateDetail DtoToEntity(StateDetailDto dto)
        {
            return new StateDetail()
            {
                Id = dto.Id,
                Type = dto.Type,
                Comments = dto.Comments,
                Deleted = dto.Deleted
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
                RoleId = dto.RoleId,
                Deleted = dto.Deleted
            };
        }

        public static UserRole DtoToEntity(UserRoleDto dto)
        {
            return new UserRole()
            {
                Id = dto.Id,
                Type = dto.Type,
                Comments = dto.Comments,
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

        public static ProjectRoleTypeDto EntityToDto(ProjectRoleType entity)
        {
            return new ProjectRoleTypeDto()
            {
                Id = entity.Id,
                Type = entity.Type,
                Comments = entity.Comments,
                Deleted = entity.Deleted
            };
        }

        public static RequestDto EntityToDto(Request entity)
        {
            return new RequestDto()
            {
                Id = entity.Id,
                RequestTypeId = entity.RequestTypeId,
                Reason = entity.Reason,
                ProjectRoleComment = entity.ProjectRoleComment,
                ProjectRoleTypeId = entity.ProjectRoleTypeId,
                UserId = entity.UserId,
                StateDetailId = entity.StateDetailId,
                DateTimeFrom = entity.DateTimeFrom,
                DateTimeTo = entity.DateTimeTo
            };
        }

        public static RequestTypeDto EntityToDto(RequestType entity)
        {
            return new RequestTypeDto()
            {
                Id = entity.Id,
                Type = entity.Type,
                Comments = entity.Comments,
                Deleted = entity.Deleted
            };
        }

        public static StateDetailDto EntityToDto(StateDetail entity)
        {
            return new StateDetailDto()
            {
                Id = entity.Id,
                Type = entity.Type,
                Comments = entity.Comments,
                Deleted = entity.Deleted
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
                RoleId = entity.RoleId,
                Deleted = entity.Deleted
            };
        }

        public static UserRoleDto EntityToDto(UserRole entity)
        {
            return new UserRoleDto()
            {
                Id = entity.Id,
                Type = entity.Type,
                Comments = entity.Comments,
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