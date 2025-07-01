create database SISTEMA_BIBLIOTECA
go

use SISTEMA_BIBLIOTECA
go

create table TIPOS_BIBLIOGRAFIAS(
	CODIGO_BIBLIOGRAFIA int not null primary key identity(1,1),
	NOMBRE_BIBLIOGRAFIA varchar(100) not null,
	DESCRIPCION varchar(max) null,
	ELIMINADO bit null
)
go

create table EDITORAS(
	CODIGO_EDITORA int not null primary key identity(1,1),
	NOMBRE_EDITORA varchar(30) not null,
	DESCRIPCION varchar(max) null,
	ELIMINADO bit null
)
go

create table IDIOMAS(
	CODIGO_IDIOMA int not null primary key identity(1,1),
	NOMBRE_IDIOMA varchar(15) not null,
	ELIMINADO bit null
)
go

create table AUTORES(
	CODIGO_AUTOR int not null primary key identity(1,1),
	NOMBRE_AUTOR varchar(60) not null,
	PAIS_ORIGEN varchar(50) null,
	CODIGO_IDIOMA int not null,
	ELIMINADO bit null
)
go

alter table AUTORES
add constraint FK_IDIOMA_AUTOR
foreign key (CODIGO_IDIOMA) references IDIOMAS(CODIGO_IDIOMA)
go

create table LIBROS(
	CODIGO_LIBRO int not null primary key identity(1,1),
	TITULO varchar(100) null,
	SIGNATURA_TOPOGRAFICA varchar(50) null,
	ISBN varchar(15) null,
	CODIGO_EDITORA int not null,
	ANIO_PUBLICACION int not null,
	CIENCIA varchar(30) null,
	CODIGO_IDIOMA int not null,
	ELIMINADO bit null
)
go

alter table LIBROS
add constraint FK_EDITORA_LIBROS
foreign key (CODIGO_EDITORA) references EDITORAS(CODIGO_EDITORA)
go

alter table LIBROS
add constraint FK_IDIOMA_LIBROS
foreign key (CODIGO_IDIOMA) references IDIOMAS(CODIGO_IDIOMA)
go

create table LIBROS_BIBLIOGRAFIAS(
	ID int not null primary key identity(1,1),
	CODIGO_LIBRO int not null,
	CODIGO_BIBLIOGRAFIA int not null
);
go

alter table LIBROS_BIBLIOGRAFIAS
add constraint FK_LIBROS_BIBLIOGRAFIAS_LIBRO
foreign key (CODIGO_LIBRO) references LIBROS(CODIGO_LIBRO)
go

alter table LIBROS_BIBLIOGRAFIAS
add constraint FK_LIBROS_BIBLIOGRAFIAS_BIBLIOGRAFIA
foreign key (CODIGO_BIBLIOGRAFIA) references TIPOS_BIBLIOGRAFIAS(CODIGO_BIBLIOGRAFIA)
go

create table LIBROS_AUTORES(
	ID int not null primary key identity(1,1),
	CODIGO_LIBRO int not null,
	CODIGO_AUTOR int not null
);
go

alter table LIBROS_AUTORES
add constraint FK_LIBROS_AUTORES_LIBRO
foreign key (CODIGO_LIBRO) references LIBROS(CODIGO_LIBRO)
go

alter table LIBROS_AUTORES
add constraint FK_LIBROS_AUTORES_AUTOR
foreign key (CODIGO_AUTOR) references AUTORES(CODIGO_AUTOR)
go

create table TIPOS_PERSONAS(
	CODIGO_TIPO int not null primary key identity(1,1),
	NOMBRE_TIPO varchar(8) not null,
	ELIMINADO bit null
);
go

insert into TIPOS_PERSONAS (NOMBRE_TIPO) values
('Fisica'),('Juridica')
go

create table USUARIOS(
	CODIGO_USUARIO int not null primary key identity(1,1),
	NOMBRE varchar(80) not null,
	APELLIDO varchar(80) not null,
	CEDULA varchar(11) not null,
	NUMERO_CARNET varchar(10) not null,
	CODIGO_TIPO int not null,
	ELIMINADO bit null
);
go

alter table USUARIOS
add constraint FK_TIPO_PERSONA_USUARIO
foreign key (CODIGO_TIPO) references TIPOS_PERSONAS(CODIGO_TIPO)
go

create table TANDA_LABOR(
	CODIGO_TANDA int not null primary key identity(1,1),
	NOMBRE_TANDA varchar(30) not null,
	HORA_INICIO time not null,
	HORA_FIN time not null,
	ELIMINADO bit null
);
go


create table EMPLEADOS(
	CODIGO_EMPLEADO int not null primary key identity(1,1),
	NOMBRE varchar(80) not null,
	APELLIDO varchar(80) not null,
	CEDULA varchar(11) not null,
	CODIGO_TANDA int not null,
	PORCENTAJE_COMISION float null,
	FECHA_INGRESO date null,
	ELIMINADO bit null
);
go

alter table EMPLEADOS
add constraint FK_TANDA_EMPLEADO
foreign key (CODIGO_TANDA) references TANDA_LABOR(CODIGO_TANDA)
go

create table PRESTAMO(
	CODIGO_PRESTAMO int not null primary key identity(1,1),
	CODIGO_EMPLEADO int not null,
	CODIGO_LIBRO int not null,
	CODIGO_USUARIO int not null,
	FECHA_PRESTAMO date not null,
	FECHA_DEVOLUCION date null,
	MONTO_DIA decimal(10,2) null,
	CANTIDAD_DIAS int null,
	COMENTARIO varchar(max) null,
	ELIMINADO bit null
);
go

alter table PRESTAMO
add constraint FK_EMPLEADO_PRESTAMO
foreign key (CODIGO_EMPLEADO) references EMPLEADOS(CODIGO_EMPLEADO)
go

alter table PRESTAMO
add constraint FK_LIBRO_PRESTAMO
foreign key (CODIGO_LIBRO) references LIBROS(CODIGO_LIBRO)
go

alter table PRESTAMO
add constraint FK_USUARIO_PRESTAMO
foreign key (CODIGO_USUARIO) references USUARIOS(CODIGO_USUARIO)
go