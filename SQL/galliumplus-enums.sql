-- Remplissage des tables d'énumération --

USE galliumplus;

INSERT INTO `Availability`(`enum`, `description`) VALUES
	(0, 'Automatique'),
	(1, 'Toujours'),
	(2, 'Jamais');

INSERT INTO `HistoryActionKind`(`enum`, `description`) VALUES
	(1, 'Connexion');
	(2, 'Gestion des produits et des catégories'),
	(3, 'Gestion des utilisateurs et des rôles'),
	(4, 'Achat');