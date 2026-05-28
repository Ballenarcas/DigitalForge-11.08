-- ============================================================
-- EVENTO: Competición de los Mejores Animes 2026
-- Script completo: crea participantes, evento y votaciones
-- ============================================================

DO $$
DECLARE
    v_evento_id UUID := '0e95e815-8e53-4de0-abe9-b2a23cfb97db';
    v_p1 UUID;  -- Ana García
    v_p2 UUID;  -- Carlos Ruiz
    v_p3 UUID;  -- María López
    v_p4 UUID;  -- Pedro Sánchez
    v_p5 UUID;  -- Laura Martínez
    v_p6 UUID;  -- David Torres
    v_p7 UUID;  -- Sofía Herrera
    v_p8 UUID;  -- Javier Moreno
    v_p9 UUID;  -- Dr. Elena Vargas (jurado)
    v_p10 UUID; -- Ing. Roberto Díaz (jurado)
    v_equipo1 UUID;
    v_equipo2 UUID;
    v_equipo3 UUID;
    v_equipo4 UUID;
    v_hash TEXT := '$2a$11$euODwkYyZ7wmRy6O35dJpOFW6eirVwUuE7qA0W6mLYNQdAva3RJou';
BEGIN

    -- Buscar participantes existentes
    SELECT id INTO v_p1 FROM participante WHERE email = 'ana.garcia@demo.com' LIMIT 1;
    SELECT id INTO v_p2 FROM participante WHERE email = 'carlos.ruiz@demo.com' LIMIT 1;
    SELECT id INTO v_p3 FROM participante WHERE email = 'maria.lopez@demo.com' LIMIT 1;
    SELECT id INTO v_p4 FROM participante WHERE email = 'pedro.sanchez@demo.com' LIMIT 1;
    SELECT id INTO v_p5 FROM participante WHERE email = 'laura.martinez@demo.com' LIMIT 1;
    SELECT id INTO v_p6 FROM participante WHERE email = 'david.torres@demo.com' LIMIT 1;
    SELECT id INTO v_p7 FROM participante WHERE email = 'sofia.herrera@demo.com' LIMIT 1;
    SELECT id INTO v_p8 FROM participante WHERE email = 'javier.moreno@demo.com' LIMIT 1;
    SELECT id INTO v_p9 FROM participante WHERE email = 'elena.vargas@demo.com' LIMIT 1;
    SELECT id INTO v_p10 FROM participante WHERE email = 'roberto.diaz@demo.com' LIMIT 1;

    -- Buscar equipos existentes
    SELECT id INTO v_equipo1 FROM equipo WHERE "Nombre" = 'CodeBreakers' LIMIT 1;
    SELECT id INTO v_equipo2 FROM equipo WHERE "Nombre" = 'PixelNomads' LIMIT 1;
    SELECT id INTO v_equipo3 FROM equipo WHERE "Nombre" = 'DataDragons' LIMIT 1;
    SELECT id INTO v_equipo4 FROM equipo WHERE "Nombre" = 'NullPointers' LIMIT 1;

    -- Crear participantes que no existan, o actualizar contraseña si ya existen
    IF v_p1 IS NULL THEN
        v_p1 := gen_random_uuid();
        INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES (v_p1, 'Ana García', 'ana.garcia@demo.com', v_hash, v_equipo1);
    ELSE
        UPDATE participante SET "PasswordHash" = v_hash WHERE id = v_p1;
    END IF;
    IF v_p2 IS NULL THEN
        v_p2 := gen_random_uuid();
        INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES (v_p2, 'Carlos Ruiz', 'carlos.ruiz@demo.com', v_hash, v_equipo1);
    ELSE
        UPDATE participante SET "PasswordHash" = v_hash WHERE id = v_p2;
    END IF;
    IF v_p3 IS NULL THEN
        v_p3 := gen_random_uuid();
        INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES (v_p3, 'María López', 'maria.lopez@demo.com', v_hash, v_equipo2);
    ELSE
        UPDATE participante SET "PasswordHash" = v_hash WHERE id = v_p3;
    END IF;
    IF v_p4 IS NULL THEN
        v_p4 := gen_random_uuid();
        INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES (v_p4, 'Pedro Sánchez', 'pedro.sanchez@demo.com', v_hash, v_equipo2);
    ELSE
        UPDATE participante SET "PasswordHash" = v_hash WHERE id = v_p4;
    END IF;
    IF v_p5 IS NULL THEN
        v_p5 := gen_random_uuid();
        INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES (v_p5, 'Laura Martínez', 'laura.martinez@demo.com', v_hash, v_equipo3);
    ELSE
        UPDATE participante SET "PasswordHash" = v_hash WHERE id = v_p5;
    END IF;
    IF v_p6 IS NULL THEN
        v_p6 := gen_random_uuid();
        INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES (v_p6, 'David Torres', 'david.torres@demo.com', v_hash, v_equipo3);
    ELSE
        UPDATE participante SET "PasswordHash" = v_hash WHERE id = v_p6;
    END IF;
    IF v_p7 IS NULL THEN
        v_p7 := gen_random_uuid();
        INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES (v_p7, 'Sofía Herrera', 'sofia.herrera@demo.com', v_hash, v_equipo4);
    ELSE
        UPDATE participante SET "PasswordHash" = v_hash WHERE id = v_p7;
    END IF;
    IF v_p8 IS NULL THEN
        v_p8 := gen_random_uuid();
        INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES (v_p8, 'Javier Moreno', 'javier.moreno@demo.com', v_hash, v_equipo4);
    ELSE
        UPDATE participante SET "PasswordHash" = v_hash WHERE id = v_p8;
    END IF;
    IF v_p9 IS NULL THEN
        v_p9 := gen_random_uuid();
        INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES (v_p9, 'Dr. Elena Vargas', 'elena.vargas@demo.com', v_hash, NULL);
    ELSE
        UPDATE participante SET "PasswordHash" = v_hash WHERE id = v_p9;
    END IF;
    IF v_p10 IS NULL THEN
        v_p10 := gen_random_uuid();
        INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES (v_p10, 'Ing. Roberto Díaz', 'roberto.diaz@demo.com', v_hash, NULL);
    ELSE
        UPDATE participante SET "PasswordHash" = v_hash WHERE id = v_p10;
    END IF;

    RAISE NOTICE 'Participantes: p1=%, p2=%, p3=%, p4=%, p5=%, p6=%, p7=%, p8=%, p9=%, p10=%', v_p1, v_p2, v_p3, v_p4, v_p5, v_p6, v_p7, v_p8, v_p9, v_p10;

    -- 1. EVENTO
    INSERT INTO evento (id, nombre, descripcion, fecha_inicio, fecha_fin, imagen_url)
    VALUES (
        v_evento_id,
        'Competición de los Mejores Animes 2026',
        'Votación basada en los Crunchyroll Anime Awards 2026. Elige tu anime, personaje, película y banda sonora favorita del año.',
        '2026-06-01 10:00:00',
        '2026-06-30 23:59:00',
        'https://images.unsplash.com/photo-1578632767115-351597cf2477?w=1200'
    );

    -- 2. PARTICIPANTE_EVENTO
    INSERT INTO participante_evento (id, participante_id, evento_id, rol) VALUES
    (gen_random_uuid(), v_p1, v_evento_id, 'ORGANIZADOR'),
    (gen_random_uuid(), v_p2, v_evento_id, 'PARTICIPANTE'),
    (gen_random_uuid(), v_p3, v_evento_id, 'PARTICIPANTE'),
    (gen_random_uuid(), v_p4, v_evento_id, 'PARTICIPANTE'),
    (gen_random_uuid(), v_p5, v_evento_id, 'PARTICIPANTE'),
    (gen_random_uuid(), v_p6, v_evento_id, 'PARTICIPANTE'),
    (gen_random_uuid(), v_p7, v_evento_id, 'PARTICIPANTE'),
    (gen_random_uuid(), v_p8, v_evento_id, 'PARTICIPANTE'),
    (gen_random_uuid(), v_p9, v_evento_id, 'JURADO'),
    (gen_random_uuid(), v_p10, v_evento_id, 'JURADO');

    -- 3. VOTACIONES
    INSERT INTO votacion (id, nombre, tipo, fecha_inicio, fecha_fin, limite_proy, comentarios, comentarios_obligatorios, "EsAnonima", evento, estado, imagen_url) VALUES
    ('686cbdaf-01d0-437d-ae72-2f16d97308ed', 'Anime del Año', 'ESTANDAR', '2026-06-01 10:00:00', '2026-06-30 23:59:00', 1, true, false, false, v_evento_id, 0, 'https://images.unsplash.com/photo-1613376023733-0a73315d9b06?w=800'),
    ('470afc87-1ffa-4e98-a088-366ba19e785e', 'Mejor Opening', 'ESTANDAR', '2026-06-01 10:00:00', '2026-06-30 23:59:00', 1, true, false, false, v_evento_id, 0, 'https://images.unsplash.com/photo-1514320291840-2e0a9bf2a9ae?w=800'),
    ('0d4952a7-8c7d-4958-b2c0-1723b170f638', 'Mejor Personaje Masculino', 'MULTICRITERIO', '2026-06-01 10:00:00', '2026-06-30 23:59:00', 1, true, true, false, v_evento_id, 0, 'https://images.unsplash.com/photo-1578632767115-351597cf2477?w=800'),
    ('cc599e4c-4cfc-4057-b942-01227268f38f', 'Mejor Película', 'MULTICRITERIO_PUBLICO', '2026-06-01 10:00:00', '2026-06-30 23:59:00', 1, true, false, true, v_evento_id, 0, 'https://images.unsplash.com/photo-1440404653325-ab127d49abc1?w=800');

    -- 4. CRITERIOS
    INSERT INTO criterio (id, votacion_id, nombre, tipo, peso) VALUES
    (gen_random_uuid(), '0d4952a7-8c7d-4958-b2c0-1723b170f638', 'Desarrollo del personaje', 'Estrellas', 40),
    (gen_random_uuid(), '0d4952a7-8c7d-4958-b2c0-1723b170f638', 'Impacto en la historia', 'Estrellas', 35),
    (gen_random_uuid(), '0d4952a7-8c7d-4958-b2c0-1723b170f638', 'Diseño y animación', 'Estrellas', 25),
    (gen_random_uuid(), 'cc599e4c-4cfc-4057-b942-01227268f38f', 'Calidad de animación', 'Estrellas', 30),
    (gen_random_uuid(), 'cc599e4c-4cfc-4057-b942-01227268f38f', 'Guión y narrativa', 'Estrellas', 40),
    (gen_random_uuid(), 'cc599e4c-4cfc-4057-b942-01227268f38f', 'Banda sonora', 'Estrellas', 30);

    -- 5. PROYECTOS
    INSERT INTO proyecto (id, nombre, descripcion, equipo, votacion_id, imagen_url) VALUES
    (gen_random_uuid(), 'Dandadan', 'Okarun y Momo descubren extraterrestres y fantasmas en una aventura frenética.', v_equipo1, '686cbdaf-01d0-437d-ae72-2f16d97308ed', 'https://images.unsplash.com/photo-1613376023733-0a73315d9b06?w=800'),
    (gen_random_uuid(), 'Solo Leveling Temporada 2', 'Sung Jin-woo se enfrenta a las mazmorras de rango S.', v_equipo2, '686cbdaf-01d0-437d-ae72-2f16d97308ed', 'https://images.unsplash.com/photo-1618336753974-aae8e04506aa?w=800'),
    (gen_random_uuid(), 'Jujutsu Kaisen: Culling Game', 'Yuji Itadori entra al mortal Culling Game para rescatar a Gojo.', v_equipo3, '686cbdaf-01d0-437d-ae72-2f16d97308ed', 'https://images.unsplash.com/photo-1601850494422-3cf14624b0b3?w=800'),
    (gen_random_uuid(), 'My Hero Academia: Final Season', 'La batalla definitiva entre héroes y villanos.', v_equipo4, '686cbdaf-01d0-437d-ae72-2f16d97308ed', 'https://images.unsplash.com/photo-1518709268805-4e9042af9f23?w=800'),
    (gen_random_uuid(), 'Bling-Bang-Bang-Born - Mashle S2', 'El opening más viral del año con coreografía global.', v_equipo1, '470afc87-1ffa-4e98-a088-366ba19e785e', 'https://images.unsplash.com/photo-1514320291840-2e0a9bf2a9ae?w=800'),
    (gen_random_uuid(), 'Zankyosanka - Demon Slayer S4', 'Aimer regresa con un tema épico de los Hashira.', v_equipo2, '470afc87-1ffa-4e98-a088-366ba19e785e', 'https://images.unsplash.com/photo-1511379938547-c1f69419868d?w=800'),
    (gen_random_uuid(), 'Ao no Sumika - Jujutsu Kaisen', 'Opening melancólico himno entre los fans.', v_equipo3, '470afc87-1ffa-4e98-a088-366ba19e785e', 'https://images.unsplash.com/photo-1493225457124-a3eb161ffa5f?w=800'),
    (gen_random_uuid(), 'SPECIALZ - JJK Shibuya', 'Fenómeno global con ritmo impredecible.', v_equipo4, '470afc87-1ffa-4e98-a088-366ba19e785e', 'https://images.unsplash.com/photo-1507838153414-b4b713384a76?w=800'),
    (gen_random_uuid(), 'Gojo Satoru - JJK', 'El hechicero más fuerte: "Nah, I''d win".', v_equipo1, '0d4952a7-8c7d-4958-b2c0-1723b170f638', 'https://images.unsplash.com/photo-1578632767115-351597cf2477?w=800'),
    (gen_random_uuid(), 'Sung Jin-woo - Solo Leveling', 'De rango E al más poderoso del mundo.', v_equipo2, '0d4952a7-8c7d-4958-b2c0-1723b170f638', 'https://images.unsplash.com/photo-1618336753974-aae8e04506aa?w=800'),
    (gen_random_uuid(), 'Okarun - Dandadan', 'Chico tímido en pelea con alienígenas. Absurdo y genial.', v_equipo3, '0d4952a7-8c7d-4958-b2c0-1723b170f638', 'https://images.unsplash.com/photo-1613376023733-0a73315d9b06?w=800'),
    (gen_random_uuid(), 'Demon Slayer: Hashira Training', 'Preparación final contra Muzan.', v_equipo1, 'cc599e4c-4cfc-4057-b942-01227268f38f', 'https://images.unsplash.com/photo-1560972550-aba3456b5564?w=800'),
    (gen_random_uuid(), 'Haikyuu!! The Dumpster Battle', 'Hinata vs Kageyama contra Nekoma.', v_equipo2, 'cc599e4c-4cfc-4057-b942-01227268f38f', 'https://images.unsplash.com/photo-1440404653325-ab127d49abc1?w=800'),
    (gen_random_uuid(), 'Look Back', 'Amistad y pasión por el manga entre dos artistas.', v_equipo3, 'cc599e4c-4cfc-4057-b942-01227268f38f', 'https://images.unsplash.com/photo-1535016120720-40c646be5580?w=800');

    RAISE NOTICE 'Evento de anime creado correctamente';

END $$;
