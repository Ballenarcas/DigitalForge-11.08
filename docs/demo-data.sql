-- ============================================================
-- VOTIFY - Datos Demo para Presentación
-- ============================================================
-- Este script inserta datos realistas para una demo completa.
-- Ejecutar en PostgreSQL después de crear las tablas (migraciones).
-- ============================================================

-- Limpiar tablas (opcional, descomentar si se quiere resetear)
-- TRUNCATE TABLE valoracion_criterio, Comentario, votos, criterio, proyecto, votacion, participante_evento, evento, participante, equipo RESTART IDENTITY CASCADE;

-- ============================================================
-- 1. EQUIPOS
-- ============================================================
INSERT INTO equipo (id, "Nombre", created_at) VALUES
('065efc0a-b6ff-4e57-a99c-bae6286329cf', 'CodeBreakers',  '2026-01-15 10:00:00'),
('bc7f2ef4-39e8-48e0-bad9-44f50d0d35ef', 'PixelNomads',   '2026-01-16 14:30:00'),
('4b697156-52fe-43cb-ad87-84fc7bdc28c7', 'DataDragons',   '2026-01-17 09:15:00'),
('0a739eec-6c62-4ab1-a112-1f0e9cf4d883', 'NullPointers',  '2026-01-18 11:45:00');

-- ============================================================
-- 2. PARTICIPANTES
-- ============================================================
-- Contraseña para todos: "Demo1234!" (hash BCrypt)
INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES
('155cf90d-37a7-4674-8404-8c45d063ec33', 'Ana García',      'ana.garcia@demo.com',      '$2a$11$K8Y8Q8Z8a8b8c8d8e8f8g8h8i8j8k8l8m8n8o8p8q8r8s8t8u8v8w', '065efc0a-b6ff-4e57-a99c-bae6286329cf'),
('ff2c1ec6-3f80-4821-9441-4ba074c9f31e', 'Carlos Ruiz',     'carlos.ruiz@demo.com',     '$2a$11$K8Y8Q8Z8a8b8c8d8e8f8g8h8i8j8k8l8m8n8o8p8q8r8s8t8u8v8w', '065efc0a-b6ff-4e57-a99c-bae6286329cf'),
('17c7167b-79e2-40bf-a63e-619206686d43', 'María López',     'maria.lopez@demo.com',     '$2a$11$K8Y8Q8Z8a8b8c8d8e8f8g8h8i8j8k8l8m8n8o8p8q8r8s8t8u8v8w', 'bc7f2ef4-39e8-48e0-bad9-44f50d0d35ef'),
('0a8338f1-0db4-4701-aa57-64e6dc11650f', 'Pedro Sánchez',   'pedro.sanchez@demo.com',   '$2a$11$K8Y8Q8Z8a8b8c8d8e8f8g8h8i8j8k8l8m8n8o8p8q8r8s8t8u8v8w', 'bc7f2ef4-39e8-48e0-bad9-44f50d0d35ef'),
('3c25a1c0-8006-4c40-8752-42bbe0b90170', 'Laura Martínez',  'laura.martinez@demo.com',  '$2a$11$K8Y8Q8Z8a8b8c8d8e8f8g8h8i8j8k8l8m8n8o8p8q8r8s8t8u8v8w', '4b697156-52fe-43cb-ad87-84fc7bdc28c7'),
('7cd337a3-5fb0-4634-9427-b188e210202d', 'David Torres',    'david.torres@demo.com',    '$2a$11$K8Y8Q8Z8a8b8c8d8e8f8g8h8i8j8k8l8m8n8o8p8q8r8s8t8u8v8w', '4b697156-52fe-43cb-ad87-84fc7bdc28c7'),
('94b53671-32c2-4bcc-b6c5-cf0d25bfe715', 'Sofía Herrera',   'sofia.herrera@demo.com',   '$2a$11$K8Y8Q8Z8a8b8c8d8e8f8g8h8i8j8k8l8m8n8o8p8q8r8s8t8u8v8w', '0a739eec-6c62-4ab1-a112-1f0e9cf4d883'),
('000120d8-5e10-4e75-af31-ac0a88f09f52', 'Javier Moreno',   'javier.moreno@demo.com',   '$2a$11$K8Y8Q8Z8a8b8c8d8e8f8g8h8i8j8k8l8m8n8o8p8q8r8s8t8u8v8w', '0a739eec-6c62-4ab1-a112-1f0e9cf4d883');

