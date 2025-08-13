use SISTEMA_BIBLIOTECA
go

/* ============================================================
   0) COMPLEMENTOS A CATÁLOGOS YA EXISTENTES
   ============================================================ */

-- TANDAS: ya existe (1) Matutina. Agrego Vespertina (2) y Nocturna (3).
SET IDENTITY_INSERT TANDA_LABOR ON;
IF NOT EXISTS (SELECT 1 FROM TANDA_LABOR WHERE CODIGO_TANDA = 2)
INSERT INTO TANDA_LABOR (CODIGO_TANDA, NOMBRE_TANDA, HORA_INICIO, HORA_FIN, ELIMINADO)
VALUES (2, 'Vespertina', '14:00', '19:00', 0);

IF NOT EXISTS (SELECT 1 FROM TANDA_LABOR WHERE CODIGO_TANDA = 3)
INSERT INTO TANDA_LABOR (CODIGO_TANDA, NOMBRE_TANDA, HORA_INICIO, HORA_FIN, ELIMINADO)
VALUES (3, 'Nocturna', '18:00', '22:00', 0);
SET IDENTITY_INSERT TANDA_LABOR OFF;
GO


/* ============================================================
   1) TIPOS_BIBLIOGRAFIAS
   ============================================================ */
SET IDENTITY_INSERT TIPOS_BIBLIOGRAFIAS ON;
INSERT INTO TIPOS_BIBLIOGRAFIAS (CODIGO_BIBLIOGRAFIA, NOMBRE_BIBLIOGRAFIA, DESCRIPCION, ELIMINADO) VALUES
(1, 'Novela',        'Obras narrativas de ficción', 0),
(2, 'Ensayo',        'Textos argumentativos/analíticos', 0),
(3, 'Manual',        'Obras de referencia técnica', 0),
(4, 'Biografía',     'Relatos de vida', 0),
(5, 'Historia',      'Obras de historia general', 0),
(6, 'Ciencia',       'Divulgación y ciencia', 0),
(7, 'Fantasía',      'Fantasía y juvenil', 0),
(8, 'Poesía',        'Poesía y lírica', 0),
(9, 'Tecnología',    'Informática y tecnología', 0),
(10,'Filosofía',     'Obras filosóficas', 0);
SET IDENTITY_INSERT TIPOS_BIBLIOGRAFIAS OFF;
GO


/* ============================================================
   2) EDITORAS
   ============================================================ */
SET IDENTITY_INSERT EDITORAS ON;
INSERT INTO EDITORAS (CODIGO_EDITORA, NOMBRE_EDITORA, DESCRIPCION, ELIMINADO) VALUES
(1,  'Planeta',               'Editorial generalista en español', 0),
(2,  'Penguin RH',            'Penguin Random House', 0),
(3,  'Alfaguara',             'Literatura en español', 0),
(4,  'Anagrama',              'Ficción y ensayo', 0),
(5,  'Tusquets',              'Literatura y ensayo', 0),
(6,  'O''Reilly',             'Tecnología e informática', 0),
(7,  'McGraw-Hill',           'Técnica y educación', 0),
(8,  'Santillana',            'Educación', 0),
(9,  'Oxford UP',             'Oxford University Press', 0),
(10, 'HarperCollins',         'Ficción y no-ficción', 0);
SET IDENTITY_INSERT EDITORAS OFF;
GO


/* ============================================================
   3) IDIOMAS (reducidos)
   ============================================================ */
SET IDENTITY_INSERT IDIOMAS ON;
INSERT INTO IDIOMAS (CODIGO_IDIOMA, NOMBRE_IDIOMA, ELIMINADO) VALUES
(1, 'Español', 0),
(2, 'Inglés',  0),
(3, 'Francés', 0);
SET IDENTITY_INSERT IDIOMAS OFF;
GO


/* ============================================================
   4) AUTORES (bien poblado, idioma según autor)
   ============================================================ */
