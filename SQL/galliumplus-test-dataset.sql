
DELETE FROM `Price`;
DELETE FROM `PricingType`;
DELETE FROM `Product`;
DELETE FROM `Item`;
DELETE FROM `Category`;

INSERT INTO `Category` VALUES
	(1, 'Boissons', 1),
	(2, 'Snacks',   1),
	(3, 'Pablo',    1);

INSERT INTO `Item` VALUES
	(1, 'Coca Cherry', 0, 0, 27, 1, NULL, NULL, 0),
	(2, 'KitKat',      0, 0, 14, 2, NULL, NULL, 0),  
	(3, 'Pablo',       0, 1,  1, 3, NULL, NULL, 0);

INSERT INTO `PricingType` VALUES
    (1, 'NON ADHÉRENT', 'Tarif test non-adhérent', 0, 1),
    (2, 'ADHÉRENT',     'Tarif test adhérent',     1, 1);
	
INSERT INTO `Price` VALUES
    (1,   1.00, 0, '2024-01-01', NULL, 0, 1, 1),
    (2,   0.80, 0, '2024-01-01', NULL, 0, 2, 1),
    (3,   0.80, 0, '2024-01-01', NULL, 0, 1, 2),
    (4,   0.60, 0, '2024-01-01', NULL, 0, 2, 2),
    (5, 999.99, 0, '2024-01-01', NULL, 0, 1, 3),
    (6, 500.00, 0, '2024-01-01', NULL, 0, 2, 3);
   
DELETE FROM `AuditLog`;
DELETE FROM `User`;
DELETE FROM `Role`;
DELETE FROM `SameSignOn`;
DELETE FROM `AppAccess`;
DELETE FROM `Client`;
DELETE FROM `Session`;
delete from `PasswordResetToken`;

INSERT INTO `Client` VALUES
	(1, 'test-api-key-normal',  'Tests (normal)',          0, 1023, 1, 0),
	(2, 'test-api-key-restric', 'Tests (restricted)',      0,  137, 1, 0),
	(3, 'test-api-key-minimum', 'Tests (minimum)',         1, 1023, 1, 0),
	(4, 'test-api-key-bot',     'Tests (bot)',             1,    1, 1, 0),
	(5, 'test-api-key-sso-dir', 'Tests (SSO, direct)',     0, 1023, 1, 0),
	(6, 'test-api-key-sso-ext', 'Tests (SSO, externe)',    0, 1023, 1, 0),
	(7, 'test-api-key-sso-bot', 'Tests (SSO, applicatif)', 0, 1023, 1, 0);

INSERT INTO `AppAccess`  VALUES
    (4, 0x6ff1904d29b818007ccbf05954bc1cd50f70148e41265cb823d54e2e3312b095, 'sel'),
    (7, 0x6ff1904d29b818007ccbf05954bc1cd50f70148e41265cb823d54e2e3312b095, 'sel');

INSERT INTO `SameSignOn` VALUES
    (5, 'test-sso-secret', 'https://example.app/login', 'https://example.app/static/logo.png', 1, 256, NULL           ),
    (6, 'test-sso-secret', 'https://example.app/login', 'https://example.app/static/logo.png', 1,   1, 'Appli Externe'),
    (7, 'test-sso-secret', 'https://example.app/login', 'https://example.app/static/logo.png', 1,   0, NULL           );

INSERT INTO `Role` VALUES
	(1, 'Adhérent', 0),
	(2, 'CA', 27),
	(3, 'Président', 1023);

INSERT INTO `User` VALUES
	(1, 'lomens', 'Nicolas', 'RESIN', 'nicolas.resin@iut-dijon.u-bourgogne.fr', 1, 'PROF', 20.00, 1, 0x6ff1904d29b818007ccbf05954bc1cd50f70148e41265cb823d54e2e3312b095, 'sel', NOW(), 0, 1, '2099-12-31'),
	(2, 'mf187870', 'Matéo', 'FAVARD', 'mateo.favard@iut-dijon.u-bourgogne.fr', 2, '3A', 9999.99, 1, 0x6ff1904d29b818007ccbf05954bc1cd50f70148e41265cb823d54e2e3312b095, 'sel', NOW(), 0, 1, '2099-12-31'),
	(3, 'eb069420', 'Evan', 'BEUGNOT', 'evan.beugnot@iut-dijon.u-bourgogne.fr', 3, '2A', 9999.99, 1, 0x6ff1904d29b818007ccbf05954bc1cd50f70148e41265cb823d54e2e3312b095, 'sel', NOW(), 0, 1, '2099-12-31');

INSERT INTO `Session` VALUES
	(1, '12345678901234567890', NOW(), '2099-12-31', 1, 1),
	(2, '09876543210987654321', NOW(), '2099-12-31', 3, 1);

insert into `PasswordResetToken` values (1, 'test-prt-1', 0x077c0ecb24275498a55cffb49abbbb83256be79ff9f29bb0ca0615a759f6cc57, 'j0REaldrjr2JPtqszJDvYcfbx43jTupu', '2099-12-31', 'mf187870');

DELETE FROM `HistoryAction`;
DELETE FROM `HistoryUser`;