-- Jurado (sin equipo)
INSERT INTO participante (id, nombre, email, "PasswordHash", equipo) VALUES
('5cd66a71-e5af-43f1-8f9a-282d31eaf46d', 'Dr. Elena Vargas', 'elena.vargas@demo.com', '$2a$11$K8Y8Q8Z8a8b8c8d8e8f8g8h8i8j8k8l8m8n8o8p8q8r8s8t8u8v8w', NULL),
('54b69dde-2d56-4c66-b70e-b1a3d7174257', 'Ing. Roberto Díaz', 'roberto.diaz@demo.com', '$2a$11$K8Y8Q8Z8a8b8c8d8e8f8g8h8i8j8k8l8m8n8o8p8q8r8s8t8u8v8w', NULL);

-- ============================================================
-- 3. EVENTOS
-- ============================================================
INSERT INTO evento (id, nombre, descripcion, fecha_inicio, fecha_fin, imagen_url) VALUES
('b11fff84-d840-4b90-8d84-c8f6d912e60f', 'Hackathon Anual 2026', 'Competencia de 48 horas para desarrollar soluciones innovadoras en equipos multidisciplinarios.', '2026-05-20 08:00:00', '2026-05-25 23:59:00', 'https://images.unsplash.com/photo-1504384308090-c54be3855463?w=1200'),
('82f55d08-5807-4bfc-8eb6-d25fdbd8c7a9', 'Feria de Proyectos PSW', 'Muestra de proyectos finales de la asignatura Proyectos de Software. Votación pública con jurado.', '2026-06-01 09:00:00', '2026-06-10 20:00:00', 'https://images.unsplash.com/photo-1556761175-5973dc0f32e7?w=1200');

-- ============================================================
-- 4. PARTICIPANTE_EVENTO (roles: ORGANIZADOR, PARTICIPANTE, JURADO)
-- ============================================================
INSERT INTO participante_evento (id, participante_id, evento_id, rol) VALUES
('a663ff10-a775-45e4-96c9-2196de728968', '155cf90d-37a7-4674-8404-8c45d063ec33', 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 'ORGANIZADOR'),
('3c994911-9deb-4a33-b9ca-dc321fc46805', 'ff2c1ec6-3f80-4821-9441-4ba074c9f31e', 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 'PARTICIPANTE'),
('037107fe-1f86-47bf-8883-eba6375999bb', '17c7167b-79e2-40bf-a63e-619206686d43', 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 'PARTICIPANTE'),
('9946c0ac-a89e-4330-bb03-0c04b7324aac', '0a8338f1-0db4-4701-aa57-64e6dc11650f', 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 'PARTICIPANTE'),
('4ca279c7-ad21-4d47-884c-7d3cb4c6f976', '3c25a1c0-8006-4c40-8752-42bbe0b90170', 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 'PARTICIPANTE'),
('3e692390-8ddb-4cf9-ace8-1509f6e731cf', '7cd337a3-5fb0-4634-9427-b188e210202d', 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 'PARTICIPANTE'),
('c3651fae-0913-4270-b616-b4afffbf7278', '94b53671-32c2-4bcc-b6c5-cf0d25bfe715', 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 'PARTICIPANTE'),
('6cb673a6-1769-49aa-8491-d5016c7be1c4', '000120d8-5e10-4e75-af31-ac0a88f09f52', 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 'PARTICIPANTE'),
('acc82047-107c-4715-8ae3-1ac937b0503c', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 'JURADO'),
('1d912953-808f-419c-93e9-05d76bb95324', '54b69dde-2d56-4c66-b70e-b1a3d7174257', 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 'JURADO');