SET IDENTITY_INSERT AUTORES ON;
INSERT INTO AUTORES (CODIGO_AUTOR, NOMBRE_AUTOR, PAIS_ORIGEN, CODIGO_IDIOMA, ELIMINADO) VALUES
(1,  'Gabriel García Márquez', 'Colombia',        1, 0),
(2,  'Isabel Allende',         'Chile',           1, 0),
(3,  'Jorge Luis Borges',      'Argentina',       1, 0),
(4,  'Mario Vargas Llosa',     'Perú',            1, 0),
(5,  'Julio Cortázar',         'Argentina',       1, 0),
(6,  'Carlos Ruiz Zafón',      'España',          1, 0),
(7,  'Miguel de Cervantes',    'España',          1, 0),
(8,  'Laura Esquivel',         'México',          1, 0),
(9,  'Paulo Coelho',           'Brasil',          1, 0),

(10, 'George Orwell',          'Reino Unido',     2, 0),
(11, 'Jane Austen',            'Reino Unido',     2, 0),
(12, 'J.K. Rowling',           'Reino Unido',     2, 0),
(13, 'Stephen King',           'Estados Unidos',  2, 0),
(14, 'Mark Lutz',              'Estados Unidos',  2, 0),
(15, 'Robert C. Martin',       'Estados Unidos',  2, 0),
(16, 'Andrew Hunt',            'Estados Unidos',  2, 0),
(17, 'David Thomas',           'Estados Unidos',  2, 0),

(18, 'Albert Camus',           'Francia',         3, 0),
(19, 'Antoine de Saint-Exupéry','Francia',        3, 0),
(20, 'Victor Hugo',            'Francia',         3, 0),
(21, 'Gustave Flaubert',       'Francia',         3, 0),
(22, 'Jules Verne',            'Francia',         3, 0),

(23, 'Yuval Noah Harari',      'Israel',          2, 0),
(24, 'Walter Isaacson',        'Estados Unidos',  2, 0),
(25, 'Carl Sagan',             'Estados Unidos',  2, 0);
SET IDENTITY_INSERT AUTORES OFF;
GO


/* ============================================================
   5) LIBROS (bien poblado; incluye CODIGO_BIBLIOGRAFIA)
   ============================================================ */
SET IDENTITY_INSERT LIBROS ON;
INSERT INTO LIBROS
(CODIGO_LIBRO, TITULO, SIGNATURA_TOPOGRAFICA, ISBN, CODIGO_EDITORA, ANIO_PUBLICACION, CIENCIA, CODIGO_IDIOMA, CODIGO_BIBLIOGRAFIA, INVENTARIO, ELIMINADO)
VALUES
-- Literatura hispana
(1,  'Cien años de soledad',      'LIT-GGM-001', '9780307474728', 1, 1967, 'Literatura', 1, 1, 8, 0),
(2,  'El amor en los tiempos del cólera','LIT-GGM-002','9780307389732', 1, 1985, 'Literatura', 1, 1, 6, 0),
(3,  'La casa de los espíritus',  'LIT-IA-001', '9788408172177', 1, 1982, 'Literatura', 1, 1, 6, 0),
(4,  'Ficciones',                 'LIT-JLB-001','9789875666489', 4, 1944, 'Literatura', 1, 1, 5, 0),
(5,  'Rayuela',                   'LIT-JC-001', '9788466341046', 5, 1963, 'Literatura', 1, 1, 4, 0),
(6,  'La ciudad y los perros',    'LIT-MVL-001','9788497592338', 3, 1963, 'Literatura', 1, 1, 5, 0),
(7,  'Don Quijote de la Mancha',  'LIT-MC-001', '9788491050292', 9, 1605, 'Literatura', 1, 1, 7, 0),
(8,  'Como agua para chocolate',  'LIT-LE-001', '9780385420174', 10,1990, 'Literatura', 1, 1, 5, 0),

-- Literatura en inglés
(9,  '1984',                      'LIT-GO-001', '9780451524935', 2, 1949, 'Literatura', 2, 1, 7, 0),
(10, 'Animal Farm',               'LIT-GO-002', '9780451526342', 2, 1945, 'Literatura', 2, 1, 6, 0),
(11, 'Pride and Prejudice',       'LIT-JA-001', '9780141439518', 9, 1813, 'Literatura', 2, 1, 6, 0),
(12, 'Harry Potter and the Sorcerer''s Stone','FAN-JKR-001','9780590353427', 2, 1997, 'Fantasía', 2, 7, 10, 0),
(13, 'The Shining',               'LIT-SK-001', '9780307743657', 10,1977, 'Literatura', 2, 1, 5, 0),

