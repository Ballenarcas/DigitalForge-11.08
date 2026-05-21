-- Migration: Add ManualVotosAsignacion table
-- Descripción: Crear tabla para almacenar asignaciones manuales de votos

CREATE TABLE IF NOT EXISTS "ManualVotosAsignacion" (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    votacion_id UUID NOT NULL,
    proyecto_id UUID NOT NULL,
    posicion_final INTEGER NOT NULL,
    votos_asignados INTEGER NOT NULL,
    fecha_creacion TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    creado_por VARCHAR(255),
    texto_justificacion TEXT,
    usuario_justificacion VARCHAR(255),
    rol_usuario_justificacion VARCHAR(50),
    fecha_justificacion TIMESTAMP
);

-- Crear índices para mejorar consultas
CREATE INDEX IF NOT EXISTS idx_manual_votos_votacion
    ON "ManualVotosAsignacion"(votacion_id);

CREATE INDEX IF NOT EXISTS idx_manual_votos_proyecto
    ON "ManualVotosAsignacion"(proyecto_id);

CREATE UNIQUE INDEX IF NOT EXISTS idx_manual_votos_votacion_proyecto
    ON "ManualVotosAsignacion"(votacion_id, proyecto_id);