-- Segundo evento
INSERT INTO participante_evento (id, participante_id, evento_id, rol) VALUES
('2015f6d9-3722-4d8f-a50e-3518b0604d86', '155cf90d-37a7-4674-8404-8c45d063ec33', '82f55d08-5807-4bfc-8eb6-d25fdbd8c7a9', 'ORGANIZADOR'),
('3d743b70-c25a-4630-9972-cc64189b3f1a', '17c7167b-79e2-40bf-a63e-619206686d43', '82f55d08-5807-4bfc-8eb6-d25fdbd8c7a9', 'PARTICIPANTE'),
('90004587-19ea-4df7-b07a-09e78f0be5b5', '3c25a1c0-8006-4c40-8752-42bbe0b90170', '82f55d08-5807-4bfc-8eb6-d25fdbd8c7a9', 'PARTICIPANTE'),
('7f57fed7-516f-4f49-84f6-56a5a25b1433', '94b53671-32c2-4bcc-b6c5-cf0d25bfe715', '82f55d08-5807-4bfc-8eb6-d25fdbd8c7a9', 'PARTICIPANTE'),
('50609522-c002-4926-8087-4108a5225513', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '82f55d08-5807-4bfc-8eb6-d25fdbd8c7a9', 'JURADO');

-- ============================================================
-- 5. VOTACIONES
-- ============================================================
-- Estado: 0=Abierta, 1=Pausada, 2=Detenida
INSERT INTO votacion (id, nombre, tipo, fecha_inicio, fecha_fin, limite_proy, comentarios, comentarios_obligatorios, "EsAnonima", evento, estado, imagen_url) VALUES
('11e3e191-ac72-47f9-b616-9e136be6af52', 'Votación Popular - Hackathon', 'ESTANDAR', '2026-05-22 10:00:00', '2026-05-24 18:00:00', 3, true, false, false, 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 0, 'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800'),
('0fe80252-d366-4a46-a4c9-a8786b30c336', 'Evaluación Jurado - Hackathon', 'MULTICRITERIO', '2026-05-23 09:00:00', '2026-05-24 20:00:00', 2, true, true, false, 'b11fff84-d840-4b90-8d84-c8f6d912e60f', 0, 'https://images.unsplash.com/photo-1551836022-d5d88e9218df?w=800'),
('96d50739-28a9-4352-9f9e-38129cc62a32', 'Votación Pública - Feria PSW', 'MULTICRITERIO_PUBLICO', '2026-06-05 10:00:00', '2026-06-09 18:00:00', 5, true, false, true, '82f55d08-5807-4bfc-8eb6-d25fdbd8c7a9', 0, 'https://images.unsplash.com/photo-1544531586-fde5298cdd40?w=800');

-- ============================================================
-- 6. CRITERIOS (solo para votaciones multicriterio)
-- ============================================================
INSERT INTO criterio (id, votacion_id, nombre, tipo, peso) VALUES
('7355feee-500c-4ef9-b2f5-16ad00c968c0', '0fe80252-d366-4a46-a4c9-a8786b30c336', 'Innovación Técnica', 'Estrellas', 35.00),
('7c062054-94b5-4114-91a0-3f3ce0fda5d7', '0fe80252-d366-4a46-a4c9-a8786b30c336', 'Usabilidad y Diseño', 'Estrellas', 30.00),
('48b4207f-9a52-4580-915f-61731f0d06db', '0fe80252-d366-4a46-a4c9-a8786b30c336', 'Viabilidad Comercial', 'Estrellas', 20.00),
('37c6d783-2ff2-43a8-ac4b-66201c980cba', '0fe80252-d366-4a46-a4c9-a8786b30c336', 'Presentación', 'Estrellas', 15.00),
('627e421f-928d-4d6a-96fb-2d17ae00fcc8', '96d50739-28a9-4352-9f9e-38129cc62a32', 'Impacto Social', 'Estrellas', 40.00),
('3bce34cb-5afc-48ec-8900-c167e1dd3224', '96d50739-28a9-4352-9f9e-38129cc62a32', 'Creatividad', 'Estrellas', 35.00),
('2660f601-70a5-4e9d-b67b-1860118ff852', '96d50739-28a9-4352-9f9e-38129cc62a32', 'Funcionalidad', 'Estrellas', 25.00);

