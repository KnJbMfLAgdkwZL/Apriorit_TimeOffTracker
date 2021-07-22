using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TimeOffTracker.Model.DTO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.EntityFramework;

namespace TimeOffTracker.Model.Repositories
{
    public class UserSignatureRepository
    {
        public async Task<List<UserSignature>> SelectAllNotApprovedByIdAsync(int id, CancellationToken token)
        {
            await using var context = new MasterContext();
            var userSignatures = await context.UserSignatures.Where(us =>
                us.RequestId == id &&
                us.Approved == false &&
                us.Deleted == false
            ).OrderBy(us => us.NInQueue).ToListAsync(token);
            return userSignatures;
        }

        public async Task<bool> ConfirmOrRemoveAllManagerSignaturesAsync(int userId, CancellationToken token)
        {
            //Этот метод нуждается в тестировании
            await using var context = new MasterContext();

            //Получаю все не попдисанные подписи менеджера
            var userSignatures = await context.UserSignatures.Where(us =>
                us.UserId == userId &&
                us.Approved == false &&
                us.Deleted == false).ToListAsync(token);

            foreach (var us in userSignatures)
            {
                if (us.NInQueue == 0) //Если подпись первая в списке то она активная и её нужно полдписать
                {
                    await ConfirmSignatureAsync(us, token);
                }
                else // иначе она не активная и её просто можно убрать
                {
                    await RemoveSignatureAsync(us, token);
                }
            }

            return true;
        }

        public async Task<bool> ConfirmSignatureAsync(UserSignature userSignature, CancellationToken token)
        {
            //Этот метод нуждается в тестировании
            //Получаю не подписанные подписи заявки, той подписи которую нужно подписать. И сортирую их по us.NInQueue
            var alluserSignatures = await SelectAllNotApprovedByIdAsync(userSignature.Id, token);
            await using var context = new MasterContext();
            for (var i = 0; i < alluserSignatures.Count; i++)
            {
                var us = alluserSignatures[i];
                if (i == 0) // Наша активная подпись которую нужно полдписать
                {
                    us.Approved = true;
                }
                else // другие подписи заявки Сдвигаем по списку
                {
                    us.NInQueue = i - 1;
                }

                context.UserSignatures.Update(us);
            }

            await context.SaveChangesAsync(token);
            return true;
        }

        public async Task<bool> RemoveSignatureAsync(UserSignature userSignature, CancellationToken token)
        {
            //Этот метод нуждается в тестировании
            var alluserSignatures = await SelectAllNotApprovedByIdAsync(userSignature.Id, token);
            await using var context = new MasterContext();
            for (var i = 0; i < alluserSignatures.Count; i++)
            {
                var us = alluserSignatures[i];
                if (i < userSignature.NInQueue)
                {
                }
                else
                {
                    if (i == userSignature.NInQueue) //Убираем подпись
                    {
                        us.Deleted = true;
                    }

                    if (i > userSignature.NInQueue) // другие следующие подписи заявки Сдвигаем по списку
                    {
                        us.NInQueue = i - 1;
                    }

                    context.UserSignatures.Update(us);
                }
            }

            await context.SaveChangesAsync(token);
            return true;
        }
    }
}