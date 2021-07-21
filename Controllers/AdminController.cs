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

namespace TimeOffTracker.Controllers
{
    /*
     Администратор (Admin): встроенный пользователь
        ◦ Регистрирует пользователя
	    ◦ определяет роль пользователя: Менеджер/Сотрудник.
    */
    [Route("[controller]/[action]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        /// <summary>
        /// POST: http://localhost:5000/Admin/GetUsers?page=3&pageSize=10
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
        [ProducesResponseType(200, Type = typeof(List<UserDto>))]
        [ProducesResponseType(404)]
        [HttpPost]
        public async Task<ActionResult<string>> GetUsers([FromBody] UserDto filter, int page, int pageSize,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userRepository = new UserRepository();
            var users = await userRepository.SelectAlldAsync(filter, token);
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

        [HttpGet]
        UserDto GetUserDetatil(int id)
        {
            //	Вернуть пользователя
            return new UserDto();
        }

        [HttpPatch]
        void ModifyUserRole(UserDto user_whit_new_rol)
        {
            //	найти user по user_whit_new_rol.id
            //	Изменить user.role = user_whit_new_rol.role
            /*
                Если  роль пользователя поменялась с Менеджер на Сотрудник, все существующие активные заявки на отпуск, 
                которые ожидают его подтверждения автоматически считаются подтвержденными. 
                Если такие заявки, еще не были ему отправлены, то человек просто исчезает из цепочки утверждающих.  
             */
        }

        [HttpPost]
        int CreateUser(UserDto user)
        {
            //	Создать пользователя
            return new User().Id;
        }
        /*
         Сценарии использования
            Управление пользователями 
                1. Пользователь с ролью Администратор входит в систему “Отпуск”, используя предопределенные логин-пароль.
                2. Администратор видит список всех пользователей
                3. Администратор.может зарегистрировать нового пользователя 
                4. Администратор может найти необходимого пользователя по имени-фамилии, или емейлу. 
                5. Для каждого пользователя показана следующая информация: 
                    a. Имя-фамилия
                    b. логин
                    c. Емейл
                    d. Роль в системе.
                6. Администратор нажимает Редактировать  (Edit). 
                7. Поле задания роли становится активным. 
                8. Администратор выбирает роль из выпадающего списка: Сотрудник (Employee) или Менеджер (Manager).
                9. Администратор нажимает Сохранить. Роль пользователя поменялась. 
                10. Если  роль пользователя поменялась с Менеджер на Сотрудник, все существующие активные заявки на отпуск, которые ожидают его подтверждения автоматически считаются подтвержденными. Если такие заявки, еще не были ему отправлены, то человек просто исчезает из цепочки утверждающих.  
         */

        [ProducesResponseType(200, Type = typeof(UserDto))]
        [ProducesResponseType(404)]
        [HttpGet("get/{id:int}")]
        public async Task<ActionResult<UserDto>> Get([FromRoute(Name = "id")] int id, CancellationToken token)
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

        [ProducesResponseType(200, Type = typeof(int))]
        [ProducesResponseType(404)]
        [HttpPost("create")]
        public async Task<ActionResult<int>> Create([FromBody] UserDto user, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var userRoleRepository = new UserRoleRepository();
            var userRole = await userRoleRepository.SelectByIdAsync(user.RoleId, token);
            if (userRole == null)
            {
                return BadRequest("Wrong Role");
            }

            var userRepository = new UserRepository();
            var r = await userRepository.InsertAsync(user, token);
            return Ok(r);
        }
    }
}