-- ============================================================
-- 7. PROYECTOS
-- ============================================================
INSERT INTO proyecto (id, nombre, descripcion, equipo, votacion_id, imagen_url) VALUES
('8061f492-53ff-4ea4-a5c0-d6131fe4a386', 'EcoTrack - Monitor de Huella de Carbono', 'Aplicación IoT que mide en tiempo real el consumo energético del hogar y sugiere optimizaciones mediante IA.', '065efc0a-b6ff-4e57-a99c-bae6286329cf', '11e3e191-ac72-47f9-b616-9e136be6af52', 'https://images.unsplash.com/photo-1497435334941-8c899ee9e8e9?w=800'),
('08b7f9e3-67a9-4e2a-8964-f2f6fd48e858', 'MediLink - Telemedicina Rural', 'Plataforma de videoconsultas con sincronización offline para zonas sin conectividad estable.', 'bc7f2ef4-39e8-48e0-bad9-44f50d0d35ef', '11e3e191-ac72-47f9-b616-9e136be6af52', 'https://images.unsplash.com/photo-1576091160399-112ba8d25d1d?w=800'),
('25183177-d92e-4d6e-a56c-7efa9eb0c1ef', 'AgroVision - Drones Agrícolas', 'Sistema de análisis de cultivos con visión computacional para detectar plagas y optimizar riego.', '4b697156-52fe-43cb-ad87-84fc7bdc28c7', '11e3e191-ac72-47f9-b616-9e136be6af52', 'https://images.unsplash.com/photo-1508614589041-895b8c9d7ef5?w=800'),
('2dffce1e-8fc9-4a72-b6c2-2bf0765293de', 'EduVR - Realidad Virtual Educativa', 'Entornos inmersivos de realidad virtual para enseñanza de anatomía y ciencias naturales.', '0a739eec-6c62-4ab1-a112-1f0e9cf4d883', '11e3e191-ac72-47f9-b616-9e136be6af52', 'https://images.unsplash.com/photo-1592478411213-61535fdd861d?w=800'),
('04898ec5-f2f2-4aab-8978-3f4cfcbc257a', 'SafeWalk - Navegación Accesible', 'App de mapas con rutas accesibles para personas con movilidad reducida y problemas visuales.', '065efc0a-b6ff-4e57-a99c-bae6286329cf', '0fe80252-d366-4a46-a4c9-a8786b30c336', 'https://images.unsplash.com/photo-1526778548025-fa2f459cd5c1?w=800'),
('a2524adf-e8e7-4926-adea-94640f3b7b63', 'GreenCart - Mercado Circular', 'Marketplace B2C de productos reciclados y upcycleados con trazabilidad blockchain.', 'bc7f2ef4-39e8-48e0-bad9-44f50d0d35ef', '0fe80252-d366-4a46-a4c9-a8786b30c336', 'https://images.unsplash.com/photo-1532996122724-e3c354a0b15b?w=800'),
('b8032b63-a52a-4ca8-9ab5-d9108d51f2f8', 'CodeLearn - Tutor Interactivo', 'Asistente de programación con ejercicios gamificados y feedback en tiempo real.', '4b697156-52fe-43cb-ad87-84fc7bdc28c7', '96d50739-28a9-4352-9f9e-38129cc62a32', 'https://images.unsplash.com/photo-1516321318423-f06f85e504b3?w=800'),
('d5e43765-a6c5-439a-bb1c-c59d07c4d50b', 'PetCare - Salud Animal', 'Wearable para mascotas que monitorea signos vitales y alerta al veterinario.', '0a739eec-6c62-4ab1-a112-1f0e9cf4d883', '96d50739-28a9-4352-9f9e-38129cc62a32', 'https://images.unsplash.com/photo-1548199973-03cce0bbc87b?w=800');