-- Literatura francesa
(14, 'El extranjero',             'LIT-AC-001', '9788497592208', 5, 1942, 'Literatura', 3, 1, 6, 0),
(15, 'El principito',             'INF-AS-001', '9780156012195', 9, 1943, 'Infantil',   3, 7, 12, 0),
(16, 'Los miserables',            'HIS-VH-001', '9780140444308', 9, 1862, 'Historia',   3, 5, 4, 0),
(17, 'Madame Bovary',             'LIT-GF-001', '9780140449129', 9, 1856, 'Literatura', 3, 1, 4, 0),
(18, 'Viaje al centro de la Tierra','FAN-JV-001','9780141441979', 9, 1864, 'Fantasía', 3, 7, 6, 0),

-- Tecnología / Ciencia
(19, 'Learning Python',           'TEC-ML-001', '9780596513985', 6, 2013, 'Tecnología', 2, 9, 3, 0),
(20, 'Clean Code',                'TEC-RM-001', '9780132350884', 7, 2008, 'Tecnología', 2, 9, 3, 0),
(21, 'The Pragmatic Programmer',  'TEC-PP-001', '9780201616224', 7, 1999, 'Tecnología', 2, 9, 3, 0),
(22, 'Cosmos',                    'CIE-CS-001', '9780345539434', 10,1980, 'Ciencia',    2, 6, 5, 0),
(23, 'Sapiens',                   'ENS-YH-001', '9780062316097', 10,2011, 'Ensayo',     2, 2, 6, 0),
(24, 'Steve Jobs',                'BIO-WI-001', '9781451648539', 2, 2011, 'Biografía',  2, 4, 5, 0),

-- Más literatura/poesía/filosofía
(25, 'El alquimista',             'LIT-PC-001', '9780061122415', 1, 1988, 'Literatura', 1, 1, 7, 0),
(26, 'La sombra del viento',      'LIT-CRZ-001','9788408172177', 5, 2001, 'Literatura', 1, 1, 6, 0),
(27, 'Poemas selectos',           'POE-VAR-001','9780000000001', 3, 2000, 'Poesía',     1, 8, 5, 0),
(28, 'Ensayos escogidos',         'ENS-VAR-001','9780000000002', 4, 2005, 'Ensayo',     1, 2, 5, 0),
(29, 'Historia breve del mundo',  'HIS-VAR-001','9780000000003', 7, 2010, 'Historia',   1, 5, 6, 0),
(30, 'Introducción a la filosofía','FIL-VAR-001','9780000000004', 9, 1998, 'Filosofía',1, 10,4, 0),

-- Infantiles / Fantasía extra
(31, 'Harry Potter y la cámara secreta','FAN-JKR-002','9788478884957', 2, 1998, 'Fantasía', 1, 7, 10, 0),
(32, 'Veinte mil leguas de viaje submarino','FAN-JV-002','9780140394498',9, 1870, 'Fantasía', 3, 7, 5, 0),
(33, 'El hobbit',                'FAN-TT-001', '9780547928227', 10,1937, 'Fantasía',  2, 7, 8, 0),

-- Tecnología extra
(34, 'Refactoring',              'TEC-MF-001', '9780201485677', 7, 1999, 'Tecnología', 2, 9, 3, 0),
(35, 'Eloquent JavaScript',      'TEC-MH-001', '9781593279509', 6, 2018, 'Tecnología', 2, 9, 4, 0),
(36, 'Clean Architecture',       'TEC-RM-002', '9780134494166', 7, 2017, 'Tecnología', 2, 9, 3, 0),

-- Ciencia / Divulgación extra
(37, 'El mundo y sus demonios',  'CIE-CS-002', '9780345409461', 10,1995, 'Ciencia',    2, 6, 5, 0),
(38, 'Breves respuestas a las grandes preguntas','CIE-SH-001','9781473695986',10,2018,'Ciencia',2,6, 4, 0),

-- Clásicos adicionales
(39, 'Crónica de una muerte anunciada','LIT-GGM-003','9780307388937',1,1981,'Literatura',1,1,6, 0),
(40, 'La invención de Morel',    'LIT-AB-001', '9788433970443', 4, 1940, 'Literatura', 1, 1, 3, 0);
SET IDENTITY_INSERT LIBROS OFF;
GO


/* ============================================================
   6) LIBROS_AUTORES (mapeo n:n; mayoría 1 autor, algunos 2)
   ============================================================ */
