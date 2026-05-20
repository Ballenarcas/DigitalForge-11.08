-- demo-data.sql
-- Datos de prueba masivos para Votify (Demo)
-- IMPORTANTE: Ejecutar en el SQL Editor de Supabase.
-- Nota: Aunque los IDs se generan automaticamente, aqui los forzamos (con UUIDs fijos) 
-- para poder enlazar las tablas (ej: asignar un proyecto a un evento especifico) en un solo script. 
-- La generacion automatica seguira funcionando cuando crees cosas nuevas desde la app.

-- ==========================================
-- 1. EVENTOS (Pasado, Presente y Futuro)
-- ==========================================
INSERT INTO evento (id, nombre, descripcion, fecha_inicio, fecha_fin, imagen_url) VALUES 
('11111111-0000-0000-0000-000000000001', 'Hackathon Pasado 2023', 'Evento que ya finalizo', '2023-01-01 00:00:00', '2023-12-31 23:59:59', NULL),
('11111111-0000-0000-0000-000000000002', 'Gran Hackathon Votify 2026', 'El evento principal activo', '2020-01-01 00:00:00', '2030-12-31 23:59:59', 'https://images.unsplash.com/photo-1504384308090-c894fdcc538d?q=80&w=1000&auto=format&fit=crop'),
('11111111-0000-0000-0000-000000000003', 'Evento Futuro 2030', 'Proximo evento en planificacion', '2030-01-01 00:00:00', '2031-12-31 23:59:59', NULL)
ON CONFLICT DO NOTHING;

-- ==========================================
-- 2. EQUIPOS
-- ==========================================
INSERT INTO equipo (id, "Nombre", created_at) VALUES 
('33333333-0000-0000-0000-000000000001', 'Equipo Alpha', NOW()),
('33333333-0000-0000-0000-000000000002', 'Beta Coders', NOW()),
('33333333-0000-0000-0000-000000000003', 'Gamma Devs', NOW()),
('33333333-0000-0000-0000-000000000004', 'Delta Hackers', NOW()),
('33333333-0000-0000-0000-000000000005', 'Epsilon Tech', NOW())
ON CONFLICT DO NOTHING;

-- ==========================================
-- 3. PARTICIPANTES (Pass: 123456 para todos)
-- ==========================================
INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES
('44444444-0000-0000-0000-000000000001', 'Admin Organizador', 'admin@votify.com', '$2a$11$eE1.M0OebfWz7pG3J.3O2.2z1fH4.jI8k3pXq8hZ6kZ5hX8qV2P5K', NULL),
('44444444-0000-0000-0000-000000000002', 'Jurado Experto 1', 'jurado1@votify.com', '$2a$11$eE1.M0OebfWz7pG3J.3O2.2z1fH4.jI8k3pXq8hZ6kZ5hX8qV2P5K', NULL),
('44444444-0000-0000-0000-000000000003', 'Jurado Experto 2', 'jurado2@votify.com', '$2a$11$eE1.M0OebfWz7pG3J.3O2.2z1fH4.jI8k3pXq8hZ6kZ5hX8qV2P5K', NULL),
('44444444-0000-0000-0000-000000000004', 'Alice (Alpha)', 'alpha@votify.com', '$2a$11$eE1.M0OebfWz7pG3J.3O2.2z1fH4.jI8k3pXq8hZ6kZ5hX8qV2P5K', '33333333-0000-0000-0000-000000000001'),
('44444444-0000-0000-0000-000000000005', 'Bob (Beta)', 'beta@votify.com', '$2a$11$eE1.M0OebfWz7pG3J.3O2.2z1fH4.jI8k3pXq8hZ6kZ5hX8qV2P5K', '33333333-0000-0000-0000-000000000002'),
('44444444-0000-0000-0000-000000000006', 'Charlie (Gamma)', 'gamma@votify.com', '$2a$11$eE1.M0OebfWz7pG3J.3O2.2z1fH4.jI8k3pXq8hZ6kZ5hX8qV2P5K', '33333333-0000-0000-0000-000000000003'),
('44444444-0000-0000-0000-000000000007', 'David (Delta)', 'delta@votify.com', '$2a$11$eE1.M0OebfWz7pG3J.3O2.2z1fH4.jI8k3pXq8hZ6kZ5hX8qV2P5K', '33333333-0000-0000-0000-000000000004'),
('44444444-0000-0000-0000-000000000008', 'Eva (Epsilon)', 'epsilon@votify.com', '$2a$11$eE1.M0OebfWz7pG3J.3O2.2z1fH4.jI8k3pXq8hZ6kZ5hX8qV2P5K', '33333333-0000-0000-0000-000000000005'),
('44444444-0000-0000-0000-000000000009', 'Publico 1', 'publico1@votify.com', '$2a$11$eE1.M0OebfWz7pG3J.3O2.2z1fH4.jI8k3pXq8hZ6kZ5hX8qV2P5K', NULL),
('44444444-0000-0000-0000-000000000010', 'Publico 2', 'publico2@votify.com', '$2a$11$eE1.M0OebfWz7pG3J.3O2.2z1fH4.jI8k3pXq8hZ6kZ5hX8qV2P5K', NULL)
ON CONFLICT DO NOTHING;

