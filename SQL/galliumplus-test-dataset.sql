
DELETE FROM `Product`;
DELETE FROM `Category`;

INSERT INTO `Category` VALUES
	(1, 'Boissons'),
	(2, 'Snacks'),
	(3, 'Pablo');

INSERT INTO `Product` VALUES
	(1, 'Coca Cherry', 27, 1.00, 0.80, 0, 1),
	(2, 'KitKat', 14, 0.80, 0.60, 0, 2),
	(3, 'Pablo', 1, 999.99, 500.00, 1, 3);


DELETE FROM `User`;
DELETE FROM `Role`;
DELETE FROM `SsoClient`;
DELETE FROM `BotClient`;
DELETE FROM `Client`;
DELETE FROM `Session`;

INSERT INTO `Client` VALUES
	(1, 'test-api-key-normal', 'Tests (normal)', 0, 0, 1),
	(2, 'test-api-key-restric', 'Tests (restricted)', 0, 374, 1),
	(3, 'test-api-key-minimum', 'Tests (minimum)', 1, 0, 1),
	(4, 'test-api-key-bot', 'Tests (bot)', 0, 0, 1);

INSERT INTO `BotClient`(`id`) VALUES (4);

INSERT INTO `Role` VALUES
	(1, 'Adhérent', 0),
	(2, 'CA', 27),
	(3, 'Président', 511);

INSERT INTO `User` VALUES
	(1, 'lomens', 'Nicolas', 'RESIN', 'nicolas.resin@iut-dijon.u-bourgogne.fr', 1, 'PROF', 20.00, 1, 0x6ff1904d29b818007ccbf05954bc1cd50f70148e41265cb823d54e2e3312b095, 'sel', NOW()),
	(2, 'mf187870', 'Matéo', 'FAVARD', 'mateo.favard@iut-dijon.u-bourgogne.fr', 2, '3A', 9999.99, 1, 0x6ff1904d29b818007ccbf05954bc1cd50f70148e41265cb823d54e2e3312b095, 'sel', NOW()),
	(3, 'eb069420', 'Evan', 'BEUGNOT', 'evan.beugnot@iut-dijon.u-bourgogne.fr', 3, '2A', 9999.99, 1, 0x6ff1904d29b818007ccbf05954bc1cd50f70148e41265cb823d54e2e3312b095, 'sel', NOW());

INSERT INTO `Session` VALUES
	(1, '12345678901234567890', NOW(), '2099-12-31', 1, 1),
	(2, '09876543210987654321', NOW(), '2099-12-31', 3, 1);

DELETE FROM historyaction;
DELETE FROM historyuser;