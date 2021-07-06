insert into _public.Request_type (id, type, comments, deleted)
values  (1, N'Paid holiday', N'Оплачиваемый отпуск', 0),
        (2, N'Admin (unpaid) planned', N'Административный (неоплачиваемый) плановый отпуск', 0),
        (3, N'Admin (unpaid) force majeure', N'Административный (неоплачиваемый) отпуск по причине форс-мажора', 0),
        (4, N'Study', N'Учебный отпуск', 0),
        (5, N'Social', N'Социальный отпуск (по причине смерти близкого)', 0),
        (6, N'Sick with docs', N'Больничный с больничным листом', 0),
        (7, N'Sick without docs', N'Больничный без больничного листа', 0);