-- ==========================================
-- 4. ROLES DE PARTICIPANTES EN EL EVENTO ACTIVO (ID ...02)
-- ==========================================
INSERT INTO participante_evento (id, participante_id, evento_id, rol) VALUES
(gen_random_uuid(), '44444444-0000-0000-0000-000000000001', '11111111-0000-0000-0000-000000000002', 'ORGANIZADOR'),
(gen_random_uuid(), '44444444-0000-0000-0000-000000000002', '11111111-0000-0000-0000-000000000002', 'JURADO'),
(gen_random_uuid(), '44444444-0000-0000-0000-000000000003', '11111111-0000-0000-0000-000000000002', 'JURADO'),
(gen_random_uuid(), '44444444-0000-0000-0000-000000000004', '11111111-0000-0000-0000-000000000002', 'COMPETIDOR'),
(gen_random_uuid(), '44444444-0000-0000-0000-000000000005', '11111111-0000-0000-0000-000000000002', 'COMPETIDOR'),
(gen_random_uuid(), '44444444-0000-0000-0000-000000000006', '11111111-0000-0000-0000-000000000002', 'COMPETIDOR'),
(gen_random_uuid(), '44444444-0000-0000-0000-000000000007', '11111111-0000-0000-0000-000000000002', 'COMPETIDOR'),
(gen_random_uuid(), '44444444-0000-0000-0000-000000000008', '11111111-0000-0000-0000-000000000002', 'COMPETIDOR'),
(gen_random_uuid(), '44444444-0000-0000-0000-000000000009', '11111111-0000-0000-0000-000000000002', 'PUBLICO'),
(gen_random_uuid(), '44444444-0000-0000-0000-000000000010', '11111111-0000-0000-0000-000000000002', 'PUBLICO')
ON CONFLICT DO NOTHING;

-- ==========================================
-- 5. VOTACIONES EN EVENTO ACTIVO
-- ==========================================
INSERT INTO votacion (id, nombre, tipo, fecha_inicio, fecha_fin, limite_proy, comentarios, comentarios_obligatorios, "EsAnonima", evento, estado, imagen_url) VALUES
('22222222-0000-0000-0000-000000000001', 'Mejor Diseño UI (Estandar)', 'ESTANDAR', '2020-01-01 00:00:00', '2030-12-31 23:59:59', 2, true, false, false, '11111111-0000-0000-0000-000000000002', 0, NULL),
('22222222-0000-0000-0000-000000000002', 'Evaluacion Tecnica (Jurado)', 'MULTICRITERIO', '2020-01-01 00:00:00', '2030-12-31 23:59:59', 1, true, true, false, '11111111-0000-0000-0000-000000000002', 0, NULL),
('22222222-0000-0000-0000-000000000003', 'Voto Popular Anonimo', 'ESTANDAR', '2020-01-01 00:00:00', '2030-12-31 23:59:59', 1, true, false, true, '11111111-0000-0000-0000-000000000002', 0, NULL)
ON CONFLICT DO NOTHING;

-- ==========================================
-- 6. CRITERIOS PARA VOTACION MULTICRITERIO
-- ==========================================
INSERT INTO criterio (id, votacion_id, nombre, tipo, peso) VALUES
('77777777-0000-0000-0000-000000000001', '22222222-0000-0000-0000-000000000002', 'Arquitectura', 'Estrellas', 40.0),
('77777777-0000-0000-0000-000000000002', '22222222-0000-0000-0000-000000000002', 'Rendimiento', 'Estrellas', 30.0),
('77777777-0000-0000-0000-000000000003', '22222222-0000-0000-0000-000000000002', 'Viabilidad Comercial', 'Estrellas', 30.0)
ON CONFLICT DO NOTHING;

-- ==========================================
-- 7. PROYECTOS (Disponibles en las 3 Votaciones)
-- ==========================================
-- Para Votacion 1 (Estandar)
INSERT INTO proyecto (id, nombre, descripcion, equipo, votacion_id, imagen_url) VALUES
('55555555-0000-0000-0001-000000000001', 'Plataforma AI', 'Una plataforma potenciada por Inteligencia Artificial', '33333333-0000-0000-0000-000000000001', '22222222-0000-0000-0000-000000000001', NULL),
('55555555-0000-0000-0001-000000000002', 'App Salud', 'Seguimiento de constantes vitales', '33333333-0000-0000-0000-000000000002', '22222222-0000-0000-0000-000000000001', NULL),
('55555555-0000-0000-0001-000000000003', 'Finanzas Web3', 'DeFi simple para todos', '33333333-0000-0000-0000-000000000003', '22222222-0000-0000-0000-000000000001', NULL);

