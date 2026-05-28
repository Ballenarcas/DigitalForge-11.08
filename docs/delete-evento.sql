-- Eliminar evento con ID: 564b2de6-9173-48d3-9f17-a25ede841d99
-- Ejecutar en la base de datos de Supabase

DO $$
DECLARE
    v_evento_id UUID := '564b2de6-9173-48d3-9f17-a25ede841d99';
    v_votacion_id UUID;
BEGIN
    -- 1. Participante-evento
    DELETE FROM participante_evento WHERE evento_id = v_evento_id;

    -- 2. Para cada votación del evento, eliminar en orden correcto
    FOR v_votacion_id IN SELECT id FROM votacion WHERE evento = v_evento_id
    LOOP
        -- Valoraciones de criterio por proyecto
        DELETE FROM valoracion_criterio
        WHERE proyecto_id IN (SELECT id FROM proyecto WHERE votacion_id = v_votacion_id);

        -- Comentarios de proyectos
        DELETE FROM "Comentario"
        WHERE proyecto_id IN (SELECT id FROM proyecto WHERE votacion_id = v_votacion_id);

        -- Asignaciones manuales
        DELETE FROM "ManualVotosAsignacion" WHERE votacion_id = v_votacion_id;

        -- Votos
        DELETE FROM votos WHERE votacion_id = v_votacion_id;

        -- Valoraciones de criterio por criterio
        DELETE FROM valoracion_criterio
        WHERE criterio_id IN (SELECT id FROM criterio WHERE votacion_id = v_votacion_id);

        -- Criterios
        DELETE FROM criterio WHERE votacion_id = v_votacion_id;

        -- Proyectos
        DELETE FROM proyecto WHERE votacion_id = v_votacion_id;
    END LOOP;

    -- 3. Votaciones
    DELETE FROM votacion WHERE evento = v_evento_id;

    -- 4. Notificaciones que referencien el evento
    DELETE FROM notificacion WHERE recurso_id = v_evento_id::text;

    -- 5. El evento
    DELETE FROM evento WHERE id = v_evento_id;

    RAISE NOTICE 'Evento eliminado correctamente';
END $$;
