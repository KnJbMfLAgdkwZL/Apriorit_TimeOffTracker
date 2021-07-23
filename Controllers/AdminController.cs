using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TimeOffTracker.Model;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TimeOffTracker.Model.DTO;
using TimeOffTracker.Model.Repositories;
using System.Collections.Generic;
using PagedList;
using TimeOffTracker.Model.Enum;

namespace TimeOffTracker.Controllers
{
    /// <summary>
    /// Администратор (Admin): встроенный пользователь
    ///     ◦ Регистрирует пользователя
    ///     ◦ определяет роль пользователя: Менеджер/Сотрудник. 
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        /// <summary>
        /// POST: /Admin/GetUsers?page=3&pageSize=10
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="filter">
        /// Body
        /// {
        ///     "email": "",
        ///     "login": "",
        ///     "firstName": "",
        ///     "secondName": ""
        /// }
        /// </param>
        /// <param name="page">Текущая страница</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="token"></param>
        /// <returns>
        /// {
        ///     "page": 3,
        ///     "pageSize": 2,
        ///     "totalPages": 12,
        ///     "users": [User1, User2, User3]
        /// }
        /// </returns>
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [HttpPost]
        public async Task<ActionResult<string>> GetUsers([FromBody] UserDto filter, [FromQuery(Name = "page")] int page,
            [FromQuery(Name = "pageSize")] int pageSize, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userRepository = new UserRepository();
            var users = await userRepository.SelectAllAsync(filter, token);
            var totalPages = (int) Math.Ceiling((double) users.Count / pageSize);
            var usersDto = users.ToPagedList(page, pageSize).Select(Converter.EntityToDto);
            var result = new
            {
                page = page,
                pageSize = pageSize,
                totalPages = totalPages,
                users = usersDto,
            };
            return Ok(result);
        }

        /// <summary>
        /// GET: /Admin/GetUserDetail?id=7
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="id">Ид пользователя</param>
        /// <param name="token"></param>
        /// <returns>
        /// {
        ///     "id": 7,
        ///     "email": "jeufrauterolo-9726@yopmail.com",
        ///     "login": "splendidwell",
        ///     "firstName": "Ali",
        ///     "secondName": "Savely",
        ///     "password": "x4eyZI3vSs",
        ///     "roleId": 3,
        ///     "deleted": false
        /// }
        /// </returns>
        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUserDetail([FromQuery(Name = "id")] int id,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userRepository = new UserRepository();
            var user = await userRepository.SelectByIdAsync(id, token);
            if (user == null)
            {
                return NoContent();
            }

            var userDto = Converter.EntityToDto(user);
            return Ok(userDto);
        }

        /// <summary>
        /// POST: /Admin/CreateUser
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="user">
        /// Body
        /// {
        ///     "email": "JohnDoe@gmail.com",
        ///     "login": "john123",
        ///     "firstName": "John",
        ///     "secondName": "Doe",
        ///     "password": "cJrhIdvN7O",
        ///     "roleId": 1
        /// }
        /// </param>
        /// <param name="token"></param>
        /// <returns>user.Id</returns>
        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpPost]
        public async Task<ActionResult<int>> CreateUser([FromBody] UserDto user, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var enumRepository = new EnumRepository();

            if (!enumRepository.Contains(user.RoleId))
            {
                return BadRequest("Wrong RoleId");
            }

            var userRepository = new UserRepository();
            var userLogin = await userRepository.SelectByLoginAsync(user.Login, token);
            if (userLogin != null)
            {
                return BadRequest("Login already exists");
            }

            var userEmail = await userRepository.SelectByEmailAsync(user.Email, token);
            if (userEmail != null)
            {
                return BadRequest("Email already exists");
            }

            var r = await userRepository.InsertAsync(user, token);
            return Ok(r);
        }

        /// GET: /Admin/GetUserRoles
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// }
        /// </summary>
        /// <param name="token"></param>
        /// <returns>
        /// [
        ///     {
        ///         "id": 1,
        ///         "type": "Admin",
        ///         "comments": "Администратор",
        ///         "deleted": false
        ///     },
        ///     {
        ///         "id": 2,
        ///         "type": "Accounting",
        ///         "comments": "Бухгалтерия",
        ///         "deleted": false
        ///     },
        /// ]
        /// </returns>
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [HttpGet]
        public async Task<ActionResult<List<EnumDto>>> GetUserRoles(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var enumRepository = new EnumRepository();
            return Ok(enumRepository.GetAll<UserRoles>());
        }

        /// <summary>
        /// PATCH: /Admin/ModifyUserRole
        /// Header
        /// {
        ///     Authorization: Bearer {TOKEN}
        /// } 
        /// </summary>
        /// <param name="userDto">
        /// Body
        /// {
        ///     "id": 3,
        ///     "roleId": 3
        /// }
        /// </param>
        /// <param name="token"></param>
        /// <returns>true</returns>
        [ProducesResponseType(200, Type = typeof(string))]
        [ProducesResponseType(404)]
        [HttpPatch]
        public async Task<ActionResult<bool>> ModifyUserRole([FromBody] UserDto userDto, CancellationToken token)
        {
            //Этот метод нуждается в тестировании
            //У нас пока нет функционала добавления заявок. У меня нет данных в таблице что бы протестировать смену роли.

            //userDto.Id ид юзера
            //userDto.RoleId новая роль

            var enumRepository = new EnumRepository();
            if (!enumRepository.Contains(userDto.RoleId))
            {
                return BadRequest("Wrong User.RoleId. Only 3 or 4");
            }

            if (userDto.RoleId != UserRoles.Employee && userDto.RoleId != UserRoles.Manager)
            {
                return BadRequest("Wrong User.RoleId. Only 3 or 4");
            }

            var userRepository = new UserRepository();
            var user = await userRepository.SelectByIdAsync(userDto.Id, token);
            if (user == null)
            {
                return BadRequest("Wrong User.Id");
            }

            /*
             * Если такие заявки, еще не были ему отправлены, то человек просто исчезает из цепочки утверждающих.
             */

            // Если  роль пользователя поменялась с Менеджер) на Сотрудник)
            if (userDto.RoleId == UserRoles.Employee && (UserRoles) user.RoleId == UserRoles.Manager)
            {
                //То нужно подтвердить все его существующие активные заявки на отпуск которые он должен подписать
                var userSignatureRepository = new UserSignatureRepository();
                await userSignatureRepository.ConfirmOrRemoveAllManagerSignaturesAsync(user.Id, token);
            }

            //Обновляем роль пользователя
            var r = await userRepository.UpdateRoleIdAsync(user.Id, (int) userDto.RoleId, token);

            return Ok(true);
        }
    }
}