-- Para Votacion 2 (Multicriterio)
INSERT INTO proyecto (id, nombre, descripcion, equipo, votacion_id, imagen_url) VALUES
('55555555-0000-0000-0002-000000000001', 'Plataforma AI', 'Arquitectura serverless en Azure', '33333333-0000-0000-0000-000000000001', '22222222-0000-0000-0000-000000000002', NULL),
('55555555-0000-0000-0002-000000000002', 'App Salud', 'Backend robusto con .NET 9', '33333333-0000-0000-0000-000000000002', '22222222-0000-0000-0000-000000000002', NULL),
('55555555-0000-0000-0002-000000000004', 'E-Commerce Bot', 'Bots de ventas automaticas', '33333333-0000-0000-0000-000000000004', '22222222-0000-0000-0000-000000000002', NULL);

-- ==========================================
-- 8. COMENTARIOS (Para probar el Resumidor IA en Votacion 1 - Plataforma AI)
-- ==========================================
INSERT INTO "Comentario" (id, texto, fecha_creacion, autor_id, proyecto_id, es_anonimo) VALUES
(gen_random_uuid(), 'El diseño esta increible, los colores y las transiciones se sienten muy modernos.', NOW() - INTERVAL '1 day', '44444444-0000-0000-0000-000000000002', '55555555-0000-0000-0001-000000000001', false),
(gen_random_uuid(), 'Me gusto la idea, pero tuve problemas al intentar cargar el dashboard, tardo mucho.', NOW() - INTERVAL '12 hours', '44444444-0000-0000-0000-000000000009', '55555555-0000-0000-0001-000000000001', false),
(gen_random_uuid(), 'Muy buena UX. Facil de entender a la primera sin leer tutoriales. Buen trabajo.', NOW() - INTERVAL '5 hours', '44444444-0000-0000-0000-000000000003', '55555555-0000-0000-0001-000000000001', false),
(gen_random_uuid(), 'Siento que el contraste en el modo oscuro podria mejorar, algunas letras no se leen.', NOW() - INTERVAL '2 hours', '44444444-0000-0000-0000-000000000010', '55555555-0000-0000-0001-000000000001', false),
(gen_random_uuid(), 'Excelente paleta de colores. El prototipo es funcional y esta muy pulido.', NOW(), '44444444-0000-0000-0000-000000000001', '55555555-0000-0000-0001-000000000001', false)
ON CONFLICT DO NOTHING;

-- ==========================================
-- 9. VOTOS YA EMITIDOS EN VOTACION 1 (ESTANDAR)
-- ==========================================
INSERT INTO votos (id, votacion_id, votante_id, proyecto_id, fecha_voto) VALUES
(gen_random_uuid(), '22222222-0000-0000-0000-000000000001', '44444444-0000-0000-0000-000000000002', '55555555-0000-0000-0001-000000000001', NOW()),
(gen_random_uuid(), '22222222-0000-0000-0000-000000000001', '44444444-0000-0000-0000-000000000009', '55555555-0000-0000-0001-000000000001', NOW()),
(gen_random_uuid(), '22222222-0000-0000-0000-000000000001', '44444444-0000-0000-0000-000000000003', '55555555-0000-0000-0001-000000000001', NOW()),
(gen_random_uuid(), '22222222-0000-0000-0000-000000000001', '44444444-0000-0000-0000-000000000010', '55555555-0000-0000-0001-000000000002', NOW())
ON CONFLICT DO NOTHING;

-- ==========================================
-- 10. VOTOS MULTICRITERIO (Jurado 1 votando en Votacion 2 - Plataforma AI)
-- ==========================================
WITH voto_id AS (
    INSERT INTO votos (id, votacion_id, votante_id, proyecto_id, fecha_voto) 
    VALUES (gen_random_uuid(), '22222222-0000-0000-0000-000000000002', '44444444-0000-0000-0000-000000000002', '55555555-0000-0000-0002-000000000001', NOW())
    RETURNING id
)
INSERT INTO valoracion_criterio (id, voto_id, criterio_id, puntuacion)
SELECT gen_random_uuid(), id, '77777777-0000-0000-0000-000000000001', 5 FROM voto_id UNION ALL -- 5 en Arquitectura
SELECT gen_random_uuid(), id, '77777777-0000-0000-0000-000000000002', 4 FROM voto_id UNION ALL -- 4 en Rendimiento
SELECT gen_random_uuid(), id, '77777777-0000-0000-0000-000000000003', 5 FROM voto_id;          -- 5 en Viabilidad Comercial

-- Comentario del jurado 1
INSERT INTO "Comentario" (id, texto, fecha_creacion, autor_id, proyecto_id, es_anonimo) VALUES
(gen_random_uuid(), 'La arquitectura es solida y bien estructurada. El rendimiento es bueno pero podria optimizarse un poco el tamaño de las imagenes.', NOW(), '44444444-0000-0000-0000-000000000002', '55555555-0000-0000-0002-000000000001', false)
ON CONFLICT DO NOTHING;
