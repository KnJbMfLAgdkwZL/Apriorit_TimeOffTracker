create table _public.Project_role_type
(
	id int identity
		constraint Project_role_type_pk
			primary key nonclustered,
	type varchar(50) not null,
	comments varchar(max),
	deleted bit default 0 not null
)
go

create table _public.Request_type
(
	id int identity
		constraint Request_type_pk
			primary key nonclustered,
	type varchar(50) not null,
	comments varchar(max),
	deleted bit default 0 not null
)
go

create table _public.State_detail
(
	id int identity
		constraint State_detail_pk
			primary key nonclustered,
	type varchar(50) not null,
	comments varchar(max),
	deleted bit default 0 not null
)
go

create table _public.User_Role
(
	id int identity
		constraint User_Role_pk
			primary key nonclustered,
	type varchar(50) not null,
	comments varchar(max),
	deleted bit default 0 not null
)
go

create table _public.[User]
(
	id int identity
		constraint User_pk
			primary key nonclustered,
	email varchar(50) not null,
	login varchar(50) not null,
	first_name varchar(50),
	second_name varchar(50),
	password varchar(50) not null,
	role_id int not null
		constraint User_User_Role_id_fk
			references _public.User_Role,
	deleted bit default 0 not null
)
go

create table _public.Request
(
	id int identity
		constraint Request_pk
			primary key nonclustered,
	request_type_id int not null
		constraint Request_Request_type_id_fk
			references _public.Request_type,
	reason varchar(max),
	project_role_comment varchar(max),
	project_role_type_id int not null
		constraint Request_Project_role_type_id_fk
			references _public.Project_role_type,
	user_id int not null
		constraint Request_User_id_fk
			references _public.[User],
	state_detail_id int not null
		constraint Request_State_detail_id_fk
			references _public.State_detail,
	date_time_from datetime not null,
	date_time_to datetime not null
)
go

create table _public.User_Signature
(
	id int identity
		constraint User_Signature_pk
			primary key nonclustered,
	N_in_queue int default 0 not null,
	request_id int not null
		constraint User_Signature_Request_id_fk
			references _public.Request,
	user_id int not null
		constraint User_Signature_User_id_fk
			references _public.[User],
	approved bit default 0 not null,
	deleted bit default 0 not null
)
go

