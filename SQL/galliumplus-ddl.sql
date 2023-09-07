-- Généré par StarUML ---

USE galliumplus;

SET FOREIGN_KEY_CHECKS = 0;
DROP TABLE IF EXISTS `User`;
DROP TABLE IF EXISTS `Role`;
DROP TABLE IF EXISTS `Session`;
DROP TABLE IF EXISTS `Client`;
DROP TABLE IF EXISTS `BotClient`;
DROP TABLE IF EXISTS `SsoClient`;
DROP TABLE IF EXISTS `Product`;
DROP TABLE IF EXISTS `Availability`;
DROP TABLE IF EXISTS `Category`;
DROP TABLE IF EXISTS `HistoryAction`;
DROP TABLE IF EXISTS `HistoryActionKind`;
DROP TABLE IF EXISTS `HistoryUser`;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE `User` (
    `id` INTEGER NOT NULL,
    `userId` VARCHAR(20) NOT NULL,
    `firstName` VARCHAR(50) NOT NULL,
    `lastName` VARCHAR(50) NOT NULL,
    `email` VARCHAR(80) NOT NULL,
    `role` INTEGER NOT NULL,
    `year` VARCHAR(10) NOT NULL,
    `deposit` DECIMAL(6,2),
    `isMember` BOOLEAN NOT NULL,
    `password` BINARY(32) NOT NULL,
    `registration` DATETIME NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE (`userId`)
);

CREATE TABLE `Role` (
    `id` INTEGER NOT NULL,
    `name` VARCHAR(50) NOT NULL,
    `permissions` INTEGER NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `Session` (
    `nid` INTEGER NOT NULL,
    `token` CHAR(20) NOT NULL,
    `lastUse` DATETIME NOT NULL,
    `expiration` DATETIME NOT NULL,
    `user` INTEGER NOT NULL,
    `client` INTEGER NOT NULL,
    PRIMARY KEY (`nid`),
    UNIQUE (`token`)
);

CREATE TABLE `Client` (
    `id` INTEGER NOT NULL,
    `apiKey` CHAR(20) NOT NULL,
    `name` VARCHAR(50) NOT NULL,
    `granted` INTEGER NOT NULL,
    `revoked` INTEGER NOT NULL,
    `isEnabled` BOOLEAN NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE (`apiKey`)
);

CREATE TABLE `BotClient` (
    `id` INTEGER NOT NULL,
    `secret` CHAR(30) NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `SsoClient` (
    `id` INTEGER NOT NULL,
    `secret` CHAR(30) NOT NULL,
    `redirectUrl` VARCHAR(120) NOT NULL,
    `logoUrl` VARCHAR(120),
    PRIMARY KEY (`id`)
);

CREATE TABLE `Product` (
    `id` INTEGER NOT NULL,
    `name` VARCHAR(50) NOT NULL,
    `stock` INTEGER NOT NULL,
    `nonMemberPrice` DECIMAL(6,2) NOT NULL,
    `memberPrice` DECIMAL(6,2) NOT NULL,
    `availability` INTEGER NOT NULL,
    `category` INTEGER NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `Availability` (
    `enum` INTEGER NOT NULL,
    PRIMARY KEY (`enum`)
);

CREATE TABLE `Category` (
    `id` INTEGER NOT NULL,
    `name` VARCHAR(50) NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `HistoryAction` (
    `id` INTEGER NOT NULL,
    `text` VARCHAR(120) NOT NULL,
    `time` DATETIME NOT NULL,
    `kind` INTEGER NOT NULL,
    `actor` INTEGER NOT NULL,
    `target` INTEGER NOT NULL,
    PRIMARY KEY (`id`)
);

CREATE TABLE `HistoryActionKind` (
    `enum` INTEGER NOT NULL,
    PRIMARY KEY (`enum`)
);

CREATE TABLE `HistoryUser` (
    `id` INTEGER NOT NULL,
    `displayName` VARCHAR(120) NOT NULL,
    `activeUser` INTEGER NOT NULL,
    PRIMARY KEY (`id`),
    UNIQUE (`displayName`, `activeUser`)
);

ALTER TABLE `User` ADD FOREIGN KEY (`role`) REFERENCES `Role`(`id`);
ALTER TABLE `Session` ADD FOREIGN KEY (`user`) REFERENCES `User`(`id`);
ALTER TABLE `Session` ADD FOREIGN KEY (`client`) REFERENCES `Client`(`id`);
ALTER TABLE `BotClient` ADD FOREIGN KEY (`id`) REFERENCES `Client`(`id`);
ALTER TABLE `SsoClient` ADD FOREIGN KEY (`id`) REFERENCES `Client`(`id`);
ALTER TABLE `Product` ADD FOREIGN KEY (`availability`) REFERENCES `Availability`(`enum`);
ALTER TABLE `Product` ADD FOREIGN KEY (`category`) REFERENCES `Category`(`id`);
ALTER TABLE `HistoryAction` ADD FOREIGN KEY (`actor`) REFERENCES `HistoryUser`(`id`);
ALTER TABLE `HistoryAction` ADD FOREIGN KEY (`target`) REFERENCES `HistoryUser`(`id`);
ALTER TABLE `HistoryUser` ADD FOREIGN KEY (`activeUser`) REFERENCES `User`(`id`);

SET FOREIGN_KEY_CHECKS = 1;