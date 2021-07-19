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
using Microsoft.AspNetCore.Mvc;
using TimeOffTracker.Model;
using Microsoft.AspNetCore.Authorization;
using TimeOffTracker.Model.DTO;


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
        [HttpGet]
        List<UserDto> GetUsers(UserDto filter = null)
        {
            //	Если фильтр filter == null то вернуть всех
            //	иначе вернуть похожие по указаному фильтру
            //	предусмотреть старныцы по 10 елементов на страницу
            return new List<UserDto>();
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
        
    }
}