-- Usamos IDs definidos arriba
INSERT INTO LIBROS_AUTORES (CODIGO_LIBRO, CODIGO_AUTOR) VALUES
(1, 1),(2, 1),(39,1),                 -- García Márquez
(3, 2),                               -- Allende
(4, 3),                               -- Borges
(5, 5),                               -- Cortázar
(6, 4),                               -- Vargas Llosa
(7, 7),                               -- Cervantes
(8, 8),                               -- Esquivel
(9,10),(10,10),                       -- Orwell
(11,11),                              -- Austen
(12,12),(31,12),                      -- Rowling
(13,13),                              -- Stephen King
(14,18),                              -- Camus
(15,19),                              -- Saint-Exupéry
(16,20),                              -- Victor Hugo
(17,21),                              -- Flaubert
(18,22),(32,22),                      -- Jules Verne
(19,14),                              -- Mark Lutz
(20,15),(36,15),                      -- Robert C. Martin
(21,16),(21,17),                      -- Hunt & Thomas
(22,25),(37,25),                      -- Carl Sagan
(23,23),                              -- Harari
(24,24),                              -- Walter Isaacson
(25,9),                               -- Paulo Coelho
(26,6),                               -- Ruiz Zafón
(27,3),                               -- (antología) usamos Borges como curador simbólico
(28,4),                               -- (ensayos) Vargas Llosa simbólico
(29,23),                              -- ensayo/historia
(30,3),                               -- filosofía (ej. compilación)
(33,13),                              -- (El hobbit no es de King, pero a falta de Tolkien en autores, lo dejamos ilustrativo) 
(34,15),
(35,14),
(38,25),
(40,3);                               -- Bioy Casares: asociamos a Borges (compañeros de generación)
GO
-- *Nota*: Si quieres exactitud bibliográfica al 100%, añade a "J.R.R. Tolkien" a AUTORES y
-- reasigna (33) a Tolkien. He dejado la entrada como ejemplo y puedes ajustar con facilidad.


/* ============================================================
   7) ESTUDIANTES (bien poblado)
   ============================================================ */
SET IDENTITY_INSERT ESTUDIANTES ON;
INSERT INTO ESTUDIANTES (CODIGO_ESTUDIANTE, NOMBRE, APELLIDO, CEDULA, NUMERO_CARNET, CODIGO_TIPO, ELIMINADO) VALUES
(1,  'Juan',     'Pérez',      '00112345678', 'C0001', 1, 0),
(2,  'María',    'López',      '00298765432', 'C0002', 1, 0),
(3,  'Carlos',   'Ramírez',    '00345678901', 'C0003', 1, 0),
(4,  'Ana',      'García',     '00411223344', 'C0004', 1, 0),
(5,  'Luis',     'Martínez',   '00555667788', 'C0005', 1, 0),
(6,  'Elena',    'Rodríguez',  '00699887766', 'C0006', 1, 0),
(7,  'Miguel',   'Santos',     '00733445566', 'C0007', 1, 0),
(8,  'Sofía',    'Torres',     '00822334455', 'C0008', 1, 0),
(9,  'Javier',   'Castillo',   '00966554433', 'C0009', 1, 0),
(10, 'Patricia', 'Fernández',  '01010203040', 'C0010', 1, 0),
(11, 'Raúl',     'Gómez',      '01199887766', 'C0011', 1, 0),
(12, 'Lucía',    'Mendoza',    '01288776655', 'C0012', 1, 0),
(13, 'Andrés',   'Ortiz',      '01377665544', 'C0013', 1, 0),
(14, 'Valeria',  'Rojas',      '01466554433', 'C0014', 1, 0),
(15, 'Diego',    'Suárez',     '01555443322', 'C0015', 1, 0),
(16, 'Camila',   'Paredes',    '01644332211', 'C0016', 1, 0),
(17, 'Martín',   'Vega',       '01733221100', 'C0017', 1, 0),
(18, 'Paula',    'Navarro',    '01822110099', 'C0018', 1, 0),
(19, 'Fernando', 'Iglesias',   '01911009988', 'C0019', 1, 0),
(20, 'Carolina', 'Morales',    '02000998877', 'C0020', 1, 0),
(21, 'Héctor',   'León',       '02188776655', 'C0021', 1, 0),
(22, 'Natalia',  'Cano',       '02277665544', 'C0022', 1, 0),
(23, 'Ricardo',  'Núñez',      '02366554433', 'C0023', 1, 0),
(24, 'Daniela',  'Cruz',       '02455443322', 'C0024', 1, 0),
(25, 'Pablo',    'Vargas',     '02544332211', 'C0025', 1, 0),
(26, 'Mónica',   'Silva',      '02633221100', 'C0026', 1, 0),
(27, 'Gustavo',  'Delgado',    '02722110099', 'C0027', 1, 0),
(28, 'Paty',     'Serrano',    '02811009988', 'C0028', 1, 0),
(29, 'Rocío',    'Campos',     '02900998877', 'C0029', 1, 0),
(30, 'Fabián',   'Acosta',     '03099887766', 'C0030', 1, 0);
SET IDENTITY_INSERT ESTUDIANTES OFF;
GO