-- ============================================================
-- 8. VOTOS (votación estándar v1)
-- ============================================================
INSERT INTO votos (id, proyecto_id, votante_id, votacion_id, fecha) VALUES
('5034ae38-c584-450e-abfc-c1e15d9259ff', '8061f492-53ff-4ea4-a5c0-d6131fe4a386', '17c7167b-79e2-40bf-a63e-619206686d43', '11e3e191-ac72-47f9-b616-9e136be6af52', '2026-05-22 14:30:00'),
('00e4086d-3a44-47f2-a24d-3538e789264b', '8061f492-53ff-4ea4-a5c0-d6131fe4a386', '3c25a1c0-8006-4c40-8752-42bbe0b90170', '11e3e191-ac72-47f9-b616-9e136be6af52', '2026-05-22 16:15:00'),
('1b39b5d3-d834-4035-ac1d-857d0044fedd', '8061f492-53ff-4ea4-a5c0-d6131fe4a386', '94b53671-32c2-4bcc-b6c5-cf0d25bfe715', '11e3e191-ac72-47f9-b616-9e136be6af52', '2026-05-23 09:45:00'),
('3ae5b489-151f-4d32-9546-dcea74e67df7', '08b7f9e3-67a9-4e2a-8964-f2f6fd48e858', '0a8338f1-0db4-4701-aa57-64e6dc11650f', '11e3e191-ac72-47f9-b616-9e136be6af52', '2026-05-22 11:00:00'),
('8a8cd6f0-d953-410f-89ea-7b7fd4cebf1e', '08b7f9e3-67a9-4e2a-8964-f2f6fd48e858', '7cd337a3-5fb0-4634-9427-b188e210202d', '11e3e191-ac72-47f9-b616-9e136be6af52', '2026-05-23 10:20:00'),
('42bd0e49-02c5-4544-907b-e337c6c6463b', '25183177-d92e-4d6e-a56c-7efa9eb0c1ef', '000120d8-5e10-4e75-af31-ac0a88f09f52', '11e3e191-ac72-47f9-b616-9e136be6af52', '2026-05-22 18:00:00'),
('58fd9fb7-ea65-458c-8eea-ca3d3c6437c3', '25183177-d92e-4d6e-a56c-7efa9eb0c1ef', 'ff2c1ec6-3f80-4821-9441-4ba074c9f31e', '11e3e191-ac72-47f9-b616-9e136be6af52', '2026-05-23 14:10:00'),
('6392fed4-1e4e-4ec4-8b43-a17b2064f5dc', '2dffce1e-8fc9-4a72-b6c2-2bf0765293de', '17c7167b-79e2-40bf-a63e-619206686d43', '11e3e191-ac72-47f9-b616-9e136be6af52', '2026-05-23 16:45:00'),
('2e22165d-1072-4426-90c7-cf81a0e05482', '2dffce1e-8fc9-4a72-b6c2-2bf0765293de', '3c25a1c0-8006-4c40-8752-42bbe0b90170', '11e3e191-ac72-47f9-b616-9e136be6af52', '2026-05-24 09:30:00');