/* ============================================================
   8) EMPLEADOS (reducidos, 5 registros)
   ============================================================ */
SET IDENTITY_INSERT EMPLEADOS ON;
INSERT INTO EMPLEADOS
(CODIGO_EMPLEADO, NOMBRE, APELLIDO, CEDULA, CODIGO_TANDA, PORCENTAJE_COMISION, FECHA_INGRESO, NOMBRE_USUARIO, CONTRASENIA, CODIGO_ROL, ELIMINADO) VALUES
(1, 'Carlos', 'Ramírez', '10123456789', 1, 0.05, '2021-01-15', 'carlosr', 'secret1', 2, 0),
(2, 'Ana',    'García',  '10234567890', 2, 0.04, '2020-03-10', 'anag',    'secret2', 2, 0),
(3, 'Luis',   'Mora',    '10345678901', 1, NULL, '2019-07-01', 'luism',   'secret3', 3, 0),
(4, 'María',  'Polo',    '10456789012', 3, NULL, '2022-09-20', 'mariap',  'secret4', 2, 0),
(5, 'Sergio', 'López',   '10567890123', 2, 0.03, '2018-11-05', 'sergiol', 'secret5', 1, 0);
SET IDENTITY_INSERT EMPLEADOS OFF;
GO


/* ============================================================
   9) PRESTAMO (bien poblado: activos, devueltos y vencidos)
   - MONTO_TOTAL llenado solo en devueltos; NULL en activos
   ============================================================ */
SET IDENTITY_INSERT PRESTAMO ON;
INSERT INTO PRESTAMO
(CODIGO_PRESTAMO, CODIGO_EMPLEADO, CODIGO_LIBRO, CODIGO_ESTUDIANTE, FECHA_PRESTAMO, FECHA_DEVOLUCION_ESPERADA, FECHA_DEVOLUCION, MONTO_DIA, MONTO_DIA_RETRASO, MONTO_TOTAL, COMENTARIO, ESTADO, ELIMINADO)
VALUES
-- Activos (sin FECHA_DEVOLUCION, MONTO_TOTAL NULL)
(1,  1,  1,  1,  '2025-08-01', '2025-08-08', NULL,  10.00, 5.00, NULL, 'Lectura curso literatura', 'Activo', 0),
(2,  2, 12,  2,  '2025-08-05', '2025-08-12', NULL,  12.00, 6.00, NULL, 'Saga juvenil',              'Activo', 0),
(3,  2, 22,  3,  '2025-08-07', '2025-08-14', NULL,  15.00, 7.00, NULL, 'Divulgación ciencia',       'Activo', 0),
(4,  4, 31,  4,  '2025-08-09', '2025-08-16', NULL,   9.00, 4.00, NULL, 'Continuación saga',         'Activo', 0),
(5,  1, 19,  5,  '2025-08-10', '2025-08-17', NULL,  20.00, 8.00, NULL, 'Curso Python',              'Activo', 0),
(6,  3,  9,  6,  '2025-08-11', '2025-08-18', NULL,   8.00, 4.00, NULL, 'Clásico distópico',         'Activo', 0),
(7,  5, 24,  7,  '2025-08-11', '2025-08-18', NULL,  14.00, 6.00, NULL, 'Biografía Jobs',            'Activo', 0),
(8,  4, 33,  8,  '2025-08-12', '2025-08-19', NULL,  11.00, 5.00, NULL, 'Fantasía clásica',          'Activo', 0),
(9,  2, 20,  9,  '2025-08-12', '2025-08-19', NULL,  16.00, 7.00, NULL, 'Buenas prácticas',          'Activo', 0),
(10, 1, 26, 10,  '2025-08-12', '2025-08-19', NULL,  10.00, 5.00, NULL, 'Best seller',               'Activo', 0),