-- Votos para votación jurado multicriterio (v2) - solo jurados votan
INSERT INTO votos (id, proyecto_id, votante_id, votacion_id, fecha) VALUES
('2eb98ae7-8539-48c6-b44c-0875c7aee34c', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '0fe80252-d366-4a46-a4c9-a8786b30c336', '2026-05-23 15:00:00'),
('1c382c76-489f-4a1e-b105-1e085a59d3ea', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', '54b69dde-2d56-4c66-b70e-b1a3d7174257', '0fe80252-d366-4a46-a4c9-a8786b30c336', '2026-05-23 16:30:00'),
('34636dde-5c6a-41e2-ac9d-cf77acab605e', 'a2524adf-e8e7-4926-adea-94640f3b7b63', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '0fe80252-d366-4a46-a4c9-a8786b30c336', '2026-05-23 17:00:00'),
('221e003c-b1ac-4aa1-9bf3-1d0b73c757f3', 'a2524adf-e8e7-4926-adea-94640f3b7b63', '54b69dde-2d56-4c66-b70e-b1a3d7174257', '0fe80252-d366-4a46-a4c9-a8786b30c336', '2026-05-24 10:00:00');

-- Votos para votación pública multicriterio (v3)
INSERT INTO votos (id, proyecto_id, votante_id, votacion_id, fecha) VALUES
('32213bbe-c781-4b47-ade9-14195e388069', 'b8032b63-a52a-4ca8-9ab5-d9108d51f2f8', '155cf90d-37a7-4674-8404-8c45d063ec33', '96d50739-28a9-4352-9f9e-38129cc62a32', '2026-06-06 11:00:00'),
('4367c283-cd6b-4add-9c76-040fa844d606', 'd5e43765-a6c5-439a-bb1c-c59d07c4d50b', '17c7167b-79e2-40bf-a63e-619206686d43', '96d50739-28a9-4352-9f9e-38129cc62a32', '2026-06-07 14:20:00'),
('42f7393e-9acd-4246-927e-1fc3915f86a3', 'd5e43765-a6c5-439a-bb1c-c59d07c4d50b', '3c25a1c0-8006-4c40-8752-42bbe0b90170', '96d50739-28a9-4352-9f9e-38129cc62a32', '2026-06-08 09:15:00');

-- ============================================================
-- 9. COMENTARIOS
-- ============================================================
INSERT INTO "Comentario" (id, proyecto_id, autor_id, texto, fecha_creacion) VALUES
('914e8554-79fe-4d18-97aa-77219538afe7', '8061f492-53ff-4ea4-a5c0-d6131fe4a386', '17c7167b-79e2-40bf-a63e-619206686d43', 'Excelente concepto, la integración con IoT es muy sólida. Me gustaría ver una versión para empresas.', '2026-05-22 15:00:00'),
('29247496-997a-478d-8fa2-6d299bbbd61f', '8061f492-53ff-4ea4-a5c0-d6131fe4a386', '3c25a1c0-8006-4c40-8752-42bbe0b90170', 'La interfaz es muy intuitiva. El dashboard de métricas es claro y útil.', '2026-05-23 10:00:00'),
('50fdb939-6896-43f0-8040-ab3b55bedf37', '08b7f9e3-67a9-4e2a-8964-f2f6fd48e858', '0a8338f1-0db4-4701-aa57-64e6dc11650f', 'Una solución realmente necesaria para el sector salud rural. La sincronización offline es clave.', '2026-05-22 12:00:00'),
('b2fc00d8-7566-404f-9d3b-5d41c64f8a91', '25183177-d92e-4d6e-a56c-7efa9eb0c1ef', '000120d8-5e10-4e75-af31-ac0a88f09f52', 'Los drones funcionan de maravilla en la demo. La precisión del análisis es impresionante.', '2026-05-22 19:00:00'),
('088517ec-90e7-4d87-a815-8be9269f45d7', '2dffce1e-8fc9-4a72-b6c2-2bf0765293de', '17c7167b-79e2-40bf-a63e-619206686d43', 'La experiencia VR es muy inmersiva. Perfecto para uso educativo en colegios rurales.', '2026-05-23 17:00:00'),
('27503802-1985-4280-a0d7-594861e73e62', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', 'Como jurado, destaco la calidad técnica y el impacto social. Buen trabajo.', '2026-05-23 16:00:00'),
('f20ccf45-cd20-4057-ab7d-f28f0d7c7733', 'a2524adf-e8e7-4926-adea-94640f3b7b63', '54b69dde-2d56-4c66-b70e-b1a3d7174257', 'El modelo de negocio es sólido. La trazabilidad blockchain añade valor real.', '2026-05-24 11:00:00'),
('50304e1c-0477-4fb0-8f3e-9f14eb687dbe', 'b8032b63-a52a-4ca8-9ab5-d9108d51f2f8', '155cf90d-37a7-4674-8404-8c45d063ec33', 'La gamificación hace que aprender a programar sea divertido. Ideal para niños.', '2026-06-06 12:30:00'),
('0d347c34-949c-4948-880f-11037e9f57b0', 'd5e43765-a6c5-439a-bb1c-c59d07c4d50b', '3c25a1c0-8006-4c40-8752-42bbe0b90170', 'Mi perro lleva 3 días con el collar y la app funciona perfectamente. Gran utilidad.', '2026-06-07 15:00:00');

-- ============================================================
-- 10. VALORACIONES CRITERIO (multicriterio)
-- ============================================================
-- Jurado 1 para proyecto SafeWalk (p5) en votación v2
INSERT INTO valoracion_criterio (id, votante_id, criterio_id, proyecto_id, valoracion) VALUES
('384683e0-1063-4a2b-b946-aac9282612dc', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '7355feee-500c-4ef9-b2f5-16ad00c968c0', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', 4),
('7de43f7d-75d8-4143-af15-ed1d61e2ea1a', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '7c062054-94b5-4114-91a0-3f3ce0fda5d7', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', 5),
('49b7d4f9-ded4-4a3d-b474-b3af0455733f', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '48b4207f-9a52-4580-915f-61731f0d06db', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', 4),
('1d9d50b2-8d3a-4dae-83c2-99c3ee632781', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '37c6d783-2ff2-43a8-ac4b-66201c980cba', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', 5);

-- Jurado 2 para proyecto SafeWalk (p5)
INSERT INTO valoracion_criterio (id, votante_id, criterio_id, proyecto_id, valoracion) VALUES
('98a9d78d-c8ab-46c5-99e9-7319f2d0c607', '54b69dde-2d56-4c66-b70e-b1a3d7174257', '7355feee-500c-4ef9-b2f5-16ad00c968c0', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', 5),
('0880b2bf-7df3-4eb4-a857-0aae4a30da9c', '54b69dde-2d56-4c66-b70e-b1a3d7174257', '7c062054-94b5-4114-91a0-3f3ce0fda5d7', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', 4),
('5b31edeb-16ff-433f-a164-8c2a1da1cd1a', '54b69dde-2d56-4c66-b70e-b1a3d7174257', '48b4207f-9a52-4580-915f-61731f0d06db', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', 5),
('6a245037-b6c3-4b83-8d06-1755dd5dd940', '54b69dde-2d56-4c66-b70e-b1a3d7174257', '37c6d783-2ff2-43a8-ac4b-66201c980cba', '04898ec5-f2f2-4aab-8978-3f4cfcbc257a', 4);

-- Jurado 1 para proyecto GreenCart (p6)
INSERT INTO valoracion_criterio (id, votante_id, criterio_id, proyecto_id, valoracion) VALUES
('5a6701b5-14b3-4660-b779-3c85058570c9', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '7355feee-500c-4ef9-b2f5-16ad00c968c0', 'a2524adf-e8e7-4926-adea-94640f3b7b63', 3),
('7eeda281-8ac5-418b-a941-3f4ccd8de10e', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '7c062054-94b5-4114-91a0-3f3ce0fda5d7', 'a2524adf-e8e7-4926-adea-94640f3b7b63', 4),
('18626789-2439-405b-b148-eded27857902', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '48b4207f-9a52-4580-915f-61731f0d06db', 'a2524adf-e8e7-4926-adea-94640f3b7b63', 4),
('12c54b17-9346-4e65-bf4f-2ce11b28b75b', '5cd66a71-e5af-43f1-8f9a-282d31eaf46d', '37c6d783-2ff2-43a8-ac4b-66201c980cba', 'a2524adf-e8e7-4926-adea-94640f3b7b63', 3);

-- Jurado 2 para proyecto GreenCart (p6)
INSERT INTO valoracion_criterio (id, votante_id, criterio_id, proyecto_id, valoracion) VALUES
('e51cb43d-3835-4aa9-8637-7ddb4f7b853c', '54b69dde-2d56-4c66-b70e-b1a3d7174257', '7355feee-500c-4ef9-b2f5-16ad00c968c0', 'a2524adf-e8e7-4926-adea-94640f3b7b63', 4),
('04d8b201-3811-4bbe-bdb5-f79f300d6641', '54b69dde-2d56-4c66-b70e-b1a3d7174257', '7c062054-94b5-4114-91a0-3f3ce0fda5d7', 'a2524adf-e8e7-4926-adea-94640f3b7b63', 4),
('1ceb087b-37e6-44da-9f3b-e60d6e43afac', '54b69dde-2d56-4c66-b70e-b1a3d7174257', '48b4207f-9a52-4580-915f-61731f0d06db', 'a2524adf-e8e7-4926-adea-94640f3b7b63', 3),
('1700d01a-547c-4c6d-82fd-73657948f3b2', '54b69dde-2d56-4c66-b70e-b1a3d7174257', '37c6d783-2ff2-43a8-ac4b-66201c980cba', 'a2524adf-e8e7-4926-adea-94640f3b7b63', 4);

-- Público para CodeLearn (p7) en votación v3
INSERT INTO valoracion_criterio (id, votante_id, criterio_id, proyecto_id, valoracion) VALUES
('69226380-dff9-44a2-ada1-64740da2d18b', '155cf90d-37a7-4674-8404-8c45d063ec33', '627e421f-928d-4d6a-96fb-2d17ae00fcc8', 'b8032b63-a52a-4ca8-9ab5-d9108d51f2f8', 5),
('948dfc1f-54ac-46da-8883-2880eda6585f', '155cf90d-37a7-4674-8404-8c45d063ec33', '3bce34cb-5afc-48ec-8900-c167e1dd3224', 'b8032b63-a52a-4ca8-9ab5-d9108d51f2f8', 4),
('d709a4f6-4f62-455e-9246-27924efd4c85', '155cf90d-37a7-4674-8404-8c45d063ec33', '2660f601-70a5-4e9d-b67b-1860118ff852', 'b8032b63-a52a-4ca8-9ab5-d9108d51f2f8', 5);

-- Público para PetCare (p8) en votación v3
INSERT INTO valoracion_criterio (id, votante_id, criterio_id, proyecto_id, valoracion) VALUES
('77a2e124-b197-4eed-b64d-8995e8b1be03', '17c7167b-79e2-40bf-a63e-619206686d43', '627e421f-928d-4d6a-96fb-2d17ae00fcc8', 'd5e43765-a6c5-439a-bb1c-c59d07c4d50b', 4),
('879213dd-f992-4f60-bad9-5c5ecf755f63', '17c7167b-79e2-40bf-a63e-619206686d43', '3bce34cb-5afc-48ec-8900-c167e1dd3224', 'd5e43765-a6c5-439a-bb1c-c59d07c4d50b', 5),
('4194d48c-8ad0-4164-be9c-e6bd533b22f1', '17c7167b-79e2-40bf-a63e-619206686d43', '2660f601-70a5-4e9d-b67b-1860118ff852', 'd5e43765-a6c5-439a-bb1c-c59d07c4d50b', 4),
('937f9fe3-7219-48d0-a4f9-5a01644b4a80', '3c25a1c0-8006-4c40-8752-42bbe0b90170', '627e421f-928d-4d6a-96fb-2d17ae00fcc8', 'd5e43765-a6c5-439a-bb1c-c59d07c4d50b', 5),
('a34b572f-9872-4660-8a55-9a1325a66516', '3c25a1c0-8006-4c40-8752-42bbe0b90170', '3bce34cb-5afc-48ec-8900-c167e1dd3224', 'd5e43765-a6c5-439a-bb1c-c59d07c4d50b', 5),
('156c2169-91d0-42ca-9ebe-6637101e959d', '3c25a1c0-8006-4c40-8752-42bbe0b90170', '2660f601-70a5-4e9d-b67b-1860118ff852', 'd5e43765-a6c5-439a-bb1c-c59d07c4d50b', 5);

-- ============================================================
-- RESUMEN DEMO
-- ============================================================
-- Eventos:        2
-- Equipos:        4
-- Participantes:  10 (8 competidores + 2 jurados)
-- Votaciones:     3 (1 estándar, 1 jurado multicriterio, 1 público multicriterio)
-- Criterios:      7
-- Proyectos:      8
-- Votos:          16
-- Comentarios:    9
-- Valoraciones:   24
-- ============================================================