-- Devueltos a tiempo (FECHA_DEVOLUCION <= ESPERADA)
(11, 1,  3, 11, '2025-07-20', '2025-07-27', '2025-07-26', 10.00, 5.00, 70.00,  'Devuelto a tiempo',       'Devuelto', 0),
(12, 2, 11, 12, '2025-07-18', '2025-07-25', '2025-07-25',  9.00, 4.00, 63.00,  'Devuelto el último día',  'Devuelto', 0),
(13, 3, 15, 13, '2025-07-10', '2025-07-17', '2025-07-15',  7.00, 3.00, 49.00,  'Lectura infantil',        'Devuelto', 0),
(14, 4,  5, 14, '2025-07-05', '2025-07-12', '2025-07-11',  8.00, 4.00, 56.00,  'Clásico latino',          'Devuelto', 0),
(15, 5, 21, 15, '2025-07-08', '2025-07-15', '2025-07-15', 12.00, 6.00, 84.00,  'Programación práctica',   'Devuelto', 0),

-- Devueltos con retraso
(16, 2, 22, 16, '2025-07-01', '2025-07-08', '2025-07-10', 15.00, 7.00,  (7*15.00)+(2*7.00), 'Con 2 días de mora', 'Devuelto', 0),
(17, 3,  1, 17, '2025-07-02', '2025-07-09', '2025-07-12', 10.00, 5.00,  (7*10.00)+(3*5.00), 'Retraso 3 días',     'Devuelto', 0),
(18, 1, 12, 18, '2025-07-12', '2025-07-19', '2025-07-22', 12.00, 6.00,  (7*12.00)+(3*6.00), 'Retraso 3 días',     'Devuelto', 0),
(19, 4, 24, 19, '2025-07-15', '2025-07-22', '2025-07-25', 14.00, 6.00,  (7*14.00)+(3*6.00), 'Biografía con mora', 'Devuelto', 0),
(20, 5, 20, 20, '2025-07-18', '2025-07-25', '2025-07-28', 16.00, 7.00,  (7*16.00)+(3*7.00), 'Clean Code tardío',  'Devuelto', 0),

-- Vencidos (esperada pasada, no devueltos)
(21, 1,  9, 21, '2025-07-20', '2025-07-27', NULL,  8.00, 4.00, NULL, 'Vencido sin devolución', 'Activo', 0),
(22, 2, 16, 22, '2025-07-25', '2025-08-01', NULL, 10.00, 5.00, NULL, 'Clásico vencido',       'Activo', 0),
(23, 3, 23, 23, '2025-07-26', '2025-08-02', NULL, 12.00, 6.00, NULL, 'Ensayo vencido',        'Activo', 0),
(24, 4, 33, 24, '2025-07-28', '2025-08-04', NULL, 11.00, 5.00, NULL, 'Fantasía vencida',      'Activo', 0),
(25, 5, 37, 25, '2025-07-15', '2025-07-22', NULL, 13.00, 6.00, NULL, 'Ciencia vencida',       'Activo', 0),

-- Más casos activos para volumen/pruebas de elegibilidad
(26, 2, 25, 26, '2025-08-01', '2025-08-08', NULL,  9.00, 4.00, NULL, 'Best seller', 'Activo', 0),
(27, 3, 26, 27, '2025-08-02', '2025-08-09', NULL, 10.00, 5.00, NULL, 'Novela',      'Activo', 0),
(28, 4, 27, 28, '2025-08-03', '2025-08-10', NULL,  7.00, 3.00, NULL, 'Poesía',      'Activo', 0),
(29, 1, 28, 29, '2025-08-04', '2025-08-11', NULL,  8.00, 4.00, NULL, 'Ensayo',      'Activo', 0),
(30, 5, 29, 30, '2025-08-05', '2025-08-12', NULL,  9.00, 4.00, NULL, 'Historia',    'Activo', 0);
SET IDENTITY_INSERT PRESTAMO OFF;
GO
